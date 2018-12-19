#region Namespaces
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
#endregion

namespace RevitAddin1
{
    class App : IExternalApplication
    {

        //Calls the method on start up
        public Result OnStartup(UIControlledApplication a)
        {
            //generates panel on ribbon
            try
            {
                RibbonPanel(a);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            };

          return Result.Succeeded;

        }

        // Method creates tab and panel on it
        //In case panel already exists, it replaces it with a newly created one
        private void RibbonPanel(UIControlledApplication a) {

            //Path to assembly
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            // Create a custom ribbon tab
            String tabName = "This Tab Name";
            a.CreateRibbonTab(tabName);

            // Create a ribbon panel
            RibbonPanel mProjectPanel = a.CreateRibbonPanel(tabName, "Cut Wall");
            
            // Create push buttons
            PushButtonData button1 = new PushButtonData("CutWall", "Wall Cutter", thisAssemblyPath, "RevitAddin1.Command");
            PushButtonData button2 = new PushButtonData("New Button 2", "Test Button", thisAssemblyPath, "RevitAddin1.Command");

            try
            {
                button1.Image = new BitmapImage(new Uri(@"C:\Users\Nikita\source\repos\RevitAddin1\RevitAddin1\bin\Debug\knife.bmp"));
                button2.LargeImage = new BitmapImage(new Uri(@"C:\Users\Nikita\source\repos\RevitAddin1\RevitAddin1\bin\Debug\knife.bmp"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Add the buttons to the panel
            var projectButtons = new List<RibbonItem>();
            
            projectButtons.AddRange(mProjectPanel.AddStackedItems(button1,button2));

        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
