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
using System.Data.Entity;
using System.Linq.Expressions;
using ShoePOSProject.DL;
using System.Xml.Linq;
using System.Collections;

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
        
        #region SalesProduct
        
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

        public ActionResult PostAddCustomerSales(CustomerSale CustomerSales,
            FormCollection fc,
            int count = -1,
            int CustomerId = -1,
            Customer customer = null)
        {
            List<int> InventoryId = new List<int>();
            List<int> Quantities = new List<int>();
            double Price = 0;
            if (CustomerSales.InventoryId == null)
            {
                return RedirectToAction("NewCustomerSales", new { message = "Fill all The Required Fields", color = "red" });
            }
            try
            {
                if (CustomerId == -1)
                {
                    Customer customer1 = new Customer();
                    customer1.Name = customer.Name;
                    customer1.EmailAddress = customer.EmailAddress;
                    customer1.DateOfBirth = customer.DateOfBirth;
                    customer1.PrimaryPhone = customer.PrimaryPhone;
                    customer1.IsActive = 1;
                    customer1.CreatedAt = gp.CurrentDateTime();
                    customer1.CreatedBy = gp.validateUser().Id;
                    CustomerSales.CustomerId = new CustomerBL().AddCustomer2(customer1, db);
                    if (CustomerSales.CustomerId == -1)
                    {
                        throw new Exception();
                    }
                }
                var getInoviceId = AddInvoice(Convert.ToInt32(fc["Discount"]));
                if (getInoviceId != -1)
                {
                    CustomerSales.CustomerId = CustomerSales.CustomerId;
                    CustomerSales.InventoryId = CustomerSales.InventoryId;
                    CustomerSales.CashPrice = CustomerSales.CashPrice;
                    CustomerSales.Quantity = CustomerSales.Quantity;
                    CustomerSales.OrderDate = gp.CurrentDateTime();
                    CustomerSales.CreatedBy = gp.validateUser().Id;
                    CustomerSales.IsActive = 1;
                    CustomerSales.CreatedAt = gp.CurrentDateTime();
                    CustomerSales.InvoiceId = getInoviceId;
                    if (new CustomerSalesBL().AddCustomerSale(CustomerSales, db))
                    {
                        Price = Convert.ToDouble(CustomerSales.CashPrice) * Convert.ToDouble(CustomerSales.Quantity);
                        InventoryId.Add((int)CustomerSales.InventoryId);
                        Quantities.Add((int)CustomerSales.Quantity);
                        if (count > 0)
                        {
                            for (int z = 1; z <= count; z++)
                            {
                                CustomerSale optionInventory = new CustomerSale()
                                {
                                    CustomerId = CustomerSales.CustomerId,
                                    InventoryId = Convert.ToInt32(fc["InventoryId" + z]),
                                    Quantity = Convert.ToInt32(fc["QuantityId" + z]),
                                    CashPrice = fc["CashPrice"+z],
                                    OrderDate = gp.CurrentDateTime(),
                                    CreatedBy = gp.validateUser().Id,
                                    IsActive = 1,
                                    CreatedAt = gp.CurrentDateTime(),
                                    InvoiceId = getInoviceId
                                };
                                bool c = new CustomerSalesBL().AddCustomerSale(optionInventory, db);
                                InventoryId.Add((int)optionInventory.InventoryId);
                                Quantities.Add((int)optionInventory.Quantity);
                                var OtherInventoriesSales = Convert.ToDouble(optionInventory.CashPrice) * Convert.ToDouble(optionInventory.Quantity);
                                Price = Price + OtherInventoriesSales;
                                if (c == false)
                                {
                                    throw new Exception();
                                }
                            }
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
                removeQuantity(InventoryId, Quantities);
                UpdateOrder(getInoviceId, (double)Price);
                return RedirectToAction("ViewCustomerSales", new { message = "Record is Added Successfully", color = "green" });
            }
            catch (Exception ex)
            {

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
        public ActionResult GetCusomersSaleList(string Invoice = "", int SalesPerson = -1,
            string Custom = "", string StartDate = "", string EndDate = "")
        {
            List<CustomerSale> CList = new List<CustomerSale>();
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

            if (gp.validateUser().Role == 1)
            {
                CList = new CustomerSalesBL().GetActiveCustomerSalesList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                CList = new CustomerSalesBL().GetActiveCustomerSalesList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
            }

            if (Custom == "Yesterday")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date == Convert.ToDateTime(yesterday).Date).ToList();
            }
            if (Custom == "Today")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date == Convert.ToDateTime(Today).Date).ToList();
            }
            if (Custom == "CurrentWeek")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date >= Convert.ToDateTime(CurrentWeekStartDate).Date &&
                Convert.ToDateTime(x.OrderDate).Date <= Convert.ToDateTime(CurrentWeekEndDate).Date).ToList();
            }
            if (Custom == "LastWeek")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date >= Convert.ToDateTime(LastWeekStartDate).Date &&
                Convert.ToDateTime(x.OrderDate).Date <= Convert.ToDateTime(LastWeekEndDate).Date).ToList();
            }
            if (Custom == "CurrentMonth")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date >= Convert.ToDateTime(thisMonthStart).Date &&
                Convert.ToDateTime(x.OrderDate).Date <= Convert.ToDateTime(thisMonthEnd).Date).ToList();
            }
            if (Custom == "LastMonth")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date >= Convert.ToDateTime(lastMonthStart).Date &&
                Convert.ToDateTime(x.OrderDate).Date <= Convert.ToDateTime(lastMonthEnd).Date).ToList();
            }
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                CList = CList.Where(x => Convert.ToDateTime(x.OrderDate).Date >= Convert.ToDateTime(StartDate).Date &&
                Convert.ToDateTime(x.OrderDate).Date <= Convert.ToDateTime(EndDate).Date).ToList();
            }

            if (Invoice != "")
            {
                CList = CList.Where(x => x.InvoiceId.ToString().ToLower().Contains(Invoice.ToLower())).ToList();
            }

            if (SalesPerson != -1)
            {
                CList = CList.Where(x => x.CreatedBy == SalesPerson).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                //if (sortDirection == "asc")
                //{
                //    if(sortColumnName == "BarcodeNo")
                //    {
                //        CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x.InventoryId)).ToList();
                //    }
                //    else if(sortColumnName == "customer")
                //    {
                //        CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x.Customer.Name)).ToList();
                //    }
                //    else
                //    {
                //        CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                //    }
                //}
                //else
                //{
                //    if (sortColumnName == "BarcodeNo")
                //    {
                //        CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x.InventoryId)).ToList();
                //    }
                //    else if (sortColumnName == "customer")
                //    {
                //        CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x.Customer.Name)).ToList();
                //    }
                //    else
                //    {
                //        CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                //    }
                //}
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
            Invoice invoice = new InvoiceBL().GetActiveInvoiceById(Convert.ToInt32(getId), db);
            List<CustomerSale> customerSale = new CustomerSalesBL().GetActiveCustomerSalesList(db).
                Where(x => x.InvoiceId == invoice.Id).ToList();

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
            ViewBag.invoice = invoice;
            ViewBag.CustomerSales = customerSale;
            ViewBag.CustomerId = customerSale.FirstOrDefault().CustomerId;
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

        public bool removeQuantity(List<int> InventoryIds = null, List<int> Quantities = null)
        {

            for(var i = 0; i < InventoryIds.Count; i++)
            {
                DatabaseEntities de = new DatabaseEntities();
                var getInventory = new InventoryBL().GetActiveInventoryById((int)InventoryIds[i], db);
                var getQuantity = Convert.ToInt32(getInventory.extra1);
                getQuantity = getQuantity - Quantities[i];
                Inventory inv = new Inventory();
                inv.Id = getInventory.Id;
                inv.BarcodeNo = getInventory.BarcodeNo;
                inv.BrandId = getInventory.BrandId;
                inv.SizeId = getInventory.SizeId;
                inv.GenderId = getInventory.GenderId;
                inv.CollectionId = getInventory.CollectionId;
                inv.ShoeStyleId = getInventory.ShoeStyleId;
                inv.ColorId = getInventory.ColorId;
                inv.AvailableAt = getInventory.AvailableAt;
                inv.Price = getInventory.Price;
                inv.SalePrice = getInventory.SalePrice;
                inv.extra1 = getQuantity.ToString();
                inv.extra2 = getInventory.extra2;
                inv.extra3 = getInventory.extra3;
                inv.InventoryDate = getInventory.InventoryDate;
                inv.IsActive = getInventory.IsActive;
                inv.CreatedAt = getInventory.CreatedAt;
                inv.CreatedBy = getInventory.CreatedBy;
                if (!new InventoryBL().UpdateInventory(inv, de))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion SalesProduct

        #region Orders
        public int AddInvoice(int discount = -1)
        {
            var getInvoice = new InvoiceBL().GetActiveInvoicesList(db).LastOrDefault();
            Invoice invoice = new Invoice();
            if(getInvoice == null)
            {
                invoice.InvoiceId = "1000000";
            }
            else
            {
                invoice.InvoiceId = getInvoice.InvoiceId;
            }
            var makeInvoiceId = Convert.ToDouble(invoice.InvoiceId) + 1;
            invoice.InvoiceId = makeInvoiceId.ToString();
            invoice.Discount = discount;
            invoice.IsActive = 1;
            invoice.CreatedAt = gp.CurrentDateTime();
            invoice.CreatedBy = gp.validateUser().Id;
            var getInvoiceId = new InvoiceBL().AddInvoice2(invoice, db);
            if (getInvoiceId != -1)
            {
                return getInvoiceId;
            }
            return -1;
        }

        public bool UpdateOrder(int InvoiceId = -1, double GrandTotal = 0)
        {
            var invoice = new InvoiceBL().GetActiveInvoiceById((int)InvoiceId, db);
            if(invoice != null)
            {
                invoice.Id = invoice.Id;
                invoice.InvoiceId = invoice.InvoiceId;
                invoice.Discount = invoice.Discount;
                invoice.GrandTotal = GrandTotal.ToString();
                invoice.IsActive = invoice.IsActive;
                invoice.CreatedAt = invoice.CreatedAt;
                invoice.CreatedBy = invoice.CreatedBy;
                double DiscountedPrice = 100 - (double)invoice.Discount;
                DiscountedPrice = DiscountedPrice / 100;
                DiscountedPrice = DiscountedPrice * GrandTotal;
                invoice.PriceAfterDiscount = Math.Round(DiscountedPrice).ToString();
                if (new InvoiceBL().UpdateInvoice(invoice, db))
                {
                    return true;
                }
            }
            return false;
        }
        
        public ActionResult Invoice(string Id = "")
        {
            var getId = StringCypher.Base64Decode(Id);
            var invoice = new InvoiceBL().GetActiveInvoiceById(Convert.ToInt32(getId), db);
            var customerSale = new CustomerSalesBL().GetActiveCustomerSalesList(db).Where(x => x.InvoiceId == invoice.Id).ToList();
            ViewBag.customerSale = customerSale;
            ViewBag.Sale = invoice;

            return View();
        }

        public ActionResult GetCustomerOrdersList(string Invoice = "", int SalesPerson = -1,
            string Custom = "", string StartDate = "", string EndDate = "")
        {
            List<Invoice> CList = new List<Invoice>();

            if (gp.validateUser().Role == 1)
            {
                CList = new InvoiceBL().GetActiveInvoicesList(db).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                CList = new InvoiceBL().GetActiveInvoicesList(db).Where(x => x.CreatedBy == gp.validateUser().Id).OrderByDescending(x => x.Id).ToList();
            }

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

            if (Custom == "Yesterday")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date == Convert.ToDateTime(yesterday).Date).ToList();
            }
            if (Custom == "Today")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date == Convert.ToDateTime(Today).Date).ToList();
            }
            if (Custom == "CurrentWeek")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(CurrentWeekStartDate).Date &&
                Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(CurrentWeekEndDate).Date).ToList();
            }
            if (Custom == "LastWeek")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(LastWeekStartDate).Date &&
                Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(LastWeekEndDate).Date).ToList();
            }
            if (Custom == "CurrentMonth")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(thisMonthStart).Date &&
                Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(thisMonthEnd).Date).ToList();
            }
            if (Custom == "LastMonth")
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(lastMonthStart).Date &&
                Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(lastMonthEnd).Date).ToList();
            }
            if (!string.IsNullOrEmpty(StartDate) && !string.IsNullOrEmpty(EndDate))
            {
                CList = CList.Where(x => Convert.ToDateTime(x.CreatedAt).Date >= Convert.ToDateTime(StartDate).Date &&
                Convert.ToDateTime(x.CreatedAt).Date <= Convert.ToDateTime(EndDate).Date).ToList();
            }

            if (Invoice != "")
            {
                CList = CList.Where(x => x.InvoiceId.ToString().ToLower().Contains(Invoice.ToLower())).ToList();
            }

            if (SalesPerson != -1)
            {
                CList = CList.Where(x => x.CreatedBy == SalesPerson).ToList();
            }

            int start = Convert.ToInt32(Request["start"]);
            int length = Convert.ToInt32(Request["length"]);
            string searchValue = Request["search[value]"];
            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
            string sortDirection = Request["order[0][dir]"];

            if (sortColumnName != "" && sortColumnName != null)
            {
                //if (sortDirection == "asc")
                //{
                //    if (sortColumnName == "BarcodeNo")
                //    {
                //        CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x.CustomerSal)).ToList();
                //    }
                //    else if (sortColumnName == "customer")
                //    {
                //        CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x.Customer.Name)).ToList();
                //    }
                //    else
                //    {
                //        CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                //    }
                //}
                //else
                //{
                //    if (sortColumnName == "BarcodeNo")
                //    {
                //        CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x.InventoryId)).ToList();
                //    }
                //    else if (sortColumnName == "customer")
                //    {
                //        CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x.Customer.Name)).ToList();
                //    }
                //    else
                //    {
                //        CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
                //    }
                //}
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                CList = CList.Where(x => x.InvoiceId.ToLower().Contains(searchValue.ToLower())
                ).ToList();
            }
            int totalrows = CList.Count();
            int totalrowsafterfilterinig = CList.Count();
            CList = CList.Skip(start).Take(length).ToList();

            List<DTO> dto = new List<DTO>();
            string customer = "", inventory = "", price = "", serialNumber = "";
            foreach (Invoice u in CList)
            {
                var getCustomerSales = new CustomerSalesBL().GetActiveCustomerSalesList(db).
                    Where(x => x.InvoiceId == u.Id).FirstOrDefault();
                if(getCustomerSales != null)
                {
                    if(getCustomerSales.CustomerId != null)
                    {
                        customer = getCustomerSales.Customer.Name;
                    }
                    else
                    {
                        customer = "";
                    }
                    DTO obj = new DTO()
                    {
                        CustomerSalesSerialNumber = u.InvoiceId,
                        CustomerName = customer,
                        CustomerSalesCashPrice = u.GrandTotal,
                        CustomerSalesCreatedBy = u.User.Name,
                        CustomerSalesId = u.Id,
                        CustomerSalesEncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
                        Role = gp.validateUser().Id
                    };
                    dto.Add(obj);
                }
            }
            return Json(new { data = dto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult Details(string Id = "")
        {
            var getId = StringCypher.Base64Decode(Id);
            var invoice = new InvoiceBL().GetActiveInvoiceById(Convert.ToInt32(getId), db);
            if (invoice != null)
            {
                var customerSale = new CustomerSalesBL().GetActiveCustomerSalesList(db).Where(x => x.InvoiceId == invoice.Id).ToList();
                ViewBag.customerSale = customerSale;
                ViewBag.Sale = invoice;
                return View();
            }
            return RedirectToAction("ViewCustomerSales");
        }
        
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
        #endregion Orders
    }
}