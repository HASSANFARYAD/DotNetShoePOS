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
    public class BsstController : Controller
    {
        DatabaseEntities db = new DatabaseEntities();
        GeneralPurpose gp = new GeneralPurpose();
        MailSender ms = new MailSender();
        User LoggedInUser = new User();
        private bool isLogedIn()
        {
            LoggedInUser = gp.validateUser();
            if (LoggedInUser != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // GET: Bsst
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
            ViewBag.UserRole = LoggedInUser.Role;
            ViewBag.UserId = LoggedInUser.Id;
            return View();
        }

        [HttpPost]
        public ActionResult GetBSSTList(int CategoryName = -1, string Style = "")
        {
            List<BSST> list = new List<BSST>();

                if(CategoryName == 1)
                {
                    list = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 1).OrderByDescending(x => x.Id).ToList();
                }
                if (CategoryName == 2)
                {
                    list = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 2).OrderByDescending(x => x.Id).ToList();
                }
                if (CategoryName == 3)
                {
                    list = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 3).OrderByDescending(x => x.Id).ToList();
                }
                if (CategoryName == 4)
                {
                    list = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 4).OrderByDescending(x => x.Id).ToList();
                }
                if (CategoryName == 5)
                {
                    list = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 5).OrderByDescending(x => x.Id).ToList();
                }
                if (CategoryName == 6)
                {
                    list = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 6).OrderByDescending(x => x.Id).ToList();
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
            foreach (BSST u in list)
            {
                DTO obj = new DTO()
                {
                    BSSTId = u.Id,
                    BSSTEncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                    BSSTName = u.Name,
                    BSSTCategory = u.Category,
                    Role = gp.validateUser().Role,
                    CreatedBy = u.CreatedBy.ToString()
                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostAddBsst(BSST _bsst, int PageId = -1)
        {
            if ((!isLogedIn()))
            {
                return RedirectToAction("Login", "Auth");
            }
            if (_bsst.Name == null || _bsst.BsstCategoryId == null)
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
            int count = new BsstBL().GetActiveBSSTsList(db).Where(x => x.IsActive == 1 && x.Name == _bsst.Name && x.BsstCategoryId == _bsst.BsstCategoryId).Count();
            if(count > 0)
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

            BSST bSST = new BSST()
            {
                Name = _bsst.Name,
                Category = _bsst.Category,
                BsstCategoryId = _bsst.BsstCategoryId,
                IsActive = 1,
                CreatedAt = gp.CurrentDateTime(),
                CreatedBy = gp.validateUser().Id
            };
            if(new BsstBL().AddBSST(bSST, db))
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

        public ActionResult CategoryById(int Id = -1)
        {
            BSST bSST = new BsstBL().GetActiveBSSTById(Id, db);
            if(bSST != null)
            {
                BSST sST = new BSST()
                {
                    Id = bSST.Id,
                    Name = bSST.Name,
                    Category = bSST.Category,
                    BsstCategoryId = bSST.BsstCategoryId
                };
                return Json(sST, JsonRequestBehavior.AllowGet);
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult PostUpdate(BSST _bSST)
        {
            int count = new BsstBL().GetActiveBSSTsList(db).Where(x => x.Name == _bSST.Name && x.Category == _bSST.Category && x.Id != _bSST.Id).Count();
            if(count > 0)
            {
                return RedirectToAction("Index", new { message = "This Category Name in This Category Already Registered", color = "red" });
            }
            BSST obj = new BsstBL().GetActiveBSSTById(_bSST.Id, db);

            obj.Name = _bSST.Name;
            obj.Category = _bSST.Category;
            obj.BsstCategoryId = _bSST.BsstCategoryId;

            if (new BsstBL().UpdateBSST(obj, db))
            {
                return RedirectToAction("Index", new { message = "Record is Updated Successfully", color = "green" });
            }

            return RedirectToAction("Index", new { message = "Something is Wrong", color = "red" });
        }

        public ActionResult Delete(int id)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            if (new BsstBL().DeleteBSST(id, db)
)
            {
                return RedirectToAction("Index", new { message = "Record Deleted Successfully", color = "green" });
            }
            else
            {
                return RedirectToAction("Index", new { message = "Something is Wrong", color = "red" });
            }
        }
    }
}