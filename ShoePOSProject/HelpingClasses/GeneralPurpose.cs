using iTextSharp.text.pdf;
using iTextSharp.text;
using ShoePOSProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.pipeline;

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

        public class HtmlPageEventHelper : PdfPageEventHelper
        {
            public HtmlPageEventHelper(string html)
            {
                this.html = html;
            }
            public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);
                ColumnText ct = new ColumnText(writer.DirectContent);
                XMLWorkerHelper.GetInstance().ParseXHtml(new ColumnTextElementHandler(ct), new StringReader(html));
                ct.SetSimpleColumn(document.Left, document.Top, document.Right, document.GetTop(-20), 10, Element.ALIGN_MIDDLE);
                ct.Go();
            }
            string html = null;
        }
        public class HtmlPageEventHelperFooter : PdfPageEventHelper
        {
            int page;
            public HtmlPageEventHelperFooter(string html)
            {
                page = 0;
                this.html = html;
            }
            public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
            {
                base.OnEndPage(writer, document);
                ColumnText ct = new ColumnText(writer.DirectContent);
                XMLWorkerHelper.GetInstance().ParseXHtml(new ColumnTextElementHandler(ct), new StringReader(html));
                ct.SetSimpleColumn(document.Left, document.Bottom, document.Right, document.GetBottom(-20), 10, Element.ALIGN_MIDDLE);
                ct.Go();
            }
            string html = null;
        }
        public class ColumnTextElementHandler : IElementHandler
        {
            public ColumnTextElementHandler(ColumnText ct)
            {
                this.ct = ct;
            }
            ColumnText ct = null;
            public void Add(IWritable w)
            {
                if (w is WritableElement)
                {
                    foreach (IElement e in ((WritableElement)w).Elements())
                    {
                        ct.AddElement(e);
                    }
                }
            }
        }
    }
}