using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ShoePOSProject.HelpingClasses
{
    public class Validations : FilterAttribute, IActionFilter, IExceptionFilter
    {
        public List<int> CheckedRole { get; set; }
        public int Role;
        public bool CheckLogin;
        private readonly GeneralPurpose gp = new GeneralPurpose();
        private readonly MailSender ms = new MailSender();

        public Validations()
        {
            CheckLogin = true;
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                ms.EmailSend(filterContext.Exception);
            }
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (CheckLogin == true) //only works when check is true
            {
                var controllerName = filterContext.RouteData.Values["controller"].ToString();
                var actionName = filterContext.RouteData.Values["action"].ToString();
                List<int> roles = CheckedRole;

                int role = Role;

                if (gp.validateUser() != null)
                {
                    if (role != 0 && role != gp.validateUser().Role)
                    {
                        if (gp.validateUser().Role == 1)
                        {
                            var values = new RouteValueDictionary(new
                            {
                                action = "Index",
                                controller = "Admin",
                                msg = "Access Denied!",
                                color = "red"
                            });

                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(values));
                        }
                        else
                        {
                            var values = new RouteValueDictionary(new
                            {
                                action = "Index",
                                controller = "Home",
                                msg = "Access Denied!",
                                color = "red"
                            });

                            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(values));
                        }
                    }
                }
                else
                {
                    var values = new RouteValueDictionary(new
                    {
                        action = "Login",
                        controller = "Auth",
                        msg = "Session Expired, Please Login",
                        color = "red"
                    });

                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(values));
                }
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary{
                    { "controller", "Error" },{ "action", "NotFound" }, });
        }
    }
}