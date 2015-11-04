using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using GoldInventory.Model;
using GoldInventory.ParseWrappers;

namespace GoldInventory.Controllers
{
    [Authorize]
    public class CustomAttributeController : Controller
    {
        // GET: CustomAttribute
        public async Task<ActionResult> Index()
        {
            var attributes = await new AttributeHelper().GetAllAttributes();
            return View(attributes);
        }

        // GET: CustomAttribute/Create
        public ActionResult Create()
        {
            ViewBag.SupportedAttributes = new List<SelectListItem>
            {
                new SelectListItem {Text = "String", Value = SupportedAttributeTypes.String},
                new SelectListItem {Text = "Number", Value = SupportedAttributeTypes.Number},
            };
            return View();
        }

        // POST: CustomAttribute/Create
        [HttpPost]
        public async Task<ActionResult> Create(CustomAttribute customAttribute)
        {
            try
            {
                await new AttributeHelper().SaveCustomAttribute(customAttribute);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomAttribute/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var attr = await new AttributeHelper().GetCustomAttributeById(id);
            ViewBag.SupportedAttributes = new List<SelectListItem>
            {
                new SelectListItem {Text = "String", Value = SupportedAttributeTypes.String},
                new SelectListItem {Text = "Number", Value = SupportedAttributeTypes.Number},
            };
            
            return View(attr);
        }

        // POST: CustomAttribute/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(string id, CustomAttribute customAttribute)
        {
            try
            {
                await new AttributeHelper().SaveCustomAttribute(customAttribute);
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.SupportedAttributes = new List<SelectListItem>
                {
                    new SelectListItem {Text = "String", Value = SupportedAttributeTypes.String},
                    new SelectListItem {Text = "Number", Value = SupportedAttributeTypes.Number},
                };
                var attr = new AttributeHelper().GetCustomAttributeById(id);
                return View(attr);
            }
        }

        // GET: CustomAttribute/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            var attr = await new AttributeHelper().GetCustomAttributeById(id);
            return View(attr);
        }

        // POST: CustomAttribute/Delete/5
        [HttpPost]
        public async Task<ActionResult> Delete(string id, CustomAttribute customAttribute)
        {
            try
            {
                await new AttributeHelper().DeleteCustomAttributeById(id);
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                var attr = await new AttributeHelper().GetCustomAttributeById(id);
                return View(attr);
            }
        }
    }
}
