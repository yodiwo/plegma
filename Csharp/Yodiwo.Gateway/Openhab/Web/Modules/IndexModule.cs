using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yodiwo.Gateway.Openhab.Web.Modules
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Before += ctx =>
            {
                ViewBag.IsPjax = Request.Headers.Keys.Contains("X-PJAX");
                return null;
            };
            Get["/"] = parameters =>
            {
                return View["Index.cshtml"];
            };

        }
    }
}
