//using ShoePOSProject.BL;
//using ShoePOSProject.HelpingClasses;
//using ShoePOSProject.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ShoePOSProject.Controllers
//{
//    public class CustomerController : Controller
//    {
//        DatabaseEntities db = new DatabaseEntities();
//        GeneralPurpose gp = new GeneralPurpose();

//        private bool isLogedIn()
//        {
//            if (gp.validateUser() != null)
//            {
//                return true;
//            }
//            else
//            {
//                return false;
//            }
//        }
//        // GET: Customer
//        public ActionResult NewCustomer(string message = "", string color = "")
//        {
//            if (!isLogedIn())
//            {
//                return RedirectToAction("Login", "Auth");
//            }
//            if (color == "red")
//            {
//                ViewBag.errormessage = message;
//                ViewBag.color = color;
//            }
//            if (color == "green")
//            {
//                ViewBag.successfullmessage = message;
//                ViewBag.color = color;
//            }
//            return View();
//        }

//        public ActionResult PostAddCustomer(Customer customer)
//        {
//            if (customer.Name == null || customer.Email == null || customer.PrimaryPhone == null || customer.MailingAddress == null)
//            {
//                return RedirectToAction("NewCustomer", new { message = "Fill all The <strong>(*)</strong> Fields", color = "red" });
//            }
//            int checkEmail = new CustomerBL().GetActiveCustomersList(db).Where(x => x.Email.ToLower().Contains(customer.Email.ToLower())).Count();
//            if(checkEmail >= 1)
//            {
//                return RedirectToAction("NewCustomer", new { message = "This Email is Already Exists", color = "red" });
//            }

//            Customer cus = new Customer()
//            {
//                Name = customer.Name,
//                PrimaryPhone = customer.PrimaryPhone,
//                MailingAddress = customer.MailingAddress,
//                secondaryPhone = customer.secondaryPhone,
//                City = customer.City,
//                State = customer.State,
//                Zip = customer.Zip,
//                County = customer.County,
//                PhysicalAddress = customer.PhysicalAddress,
//                PhysicalState = customer.PhysicalState,
//                PhysicalZip = customer.PhysicalZip,
//                Email = customer.Email,
//                DateOfBirth = customer.DateOfBirth,
//                SocialSecurity = customer.SocialSecurity,
//                ReferenceName = customer.ReferenceName,
//                ReferencePhone = customer.ReferencePhone,
//                extra1 = customer.extra1,
//                extra2 = customer.extra2,
//                extra3 = customer.extra3,
//                CreatedBy = gp.validateUser().Id,
//                IsActive = 1,
//                CreatedAt = gp.CurrentDateTime()
//            };
//            if(new CustomerBL().AddCustomer(cus, db))
//            {
//                return RedirectToAction("ViewCustomer", new { message = "Record is Added Successfully", color = "green"});
//            }
//            return RedirectToAction("NewCustomer", new { message = "Something is Wrong", color = "red" });
//        }

//        public ActionResult ViewCustomer(string message = "", string color = "")
//        {
//            if (!isLogedIn())
//            {
//                return RedirectToAction("Login", "Auth");
//            }
//            if (color == "red")
//            {
//                ViewBag.errormessage = message;
//                ViewBag.color = color;
//            }
//            if (color == "green")
//            {
//                ViewBag.successfullmessage = message;
//                ViewBag.color = color;
//            }
//            return View();
//        }

//        [HttpPost]
//        public ActionResult GetCusomersList(string Name = "", string Email = "", string SocialSecurity = "")
//        {
//            List<Customer> CList = new List<Customer>();

//            CList = new CustomerBL().GetActiveCustomersList(db).OrderByDescending(x => x.Id).ToList();

//            if(Name != "")
//            {
//                CList = CList.Where(x => x.Name.ToLower().Contains(Name.ToLower())).ToList();
//            }

//            if(Email != "")
//            {
//                CList = CList.Where(x => x.Email.ToLower().Contains(Email.ToLower())).ToList();
//            }

//            if(SocialSecurity != "")
//            {
//                CList = CList.Where(x => x.SocialSecurity.ToLower().Contains(SocialSecurity.ToLower())).ToList();
//            }

//            int start = Convert.ToInt32(Request["start"]);
//            int length = Convert.ToInt32(Request["length"]);
//            string searchValue = Request["search[value]"];
//            string sortColumnName = Request["columns[" + Request["order[0][column]"] + "][name]"];
//            string sortDirection = Request["order[0][dir]"];

//            if (sortColumnName != "" && sortColumnName != null)
//            {
//                if (sortDirection == "asc")
//                {
//                    CList = CList.OrderByDescending(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
//                }
//                else
//                {
//                    CList = CList.OrderBy(x => x.GetType().GetProperty(sortColumnName).GetValue(x)).ToList();
//                }
//            }

//            if (!string.IsNullOrEmpty(searchValue))
//            {
//                CList = CList.Where(x => x.Name.ToLower().Contains(searchValue.ToLower()) ||
//                        x.Email.ToLower().Contains(searchValue.ToLower()) ||
//                        (x.ReferenceName != null && x.ReferenceName.ToLower().Contains(searchValue.ToLower()))).ToList();
//            }
//            int totalrows = CList.Count();
//            int totalrowsafterfilterinig = CList.Count();
//            CList = CList.Skip(start).Take(length).ToList();

