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
    public class SubCategoryController : BaseController
    {
        //
        // GET: /SubCategory/
        [Authorize]
        public ViewResult Index()
        {
            var subcategories = db.SubCategories.Include("Category").OrderBy(x => x.Category.Name);
            return View(subcategories.ToList());
        }

        //
        // GET: /SubCategory/Details/5

        public ViewResult Details(Guid id)
        {
            SubCategory subcategory = db.SubCategories.Single(s => s.Id == id);
            return View(subcategory);
        }

        //
        // GET: /SubCategory/Create

        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name");
            return View();
        } 

        //
        // POST: /SubCategory/Create

        [HttpPost]
        public ActionResult Create(SubCategory subcategory)
        {
            if (ModelState.IsValid)
            {
                subcategory.Id = Guid.NewGuid();
                db.SubCategories.Add(subcategory);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", subcategory.CategoryId);
            return View(subcategory);
        }
        
        //
        // GET: /SubCategory/Edit/5
 
        public ActionResult Edit(Guid id)
        {
            SubCategory subcategory = db.SubCategories.Single(s => s.Id == id);
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", subcategory.CategoryId);
            return View(subcategory);
        }

        //
        // POST: /SubCategory/Edit/5

        [HttpPost]
        public ActionResult Edit(SubCategory subcategory)
        {
            if (ModelState.IsValid)
            {
                db.SubCategories.Attach(subcategory);
                db.Entry(subcategory).State = System.Data.Entity.EntityState.Modified;
                //db.ChangeObjectState(subcategory, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", subcategory.CategoryId);
            return View(subcategory);
        }

        //
        // GET: /SubCategory/Delete/5
 
        public ActionResult Delete(Guid id)
        {
            SubCategory subcategory = db.SubCategories.Single(s => s.Id == id);
            return View(subcategory);
        }

        //
        // POST: /SubCategory/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {            
            SubCategory subcategory = db.SubCategories.Single(s => s.Id == id);
            db.SubCategories.Remove(subcategory);
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