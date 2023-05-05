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
    public class OptionController : Controller
    {
        // GET: Option
        DatabaseEntities db = new DatabaseEntities();
        GeneralPurpose gp = new GeneralPurpose();
        MailSender ms = new MailSender();
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
        // GET: NewOption
        public ActionResult Index(string message = "", string color = "black")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            if (color == "red")
            {
                ViewBag.errormessage = message;
                ViewBag.color = color;
            }
            if (color == "green")
            {
                ViewBag.successfullmessage = message;
                ViewBag.color = color;
            }

            return View();
        }

        [HttpPost]
        public ActionResult GetOptionsList(string Brand = "", string Style = "")
        {
            List<NewOption> list = new List<NewOption>();
            if (gp.validateUser().Role == 1)
            {
                list = new NewOptionBL().GetActiveNewOptionsList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                list = new NewOptionBL().GetActiveNewOptionsList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                if (sortDirection == "asc")
                {
                    list = list.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
                else
                {
                    list = list.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                list = list.Where(x => x.Name.ToLower().Contains(searchValue.ToLower())
                        ).ToList();
            }
            int totalrows = list.Count();
            int totalrowsafterfilterinig = list.Count();
            list = list.Skip(start).Take(length).ToList();

            List<DTO> udto = new List<DTO>();
            foreach (NewOption u in list)
            {
                DTO obj = new DTO()
                {
                    OptionId = u.Id,
                    OptionEncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                    OptionName = u.Name,
                    OptionPrice = u.Price,
                    Role = gp.validateUser().Role
                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostAddNewOption(NewOption _NewOption, int PageId = -1)
        {
            if ((!isLogedIn()))
            {
                return RedirectToAction("Login", "Auth");
            }
            if (_NewOption.Name == null || _NewOption.Price == null)
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
                }
                else
                {
                    return RedirectToAction("Index", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
                }
            }
            int count = new NewOptionBL().GetActiveNewOptionsList(db).Where(x => x.IsActive == 1 && x.Name == _NewOption.Name).Count();
            if (count > 0)
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "This Category Name in This Category Already Registered", color = "red" });
                }
                else
                {
                    return RedirectToAction("Index", new { message = "This Category Name in This Category Already Registered", color = "red" });
                }
            }

            NewOption NewOption = new NewOption()
            {
                Name = _NewOption.Name,
                Price = _NewOption.Price,
                IsActive = 1,
                CreatedAt = gp.CurrentDateTime(),
                CreatedBy = gp.validateUser().Id
            };
            if (new NewOptionBL().AddNewOption(NewOption, db))
            {
                if (PageId == 2)
                {
                    return RedirectToAction("NewInventory", "Inventory", new { message = "Record is Added Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("Index", new { message = "Record is Added Successfully", color = "green" });
                }
            }
            if (PageId == 2)
            {
                return RedirectToAction("NewInventory", "Inventory", new { message = "Record connot be added to database", color = "red" });
            }
            else
            {
                return RedirectToAction("Index", new { message = "Record connot be added to database", color = "red" });
            }
        }

        public ActionResult OptionById(int Id = -1)
        {
            NewOption NewOption = new NewOptionBL().GetActiveNewOptionById(Id, db);
            if (NewOption != null)
            {
                NewOption sST = new NewOption()
                {
                    Id = NewOption.Id,
                    Name = NewOption.Name,
                    Price = NewOption.Price,
                };
                return Json(sST, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostUpdate(NewOption _NewOption)
        {
            int count = new NewOptionBL().GetActiveNewOptionsList(db).Where(x => x.Name == _NewOption.Name && x.Id != _NewOption.Id).Count();
            if (count > 0)
            {
                return RedirectToAction("Index", new { message = "This Option is Already Registered", color = "red" });
            }
            NewOption obj = new NewOptionBL().GetActiveNewOptionById(_NewOption.Id, db);

            obj.Name = _NewOption.Name;
            obj.Price = _NewOption.Price;

            if (new NewOptionBL().UpdateNewOption(obj, db))
            {
                return RedirectToAction("Index", new { message = "Record is Updated Successfully", color = "green" });
            }

            return RedirectToAction("Index", new { message = "Something is Wrong", color = "red" });
        }

        public ActionResult Delete(int id, int PageId = -1, string InventoryId = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            if (new NewOptionBL().DeleteNewOption(id, db))
            {
                if (PageId == 2)
                {
                    return RedirectToAction("UpdateInventory", "Inventory", new { Id = InventoryId, message = "Record is Deleted Successfully", color = "green" });
                }
                else
                {
                    return RedirectToAction("Index", new { message = "Record is Deleted Successfully", color = "green" });
                }
            }
            else
            {
                if (PageId == 2)
                {
                    return RedirectToAction("UpdateInventory", "Inventory", new { Id = InventoryId, message = "Something is Wrong", color = "red" });
                }
                else
                {
                    return RedirectToAction("Index", new { message = "Something is Wrong", color = "red" });
                }
            }
        }
    }
}