using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GoogleMap.Models;

namespace GoogleMap.Controllers
{
    public class BaseController : Controller
    {
        protected GoogleMapEntities db = new GoogleMapEntities();
    }
}
