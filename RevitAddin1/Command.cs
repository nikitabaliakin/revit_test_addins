#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace RevitAddin1
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // The first argument, commandData, provides access to the top most object model. 
            // You will get the necessary information from commandData. 
            // To see what's in there, print out a few data accessed from commandData 
            // 
            // Exercise: Place a break point at commandData and drill down the data. 

            UIApplication uiapp = commandData.Application;
            Application app = uiapp.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            //print out some info from commandData

            string version = app.VersionName;
            string documentTitle = doc.Title;

            TaskDialog.Show("Revit Intro Lab","Revit version = " + version + "\nDocument title = "+documentTitle);

            // Access current selection

      //      Selection sel = uidoc.Selection;

            // Retrieve elements from database

            FilteredElementCollector col
              = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.INVALID)
                .OfClass(typeof(Wall));

            // Create list of wall types

            string walls = "";
            try
            {
                foreach (Wall wall in col)
                {
                    walls += wall.Name + "\r\n";
                }
            }
            catch (Exception e)
            {
                Debug.Print(e.Message);
            }

            //show types of walls on screen
            TaskDialog.Show(
                "Revit Intro Lab",
                "Wall Types (in main instruction):\n\n" + walls);


            // Modify document within a transaction

//            using (Transaction tx = new Transaction(doc))
//            {
//                tx.Start("Transaction Name");
//                tx.Commit();
//            }

            return Result.Succeeded;
        }
    }
}
