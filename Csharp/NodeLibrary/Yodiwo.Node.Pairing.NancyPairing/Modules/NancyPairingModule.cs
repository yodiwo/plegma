using System;
using System.Linq;

using Nancy;
using Nancy.Helpers;
using Nancy.ModelBinding;
using Nancy.Security;
using Newtonsoft.Json;

using Yodiwo.API.Plegma.NodePairing;
using Yodiwo;

namespace Yodiwo.NodeLibrary.Pairing.NancyPairing
{
    public class NancyPairingModule : NancyModule
    {
        public static Yodiwo.NodeLibrary.Pairing.NodePairingBackend backend;

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
                    return Response.AsRedirect(backend.pairingPostUrl + "/" + API.Plegma.NodePairing.NodePairingConstants.UserConfirmPageURI + "?token2=" + token2);
                else
                    return "Could not start pairing procedure";
            };

            Get["/retrievenodesinfo"] = parameters =>
            {
                var infos = parameters.infos;
                return "Ok";
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
                        return View["Pairing/PairSuccess", backend.frontendUrl];
                    else
                        return View["Pairing/PairFailed"];
                };
        }

    }
}
