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
            //For Admin
            if(gp.validateUser().Role == 1)
            {
                //Admin
                ViewBag.Admin = new UserBL().GetActiveUsersList(db).Where(x => x.IsActive == 1 && x.Role == 1).ToList().Count;
                //Managers
                ViewBag.User = new UserBL().GetActiveUsersList(db).Where(x => x.IsActive == 1 && x.Role == 3).ToList().Count;
                //Company or Customer
                //ViewBag.Customer = new CustomerBL().GetActiveCustomersList(db).Where(x => x.IsActive == 1).ToList().Count;
            }
            // For Manager
            if (gp.validateUser().Role == 2)
            {
                ViewBag.User = new UserBL().GetActiveUsersList(db).Where(x => x.IsActive == 1 && x.Role == 3).ToList().Count;
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