using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoogleMap.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace GoogleMap.Controllers
{
    public class MarkerComplexType
    {
        public Guid Id { set; get; }
        public string Name { set; get; }
        public string Latitude { set; get; }
        public string Longitude { set; get; }
        public string Address { set; get; }
    }

    public class MarkerController : BaseController
    {
        [Authorize]
        public ActionResult Index()
        {
            return View(db.Markers.OrderBy(x => x.SubCategory.Category.Name).ThenBy(x => x.SubCategory.Name).ThenBy(x => x.Name));
        }

        [Authorize]
        public JsonResult AjaxIndex([DataSourceRequest]DataSourceRequest request)
        {
            var markers = db.Markers.OrderBy(x => x.Id);
            var result = markers.Select(x => new {x.Id, x.Name, x.Latitude, x.Longitude, x.Address});

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        //
        // GET: /Marker/Details/5

        public ViewResult Details(Guid id)
        {
            Marker marker = db.Markers.Single(m => m.Id == id);
            return View(marker);
        }

        //
        // GET: /Marker/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Message = "Welcome to GOOGLE MAP!";
            ViewBag.Categories = db.Categories.OrderBy(x => x.Name).ToList();
            ViewBag.SubCategories = new List<SubCategory>(0); //db.SubCategories.Where(x => false).OrderBy(x => x.Name).ToList();}
            ViewBag.Markers = new List<Marker>(0); //db.Markers.Where(x => false).OrderBy(x => x.Name).ToList();
            return View();
        }

        //
        // POST: /Marker/Create

        [HttpPost]
        [Authorize]
        public ActionResult Create(Marker marker)
        {
            if (ModelState.IsValid)
            {
                marker.Id = Guid.NewGuid();
                marker.TotalComment = marker.TotalLikes = 0;
                marker.Status = "DEACTIVE";

                db.Markers.Add(marker);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "Id", "Name", marker.SubCategoryId);
            return View(marker);
        }

        //
        // GET: /Marker/Edit/5

        public ActionResult Edit(Guid id)
        {
            Marker marker = db.Markers.Single(m => m.Id == id);
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "Id", "Name", marker.SubCategoryId);
            return View(marker);
        }

        //
        // POST: /Marker/Edit/5

        [HttpPost]
        public ActionResult Edit(Marker marker)
        {
            if (ModelState.IsValid)
            {
                marker.Status = "ACTIVE";
                marker.CreatedBy = "admin";
                marker.CreatedDate = DateTime.Now;
                db.Markers.Attach(marker);
                db.Entry(marker).State = System.Data.Entity.EntityState.Modified;
                //db.ObjectStateManager.ChangeObjectState(marker, EntityState.Modified);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "Id", "Name", marker.SubCategoryId);
            return View(marker);
        }

        //
        // GET: /Marker/Delete/5

        public ActionResult Delete(Guid id)
        {
            Marker marker = db.Markers.Single(m => m.Id == id);
            return View(marker);
        }

        //
        // POST: /Marker/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Marker marker = db.Markers.Single(m => m.Id == id);
            db.Markers.Remove(marker);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult UploadPicture(Guid id)
        {
            return View(db.Markers.SingleOrDefault(x => x.Id == id));
        }

        // This action handles the form POST and the upload
        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase file, Guid id, FormCollection form)
        {
            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the fielname
                var fileName = Path.GetFileName(file.FileName);
                // store the file inside ~/App_Data/uploads folder
                if (fileName != null)
                {
                    var path = Path.Combine(Server.MapPath("~/Data/Images/Markers/"), fileName);

                    var i = 1;
                    while (System.IO.File.Exists(path))
                    {
                        path = Path.Combine(Server.MapPath("~/Data/Images/Markers/"), i + "_" + fileName);
                        i++;
                    }

                    file.SaveAs(path);

                    // Save to database.
                    var picture = new Picture();
                    picture.Id = Guid.NewGuid();
                    picture.Name = form["txtName"];
                    picture.MarkerId = id;                    
                    picture.ImageUrl = "Data/Images/Markers/" + fileName;

                    db.Pictures.Add(picture);
                    db.SaveChanges();
                }
            }

            // redirect back to the index action to show the form once again
            return RedirectToAction("UploadPicture", new {id});
        }

        public ActionResult AddMarker()
        {
            ViewBag.Categories = db.Categories.OrderBy(x => x.Name).ToList();
            ViewBag.SubCategories = new List<SubCategory>(0); 
            //db.SubCategories.Where(x => false).OrderBy(x => x.Name).ToList();}
            //ViewBag.SubCategoryId = new SelectList(db.SubCategories.OrderBy(x => x.Name), "Id", "Name");
            return View(new Marker());
        }

        [HttpPost]
        public ActionResult AddMarker(Marker marker)
        {
            if (ModelState.IsValid)
            {
                marker.Id = Guid.NewGuid();
                marker.Latitude = Request.QueryString["latitude"];
                marker.Longitude = Request.QueryString["longitude"];
                marker.Status = "ACTIVE";
                marker.CreatedBy = User.Identity.Name;
                marker.CreatedDate = DateTime.Now;
                marker.TotalComment = marker.TotalLikes = 0;
                db.Markers.Add(marker);
                db.Entry(marker).State = System.Data.Entity.EntityState.Added;
                //db.ObjectStateManager.ChangeObjectState(marker, EntityState.Added);
                db.SaveChanges();
                return RedirectToAction("UploadPicture", new {id = marker.Id});
            }
            ViewBag.SubCategoryId = new SelectList(db.SubCategories, "Id", "Name", marker.SubCategoryId);
            return View(marker);
        }

        //
        // GET: /Marker/
        [Authorize(Roles = "admin")]
        public ViewResult ApproveMarker()
        {
            var markers = db.Markers.Where(x => x.Status != "ACTIVE");
            return View(markers);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}