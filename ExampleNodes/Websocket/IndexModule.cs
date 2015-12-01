using System;
using Nancy;

namespace Yodiwo.Projects.WebsocketNode
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