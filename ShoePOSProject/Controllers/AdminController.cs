using OfficeOpenXml;
using ShoePOSProject.BL;
using ShoePOSProject.HelpingClasses;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShoePOSProject.Controllers
{
    [Validations(Role = 1)]
    public class AdminController : Controller
    {
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

        // GET: Admin
        #region ManageUsers
        public ActionResult AddUser(string message = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            else
            {
                if(color == "red")
                {
                    ViewBag.errormessage = message;
                    ViewBag.color = color;
                }
                else
                {
                    ViewBag.successfullmessage = message;
                    ViewBag.color = color;
                }
                return View();
            }
        }

        [HttpPost]
        public ActionResult PostAddUser(User newUser, HttpPostedFileBase Profile)
        {
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            double number = _rdm.Next(_min, _max);
            if ((!isLogedIn()))
            {
                return RedirectToAction("Login", "Auth");
            }
            if (newUser.Name == null || newUser.Email == null || newUser.Password == null)
            {
                return RedirectToAction("AddUser", new { message = "Fill all the fieldds first", color = "red" });
            }
            User u = new UserBL().GetActiveUsersList(db).Where(x => x.Email.ToLower() == newUser.Email.ToLower()).FirstOrDefault();
            if (u != null)
            {
                return RedirectToAction("AddUser", new { message = "Email already exist", color = "red" });
            }

            newUser.Password = StringCypher.Encrypt(newUser.Password);
            newUser.IsActive = 1;
            newUser.Role = newUser.Role;
            newUser.CreatedAt = gp.CurrentDateTime();
            newUser.CreatedBy = gp.validateUser().Id;
            newUser.UserActivation = Convert.ToString(number);

            if (Profile != null)
            {
                Directory.CreateDirectory(Server.MapPath("~") + "Content/UserPictures/");
                string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(Profile.FileName);
                string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(Profile.FileName));
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                {
                    int filesize;
                    filesize = Profile.ContentLength;
                    if (filesize >= 2000000)
                    {
                        return RedirectToAction("AddUser", new { message = "Your can Upload Maximum 2MB file", color = "red" });
                    }
                    else
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(Profile.InputStream);
                        int height = img.Height;
                        int width = img.Width;
                        //if (height != 70 && width != 200)
                        //{
                        //    return RedirectToAction("AddUser",new { message = "Width and Height of Images should be 200 x 70", color = "red" });
                        //}
                        filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + Profile.FileName;
                        string path = Path.Combine(Server.MapPath("~/Content/UserPictures"), filenamenoext);
                        // file is uploaded
                        Profile.SaveAs(path);
                        newUser.Profile = "/Content/UserPictures/" + filenamenoext;
                    }
                }
                else
                {
                    ViewBag.wrongFile = "Wrong file format!";
                    return View("AddUser", new { message = ViewBag.wrongFile, color = "red" });
                }
            }

            if (new UserBL().AddUser(newUser, db))
            {
                bool verify = MailSender.EmailVerify(newUser.UserActivation, newUser.Email, BaseUrl);
                return RedirectToAction("AddUser", new { message = "Record Added successfully", color = "green" });
            }
            return RedirectToAction("AddUser", new { message = "Record connot be added to database", color = "red" });
        }
        
        public ActionResult ViewUser(string message = "", string color = "black")
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
            if(color == "green")
            {
                ViewBag.successfullmessage = message;
                ViewBag.color = color;
            }

            return View();
        }
        [HttpPost]
        public ActionResult GetUserList(string name = "", string email = "", int AccessRole = -1)
        {
            List<User> ulist = new List<User>();

            ulist = new UserBL().GetActiveUsersList(db).OrderByDescending(x => x.Id).ToList();

            if (name != "")
            {
                ulist = ulist.Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
            }
            if (email != "")
            {
                ulist = ulist.Where(x => x.Email.ToLower().Contains(email.ToLower())).ToList();
            }
            if (AccessRole != -1)
            {
                ulist = ulist.Where(x => x.Role == AccessRole).ToList();
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
                    ulist = ulist.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
                else
                {
                    ulist = ulist.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                ulist = ulist.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) ||
                        x.Email.ToLower().Contains(searchValue.ToLower()) ||
                        (x.Address != null && x.Address.ToLower().Contains(searchValue.ToLower()))).ToList();
            }
            int totalrows = ulist.Count();
            int totalrowsafterfilterinig = ulist.Count();
            ulist = ulist.Skip(start).Take(length).ToList();

            List<UserDTO> udto = new List<UserDTO>();
            string Role = "";
            foreach (User u in ulist)
            {
                if(u.Role == 1)
                {
                    Role = "Admin";
                }
                else if(u.Role == 2)
                {
                    Role = "Dealer";
                }
                else
                {
                    Role = "Customer";
                }

                UserDTO obj = new UserDTO()
                {
                    Id = u.Id,
                    EncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                    Name = u.Name,
                    Phone = u.Phone,
                    Email = u.Email,
                    Address = u.Address,
                    Role = Role
                };

                udto.Add(obj);
            }
            return Json(new { data = udto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateUser(string UserId = "", string message = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var uId = StringCypher.Base64Decode(UserId);
            User u = new UserBL().GetActiveUsersList(db).Where(x => x.Id == Convert.ToInt32(uId)).FirstOrDefault();
            ViewBag.User = u;
            if(color == "red")
            {
                ViewBag.errormessage = message;
                ViewBag.color = color;
            }
            if(color == "green")
            {
                ViewBag.successfullmessage = message;
                ViewBag.color = color;
            }
            return View(u);
        }

        public ActionResult PostUpdateUser(User _user, HttpPostedFileBase Profile)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            if (new UserBL().GetActiveUsersList(db).Where(x => x.Email.ToLower() == _user.Email.ToLower() && x.Id != _user.Id).FirstOrDefault() != null)
            {

            }
            User u = new UserBL().GetActiveUserById(_user.Id, db);
            u.Name = _user.Name.Trim();
            u.Phone = _user.Phone;
            u.Address = _user.Address;

            if (Profile != null)
            {
                Directory.CreateDirectory(Server.MapPath("~") + "Content/UserPictures/");
                string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(Profile.FileName);
                string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(Profile.FileName));
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                {
                    int filesize;
                    filesize = Profile.ContentLength;
                    if (filesize >= 2000000)
                    {
                        return RedirectToAction("UpdateUser", new { UserId = StringCypher.Base64Encode(Convert.ToString(u.Id)), message = "Your can Upload Maximum 2MB file", color = "red" });
                    }
                    else
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(Profile.InputStream);
                        int height = img.Height;
                        int width = img.Width;
                        //if (height != 70 && width != 200)
                        //{
                        //    return RedirectToAction("UpdateUser", new { UserId = StringCypher.Base64Encode(Convert.ToString(u.Id)), message = "Width and Height of Image/Logo should be 200 x 70", color = "red" });
                        //}
                        filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + Profile.FileName;
                        string path = Path.Combine(Server.MapPath("~/Content/UserPictures"), filenamenoext);
                        // file is uploaded
                        Profile.SaveAs(path);
                        u.Profile = "/Content/UserPictures/" + filenamenoext;
                    }
                }
                else
                {
                    ViewBag.wrongFile = "Wrong file format!";
                    return View("UpdateUser", new { UserId = StringCypher.Base64Encode(Convert.ToString(u.Id)), message = ViewBag.wrongFile, color = "red" });
                }
            }

            bool chkUser = new UserBL().UpdateUser(u, db);

            if (chkUser)
            {
                return RedirectToAction("ViewUser", new { message = "User updated successfully", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdateUser", new { UserId = StringCypher.Base64Encode(Convert.ToString(u.Id)), message = "Somethings' wrong", color = "red" });
            }
        }
        [HttpPost]
        public ActionResult UserById(int id)
        {
            User user = new UserBL().GetActiveUserById(id, db);
            User obj = new User()
            {
                Id = user.Id,
                Name = user.Name,
                Phone = user.Phone,
                Address = user.Address,
                Email = user.Email
            };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteUser(int id)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            if (new UserBL().DeleteUser(id, db)
)
            {
                return RedirectToAction("ViewUser", new { message = "Record deleted successfully", color = "green" });
            }
            else
            {
                return RedirectToAction("ViewUser", new { message = "Somethings' wrong", color = "red" });
            }
        }
        #endregion

        #region Excel
        [HttpPost]
        public ActionResult PostImportExcel(FormCollection formCollection, string way = "")
        {
            try
            {
                DatabaseEntities dc = new DatabaseEntities();
                if (Request != null)
                {
                    HttpPostedFileBase file = Request.Files["PostedFile"];
                    if ((file != null && file.ContentLength > 0 && !string.IsNullOrEmpty(file.FileName)))
                    {
                        string fileName = file.FileName;
                        string fileContentType = file.ContentType;
                        byte[] filebytes = new byte[file.ContentLength];
                        var data = file.InputStream.Read(filebytes, 0, Convert.ToInt32(file.ContentLength));

                        using (var package = new ExcelPackage(file.InputStream))
                        {
                            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            var noOfColumns = workSheet.Dimension.End.Column;
                            var noOfRows = workSheet.Dimension.End.Row;

                            for (int rowIterator = 2; rowIterator <= noOfRows; rowIterator++)
                            {
                                var BrandId = 0;
                                var SizeId = 0;
                                var CollectionId = 0;
                                var ColorId = 0;
                                var GenderId = 0;
                                var ShoeStyleId = 0;
                                List<Inventory> listTask = new InventoryBL().GetActiveInventoriesList(db).ToList();
                                List<BSST> bsstList = new BsstBL().GetActiveBSSTsList(db).ToList();
                                if (workSheet.Cells[rowIterator, 1] == null)//can this be null?
                                {
                                    break; //if it's null exit from the for loop
                                }

                                var ShoeStyle = workSheet.Cells[rowIterator, 3].Value.ToString();
                                ShoeStyle = ShoeStyle.Trim(' ');
                                var Gender = workSheet.Cells[rowIterator, 4].Value.ToString();
                                Gender = Gender.Trim(' ');
                                var Color = workSheet.Cells[rowIterator, 6].Value.ToString();
                                Color = Color.Trim(' ');
                                var Size = workSheet.Cells[rowIterator, 7].Value.ToString();
                                Size = Size.Trim(' ');
                                var Brand = workSheet.Cells[rowIterator, 10].Value.ToString();
                                Brand = Brand.Trim(' ');
                                var Collection = workSheet.Cells[rowIterator, 11].Value.ToString();
                                Collection = Collection.Trim(' ');


                                var barCode = workSheet.Cells[rowIterator, 5].Value.ToString();
                                barCode = barCode.Trim(' ');

                                if (workSheet.Cells[rowIterator, 3] != null)
                                {
                                    BSST e = bsstList.Where(x => x.Name.Trim() == ShoeStyle && x.BsstCategoryId == 6).FirstOrDefault();
                                    if (e != null)
                                    {
                                        ShoeStyleId = e.Id;
                                    }
                                    else
                                    {
                                        var equip = new BSST();
                                        equip.Name = ShoeStyle;
                                        equip.IsActive = 1;
                                        equip.BsstCategoryId = 6;
                                        equip.CreatedAt = gp.CurrentDateTime();
                                        new BsstBL().AddBSST2(equip, db);
                                        ShoeStyleId = equip.Id;
                                    }
                                }
                                if (workSheet.Cells[rowIterator, 4] != null)
                                {
                                    BSST e = bsstList.Where(x => x.Name.Trim() == Gender && x.BsstCategoryId == 5).FirstOrDefault();
                                    if (e != null)
                                    {
                                        GenderId = e.Id;
                                    }
                                    else
                                    {
                                        var equip = new BSST();
                                        equip.Name = Gender;
                                        equip.IsActive = 1;
                                        equip.BsstCategoryId = 5;
                                        equip.CreatedAt = gp.CurrentDateTime();
                                        equip.CreatedBy = gp.validateUser().Id;
                                        new BsstBL().AddBSST2(equip, db);
                                        GenderId = equip.Id;
                                    }
                                }
                                if (workSheet.Cells[rowIterator, 6] != null)
                                {
                                    BSST e = bsstList.Where(x => x.Name.Trim() == Color && x.BsstCategoryId == 4).FirstOrDefault();
                                    if (e != null)
                                    {
                                        ColorId = e.Id;
                                    }
                                    else
                                    {
                                        var equip = new BSST();
                                        equip.Name = Color;
                                        equip.IsActive = 1;
                                        equip.BsstCategoryId = 4;
                                        equip.CreatedAt = gp.CurrentDateTime();
                                        equip.CreatedBy = gp.validateUser().Id;
                                        new BsstBL().AddBSST2(equip, db);
                                        ColorId = equip.Id;
                                    }
                                }
                                if (workSheet.Cells[rowIterator, 7] != null)
                                {
                                    BSST e = bsstList.Where(x => x.Name.Trim() == Size && x.BsstCategoryId == 2).FirstOrDefault();
                                    if (e != null)
                                    {
                                        SizeId = e.Id;
                                    }
                                    else
                                    {
                                        var equip = new BSST();
                                        equip.Name = Size;
                                        equip.IsActive = 1;
                                        equip.BsstCategoryId = 2;
                                        equip.CreatedAt = gp.CurrentDateTime();
                                        equip.CreatedBy = gp.validateUser().Id;
                                        new BsstBL().AddBSST2(equip, db);
                                        SizeId = equip.Id;
                                    }
                                }
                                if (workSheet.Cells[rowIterator, 10] != null)
                                {
                                    BSST e = bsstList.Where(x => x.Name.Trim() == Brand && x.BsstCategoryId == 1).FirstOrDefault();
                                    if (e != null)
                                    {
                                        BrandId = e.Id;
                                    }
                                    else
                                    {
                                        var equip = new BSST();
                                        equip.Name = Brand;
                                        equip.IsActive = 1;
                                        equip.BsstCategoryId = 1;
                                        equip.CreatedAt = gp.CurrentDateTime();
                                        equip.CreatedBy = gp.validateUser().Id;
                                        new BsstBL().AddBSST2(equip, db);
                                        BrandId = equip.Id;
                                    }
                                }
                                if (workSheet.Cells[rowIterator, 11] != null)
                                {
                                    BSST e = bsstList.Where(x => x.Name.Trim() == Collection && x.BsstCategoryId == 3).FirstOrDefault();
                                    if (e != null)
                                    {
                                        CollectionId = e.Id;
                                    }
                                    else
                                    {
                                        var equip = new BSST();
                                        equip.Name = Collection;
                                        equip.IsActive = 1;
                                        equip.BsstCategoryId = 3;
                                        equip.CreatedAt = gp.CurrentDateTime();
                                        equip.CreatedBy = gp.validateUser().Id;
                                        new BsstBL().AddBSST2(equip, db);
                                        CollectionId = equip.Id;
                                    }
                                }
                                if (workSheet.Cells[rowIterator, 5] != null)
                                {
                                    var BarCodeNumber = barCode + Color + Size;
                                    Inventory v = listTask.Where(x => x.BarcodeNo.ToLower().Trim() == BarCodeNumber.ToLower()).FirstOrDefault();

                                    if (v == null)
                                    {
                                        var claim = new Inventory();

                                        claim.BarcodeNo = BarCodeNumber;
                                        claim.BrandId = BrandId;
                                        claim.SizeId = SizeId;
                                        claim.CollectionId = CollectionId;
                                        claim.ColorId = ColorId;
                                        claim.GenderId = GenderId;
                                        claim.ShoeStyleId = ShoeStyleId;
                                        claim.extra1 = workSheet.Cells[rowIterator, 8].Value == null ? string.Empty : workSheet.Cells[rowIterator, 8].Value.ToString();
                                        claim.InventoryDate = gp.CurrentDateTime();
                                        claim.IsActive = 1;
                                        claim.CreatedAt = DateTime.Now;
                                        claim.CreatedBy = gp.validateUser().Id;

                                        if (!new InventoryBL().AddInventory(claim, db))
                                        {
                                            return RedirectToAction("ViewInventory", "Inventory", new { msg = "Something went Wrong", color = "red" });
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return RedirectToAction("ViewInventory", "Inventory", new { msg = "File Uploaded Successfully", color = "green" });
                }

            }
            catch (Exception ex)
            {

                if (ex.InnerException != null)
                {
                    MailSender.SendErrorEmail(ex.InnerException.Message.ToString() + "_" + ex.Message);
                }
                else
                {
                    MailSender.SendErrorEmail(ex.Message.ToString());
                }
            }
            return RedirectToAction("ViewInventory", "Inventory", new { msg = "Something went Wrong", color = "red" });
        }

        #endregion
    }
}