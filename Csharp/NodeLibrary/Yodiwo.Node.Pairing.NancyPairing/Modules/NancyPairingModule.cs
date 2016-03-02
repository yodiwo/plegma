using System;
using System.Linq;

using Nancy;
using Nancy.Helpers;
using Nancy.ModelBinding;
using Nancy.Security;
using Newtonsoft.Json;

using Yodiwo.API.Plegma.NodePairing;
using Yodiwo;

namespace Yodiwo.Node.Pairing.NancyPairing
{
    public class NancyPairingModule : NancyModule
    {
        public static Yodiwo.Node.Pairing.NodePairingBackend backend;

        public NancyPairingModule()
            : base("/pairing")
        {

            Before += ctx =>
            {
                ViewBag.IsPjax = Request.Headers.Keys.Contains("X-PJAX");
                return null;
            };


            Get["/"] = parameters =>
            {
                return View["Index"];
            };


            Get["/pair"] = parameters =>
            {
                if (backend == null)
                {
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                    };
                }
                return View["Pairing/Pair"];

            };

            Get["/startpairing"] = parameters =>
            {
                if (backend == null)
                {
                    return new Response()
                    {
                        StatusCode = HttpStatusCode.ExpectationFailed,
                    };
                }
                string redirectUrl = this.Request.Url.SiteBase + "/pairing/next";
                string token2 = backend.pairGetTokens(redirectUrl);
                if (token2 != null)
                {
                    return Response.AsRedirect(backend.userUrl + "?token2=" + token2);
                }
                else
                {
                    return new Response { StatusCode = HttpStatusCode.Forbidden };
                }
            };

            Get["/next"] = parameters =>
                {
                    if (backend == null)
                    {
                        return new Response()
                        {
                            StatusCode = HttpStatusCode.ExpectationFailed,
                        };
                    }
                    object keys = backend.pairGetKeys();
                    if (keys != null)
                    {
                        return View["Pairing/PairSuccess"];
                    }
                    else
                    {
                        return new Response { StatusCode = HttpStatusCode.Forbidden };
                    }
                };
        }

    }
}
