using Spire.Pdf;
using Spire.Pdf.HtmlConverter;
using ShoePOSProject.BL;
using ShoePOSProject.HelpingClasses;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ShoePOSProject.Controllers
{
    public class CustomerSalesController : Controller
    {
        // GET: CustomerSalesSales
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
        // GET: CustomerSales
        public ActionResult NewCustomerSales(string message = "", string color = "")
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
            List<Customer> c = new List<Customer>();
            List<Inventory> i = new List<Inventory>();
            if (gp.validateUser().Role == 1)
            {
                c = new CustomerBL().GetActiveCustomersList(db).OrderByDescending(x => x.Id).ToList();
                i = new InventoryBL().GetActiveInventoriesList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                c = new CustomerBL().GetActiveCustomersList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
                i = new InventoryBL().GetActiveInventoriesList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
            }
            ViewBag.customers = c;
            ViewBag.inventory = i;
            return View();
        }

        public ActionResult PostAddCustomerSales(CustomerSale CustomerSales)
        {
            if (CustomerSales.CustomerId == null || CustomerSales.InventoryId == null)
            {
                return RedirectToAction("NewCustomerSales", new { message = "Fill all The Required Fields", color = "red" });
            }


            CustomerSales.CustomerId = CustomerSales.CustomerId;
            CustomerSales.InventoryId = CustomerSales.InventoryId;
            CustomerSales.CashPrice = CustomerSales.CashPrice;
            CustomerSales.Quantity = CustomerSales.Quantity;
            CustomerSales.OrderDate = CustomerSales.OrderDate;
            CustomerSales.CreatedBy = gp.validateUser().Id;
            CustomerSales.IsActive = 1;
            CustomerSales.CreatedAt = gp.CurrentDateTime();
            if (new CustomerSalesBL().AddCustomerSale(CustomerSales, db))
            {
                return RedirectToAction("ViewCustomerSales", new { message = "Record is Added Successfully", color = "green" });
            }
            return RedirectToAction("NewCustomerSales", new { message = "Something is Wrong", color = "red" });
        }

        public ActionResult ViewCustomerSales(string message = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            List<User> u = new UserBL().GetActiveUsersList(db).OrderByDescending(x => x.Id).ToList();
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
            ViewBag.users = u;
            return View();
        }

        [HttpPost]
        public ActionResult GetCusomersSaleList(string Name = "", string Brand = "", string Serial = "", int Dealer = -1)
        {
            List<CustomerSale> CList = new List<CustomerSale>();

            if (gp.validateUser().Role == 1)
            {
                CList = new CustomerSalesBL().GetActiveCustomerSalesList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                CList = new CustomerSalesBL().GetActiveCustomerSalesList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
            }

            if (Name != "")
            {
                CList = CList.Where(x => x.Customer.Name.ToLower().Contains(Name.ToLower())).ToList();
            }

            if (Serial != "")
            {
                CList = CList.Where(x => x.Inventory.BarcodeNo.ToLower().Contains(Serial.ToLower())).ToList();
            }

            if (Dealer != -1)
            {
                CList = CList.Where(x => x.CreatedBy == Dealer).ToList();
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
                    CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
                else
                {
                    CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                }
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                CList = CList.Where(x => x.Customer.Name.ToLower().Contains(searchValue.ToLower()) ||
                        (x.Inventory.Price != null && x.Inventory.Price.ToLower().Contains(searchValue.ToLower()))).ToList();
            }
            int totalrows = CList.Count();
            int totalrowsafterfilterinig = CList.Count();
            CList = CList.Skip(start).Take(length).ToList();

            List<DTO> dto = new List<DTO>();
            string customer, inventory = "", price = "", serialNumber = "";
            foreach (CustomerSale u in CList)
            {
                if (u.CustomerId != null)
                {
                    customer = u.Customer.Name;
                }
                else
                {
                    customer = "";
                }
                if (u.InventoryId != null)
                {
                    serialNumber = u.Inventory.BarcodeNo;
                }
                else
                {
                    serialNumber = "";
                }
                if (u.CashPrice != null)
                {
                    price = Convert.ToDecimal(u.CashPrice).ToString("c");
                }
                DTO obj = new DTO()
                {
                    CustomerSalesId = u.Id,
                    CustomerSalesEncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                    CustomerSalesCashPrice = price,
                    CustomerSalesSerialNumber = serialNumber,
                    CustomerSalesCreatedBy = u.User.Name,
                    CustomerName = customer,
                    Role = gp.validateUser().Id
                };

                dto.Add(obj);
            }
            return Json(new { data = dto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateCustomerSales(string Id = "", string message = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            List<Customer> c = new List<Customer>();
            List<Inventory> i = new List<Inventory>();
            var getId = StringCypher.Base64Decode(Id);
            CustomerSale customerSale = new CustomerSalesBL().GetActiveCustomerSaleById(Convert.ToInt32(getId), db);
            if (gp.validateUser().Role == 1)
            {
                c = new CustomerBL().GetActiveCustomersList(db).OrderByDescending(x => x.Id).ToList();
                i = new InventoryBL().GetActiveInventoriesList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                c = new CustomerBL().GetActiveCustomersList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
                i = new InventoryBL().GetActiveInventoriesList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
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
            ViewBag.customers = c;
            ViewBag.inventory = i;
            ViewBag.CustomerSales = customerSale;
            return View();
        }

        public ActionResult PostUpdateCustomerSales(CustomerSale CustomerSales)
        {

            CustomerSale c = new CustomerSalesBL().GetActiveCustomerSaleById(CustomerSales.Id, db);

            c.Id = c.Id;
            c.CustomerId = CustomerSales.CustomerId;
            c.InventoryId = CustomerSales.InventoryId;
            c.CashPrice = CustomerSales.CashPrice;
            c.Quantity = CustomerSales.Quantity;
            c.OrderDate = CustomerSales.OrderDate;
            c.CreatedAt = c.CreatedAt;
            c.IsActive = c.IsActive;
            c.CreatedBy = c.CreatedBy;

            if (new CustomerSalesBL().UpdateCustomerSale(c, db))
            {
                return RedirectToAction("ViewCustomerSales", new { message = "Record Updated Successfully", color = "green" });
            }
            return RedirectToAction("UpdateCustomerSales", new { Id = StringCypher.Base64Encode(Convert.ToString(c.Id)), message = "Something is Wrong", color = "green" });
        }

        public ActionResult DeleteCustomerSales(int Id = -1)
        {
            if (Id != -1)
            {
                CustomerSale CustomerSales = new CustomerSalesBL().GetActiveCustomerSaleById(Id, db);
                if (CustomerSales != null)
                {
                    if (new CustomerSalesBL().DeleteCustomerSale(Id, db))
                    {
                        return RedirectToAction("ViewCustomerSales", new { message = "Record Deleted Successfully", color = "green" });
                    }
                }
            }
            return RedirectToAction("ViewCustomerSales", new { message = "Something is Wrong", color = "red" });
        }

        //[Validations(CheckLogin = false)]
        //public ActionResult Invoice(string Id = "")
        //{

        //    var getId = StringCypher.Base64Decode(Id);
        //    CustomerSale customerSale = new CustomerSalesBL().GetActiveCustomerSaleById(Convert.ToInt32(getId), db);
        //    CustomerInvoice invoice = new InvoiceBL().GetActiveInventoriesList(db).Where(x => x.CustomerSalesId == customerSale.Id).FirstOrDefault();
        //    if (invoice != null)
        //    {
        //        ViewBag.Signature = invoice.Signature;
        //    }
        //    ViewBag.Sale = customerSale;
        //    string BaseUrl = Request.Url.AbsoluteUri;
        //    ViewBag.Url = BaseUrl;
        //    return View();
        //}

        //public ActionResult PostAddSignature(int CustomerSalesId = -1, string Signature = "")
        //{
        //    CustomerInvoice invoice = new CustomerInvoice()
        //    {
        //        CustomerSalesId = CustomerSalesId,
        //        CreatedBy = gp.validateUser().Id,
        //        Signature = Signature,
        //        IsActive = 1,
        //        CreatedAt = gp.CurrentDateTime()
        //    };
        //    if (new InvoiceBL().AddCustomerInvoice(invoice, db))
        //    {
        //        //return RedirectToAction("DocuSign", new { CustomerId = CustomerId, InventoryId = InventoryId, CustomerSalesId = CustomerSalesId, message = "Signature Saved Successfully" });
        //        return Json(invoice, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(JsonRequestBehavior.AllowGet);
        //}

        //Spire Pdf
        [Validations(CheckLogin = false)]
        public ActionResult PostSavePdf(int Id = -1, string Url = "")
        {
            //CustomerInvoice customerInvoice = new InvoiceBL().GetActiveCustomerInvoiceById(Id, db);
            Directory.CreateDirectory(Server.MapPath("~") + "Content/PdfFiles/");
            PdfDocument doc = new PdfDocument();

            PdfPageSettings setting = new PdfPageSettings();

            setting.Size = new SizeF(800, 1130);
            setting.Margins = new Spire.Pdf.Graphics.PdfMargins(20);
            string outputFile = "Invoice_" + DateTime.Now + ".pdf";
            string abc = outputFile.Replace(" ", "_");
            string def = abc.Replace(":", "_");
            PdfHtmlLayoutFormat htmlLayoutFormat = new PdfHtmlLayoutFormat();
            htmlLayoutFormat.IsWaiting = true;

            Thread thread = new Thread(() =>
            { doc.LoadFromHTML(Url, false, true, true, setting, htmlLayoutFormat); });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            //Save pdf file.
            doc.SaveToFile(Server.MapPath("~/Content/PdfFiles/" + def));
            doc.Close();
            System.Diagnostics.Process.Start(Server.MapPath("~/Content/PdfFiles/" + def));

            return Json(JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string Id = "")
        {
            var getId = StringCypher.Base64Decode(Id);
            CustomerSale customerSale = new CustomerSalesBL().GetActiveCustomerSaleById(Convert.ToInt32(getId), db);
            Customer c = new CustomerBL().GetActiveCustomersList(db).Where(x => x.Id == customerSale.CustomerId).OrderByDescending(x => x.Id).FirstOrDefault();
            Inventory i = new InventoryBL().GetActiveInventoriesList(db).Where(y => y.Id == customerSale.InventoryId).OrderByDescending(x => x.Id).FirstOrDefault();
            if (i != null)
            {
                List<InventoryImage> images = new InventoryImageBL().GetActiveInventoriesList(db).Where(z => z.InventoryId == i.Id).ToList();
                ViewBag.IImage = images;
            }

            ViewBag.CustomerSales = customerSale;
            ViewBag.customers = c;
            ViewBag.inventory = i;
            return View();
        }
    }
}