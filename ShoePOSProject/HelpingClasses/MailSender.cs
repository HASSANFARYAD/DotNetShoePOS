using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Helpers;

namespace ShoePOSProject.HelpingClasses
{
    public class MailSender
    {
        public static bool SendForgotPasswordEmail(string email, string BaseUrl = "")
        {
            try
            {
                string Subject = "Forgot Password (Shoe POS)";
                string MailBody = "<html>" +
                    "<head></head>" +
                    "<body>" +
                    "<center>" + "<div> <h1 class='text-center' style='color:#000000'> Password Reset </h1> " +
                    "<p class='text-center' style='color:#000000'> " +
                          "You are Getting this Email Because You Requested To Reset Your Account Password.<br>Click the Button Below To Change Your Password" +
                    " </p>" +
                    "<p style='color:#000000' class='text-center'>" +
                            "If you did not request a password reset, Please Ignore This Email" +
                    "</p>" +
                    "<h3 style='color:#000000'>" + "Thanks" + "</h3>" +
                    "<br/>" +
                    "<button style='background-color: #ce2029; padding:12px 16px; border:1px solid #ce2029; border-radius:3px;'>" +
                            "<a href='" + BaseUrl + "Auth/ResetPassword?email=" + StringCypher.Base64Encode(email) + "&time=" + DateTime.Now.Date.ToString() + "'  style='text-decoration:none; font-size:15px; color:white;'> Reset Password </a>" +
                    "</button>" +
                    "<p style='color:#FF0000'>Link will Expire after Date Change.<br>" +
                    "Link will not work in spam. Please move this mail into your inbox.</p>" +
                    "</div>" + "</center>" +
                            "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";

                if (SendEmail(email, Subject, MailBody))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        //Error Email Send
        public bool EmailSend(Exception e)
        {
            MailMessage msg = new MailMessage();
            string Subject = "Exception (Shoe POS)";
            string text = "<link href='https://fonts.googleapis.com/css?family=Bree+Serif' rel='stylesheet'><style>  * {";
            text += "  font-family: 'Bree Serif', serif; }";
            text += " .list-group-item {       border: none;  }    .hor {      border-bottom: 5px solid black;   }";
            text += " .line {       margin-bottom: 20px; }";
            
            string temp = "Error" + e ;
            temp += "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";

            if (SendEmail(ProjectVariables.ToEmail, Subject, temp))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Email Verification
        public static bool EmailVerify(string number, string email, string BaseUrl = "")
        {
            try
            {
                string Subject = "Verification Email";
                string MailBody = "<html>" +
                    "<head></head>" +
                    "<body>" +
                    "<center>" + "<div> <h1 class='text-center' style='color:#000000'> Account Activation </h1> " +
                    "<p class='text-center' style='color:#000000'> " +
                          "Click the button below to Activate Your Account"+
                    " </p>" +
                    "<p style='color:#000000' class='text-center'>" +
                            "If you did not request a email verification, Please Ignore This Email" +
                    "</p>" +
                    "<h3 style='color:#000000'>" + "Thanks" + "</h3>" +
                    "<br/>" +
                    "<button style='background-color: #00A300; padding:12px 16px; border:1px solid #00A300; border-radius:3px;'>" +
                            "<a href='" + BaseUrl + "Auth/ActivateAccount?email=" + StringCypher.Base64Encode(email) + "&time=" + DateTime.Now.Date.ToString() + "&ActivationCode="+number+"'  style='text-decoration:none; font-size:15px; color:white;'> Activate Account </a>" +
                    "</button>" +
                    "<p style='color:#FF0000'>Link will Expire after Date Change.<br>" +
                    "Link will not work in spam. Please move this mail into your inbox.</p>" +
                    "</div>" + "</center>" +
                            "<script src = 'https://ajax.googleapis.com/ajax/libs/jquery/3.2.1/jquery.min.js' ></ script ></ body ></ html >";


                if (SendEmail(email, Subject, MailBody))
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch
            {
                return false;
            }
        }

        public static bool SendErrorEmail(string msg)
        {
            try
            {
                string SubjectBody = "Error Message";
                string MailBody = "<html>" +
                    "<head></head>" +
                    "<body>" +
                    msg +
                    "</body></html>";

                if(SendEmail(ProjectVariables.ToEmail, SubjectBody, MailBody))
                {
                    return true; 
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool SendEmail(string email, string subject, string MailBody)
        {
            try
            {
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress(ProjectVariables.FromEmail);
                msg.To.Add(email);
                msg.Subject = subject;
                msg.IsBodyHtml = true;
                msg.Body = MailBody;
                using (SmtpClient client = new SmtpClient())
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("muhammad.hassan93b@gmail.com", "cduldnlpwqzxaxxd");
                    client.Host = "smtp.gmail.com";
                    client.Port = 587;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Send(msg);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}