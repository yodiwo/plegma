using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yodiwo.mNode.Plugins.UI.ContextMenu;
using Yodiwo.mNode.Plugins.UI.Forms;
using Yodiwo.mNode.Plugins.UI.Forms.Controls;

namespace Yodiwo.mNode.Plugins.TestPlugin
{
    public class PluginMain : Plugin
    {
        //------------------------------------------------------------------------------------------------------------------------
        public override bool Initialize(mNodeConfig mNodeConfig, string PluginConfig, string UserConfig)
        {
            //init base
            if (base.Initialize(mNodeConfig, PluginConfig, UserConfig) == false)
                return false;

            //add a sample notfication context menu item (systray icon)
            _add_sample_NotificationContextMenuItem();

            //show sample notification
            UI_ShowNotification(UI.InformationType.Info, "", "Plugin test notification");

            //create a test text thing
            var thing = AddThing(ThingTools.TextThing.CreateThing(NodeKey));

            //init plugin
            DebugEx.TraceLog("Test plugin up and running !! ");
            //done
            return true;
        }
        //------------------------------------------------------------------------------------------------------------------------
        //You can test this using : http://localhost:8081/REST/Yodiwo.mNode.Plugins.Test_Plugin/foo1/foo2?param1=hi&param2=test
        public override RestResponse HandleRestRequest(RestRequest req)
        {
            var rsp = new RestResponse()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Body = "Got it! the uri is " + req.RequestURI
            };
            return rsp;
        }
        //------------------------------------------------------------------------------------------------------------------------
        void _add_sample_NotificationContextMenuItem()
        {
            var menuItem = new UI.ContextMenu.MenuItemDescriptor()
            {
                Id = "item1",
                Text = "Item 1 test",
                SubMenuItems = new List<MenuItemDescriptor>()
                {
                     new MenuItemDescriptor()
                     {
                         Id = "item2",
                         Text = "Item 2 test",
                     },
                     new MenuItemDescriptor()
                     {
                         Id = "item3",
                         Text = "Item 3 test",
                         SubMenuItems = new List<MenuItemDescriptor>()
                         {
                            new MenuItemDescriptor()
                            {
                                Id = "item2",
                                Text = "Item 2 test",
                            },
                         }
                     }
                },
            };
            UI_NotificationContextMenu_AddOrUpdate(menuItem);
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void UI_NotificationContextMenu_HandleClick(string[] ids, MenuItemDescriptor desc)
        {
            UI_ShowMessageBox(UI.InformationType.None, "MenuClicked", "Menu item was clicked");
        }
        //------------------------------------------------------------------------------------------------------------------------
        public override void UI_mNodeForms_OnNewNavigationContext(NavigationContext navCtx, string StartPageUri)
        {
            base.UI_mNodeForms_OnNewNavigationContext(navCtx, StartPageUri);
            //navigate to start page
            navCtx.NavigateTo(createPage1("Sample Text"));
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Page createPage1(string cnt)
        {
            //create main page
            return new Page()
            {
                Uri = "main",
                Name = "main",
                Title = "Main page",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                Text= " Hi from plugin, rnd=" + cnt,
                             },
                             new UI.Forms.Controls.Button()
                             {
                                Text = "Go to Page 2",
                                Clicked = (ctx,c,args)=> ctx.NavigateTo(createPage2()),
                             },
                             new UI.Forms.Controls.Button()
                             {
                                Text = "MessageBox",
                                Clicked = (ctx,c,args)=> ctx.Show_MessageBox(UI.InformationType.Info, "Clicked", "Clicked button 1"),
                             },
                             new UI.Forms.Controls.Button()
                             {
                                Text = "Crash Plugin",
                                Clicked = btnCrash_Clicked,
                             },
                             new UI.Forms.Controls.Button()
                             {
                                Text = "Update Page",
                                Clicked = btnUpdatePage_Clicked,
                             },
                             new UI.Forms.Controls.TextBox()
                             {
                                Name = "txtBox",
                                Text = cnt,
                             },
                             new UI.Forms.Controls.Button()
                             {
                                Text = "Process Text",
                                Clicked = btnProcessText_Clicked,
                             },
                        }
                    }
                },
            };
        }
        //------------------------------------------------------------------------------------------------------------------------
        public Page createPage2()
        {
            //create main page
            return new Page()
            {
                Uri = "page2",
                Name = "page2",
                Title = "Page 2",
                Controls = new ListTS<Control>()
                {
                    new UI.Forms.Controls.StackPanel()
                    {
                        Items = new ListTS<Control>()
                        {
                             new UI.Forms.Controls.Label()
                             {
                                 Text= " Hi again from plugin.",
                             },
                             new UI.Forms.Controls.Label()
                             {
                                 Text= " This is page 2.. ",
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Dummy button",
                             },
                             new UI.Forms.Controls.Button()
                             {
                                 Text = "Go Back",
                                 Clicked = (ctx,c,args)=> ctx.GoBack(),
                            },
                        }
                    }
                },
            };
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnCrash_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            var t = new System.Threading.Thread(() =>
            {
                //var domain = AppDomain.CurrentDomain.FriendlyName;
                throw new Exception("Plugin crash");
            });
            t.Start();
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnUpdatePage_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            ctx.UpdateCurrentPage(createPage1(MathTools.GetRandomNumber(0, 100).ToStringInvariant()));
        }
        //------------------------------------------------------------------------------------------------------------------------
        void btnProcessText_Clicked(NavigationContext ctx, Button control, UIEvent args)
        {
            var txtBox = control.ParentPage.Children().FirstOrDefault(c => c.Name == "txtBox") as TextBox;
            if (txtBox != null)
            {
                ctx.UpdateCurrentPage(createPage1(txtBox.Text));
                SetPortState(ThingTools.TextThing.PortKey_Text, txtBox.Text);
            }
        }
        //------------------------------------------------------------------------------------------------------------------------
    }
}
