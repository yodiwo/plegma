using System;
using Nancy;


using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nancy.ViewEngines.Razor;
using Nancy.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nancy.TinyIoc;
using Nancy.Diagnostics;
using Nancy.Bootstrapper;
using Nancy.Conventions;


namespace Yodiwo.Projects.RestNode
{
    public class IndexModule : NancyModule
    {
        public IndexModule()
        {
            Get["/"] = parameters =>
            {
                return View["index.html"];
            };
        }
    }
}