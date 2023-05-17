using ShoePOSProject.BL;
using ShoePOSProject.HelpingClasses;
using ShoePOSProject.Models;
using Spire.Pdf.Exporting.XPS.Schema;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Path = System.IO.Path;

namespace ShoePOSProject.Controllers
{
    [Validations]
    public class InventoryController : Controller
    {
        // GET: Inventory
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
        
        public ActionResult ViewInventory(string message = "", string color = "")
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
            return View();
        }

        [HttpPost]
        public ActionResult GetInventoryList(string Serial = "", int Style = -1,
            int Brand = -1, int Color = -1, int Collection = -1, int Gender = -1, int Size = -1,
            string Price = "", string SalePrice = "", string Date = "", int Discount = -1,
            string Custom = "", string StartDate = "", string EndDate = "")
        {
            List<Inventory> IList = new List<Inventory>();
            DateTime currentDateTime = gp.CurrentDateTime().Date;
            bool isSunday = currentDateTime.DayOfWeek == 0;
            var dayOfweek = isSunday == false ? (int)currentDateTime.DayOfWeek : 7;

            DateTime Today = currentDateTime;
            DateTime yesterday = currentDateTime.AddDays(-1);
            DateTime CurrentWeekStartDate = currentDateTime.AddDays(((int)dayOfweek * -1) + 1);
            DateTime CurrentWeekEndDate = CurrentWeekStartDate.AddDays(7).AddSeconds(-1);
            DateTime LastWeekStartDate = CurrentWeekStartDate.AddDays(-7);
            DateTime LastWeekEndDate = CurrentWeekStartDate.AddSeconds(-1);
            DateTime thisMonthStart = currentDateTime.AddDays(1 - currentDateTime.Day);
            DateTime thisMonthEnd = thisMonthStart.AddMonths(1).AddSeconds(-1);
            DateTime lastMonthStart = thisMonthStart.AddMonths(-1);
            DateTime lastMonthEnd = thisMonthStart.AddSeconds(-1);

            IList = new InventoryBL().GetActiveInventoriesList(db).OrderByDescending(x => x.Id).ToList();
            

            if (Custom == "Yesterday")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date == Convert.ToDateTime(yesterday).Date).ToList();
            }
            if (Custom == "Today")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date == Convert.ToDateTime(Today).Date).ToList();
            }
            if (Custom == "CurrentWeek")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date >= Convert.ToDateTime(CurrentWeekStartDate).Date &&
                Convert.ToDateTime(x.InventoryDate).Date <= Convert.ToDateTime(CurrentWeekEndDate).Date).ToList();
            }
            if (Custom == "LastWeek")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date >= Convert.ToDateTime(LastWeekStartDate).Date &&
                Convert.ToDateTime(x.InventoryDate).Date <= Convert.ToDateTime(LastWeekEndDate).Date).ToList();
            }
            if (Custom == "CurrentMonth")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date >= Convert.ToDateTime(thisMonthStart).Date &&
                Convert.ToDateTime(x.InventoryDate).Date <= Convert.ToDateTime(thisMonthEnd).Date).ToList();
            }
            if (Custom == "LastMonth")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date >= Convert.ToDateTime(lastMonthStart).Date &&
                Convert.ToDateTime(x.InventoryDate).Date <= Convert.ToDateTime(lastMonthEnd).Date).ToList();
            }
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date >= Convert.ToDateTime(StartDate).Date &&
                Convert.ToDateTime(x.InventoryDate).Date <= Convert.ToDateTime(EndDate).Date).ToList();
            }

            if (Serial != "")
            {
                IList = IList.Where(x => x.BarcodeNo.ToLower().Contains(Serial.ToLower())).ToList();
            }
            if (Brand != -1)
            {
                IList = IList.Where(x => x.BrandId == Brand).ToList();
            }
            if (Size != -1)
            {
                IList = IList.Where(x => x.SizeId == Size).ToList();
            }
            if (Collection != -1)
            {
                IList = IList.Where(x => x.CollectionId == Collection).ToList();
            }
            if (Color != -1)
            {
                IList = IList.Where(x => x.ColorId == Color).ToList();
            }
            if (Gender != -1)
            {
                IList = IList.Where(x => x.GenderId == Gender).ToList();
            }
            if (Style != -1)
            {
                IList = IList.Where(x => x.ShoeStyleId == Style).ToList();
            }
            if (Price != "")
            {
                IList = IList.Where(x => x.Price.ToLower().Contains(Price.ToLower())).ToList();
            }
            if (SalePrice != "")
            {
                IList = IList.Where(x => x.SalePrice.ToLower().Contains(SalePrice.ToLower())).ToList();
            }
            if (Date != "")
            {
                IList = IList.Where(x => Convert.ToDateTime(x.InventoryDate).Date == Convert.ToDateTime(Date).Date).ToList();
            }
            if (Discount != -1)
            {
                UpdateDiscount(IList, Discount);
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
                    IList = IList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
                else
                {
                    IList = IList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                IList = IList.Where(x => x.extra1 != null && x.extra1.ToLower().Contains(searchValue.ToLower()) ||
                        x.extra2 != null && x.extra2.ToLower().Contains(searchValue.ToLower())
                        ).ToList();
            }
            int totalrows = IList.Count();
            int totalrowsafterfilterinig = IList.Count();
            IList = IList.Skip(start).Take(length).ToList();

            List<DTO> dto = new List<DTO>();
            string price = "", Models = "", salePrice = "";
            int Daysonlot = 0;
            var TotalPrice = 0.0;
            var TotalSalePrice = 0.0;
            
            foreach (Inventory u in IList)
            {
                TotalPrice = TotalPrice + Convert.ToInt32(u.Price);
                TotalSalePrice = TotalSalePrice + Convert.ToInt32(u.SalePrice);
                if (u.Price != null)
                {
                    price = Convert.ToDecimal(u.Price).ToString("Rs.");
                }
                else
                {
                    price = "";
                }
                if (u.SalePrice != null)
                {
                    salePrice = Convert.ToDecimal(u.SalePrice).ToString("Rs.");
                }
                else
                {
                    salePrice = "";
                }
                if(u.InventoryDate != null)
                {
                    DateTime date = Convert.ToDateTime(u.InventoryDate);
                    Daysonlot = (int)(currentDateTime - date).TotalDays;
                }
                DTO obj = new DTO()
                {
                    Id = u.Id,
                    EncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                    SerialNumber = u.BarcodeNo,
                    InventoryDate = u.InventoryDate != null ? u.InventoryDate.Value.ToString("d") : "",
                    DaysofLot = u.InventoryDate != null ? Convert.ToString(Daysonlot) : "",
                    Address = u.AvailableAt,
                    Price = price,
                    SalePrice = salePrice,
                    Role = gp.validateUser().Role,
                    TotalPrice = TotalPrice,
                    TotalSalePrice = TotalSalePrice,
                    Discount = u.Discount,
                    Quantity = u.extra1
                };
                if(u.BrandId != null)
                {
                    obj.Brand = u.BSST.Name;
                }
                if(u.SizeId != null)
                {
                    obj.Size = u.BSST5.Name;
                }
                if(u.CollectionId != null)
                {
                    obj.Collection = u.BSST1.Name;
                }
                if (u.ColorId != null)
                {
                    obj.Color = u.BSST2.Name;
                }
                if (u.GenderId != null)
                {
                    obj.Gender = u.BSST3.Name;
                }
                if (u.ShoeStyleId != null)
                {
                    obj.ShoeStyle = u.BSST4.Name;
                }

                dto.Add(obj);
            }
            return Json(new { data = dto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewInventory(string message = "", string color = "")
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

        public ActionResult PostNewInventory(Inventory inventory, FormCollection fc, IEnumerable<HttpPostedFileBase> Images, int count = -1, string SignedImageUrl = "")
        {
            if ((!isLogedIn()))
            {
                return RedirectToAction("Login", "Auth");
            }
            string SignaturePath = "";
            string test = "";
            var list = new InventoryBL().GetActiveInventoriesList(db).Where(x => x.BarcodeNo == inventory.BarcodeNo).ToList();
            if(list.Count > 0)
            {
                return RedirectToAction("NewInventory", new { message = "This Serial Number already Exists"});
            }
            if (SignedImageUrl != "")
            {
                byte[] imageBytes = Convert.FromBase64String(SignedImageUrl.Replace("data:image/png;base64,", String.Empty));
                Image image;
                using (MemoryStream ms = new MemoryStream(imageBytes))
                {
                    Directory.CreateDirectory(Server.MapPath("~") + "Content/BarnDiagram/");
                    image = Image.FromStream(ms);
                    string fileName = "BarnDiagram-" + DateTime.Now.ToString("yymmddfff") + ".png";
                    test = "../Content/BarnDiagram/" + fileName;
                    SignaturePath = Path.Combine(Server.MapPath("/Content/BarnDiagram/"), fileName);
                    // file is uploaded
                    image.Save(SignaturePath, System.Drawing.Imaging.ImageFormat.Png);
                    // releasing resources
                    image.Dispose();
                }
            }

            if (Images != null)
            {
                foreach (var files in Images)
                {
                    if (files != null)
                    {
                        string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(files.FileName));
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                        {
                            int filesize;
                            filesize = files.ContentLength;
                            if (filesize > 2000000)
                            {
                                return RedirectToAction("NewInventory", new { message = "You can Upload Maximum 2MB file", color = "red" });
                            }
                        }
                        else
                        {
                            return RedirectToAction("NewInventory", new { message = "Images are only acceptables in PNG, JPG and JPEG" });
                        }
                    }
                }
            }

            using (var context = new DatabaseEntities())
            {
                using(DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Inventory i = new Inventory()
                        {
                            BrandId = inventory.BrandId,
                            SizeId = inventory.SizeId,
                            CollectionId = inventory.CollectionId,
                            ColorId = inventory.ColorId,
                            ShoeStyleId = inventory.ShoeStyleId,
                            GenderId = inventory.GenderId,
                            Price = inventory.Price,
                            SalePrice = inventory.SalePrice, 
                            Discount = inventory.Discount,
                            AvailableAt = inventory.AvailableAt,
                            InventoryDate = inventory.InventoryDate,
                            extra1 = inventory.extra1, //Serial/Bar Code Number
                            extra2 = test,
                            extra3 = inventory.extra3,
                            IsActive = 1,
                            CreatedBy = gp.validateUser().Id,
                            CreatedAt = gp.CurrentDateTime()
                        };
                        var getColor = "";
                        var getSize = "";
                        if (i.ColorId != null)
                        {
                            getColor = new BsstBL().GetActiveBSSTById((int)i.ColorId, db).Name;
                        }
                        if(i.SizeId != null)
                        {
                            getSize = new BsstBL().GetActiveBSSTById((int)i.SizeId, db).Name;
                        }
                        i.BarcodeNo = inventory.BarcodeNo + getColor + getSize;
                        if (new InventoryBL().AddInventory(i, context))
                        {
                            if (Images != null)
                            {
                                Inventory inv = new InventoryBL().GetActiveInventoryById(i.Id, context);
                                foreach (var file in Images)
                                {
                                    if (file != null)
                                    {
                                        string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                                        string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(file.FileName));
                                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                                        {
                                            Directory.CreateDirectory(Server.MapPath("~") + "Content/Inventory/");
                                            filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + file.FileName;
                                            string path = Path.Combine(Server.MapPath("~/Content/Inventory"), filenamenoext);

                                            // file is uploaded
                                            file.SaveAs(path);

                                            InventoryImage IImage = new InventoryImage()
                                            {
                                                ImageName = file.FileName,
                                                InventoryId = inv.Id,
                                                IsActive = 1,
                                                CreatedAt = DateTime.Now
                                            };

                                            IImage.ImagePath = "/Content/Inventory/" + filenamenoext;

                                            bool c = new InventoryImageBL().AddInventoryImage(IImage, context);
                                            if (c == false)
                                            {
                                                throw new Exception();
                                            }

                                        }
                                        else
                                        {
                                            ViewBag.wrongFile = "Wrong File ormat!";
                                            return View("NewInventory", new { ViewBag.wrongFile, color = "red" });
                                        }
                                    }
                                }
                            }
                            if (count > 0)
                            {
                                for (int z = 1; z <= count; z++)
                                {
                                    NewOption optionInventory = new NewOption()
                                    {
                                        Items = Convert.ToInt32(fc["Items" + z]),
                                        Name = fc["Name" + z],
                                        Price = fc["Price" + z],
                                        IsActive = 1,
                                        InvId = i.Id,
                                        CreatedAt = gp.CurrentDateTime(),
                                        CreatedBy = gp.validateUser().Id,
                                    };
                                    bool c = new NewOptionBL().AddNewOption(optionInventory, context);
                                    if (c == false)
                                    {
                                        throw new Exception();
                                    }
                                }
                            }
                            transaction.Commit();
                            return RedirectToAction("NewInventory", new { message = "Record Added successfully", color = "green" });
                        }
                        else
                        {
                            throw new Exception();
                        }
                    }
                    catch(Exception e)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return RedirectToAction("NewInventory", new { message = "Record connot be added to database", color = "red" });
        }

        public ActionResult UpdateInventory(string Id = "", string message = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var InvId = StringCypher.Base64Decode(Id);
            Inventory i = new InventoryBL().GetActiveInventoriesList(db).Where(x => x.Id == Convert.ToInt32(InvId)).FirstOrDefault();
            List<InventoryImage> inventoryImage = new InventoryImageBL().GetActiveInventoriesList(db).Where(y => y.InventoryId == i.Id).ToList();
            List<NewOption> newOptions = new NewOptionBL().GetActiveNewOptionsList(db).Where(x => x.InvId == i.Id).ToList();
            List<BSST> Brand = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 1).ToList();
            List<BSST> ShoeStyle = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 6).ToList();
            List<BSST> Collection = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 3).ToList();
            List<BSST> Size = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 2).ToList();
            List<BSST> ShoeColor = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 4).ToList();
            List<BSST> Gender = new BsstBL().GetActiveBSSTsList(db).Where(x => x.BsstCategoryId == 5).ToList();
            ViewBag.Brand = Brand;
            ViewBag.ShoeStyle = ShoeStyle;
            ViewBag.Size = Size;
            ViewBag.ShoeColor = ShoeColor;
            ViewBag.Gender = Gender;
            ViewBag.Collection = Collection;
            ViewBag.IImage = inventoryImage;
            ViewBag.Inventory = i;
            ViewBag.Options = newOptions;
            ViewBag.count = 1;
            ViewBag.EncryptedId = Id;
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
            return View(i);
        }

        public ActionResult PostUpdateInventory(Inventory inventory, FormCollection fc, IEnumerable<HttpPostedFileBase> Images, int count = -1)
        {
            Inventory getData = new InventoryBL().GetActiveInventoryById(inventory.Id, db);

            if (Images != null)
            {
                foreach (var files in Images)
                {
                    if (files != null)
                    {
                        string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(files.FileName));
                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                        {
                            int filesize;
                            filesize = files.ContentLength;
                            if (filesize > 2000000)
                            {
                                return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(getData.Id)), message = "You can Upload Maximum 2MB file", color = "red" });
                            }
                        }
                        else
                        {
                            return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(getData.Id)), message = "Images are only acceptables in PNG, JPG and JPEG" });
                        }
                    }
                }
            }

            using (var context = db)
            {
                using(DbContextTransaction transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        getData.BarcodeNo = inventory.BarcodeNo;
                        getData.BrandId = inventory.BrandId;
                        getData.SizeId = inventory.SizeId;
                        getData.GenderId = inventory.GenderId;
                        getData.CollectionId = inventory.CollectionId;
                        getData.ShoeStyleId = inventory.ShoeStyleId;
                        getData.ColorId = inventory.ColorId; 
                        getData.AvailableAt = inventory.AvailableAt; 
                        getData.Price = inventory.Price;
                        getData.SalePrice = inventory.SalePrice;
                        getData.extra1 = inventory.extra1;
                        getData.extra2 = inventory.extra2;
                        getData.extra3 = inventory.extra3;
                        getData.InventoryDate = inventory.InventoryDate;

                        bool check = new InventoryBL().UpdateInventory(getData, context);
                        if (Images != null)
                        {
                            if (check == true)
                            {
                                Inventory inv = new InventoryBL().GetActiveInventoryById(getData.Id, context);
                                foreach (var file in Images)
                                {
                                    if (file != null)
                                    {
                                        string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                                        string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(file.FileName));
                                        if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                                        {
                                            Directory.CreateDirectory(Server.MapPath("~") + "Content/Inventory/");
                                            filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + file.FileName;
                                            string path = Path.Combine(Server.MapPath("~/Content/Inventory"), filenamenoext);

                                            // file is uploaded
                                            file.SaveAs(path);

                                            InventoryImage IImage = new InventoryImage()
                                            {
                                                ImageName = file.FileName,
                                                InventoryId = inv.Id,
                                                IsActive = 1,
                                                CreatedAt = DateTime.Now
                                            };

                                            IImage.ImagePath = "/Content/Inventory/" + filenamenoext;

                                            bool c = new InventoryImageBL().AddInventoryImage(IImage, context);
                                            if (c == false)
                                            {
                                                throw new Exception();
                                            }

                                        }
                                        else
                                        {
                                            ViewBag.wrongFile = "Wrong File ormat!";
                                            return View("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(getData.Id)), ViewBag.wrongFile, color = "red" });
                                        }
                                    }
                                }
                                if (count > 0)
                                {
                                    for (int z = 1; z <= count; z++)
                                    {
                                        string fc_id = fc["Id-" + z];
                                        if (fc_id != null)
                                        {
                                            string id = fc_id.Split('-')[0];
                                            NewOption newOption = new NewOptionBL().GetActiveNewOptionById(Convert.ToInt32(id), db);
                                            newOption.Items = Convert.ToInt32(fc["Items" + z]);
                                            newOption.Name = fc["Name" + z];
                                            newOption.Price = fc["Price" + z];
                                            bool c = new NewOptionBL().UpdateNewOption(newOption, context);
                                            if (c == false)
                                            {
                                                throw new Exception();
                                            }
                                        }
                                        else
                                        {
                                            NewOption optionInventory = new NewOption()
                                            {
                                                Items = Convert.ToInt32(fc["Items" + z]),
                                                Name = fc["Name" + z],
                                                Price = fc["Price" + z],
                                                IsActive = 1,
                                                InvId = inv.Id,
                                                CreatedAt = gp.CurrentDateTime(),
                                                CreatedBy = gp.validateUser().Id,
                                            };
                                            bool c = new NewOptionBL().AddNewOption(optionInventory, context);
                                            if (c == false)
                                            {
                                                throw new Exception();
                                            }
                                        }

                                    }
                                }
                                transaction.Commit();
                            }

                            if (!new InventoryBL().UpdateInventory(getData, context))
                            {
                                return RedirectToAction("ViewInventory", new { message = "Something Went Wrong", color = "green" });
                            }
                        }
                        else
                        {
                            transaction.Commit();
                        }
                        return RedirectToAction("ViewInventory", new { message = "Record Updated Successfully", color = "green" });
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }
                }
            }
            return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(getData.Id)), message = "Something's Wrong", color = "red" });
        }

        public ActionResult DeleteInventory(int id)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            //int count = new CustomerSalesBL().GetActiveCustomerSalesList(db).Where(x => x.InventoryId == id).Count();
            //if(count >= 1)
            //{
            //    List<CustomerSale> customerSales = new CustomerSalesBL().GetActiveCustomerSalesList(db).Where(x => x.InventoryId == id).ToList();
            //    foreach(CustomerSale sale in customerSales)
            //    {
            //        if (!new CustomerSalesBL().DeleteCustomerSale(sale.Id, db))
            //        {
            //            return RedirectToAction("ViewInventory", new { message = "Something is Wrong", color = "red" });
            //        }
            //    }

            //}
            if (new InventoryBL().DeleteInventory(id, db))
            {
                return RedirectToAction("ViewInventory", new { message = "Record deleted successfully", color = "green" });
            }
            else
            {
                return RedirectToAction("ViewInventory", new { message = "Something is Wrong", color = "red" });
            }
        }

        #region ManageImagesOnly
        public ActionResult UpdateImage(HttpPostedFileBase Image, int UpdateImageId = -1)
        {
            InventoryImage VP = new InventoryImageBL().GetActiveInventoryImageById(UpdateImageId, db);
            
            if (Image != null)
            {
                string filenamenoext = System.IO.Path.GetFileNameWithoutExtension(Image.FileName);
                string ext = System.IO.Path.GetExtension(System.IO.Path.GetFileName(Image.FileName));
                if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".JPG" || ext == ".JPEG" || ext == ".PNG")
                {
                    int filesize;
                    filesize = Image.ContentLength;
                    if (filesize < 2000000)
                    {
                        filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + Image.FileName;
                        string path = Path.Combine(Server.MapPath("~/Content/Inventory/"), filenamenoext);
                        // file is uploaded
                        Image.SaveAs(path);
                        VP.ImageName = filenamenoext;
                        VP.ImagePath = "/Content/Inventory/" + filenamenoext;

                        if (new InventoryImageBL().UpdateInventoryImage(VP, db))
                        {
                            return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Image is Updated Successfully", color = "green" });
                        }
                        else
                        {
                            return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Image is Not Updated", color = "red" });
                        }
                    }
                    else
                    {
                        return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Your Images Should be Less than 2MB", color = "red" });
                    }
                }
                else
                {
                    return View("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Your Images Should be in JPG, PNG and JPEG Format", color = "red" });
                }
            }
            else
            {
                return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Sorry You Can't Upload Image", color = "red" });
            }

        }

        public ActionResult DeleteImages(int ImageId)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            InventoryImage VP = new InventoryImageBL().GetActiveInventoryImageById(ImageId, db);
            if (new InventoryImageBL().DeleteInventoryImage(ImageId, db))
            {
                return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Image deleted successfully", color = "green" });
            }
            else
            {
                return RedirectToAction("UpdateInventory", new { Id = StringCypher.Base64Encode(Convert.ToString(VP.InventoryId)), message = "Something's wrong", color = "red" });
            }
        }
        #endregion

        public ActionResult Details(string Id = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }

            var InvId = StringCypher.Base64Decode(Id);
            Inventory i = new InventoryBL().GetActiveInventoriesList(db).Where(x => x.Id == Convert.ToInt32(InvId)).FirstOrDefault();
            List<InventoryImage> inventoryImage = new InventoryImageBL().GetActiveInventoriesList(db).Where(y => y.InventoryId == i.Id).ToList();
            List<NewOption> newOptions = new NewOptionBL().GetActiveNewOptionsList(db).Where(x => x.InvId == i.Id).ToList();
            ViewBag.IImage = inventoryImage;
            ViewBag.Inventory = i;
            ViewBag.Options = newOptions;
            ViewBag.Url = gp.GetBaseUrl();
            return View(i);
        }

        public ActionResult Ledger()
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> getBsst(int categoryName = -1)
        {
            List<BSST> list = new BsstBL().GetActiveBSSTsList(db).OrderByDescending(x => x.Id).ToList();
            
                if (categoryName == 1)
                {
                    list = list.Where(x => x.BsstCategoryId == 1).ToList();
                }
                if (categoryName == 2)
                {
                    list = list.Where(x => x.BsstCategoryId == 2).ToList();
                }
                if (categoryName == 3)
                {
                    list = list.Where(x => x.BsstCategoryId == 3).ToList();
                }
                if (categoryName == 5)
                {
                    list = list.Where(x => x.BsstCategoryId == 5).ToList();
                }
                if (categoryName == 6)
                {
                    list = list.Where(x => x.BsstCategoryId == 6).ToList();
                }
                if (categoryName == 4)
                {
                    list = list.Where(x => x.BsstCategoryId == 4).ToList();
                }

            List<DTO> bsstDto = new List<DTO>();
            foreach(var item in list)
            {
                DTO obj = new DTO()
                {
                    Id = item.Id,
                    EncryptedId = StringCypher.Base64Encode(item.Id.ToString()),
                    BSSTName = item.Name,
                    BSSTCategory = item.Category,
                    BSSTCategoryId = item.BsstCategoryId,
                };
                bsstDto.Add(obj);
            }

            return Json(bsstDto, JsonRequestBehavior.AllowGet);
        } 

        public ActionResult UploadExcel(string message = "", string color = "")
        {
            ViewBag.Message = message;
            ViewBag.Color = color;
            return View();
        }

        public bool UpdateDiscount(List<Inventory> inventories, int Discount = -1)
        {
            if(inventories.Count > 0)
            {
                foreach(var item in inventories)
                {
                    item.Discount = Discount.ToString();
                    if(!new InventoryBL().UpdateInventory(item, db)){
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public ActionResult GetInventory(int InventoryId = -1)
        {
            List<Inventory> IList = new List<Inventory>();
            IList = new InventoryBL().GetActiveInventoriesList(db).OrderByDescending(x => x.Id).ToList();
            
            if(InventoryId != -1)
            {
                IList = IList.Where(x => x.Id ==  InventoryId).ToList();
            }
            List<DTO> dto = new List<DTO>();
            string price = "", Models = "", salePrice = "";
            int Daysonlot = 0;
            var TotalPrice = 0.0;
            var TotalSalePrice = 0.0;

            foreach (Inventory u in IList)
            {
                TotalPrice = TotalPrice + Convert.ToInt32(u.Price);
                TotalSalePrice = TotalSalePrice + Convert.ToInt32(u.SalePrice);
                if (u.Price != null)
                {
                    price = u.Price;
                }
                if (u.SalePrice != null)
                {
                    salePrice = u.SalePrice;
                }
                if (u.InventoryDate != null)
                {
                    DateTime dt = DateTime.Now;
                    DateTime date = Convert.ToDateTime(u.InventoryDate);
                    Daysonlot = (int)(dt - date).TotalDays;
                }
                DTO obj = new DTO()
                {
                    Id = u.Id,
                    EncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                    SerialNumber = u.BarcodeNo,
                    InventoryDate = u.InventoryDate != null ? u.InventoryDate.Value.ToString("d") : "",
                    DaysofLot = u.InventoryDate != null ? Convert.ToString(Daysonlot) : "",
                    Address = u.AvailableAt,
                    Price = price,
                    SalePrice = salePrice,
                    Role = gp.validateUser().Role,
                    TotalPrice = TotalPrice,
                    TotalSalePrice = TotalSalePrice,
                    Discount = u.Discount,
                    Quantity = u.extra1
                };
                if (u.BrandId != null)
                {
                    obj.Brand = u.BSST.Name;
                }
                if (u.SizeId != null)
                {
                    obj.Size = u.BSST5.Name;
                }
                if (u.CollectionId != null)
                {
                    obj.Collection = u.BSST1.Name;
                }
                if (u.ColorId != null)
                {
                    obj.Color = u.BSST2.Name;
                }
                if (u.GenderId != null)
                {
                    obj.Gender = u.BSST3.Name;
                }
                if (u.ShoeStyleId != null)
                {
                    obj.ShoeStyle = u.BSST4.Name;
                }

                dto.Add(obj);
            }
            return Json(dto, JsonRequestBehavior.AllowGet);
        }
    }
}