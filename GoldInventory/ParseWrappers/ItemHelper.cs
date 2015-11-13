using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GoldInventory.Model;
using GoldInventory.Models;
using ImageResizer;
using Parse;
using WebGrease.Css.Extensions;

namespace GoldInventory.ParseWrappers
{
    public class SupportedAttributeTypes
    {
        public static readonly string String = "string";
        public static readonly string Number = "number";
    }

    public class ItemHelper
    {
        public async Task<IEnumerable<Item>> GetAllItems()
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<Item>();

            var itemQuery = ParseObject.GetQuery("Item");
            itemQuery.WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString());
            var items = await itemQuery.FindAsync();
            if (items == null)
                return new List<Item>();

            var allCategories = (await new ItemCategoryHelper().GetAllItemCategories()).ToList();
            var filteredItems = new List<Item>();
            foreach (var item in items)
            {
                if (!item.ContainsKey("CompanyId") || !item.ContainsKey("CategoryId"))
                    continue;

                if (item["CompanyId"].ToString() != currentUser["CompanyId"].ToString())
                    continue;

                filteredItems.Add(new Item
                {
                    Id = item.ObjectId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    Name = item["Name"]?.ToString(),
                    CategoryName = allCategories.FirstOrDefault(c => c.Id == item.Get<string>("CategoryId"))?.Name
                });
            }

            await AssociateAllItems(filteredItems);
            return filteredItems;
        }

        private async Task AssociateAllItems(List<Item> items)
        {
            var allAttrs = await new AttributeHelper().GetAllAttributes();
            var allItemAttrs = (await new ItemAttributeHelper().GetAllItemAttributesByItemIds(items.Select(i => i.Id))).ToList();
            allItemAttrs.ForEach(i => i.AttributeName = allAttrs.FirstOrDefault(a => a.Id == i.AttributeId)?.Name ?? "Error");
            items.ForEach(i =>
            {
                var attrs = allItemAttrs.Where(a => a.ItemId == i.Id).ToList();
                attrs.ForEach(a => a.AttributeType = allAttrs.FirstOrDefault(aa => aa.Id == a.AttributeId)?.Type);
                var bareAttrsNotPresent = allAttrs.Where(a => attrs.All(l => l.AttributeId != a.Id)).ToList();
                if (bareAttrsNotPresent.Any())
                {
                    attrs.AddRange(bareAttrsNotPresent.Select(b => new ItemAttribute
                    {
                        ItemId = i.Id,
                        Value = "",
                        AttributeId = b.Id,
                        AttributeName = b.Name,
                        AttributeType = b.Type
                    }).ToList());
                }

                i.AssociatedAttributes = attrs.OrderBy(a => a.AttributeName);
            });
        }

        public async Task SaveItem(Item item)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return;

            var itemObject = new ParseObject("Item");
            if (!string.IsNullOrEmpty(item.Id))
                itemObject.ObjectId = item.Id;

            itemObject["Name"] = item.Name;
            itemObject["CategoryId"] = item.CategoryId;
            itemObject["CompanyId"] = currentUser["CompanyId"].ToString();
            itemObject["PhotoId"] = (item.PhotoStream != null && item.PhotoStream.Length > 0)
                ? await UploadPhoto(item.PhotoStream, item.PhotoContentType, item.Name, item.PhotoId)
                : null;
            await itemObject.SaveAsync();

            item.AssociatedAttributes.ForEach(attr => attr.ItemId = itemObject.ObjectId);
            await new ItemAttributeHelper().SaveAllItemAttributes(item.AssociatedAttributes);
        }

        public async Task<string> UploadPhoto(Stream imageStream, string contentType, string name, string photoId)
        {
            ParseFile parsePhoto;
            using (var originalImageStream = imageStream)
            {
                parsePhoto = new ParseFile(name, originalImageStream, contentType);
                await parsePhoto.SaveAsync();
            }
           
            var itemImage = new ParseObject("Photo")
            {
                ["RawData"] = parsePhoto,
                ["Name"] = name
            };
            if (!string.IsNullOrWhiteSpace(photoId))
                itemImage.ObjectId = photoId;

            await itemImage.SaveAsync();
            return itemImage.ObjectId;
        }

        public Stream ResizeImage(Stream image, int width, int height)
        {
            var config = new ImageResizer.Configuration.Config();
            var buffer = new MemoryStream();
            var job = new ImageJob(image, buffer, new Instructions($"width={width}"));
            config.Build(job);
            return buffer;

            //var resizedImage = new Bitmap(image);
            //var result = new Bitmap(width, height);
            //result.SetResolution(resizedImage.HorizontalResolution, resizedImage.VerticalResolution);
            //using (Graphics graphics = Graphics.FromImage(result))
            //{
            //    graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //    graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //    graphics.DrawImage(resizedImage, 0, 0, width, height);
            //}

            //resizedImage.Dispose();
            //await _storageHelper.Storefile(result, _thumbnailImage);
            //return await _storageHelper.GetFile(_thumbnailImage);
        }

        public async Task<ParseObject> GetPhotoById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var photo = await ParseObject.GetQuery("Photo").GetAsync(id);
            return photo;
        }

        public async Task<Item> GetItemById(string id)
        {
            var itemObject = await GetRawItemObjectById(id);
            var category = new ItemCategoryHelper().GetItemCategoryById(itemObject.Get<string>("CategoryId"));

            Uri photoUri = null;
            Uri thumbUri = null;
            if (itemObject.ContainsKey("PhotoId"))
            {
                var photo = await GetPhotoById(itemObject.Get<string>("PhotoId"));
                photoUri = (photo?["RawData"] as ParseFile)?.Url;
                thumbUri = (photo?["RawData"] as ParseFile)?.Url;
            }

            var item = new Item
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                CategoryName = category?.Name,
                CategoryId = category?.Id,
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt,
                PhotoUri = photoUri,
                PhotoId = (itemObject.ContainsKey("PhotoId")) ? itemObject.Get<string>("PhotoId") : null,
                ThumbnailUri = thumbUri
            };
            await AssociateAllItems(new List<Item> {item});
            return item;
        }

        private async Task<ParseObject> GetRawItemObjectById(string id)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return null;

            var itemQuery = ParseObject.GetQuery("Item");
            var itemObject = await itemQuery.GetAsync(id);
            if (currentUser["CompanyId"].ToString() != itemObject["CompanyId"].ToString())
                return null;

            return itemObject;
        }

        public async Task<bool> DeleteItemById(string id)
        {
            var rawItem = await GetRawItemObjectById(id);
            if (rawItem == null)
                return false;

            var result = await new ItemAttributeHelper().DeleteItemAttributeByItemId(id);
            if (!result)
                return false;

            await rawItem.DeleteAsync();
            return true;
        } 

        public async Task<IEnumerable<Item>> GetItemsByName(string name, bool associateAttributes = true)
        {
            var currentUser = await UserUtility.GetCurrentParseUser();
            if (currentUser == null)
                return new List<Item>();

            var itemQuery = ParseObject.GetQuery("Item").WhereEqualTo("CompanyId", currentUser["CompanyId"].ToString()).WhereContains("Name", name);
            var items = await itemQuery.FindAsync();
            var itemObjects = items?.Select(itemObject => new Item
            {
                Id = itemObject.ObjectId,
                Name = itemObject.Get<string>("Name"),
                CategoryId = itemObject.Get<string>("CategoryId"),
                UpdatedAt = itemObject.UpdatedAt,
                CreatedAt = itemObject.CreatedAt
            }).ToList();
            if (associateAttributes)
                await AssociateAllItems(itemObjects);

            return itemObjects;
        }
    }
}