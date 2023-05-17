using ShoePOSProject.BL;
using ShoePOSProject.HelpingClasses;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ShoePOSProject.Controllers
{
    [Validations(CheckLogin = false)]
    public class AuthController : Controller
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
        // GET: Auth
        public ActionResult Login(string msg = "", string color = "")
        {
            int c = new UserBL().GetActiveUsersList(db).Count();
            if (c == 0)
            {
                User obj = new User()
                {
                    Name = "Has San",
                    Email = "muhammad.hassan93b@gmail.com",
                    Phone = "123",
                    Address = "Lahore",
                    Password = StringCypher.Encrypt("123"),
                    Role = 1,
                    IsActive = 1,
                    CreatedAt = DateTime.Now

                };
                new UserBL().AddUser(obj, db);

            }

            if (gp.validateUser() != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.message = msg;
            ViewBag.color = color;
            return View();
        }

        public ActionResult PostLogin(string Email = "", string Password = "")
        {
            User user = new UserBL().GetActiveUsersList(db).Where(x => x.Email.Trim().ToLower() == Email.Trim().ToLower() && StringCypher.Decrypt(x.Password).Equals(Password)).FirstOrDefault();
            if (user == null)
            {
                return RedirectToAction("Login", new { msg = "Incorrect Email/Password!", color = "red" });
            }
            var identity = new ClaimsIdentity(new[]
            {
                //new Claim(ClaimTypes.Name,user.Name),
                //new Claim(ClaimTypes.Surname,user.LastName),
                new Claim(ClaimTypes.Sid,user.Id.ToString()),
                new Claim("UserName", user.Name),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("Role", user.Role.ToString())
            }, "ApplicationCookie");
            var claimsPrincipal = new ClaimsPrincipal(identity); // Set current principal
            Thread.CurrentPrincipal = claimsPrincipal;
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignIn(identity);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult UpdatePassword(string msg = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            ViewBag.message = msg;
            ViewBag.color = color;
            return View();
        }
        [HttpPost]
        public ActionResult PostUpdatePassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            if (newPassword != confirmNewPassword)
            {
                return RedirectToAction("UpdatePassword", new { msg = "New password and Confirm password must be same", color = "red" });
            }
            User user = new UserBL().GetActiveUsersList(db).Where(x => x.Email == gp.validateUser().Email).FirstOrDefault();
            if (oldPassword != StringCypher.Decrypt(user.Password))
            {
                return RedirectToAction("UpdatePassword", new { msg = "Incorrect Old Password", color = "red" });
            }
            user.Password = StringCypher.Encrypt(newPassword);
            if (new UserBL().UpdateUser(user, db))
            {
                return RedirectToAction("UpdatePassword", new { msg = "Password Updated Successfully", color = "green" });
            }
            return RedirectToAction("UpdatePassword", new { msg = "Error while updating password", color = "red" });
        }

        #region ForgotPassword

        public ActionResult ForgotPassword(string msg = "", string color = "black")
        {
            ViewBag.Color = color;
            ViewBag.message = msg;

            return View();
        }

        [HttpPost]
        public ActionResult PostForgotPassword(string Email = "")
        {
            User user = new UserBL().GetActiveUsersList(db).Where(x => x.Email == Email).FirstOrDefault();

            if (user != null)
            {
                string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");

                bool checkMail = MailSender.SendForgotPasswordEmail(Email, BaseUrl);

                if (checkMail == true)
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Please check your inbox/spam", color = "green" });
                }
                else
                {
                    return RedirectToAction("ForgotPassword", "Auth", new { msg = "Mail sending fail!", color = "red" });
                }
            }
            else
            {
                return RedirectToAction("ForgotPassword", "Auth", new { msg = "Email does not belong to our record!!", color = "red" });
            }

        }
        
        public ActionResult ResetPassword(string email = "", string time = "", string msg = "", string color = "")
        {
            DateTime dt = Convert.ToDateTime(time);
            var passDate = dt.ToString("MM/dd/yyyy");
            var currDate = DateTime.Now.ToString("MM/dd/yyyy");
            if (passDate != currDate)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Link expired, Please try again!", color = "red" });
            }
            ViewBag.Time = time;
            ViewBag.Email = email;
            ViewBag.message = msg;
            ViewBag.Color = color;

            return View();
        }

        [HttpPost]
        public ActionResult PostResetPassword(string Email = "", string Time = "", string NewPassword = "", string ConfirmPassword = "")
        {
            if (NewPassword != ConfirmPassword)
            {
                return RedirectToAction("ResetPassword", "Auth", new { email = Email, time = Time, msg = "Password and confirm password did not match", color = "red" });
            }

            string DecryptEmail = StringCypher.Base64Decode(Email);

            DatabaseEntities de = new DatabaseEntities();
            User user = new UserBL().GetActiveUsersList(db).Where(x => x.Email == DecryptEmail).FirstOrDefault();
            user.Password = StringCypher.Encrypt(NewPassword);
            bool check = false;
            try
            {
                new UserBL().UpdateUser(user, db);

                check = true;
            }
            catch
            {
                check = false;
            }

            if (check == true)
            {
                return RedirectToAction("Login", "Auth", new { msg = "Password reset successful, Try login", color = "green" });
            }
            else
            {
                return RedirectToAction("ResetPassword", "Auth", new { email = Email, time = Time, msg = "Somethings' wrong!", color = "red" });
            }
        }
        #endregion

        #region UserProfile
        public ActionResult EditProfile(string message = "", string color = "")
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            User u = new UserBL().GetActiveUsersList(db).Where(x => x.Email.ToLower() == gp.validateUser().Email.ToLower()).FirstOrDefault();
            ViewBag.User = u;
            ViewBag.message = message;
            ViewBag.color = color;
            return View();
        }
        [HttpPost]
        public ActionResult PostEditProfile(User newData, HttpPostedFileBase Profile)
        {
            if (!isLogedIn())
            {
                return RedirectToAction("Login", "Auth");
            }
            User user = new UserBL().GetActiveUsersList(db).Where(x => x.Id == newData.Id).FirstOrDefault();
            
            if (new UserBL().GetActiveUsersList(db).Where(x => x.Email == newData.Email && x.Id != newData.Id).FirstOrDefault() != null)
            {
                return RedirectToAction("EditProfile", new { msg = "Email has already been taken!", color = "red" });
            }
            user.Email = newData.Email;
            user.Name = newData.Name;
            user.Phone = newData.Phone;
            user.Address = newData.Address;

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
                        return RedirectToAction("AddUser", new { msg = "Your can Upload Maximum 2MB file", color = "red" });
                    }
                    else
                    {
                        System.Drawing.Image img = System.Drawing.Image.FromStream(Profile.InputStream);
                        int height = img.Height;
                        int width = img.Width;
                        if (height != 70 && width != 200)
                        {
                            //return RedirectToAction("AddUser", new { msg = "Width and Height of Image/Logo should be 200 x 70", color = "red" });
                        }
                        filenamenoext = "_" + DateTime.Now.ToString("yymmddfff") + "_" + Profile.FileName;
                        string path = Path.Combine(Server.MapPath("~/Content/UserPictures"), filenamenoext);
                        // file is uploaded
                        Profile.SaveAs(path);
                        user.Profile = "/Content/UserPictures/" + filenamenoext;
                    }
                }
                else
                {
                    ViewBag.wrongFile = "Wrong file format!";
                    return View("EditProfile", new { msg = ViewBag.wrongFile, color = "red"});
                }
            }
            else
            {
                User usedd = new UserBL().GetActiveUserById(user.Id, db);
                user.Profile = usedd.Profile;
            }

            if (new UserBL().UpdateUser(user, db))
            {
                return RedirectToAction("EditProfile", new { msg = "Profile Updated Successfully!", color = "green" });
            }
            return RedirectToAction("EditProfile", new { msg = "Server Error", color = "red" });
        }

        public ActionResult Signup(User newUser)
        {
            string BaseUrl = string.Format("{0}://{1}{2}", HttpContext.Request.Url.Scheme, HttpContext.Request.Url.Authority, "/");
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            double number = _rdm.Next(_min, _max);

            if (newUser.Name == null || newUser.Email == null || newUser.Password == null)
            {
                return RedirectToAction("Login", new { msg = "Fill all the fields first", color = "red" });
            }
            User u = new UserBL().GetActiveUsersList(db).Where(x => x.Email.ToLower() == newUser.Email.ToLower()).FirstOrDefault();
            if (u != null)
            {
                return RedirectToAction("Login", new { msg = "Email already exist", color = "red" });
            }

            newUser.Password = StringCypher.Encrypt(newUser.Password);
            newUser.IsActive = 2;
            newUser.Role = 2;
            newUser.CreatedAt = gp.CurrentDateTime();
            newUser.CreatedBy = 1;
            newUser.UserActivation = Convert.ToString(number);

            if (new UserBL().AddUser(newUser, db))
            {
                bool verify = MailSender.EmailVerify(newUser.UserActivation, newUser.Email, BaseUrl);
                return RedirectToAction("Login", new { msg = "Verify Your Email to Login", color = "green" });
            }
            return RedirectToAction("Login", new { msg = "Record connot be added to database", color = "red" });
        }
        #endregion

        #region ActivateAccount
        public ActionResult ActivateAccount(string email = "", string ActivationCode = "")
        {
            string DecryptEmail = StringCypher.Base64Decode(email);
            if (ActivationCode != "")
            {
                User u = new UserBL().GetUsersList(db).Where(x => x.IsActive != 0  && x.Email == DecryptEmail).FirstOrDefault();
                if(u != null)
                {
                    if(ActivationCode == u.UserActivation)
                    {
                        u.IsActive = 1;
                        if(!new UserBL().UpdateUser(u, db))
                        {
                            return RedirectToAction("Login", new { msg = "Your Account is not Activated. Please Contact with Admin", color = "red" });
                        }
                        return RedirectToAction("Login", new { msg = "Your Account is Activated. Please Login to Continue", color = "green" });
                    }
                }
            }
            return RedirectToAction("Login", new { msg = "Your Account is not Activated. Please Contact with Admin", color = "red" });
        }
        #endregion
        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login");
        }
    }
}