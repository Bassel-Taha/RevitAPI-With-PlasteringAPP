using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace REVIT_API_Day_3
{
    public class External_Application : IExternalApplication
    {

        public Result OnStartup(UIControlledApplication application)
        {
            //create a ribbon tab
            application.CreateRibbonTab("my application for revit");

            //create a ribbon panel
            var initiation =  application.CreateRibbonPanel("my application for revit", "levels , sheets and views");

            //create a push button
            string path = Assembly.GetExecutingAssembly().Location;
            PushButtonData creat_planes = new PushButtonData("creat planes , levels and sheets", "inetiat the model by creating planes and sheet and levels", path, "REVIT_API_Day_3.DAY3");
            PushButton inetiatingModel =  initiation.AddItem(creat_planes)as PushButton;

            inetiatingModel.Image = new BitmapImage(new Uri("C:\\Users\\Bassel Taha\\source\\repos\\REVIT_API\\REVIT_API Day 3\\level.png"));
            inetiatingModel.LargeImage = new BitmapImage(new Uri("C:\\Users\\Bassel Taha\\source\\repos\\REVIT_API\\REVIT_API Day 3\\level.png")); 
            return Result.Succeeded;
        }
            
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}
