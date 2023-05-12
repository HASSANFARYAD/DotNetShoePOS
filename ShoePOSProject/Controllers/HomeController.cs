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
                ViewBag.Customer = new CustomerBL().GetActiveCustomersList(db).Count;
                ViewBag.Products = new InventoryBL().GetActiveInventoriesList(db).Count;
            }
            else
            {
                ViewBag.Brand = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 1 && x.CreatedBy == LoggedInUser.Id).ToList().Count;
                ViewBag.Size = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 2 && x.CreatedBy == LoggedInUser.Id).ToList().Count;
                ViewBag.Color = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 4 && x.CreatedBy == LoggedInUser.Id).ToList().Count;
                ViewBag.Customer = new CustomerBL().GetActiveCustomersList(db).Where(x => x.CreatedBy== LoggedInUser.Id).ToList().Count;
                ViewBag.Products = new InventoryBL().GetActiveInventoriesList(db).Where(x => x.CreatedBy == LoggedInUser.Id).ToList().Count;
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

        [HttpPost]
        public ActionResult SalesPurchaseGraph()
        {
            List<string> Months = new List<string>();
            var getCurrentDate = DateTime.Now;
            List<List<Inventory>> Inventory = new List<List<Inventory>>();
            List<Inventory> inventories = new InventoryBL().GetActiveInventoriesList(db).ToList();
            for(var i = 1; i <=12; i++)
            {
                var MonthNumber = "";
                if(i < 10)
                {
                    MonthNumber = "0" + i;
                }
                var inventory = inventories.Where(x => Convert.ToDateTime(x.InventoryDate).Date.ToString("MM").ToLower().Equals(MonthNumber)).ToList();
                Inventory.Add(inventory);
            }

            if (Inventory != null)
            {
                var SalePrice = 0;
                var PurchasePrice = 0;
                List<List<Abc>> dTOs1 = new List<List<Abc>>();
                List<Abc> dTOs = new List<Abc>();
                var getMonth = "";
                var list = 1;
                foreach (var item in Inventory)
                {
                    foreach(var item2 in item)
                    {
                        getMonth = Convert.ToDateTime(item2.InventoryDate).ToString("MM");
                        if (item2.SalePrice != null)
                        {
                            SalePrice = SalePrice + Convert.ToInt32(item2.SalePrice);
                        }
                        if(item2.Price != null)
                        {
                            PurchasePrice = PurchasePrice + Convert.ToInt32(item2.Price);
                        }
                        
                    }
                    Abc dTO = new Abc()
                    {
                        SalePrice = SalePrice,
                        PurchasePrice = PurchasePrice,
                        Month = list.ToString()
                    };
                    dTOs.Add(dTO);
                    list = list + 1;
                }
                return Json(dTOs, JsonRequestBehavior.AllowGet);
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
    }

    public class Abc
    {
        public double SalePrice { get; set; }
        public double PurchasePrice { get; set; }
        public string Month { get; set; }
    }
}