namespace Yodiwo.Gateway.Openhab.Web
{
    using Nancy;
    using Nancy.Serialization.JsonNet;
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        // The bootstrapper enables you to reconfigure the composition of the framework,
        // by overriding the various methods and properties.
        // For more information https://github.com/NancyFx/Nancy/wiki/Bootstrapper

        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);

            //Enable CSRF protection
            Nancy.Security.Csrf.Enable(pipelines);

            //enabled cookie sessions
            Nancy.Session.CookieBasedSessions.Enable(pipelines);

            //Setup frame and origin options ( https://www.owasp.org/index.php/List_of_useful_HTTP_headers )
            //may be overwritten by server (apache,ngix,iis,..) for config see https://developer.mozilla.org/en-US/docs/Web/HTTP/X-Frame-Options
            pipelines.AfterRequest.AddItemToEndOfPipeline((ctx) =>
            {
                if (ctx.Response.StatusCode == HttpStatusCode.InternalServerError) return;

                ctx.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
                ctx.Response.Headers.Add("X-Download-Options", "noopen"); // IE extension
                ctx.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                ctx.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            });

            // Set the json settings.
            Nancy.Json.JsonSettings.RetainCasing = true;

            // Dummy call to force the include of the Nancy.Serialization.JsonNet dll
            JsonNetSerializer a = new JsonNetSerializer();
            a.CanSerialize("{}");
        }
    }
}