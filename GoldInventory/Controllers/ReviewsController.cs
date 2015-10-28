using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoldInventory.Models;

namespace GoldInventory.Controllers
{
    public class ReviewsController : Controller
    {
        // GET: Reviews
        public ActionResult Index()
        {
            var model = from r in _items
                        orderby r.Id
                        select r;

            return View(model);
        }

        // GET: Reviews/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reviews/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reviews/Edit/5
        public ActionResult Edit(int id)
        {
            var review = _items.Single(r => r.Id == id);
            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            var review = _items.Single(r => r.Id == id);
            if (TryUpdateModel(review))
            {
                // TODO Update the data
                return RedirectToAction("Index");
            }

            return View(review);
        }
        
        [ChildActionOnly]
        public ActionResult BestReview()
        {
            var review = _items.First();
            return PartialView("_Review", review);
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reviews/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        static List<ItemReview> _items = new List<ItemReview>
        {
            new ItemReview
            {
                Id = 12,
                CategoryName = "Chain",
                Name = "Arab Latest Model",
                Type = "Knot-Chain"
            },
            new ItemReview
            {
                Id = 13,
                CategoryName = "Chain",
                Name = "Dubai Latest Model",
                Type = "Knot"
            },
            new ItemReview
            {
                Id = 14,
                CategoryName = "Ring",
                Name = "Traditional",
                Type = "Regular"
            }
        };
    }
}
