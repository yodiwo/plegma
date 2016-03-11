using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yodiwo.Gateway.Openhab.Rest.Model;
using Yodiwo.Gateway.Openhab.Rest.Services;
using Nancy.ModelBinding;

namespace Yodiwo.Gateway.Openhab.Web.Modules
{
    public class ThingModel
    {
        public Thing thing { get; set; }
        public ThingType type { get; set; }
        public IEnumerable<GroupItem> groups { get; set; }
        public ThingModel(Thing thing, ThingType type, IEnumerable<GroupItem> groups)
        {
            this.thing = thing;
            this.type = type;
            this.groups = groups;
        }

    }
    public class ThingID
    {
        public string prefix;
        public string id;
    }
    public class ThingEdit
    {
        public string name;
        public string groups;
    }
    public class ThingConfig
    {
        public string name;
        public string value;
    }
    public class GroupID
    {
        public string name;
    }
    public class HABModule : Nancy.NancyModule
    {
        private HABService hab;
        public HABModule()
            : base("/hab")
        {
            #region INIT
            hab = new HABService("http://localhost:8080");
            #endregion
            Before += ctx =>
            {
                ViewBag.IsPjax = Request.Headers.Keys.Contains("X-PJAX");
                return null;
            };
            #region CONTROL
            Get["/control"] = parameters =>
            {
                IEnumerable<Thing> items = hab.Things.All();
                return View["partial/control.cshtml", items];
            };
            Get["/control/{item}"] = parameters =>
            {
                IEnumerable<Item> items = hab.Setup.AllGroup();
                return View["partial/control.cshtml", items];
            };
            /*Get["/bindings"] = parameters =>
            {
                IEnumerable<Binding> bindings = hab.Bindings.All();
                //var binding = new Binding();
                return View["partial/bindings.cshtml", bindings];
                //return View["partial/control.cshtml", new BlogPost()];
            };
            Get["/bindings/{id}"] = parameters =>
            {
                IEnumerable<Binding> bindings = hab.Bindings.All();
                //var binding = new Binding();
                return View["partial/binding-group.cshtml", bindings.FirstOrDefault(h => h.id == parameters.id)];
                //return View["partial/control.cshtml", new BlogPost()];
            };*/
            #endregion
            #region DISCOVERY
            Get["/discovery"] = parameters =>
            {
                IEnumerable<string> items = hab.Discovery.All();
                return View["partial/discovery.cshtml", items];
            };
            Post["/discovery/{id}"] = parameters =>
            {
                Console.WriteLine("scanStart for {0}", parameters.id);
                if (hab.Discovery.Scan(parameters.id))
                {
                    Console.WriteLine("scanStart End {0}", parameters.id);
                    return HttpStatusCode.OK;
                }
                else
                {
                    return HttpStatusCode.NotFound;
                }
            };
            Get["/inbox"] = parameters =>
            {
                IEnumerable<DiscoveryResult> items = hab.Inbox.All();
                //return View["partial/inbox.cshtml", items];
                return View["partial/inbox.cshtml", items];
            };
            Post["/inbox/approve/{id}"] = parameters =>
            {
                var thing = (string)parameters.id;

                if (hab.Inbox.Approve(thing))
                {
                    return Response.AsRedirect("/hab/inbox");
                }

                return View["Error.cshtml"];
            };
            Post["/inbox/ignore/{id}"] = parameters =>
            {
                var thing = (string)parameters.id;

                if (hab.Inbox.Ignore(thing))
                {
                    return Response.AsRedirect("/hab/inbox");
                }

                return View["Error.cshtml"];
            };
            Post["/inbox/delete/{id}"] = parameters =>
            {
                var thing = (string)parameters.id;

                if (hab.Inbox.Remove(thing))
                {
                    return Response.AsRedirect("/hab/inbox");
                }

                return View["Error.cshtml"];
            };
            #endregion
            #region CONFIGURATION
            Get["/bindings"] = parameters =>
            {
                IEnumerable<Binding> bindings = hab.Bindings.All();
                return View["partial/bindings.cshtml", bindings];
            };
            Get["/groups"] = parameters =>
            {
                IEnumerable<GroupItem> groups = hab.Setup.AllGroup();
                return View["partial/groups.cshtml", groups];
            };
            Get["/groups/add"] = parameters =>
            {
                return View["partial/groupAdd.cshtml"];
            };
            Post["/groups/add"] = parameters =>
            {
                Console.WriteLine("POST on /groups/add");
                GroupID id = this.Bind();
                Item item = new Item();
                if (item != null)
                {
                    item.name = "group_" + id.name;
                    item.label = id.name;

                    if (hab.Setup.AddGroup(item))
                    {
                        return Response.AsRedirect("/hab/groups");
                    }
                }
                return View["Error.cshtml"];
            };
            Post["/groups/delete"] = parameters =>
            {
                Console.WriteLine("POST on /groups/delete");
                GroupID id = this.Bind();

                if (hab.Setup.DeleteGroup(id.name))
                {
                    return Response.AsRedirect("/hab/groups");
                }
                return View["Error.cshtml"];
            };
            Get["/things"] = parameters =>
            {
                IEnumerable<Thing> things = hab.Things.All();
                return View["partial/things.cshtml", things];
            };
            Get["/things/{thingUID}"] = parameters =>
            {
                //Thing thing = hab.Things.Get(parameters.thingUID};
                return View["NotImplemented.cshtml"];
            };
            Get["/things/add/{id}"] = parameters =>
            {
                ThingType thingType = hab.ThingTypes.Get(parameters.id);
                return View["partial/thingAdd.cshtml", thingType];
            };
            Post["/things/add"] = parameters =>
            {
                Console.WriteLine("POST on /things/add");
                ThingID id = this.Bind();
                List<ThingConfig> config = this.Bind();
                Thing thing = new Thing();
                thing.UID = id.prefix + ":" + id.id;
                foreach (ThingConfig cfg in config)
                {
                    thing.configuration.Add(cfg.name, cfg.value);
                }
                if (hab.Setup.AddThing(thing))
                {
                    return Response.AsRedirect("/hab/things/edit/" + thing.UID);
                }
                return View["partial/Error.cshtml"];
            };
            Get["/things/edit/{id}"] = parameters =>
            {
                Thing thing = hab.Things.Get(parameters.id);
                string value = (string)parameters.id;
                string[] partial = value.Split(':');
                ThingType thingType = hab.ThingTypes.Get(partial[0] + ':' + partial[1]);
                IEnumerable<GroupItem> groups = hab.Setup.AllGroup();
                dynamic model = new ThingModel(thing, thingType, groups);
                return View["partial/thingEdit.cshtml", model];
            };
            Post["/things/edit/{id}"] = parameters =>
            {
                Console.WriteLine("POST on /things/edit");
                ThingEdit edit = this.Bind();
                List<ThingConfig> config = this.Bind();
                Thing thing = hab.Things.Get(parameters.id);
                if (thing != null)
                {
                    thing.item.label = edit.name;
                    thing.item.groupNames.Clear();
                    foreach (string group in edit.groups.TrimEnd(' ').TrimStart(' ').Split(' '))
                    {
                        thing.item.groupNames.Add(group);
                    }
                    thing.configuration.Clear();
                    foreach (ThingConfig cfg in config)
                    {
                        thing.configuration.Add(cfg.name, cfg.value);
                    }
                    if (hab.Setup.UpdateThing(thing))
                    {
                        return Response.AsRedirect("/hab/things/edit/" + thing.UID);
                    }
                }
                return View["partial/Error.cshtml"];
            };
            Post["/things/delete/{id}"] = parameters =>
            {
                Console.WriteLine("POST on /things/delete");

                if (hab.Setup.DeleteThing(parameters.id))
                {
                    return Response.AsRedirect("/hab/things");
                }
                return View["Error.cshtml"];
            };
            Get["/things/channels/{id}"] = parameters =>
            {
                Thing thing = hab.Things.Get(parameters.id);
                string value = (string)parameters.id;
                string[] partial = value.Split(':');
                ThingType thingType = hab.ThingTypes.Get(partial[0] + ':' + partial[1]);
                dynamic model = new ThingModel(thing, thingType, null);
                return View["partial/thingChannels.cshtml", model];
            };
            Post["/things/{thing}/channels/unlink/{id}"] = parameters =>
            {
                var thing = (string)parameters.thing;
                if (hab.Setup.DisableChannel(parameters.id))
                {
                    return Response.AsRedirect("/hab/things/channels/" + thing);
                }
                return View["Error.cshtml"];
            };
            Post["/things/{thing}/channels/link/{id}"] = parameters =>
            {
                var thing = (string)parameters.thing;
                if (hab.Setup.EnableChannel(parameters.id))
                {
                    return Response.AsRedirect("/hab/things/channels/" + thing);
                }
                return View["Error.cshtml"];
            };
            Get["/items"] = parameters =>
            {
                IEnumerable<Item> items = hab.Items.All();
                return View["partial/items.cshtml", items];
            };
            Get["/items/{name}"] = parameters =>
            {
                Item item = hab.Items.Get(parameters.name);
                return View["NotImplemented.cshtml"];
            };
            #endregion

        }
    }
}
