using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoogleMap.Models;

namespace GoogleMap.Controllers
{
    public class CategoryController : Controller
    {
        private GoogleMapEntities db = new GoogleMapEntities();

        //
        // GET: /Category/
        [Authorize(Roles = "admin")]
        public ViewResult Index()
        {
            return View(db.Categories.ToList());
        }

        //
        // GET: /Category/Details/5
        [Authorize(Roles = "admin")]
        public ViewResult Details(Guid id)
        {
            Category category = db.Categories.Single(c => c.Id == id);
            return View(category);
        }

        //
        // GET: /Category/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            var folders = System.IO.Directory.GetDirectories(Server.MapPath("~/Images/Icons"));
            var result = folders.Select(folder => folder.Substring(folder.LastIndexOf("\\") + 1));

            ViewBag.CategoryIconType = new SelectList(result);
            return View();
        }

        //
        // POST: /Category/Create

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Id = Guid.NewGuid();
                category.LayerId = new Guid("2c87ba4a-1d37-4c3a-9ce6-409ed8f36d65");
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        //
        // GET: /Category/Edit/5
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Guid id)
        {
            var category = db.Categories.Single(c => c.Id == id);
            category.LayerId = new Guid("2c87ba4a-1d37-4c3a-9ce6-409ed8f36d65");
            return View(category);
        }

        //
        // POST: /Category/Edit/5

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                category.LayerId = new Guid("2c87ba4a-1d37-4c3a-9ce6-409ed8f36d65");
                db.Categories.Attach(category);
                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                //db.ObjectStateManager.ChangeObjectState(category, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        //
        // GET: /Category/Delete/5
        [Authorize(Roles = "admin")]
        public ActionResult Delete(Guid id)
        {
            var category = db.Categories.Single(c => c.Id == id);
            return View(category);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "admin")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            var category = db.Categories.Single(c => c.Id == id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}