using ShoePOSProject.BL;
using ShoePOSProject.HelpingClasses;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoePOSProject.Controllers
{
    public class HomeController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        GeneralPurpose gp = new GeneralPurpose();

        private bool isLogedIn()
        {
            if (gp.validateUser() != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Index()
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            User LoggedInUser = gp.validateUser();
            //For Admin
            if(LoggedInUser.Role == 1)
            {
                ViewBag.Brand = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 1).ToList().Count;
                ViewBag.Size = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 2).ToList().Count;
                ViewBag.Color = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 4).ToList().Count;
            }
            else
            {
                ViewBag.Brand = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 1 && x.CreatedBy == LoggedInUser.Id).ToList().Count;
                ViewBag.Size = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 2 && x.CreatedBy == LoggedInUser.Id).ToList().Count;
                ViewBag.Color = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 4 && x.CreatedBy == LoggedInUser.Id).ToList().Count;
            }
            ViewBag.Role = gp.validateUser().Role;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [Route("Calculator")]
        public ActionResult Calculator()
        {
            return View();
        }
    }
}