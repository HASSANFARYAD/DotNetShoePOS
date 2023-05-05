using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;

namespace ShoePOSProject.HelpingClasses
{
    public class GeneralPurpose
    {
        DatabaseEntities db = new DatabaseEntities();
        public User validateUser()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal; // Get the claims values
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.Sid).Select(c => c.Value).SingleOrDefault();
            int id = Convert.ToInt32(userId);
            User loggedInUser = db.Users.Where(x => x.Id == id).FirstOrDefault();
            return loggedInUser;
        }

        public DateTime CurrentDateTime()
        {
            var timeUtc = DateTime.UtcNow;
            var dt = DateTime.Now;
            return dt;
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            var number = _rdm.Next(_min, _max);
            return number;
        }

        public string GetBaseUrl()
        {
            var request = HttpContext.Current.Request;
            var appUrl = HttpRuntime.AppDomainAppVirtualPath;

            if (appUrl != "/")
                appUrl = "/" + appUrl;

            var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

            return baseUrl;
        }
    }
}