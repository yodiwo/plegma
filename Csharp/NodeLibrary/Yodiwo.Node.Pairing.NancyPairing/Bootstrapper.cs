using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nancy;
using Nancy.ViewEngines.Razor;
using Nancy.Cryptography;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nancy.TinyIoc;
using Nancy.Diagnostics;
using Nancy.Serialization.JsonNet;
using Nancy.Bootstrapper;
using Nancy.Conventions;

namespace Yodiwo.Node.Pairing.NancyPairing
{
    public class RazorConfig : IRazorConfiguration
    {
        public IEnumerable<string> GetAssemblyNames()
        {
            return null;
        }

        public IEnumerable<string> GetDefaultNamespaces()
        {
            return null;
        }

        public bool AutoIncludeModelNamespace
        {
            get { return false; }
        }
    }

    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override IRootPathProvider RootPathProvider
        {
            get { return new CustomRootPathProvider(); }
        }

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            // Enabled cookie sessions
            Nancy.Session.CookieBasedSessions.Enable(pipelines);

            // Retain the casing in serialization of nancy json
            Nancy.Json.JsonSettings.RetainCasing = true;

            StaticConfiguration.CaseSensitive = false;

            // Enable debugging of nancy
            StaticConfiguration.EnableRequestTracing = false;

            // Dummy call to force the include of the Nancy.Serialization.JsonNet dll
            JsonNetSerializer a = new JsonNetSerializer();
            a.CanSerialize("{}");
        }

        // ---------------------------------------------------------------------------------------------------------------------------------

#if true
        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get { return new DiagnosticsConfiguration { Password = @"1234" }; }
        }
#endif

        // ---------------------------------------------------------------------------------------------------------------------------------

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            base.ConfigureConventions(conventions);
        }
    }

    public class CustomRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Environment.CurrentDirectory;
        }
    }
}
