using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using GoldInventory.Model;

namespace GoldInventory.Models
{
    public class Item
    {
        public string Id { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(512)]
        public string Name { get; set; }

        public string CategoryId { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        public Stream PhotoStream { get; set; }

        public Uri PhotoUri { get; set; }

        public string PhotoId { get; set; }

        public string PhotoContentType { get; set; }

        public IEnumerable<ItemAttribute> AssociatedAttributes { get; set; }

        public IEnumerable<ItemCategory> AvailableCategories { get; set; }

        public IEnumerable<ItemCategory> SelectedCategories { get; set; }
    }
}