//            List<DTO> dto = new List<DTO>();
//            foreach (Customer u in CList)
//            {
//                DTO obj = new DTO()
//                {
//                    CustomerId = u.Id,
//                    CustomerEncryptedId = StringCypher.Base64Encode(Convert.ToString(u.Id)),
//                    CustomerName = u.Name,
//                    CustomerPrimaryPhone = u.PrimaryPhone,
//                    CustomerMailingAddress = u.MailingAddress,
//                    CustomerSecondaryPhone = u.secondaryPhone,
//                    CustomerMailingCity = u.City,
//                    CustomerMailingState = u.State,
//                    CustomerMailingZip = u.Zip,
//                    CustomerMailingCounty = u.County,
//                    CustomerPhysicalAddress = u.PhysicalAddress,
//                    CustomerPhysicalState = u.PhysicalState,
//                    CustomerPhysicalZip = u.PhysicalZip,
//                    CustomerEmail = u.Email,
//                    CustomerDateofBirth = u.DateOfBirth,
//                    CustomerSocialSecurity = u.SocialSecurity,
//                    CustomerReferenceName = u.ReferenceName,
//                    CustomerReferencePhone = u.ReferencePhone,
//                    CustomerExtra1 = u.extra1,
//                    CustomerExtra2 = u.extra2,
//                    CustomerExtra3 = u.extra3,
//                    CustomerCreatedBy = u.User.Name
//                };

//                dto.Add(obj);
//            }
//            return Json(new { data = dto, draw = Request["draw"], recordsTotal = totalrows, recordsFiltered = totalrowsafterfilterinig }, JsonRequestBehavior.AllowGet);
//        }

//        public ActionResult UpdateCustomer(string Id = "", string message = "", string color = "")
//        {
//            if (!isLogedIn())
//            {
//                return RedirectToAction("Login", "Auth");
//            }
//            var getId = StringCypher.Base64Decode(Id);
//            Customer c = new CustomerBL().GetActiveCustomerById(Convert.ToInt32(getId), db);
//            if (color == "red")
//            {
//                ViewBag.errormessage = message;
//                ViewBag.color = color;
//            }
//            if (color == "green")
//            {
//                ViewBag.successfullmessage = message;
//                ViewBag.color = color;
//            }
//            ViewBag.Customer = c;
//            return View();
//        }

//        public ActionResult PostUpdateCustomer(Customer customer)
//        {
//            int checkEmail = new CustomerBL().GetActiveCustomersList(db).Where(x => x.Id != customer.Id && x.Email.ToLower().Contains(customer.Email.ToLower())).Count();
//            if (checkEmail >= 1)
//            {
//                return RedirectToAction("UpdateCustomer", new { Id = StringCypher.Base64Encode(Convert.ToString(customer.Id)), message = "This Email is already Exists", color = "red" });
//            }

//            Customer c = new CustomerBL().GetActiveCustomerById(customer.Id, db);

//            c.Id = c.Id;
//            c.Name = customer.Name;
//            c.PrimaryPhone = customer.PrimaryPhone;
//            c.MailingAddress = customer.MailingAddress;
//            c.secondaryPhone = customer.secondaryPhone;
//            c.City = customer.City;
//            c.State = customer.State;
//            c.Zip = customer.Zip;
//            c.County = customer.County;
//            c.PhysicalAddress = customer.PhysicalAddress;
//            c.PhysicalState = customer.PhysicalState;
//            c.PhysicalZip = customer.PhysicalZip;
//            c.Email = customer.Email;
//            c.DateOfBirth = customer.DateOfBirth;
//            c.SocialSecurity = customer.SocialSecurity;
//            c.ReferenceName = customer.ReferenceName;
//            c.ReferencePhone = customer.ReferencePhone;
//            c.extra1 = customer.extra1;
//            c.extra2 = customer.extra2;
//            c.extra3 = customer.extra3;
//            c.CreatedAt = c.CreatedAt;
//            c.IsActive = c.IsActive;
//            c.CreatedBy = c.CreatedBy;

//            if(new CustomerBL().UpdateCustomer(c, db))
//            {
//                return RedirectToAction("ViewCustomer", new { message = "Record Updated Successfully", color = "green" });
//            }
//            return RedirectToAction("UpdateCustomer", new { Id = StringCypher.Base64Encode(Convert.ToString(c.Id)), message = "Something is Wrong", color = "green" });
//        }

//        public ActionResult DeleteCustomer(int Id = -1)
//        {
//            if(Id != -1)
//            {
//                Customer customer = new CustomerBL().GetActiveCustomerById(Id, db);
//                if(customer != null)
//                {
//                    if (new CustomerBL().DeleteCustomer(Id, db))
//                    {
//                        return RedirectToAction("ViewCustomer", new { message = "Customer Deleted Successfully", color = "green" });
//                    }
//                }
//            }
//            return RedirectToAction("ViewCustomer", new { message = "Something is Wrong", color= "red" });
//        }
//    }
//}