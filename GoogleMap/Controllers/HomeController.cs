using System;
using System.Linq;
using System.Web.Mvc;
using GoogleMap.Models;
using System.Collections.Generic;
using Kendo.Mvc.UI;

namespace GoogleMap.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "GOOGLE MAP";
            var categories = db.Categories.ToList();
            var kendoListItems = categories.OrderBy(x => x.Name).Select(item => new DropDownListItem { Text = item.Name, Value = item.Id.ToString() });

            ViewBag.KendoDropDownListDataSource = kendoListItems;
            ViewBag.Categories = db.Categories.OrderBy(x => x.Name);
            ViewBag.SubCategories = new List<SubCategory>(0); //db.SubCategories.Where(x => false).OrderBy(x => x.Name).ToList();}
            ViewBag.Markers = new List<Marker>(0); //db.Markers.Where(x => false).OrderBy(x => x.Name).ToList();
            return View();
        }

        public ViewResult Details(Guid id)
        {
            Marker marker = db.Markers.Find(id);
            return View(marker);
        }

        [HttpGet]
        public JsonResult GetAllMarkersForAutoComplete(string term)
        {
            var markers = this.db.Markers.Where(x => x.Name.Contains(term)).OrderBy(x => x.Name).Select(x => new { value = x.Id, label = x.Name });
            return Json(markers.ToList(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchMarker(string SearchValue)
        {
            var markers = this.db.Markers.Where(x => x.Name.Contains(SearchValue)).OrderBy(x => x.Name).Select(x => new { value = x.Id, label = x.Name + ", Địa chỉ: " + x.Address });
            return Json(markers.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllMarkers(Guid? CategoryID)
        {
            var markers = this.db.Markers.Where(x => x.Status == "ACTIVE").Select(x => new { x.Id, SubCategoryID = x.SubCategory.Id, CategoryID = x.SubCategory.CategoryId, x.Name, x.Phone, x.Website, x.Latitude, x.Longitude, x.Address, x.SubCategory.SubCategoryIconType, x.SubCategory.Category.CategoryIconType });
            if (CategoryID.HasValue && CategoryID != Guid.Empty)
            {
                markers = markers.Where(x => x.CategoryID == CategoryID);
            }

            return Json(markers.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCategories()
        {
            var categories = this.db.Categories.Select(x => new { Value = x.Id, Text = x.Name });
            return Json(categories.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSubCategories(string CategoryId)
        {
            if (CategoryId.Length == 0) return Json("", JsonRequestBehavior.AllowGet);

            var id = new Guid(CategoryId);
            var subCategories = this.db.SubCategories.Where(x => x.CategoryId == id).Select(x => new {Value = x.Id, Text = x.Name, Id = x.Id, Name = x.Name });
            return Json(subCategories.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetMarkers(string subCategoryId)
        {
            var guid = Guid.Empty;
            if (subCategoryId != "")
            {
                guid = new Guid(subCategoryId);
            }

            var subCategories = this.db.Markers.Where(x => x.SubCategoryId == guid).Select(x => new { Value = x.Id, Text = x.Name, Id = x.Id, Name = x.Name });
            return Json(subCategories.ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetPictures(string markerId)
        {
            var guid = Guid.Empty;
            if (markerId != "")
            {
                guid = new Guid(markerId);
            }

            var subCategories = this.db.Pictures.Where(x => x.MarkerId == guid).Select(x => new { Value = x.Id, Text = x.Name, Url = x.ImageUrl });
            return Json(subCategories.ToList(), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize]
        public JsonResult CreateMarker(string name, string subCategoryId, string latitude, string longitude, string address, string phone, string website)
        {

            var marker = new Marker
                             {
                                 Id = Guid.NewGuid(),
                                 Name = name,
                                 SubCategoryId = new Guid(subCategoryId),
                                 Latitude = latitude,
                                 Longitude = longitude,
                                 Address = address,
                                 Phone = phone,
                                 Website = website,
                                 CreatedBy = User.Identity.Name,
                                 CreatedDate = DateTime.Now,
                                 Status = "ACTIVE"
                             };
            if (User.Identity.Name != "admin")
            {
                marker.Status = "DEACTIVE";
            }
            this.db.Markers.Add(marker);
            this.db.SaveChanges();
            return Json(new
            {
                Message = "Lưu thông tin thành công!"
            });
        }

        [HttpPost]
        [Authorize]
        public JsonResult UpdateMarker(string markerId, string latitude, string longitude)
        {
            var id = new Guid(markerId);
            Marker marker = this.db.Markers.Single(x => x.Id == id);
            marker.Latitude = latitude;
            marker.Longitude = longitude;
            this.db.SaveChanges();
            return Json(new
            {
                Message = "Data Saved!"
            });
        }

        [HttpPost]
        public JsonResult GetMarker(string markerId)
        {
            var id = new Guid(markerId);
            Marker marker = db.Markers.Single(x => x.Id == id);

            return Json(new
            {
                name = marker.Name,
                latitude = marker.Latitude,
                longitude = marker.Longitude
            });
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult ViewMarker(string markerId)
        {
            var guid = Guid.Empty;
            if (markerId != "")
            {
                guid = new Guid(markerId);
            }

            var markers = this.db.Markers.Include("Pictures").SingleOrDefault(x => x.Id == guid);
            if (markers != null)
            {
                ViewBag.Pictures = markers.Pictures;
            }
            return View(markers);
        }

        public ActionResult ViewPictures(string markerId)
        {
            var guid = Guid.Empty;
            if (markerId != "")
            {
                guid = new Guid(markerId);
            }

            var pictures = this.db.Pictures.Where(x => x.MarkerId == guid).ToList();
            return View(pictures);
        }

        public ActionResult ViewPicture(string pictureId)
        {
            var guid = Guid.Empty;
            if (pictureId != "")
            {
                guid = new Guid(pictureId);
            }

            var picture = this.db.Pictures.FirstOrDefault(x => x.Id == guid);
            return View(picture);
        }

        [HttpGet]
        [Authorize]
        public ViewResult CreateMarkerComment(string markerId)
        {
            var guid = new Guid(markerId);
            var comments = this.db.MarkerComments.Where(x => x.MarkerId == guid).OrderBy(x => x.CreatedDate).ToList();
            var pictures = this.db.Pictures.Where(x => x.MarkerId == guid).ToList();
            var marker = this.db.Markers.SingleOrDefault(x => x.Id == guid);

            this.ViewBag.Comments = comments;
            this.ViewBag.Pictures = pictures;

            return View(marker);
        }

        [HttpPost]
        [Authorize]
        public JsonResult CreateMarkerComment(string markerId, string commentText)
        {
            var comment = new MarkerComment
            {
                Id = Guid.NewGuid(),
                CommentText = commentText,
                MarkerId = new Guid(markerId),
                UserName = User.Identity.Name,
                CreatedDate = DateTime.Now
            };

            this.db.MarkerComments.Add(comment);
            this.db.SaveChanges();
            return Json(new
            {
                Message = "Lưu thông tin thành công!"
            });
        }

        [HttpPost]
        [Authorize]
        public JsonResult CreateMarkerLike(string markerId)
        {
            var like = new MarkerLike()
            {
                Id = Guid.NewGuid(),
                MarkerId = new Guid(markerId),
                UserName = User.Identity.Name,
                CreatedDate = DateTime.Now
            };

            this.db.MarkerLikes.Add(like);
            this.db.SaveChanges();
            return Json(new
            {
                Message = "Lưu thông tin thành công!"
            });
        }

    }
}
