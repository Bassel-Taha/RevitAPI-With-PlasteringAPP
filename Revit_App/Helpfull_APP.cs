using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
namespace Revit_App
{
    using System.Reflection;

    public class Helpful_APP : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // creating ribbon tap
            application.CreateRibbonTab("my plugins" );

            // creating panel for the creating plaster walls
            var plaster = application.CreateRibbonPanel("my plugins", "plaster");
            string path = Assembly.GetExecutingAssembly().Location;
            var classname = "Revit_App.plaster";
            PushButtonData plaster_bottun_data = new PushButtonData("plaster", "plaster", path, classname);
            var plaster_button =  plaster.AddItem(plaster_bottun_data)as PushButton;

            //  creating button for joining walls
            var joiningCurtainWalls = new PushButtonData("joining walls" , "joining walls" , path , "Revit_App.testing");
            var joiningwallspanal = application.CreateRibbonPanel("my plugins", "joining walls");
            var joiningCurtainWalls_button = joiningwallspanal.AddItem(joiningCurtainWalls) as PushButton;
            
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
