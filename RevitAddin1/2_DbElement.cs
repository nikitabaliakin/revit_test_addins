using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


namespace RevitAddin1
{
    [Transaction(TransactionMode.Manual)]
    class DbElement : IExternalCommand
    {
        //  Member variables 
        Application m_rvtApp;
        Document m_rvtDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //  Get the access to the top most objects
            UIApplication rvtUIApp = commandData.Application;
            UIDocument rvtUIDoc = rvtUIApp.ActiveUIDocument;
            m_rvtApp = rvtUIApp.Application;
            m_rvtDoc = rvtUIDoc.Document;

            // (1) Pick an object on the screen
            Reference refPick = rvtUIDoc.Selection.PickObject(ObjectType.Element,"Pick an Element");

            // Store picked object
            Element element = m_rvtDoc.GetElement(refPick);


            //(2) See what type of Element user picked

            DisplayElementDetails(element);

            return Result.Succeeded;
        }


        private void DisplayElementDetails(Element e)
        {
            if (e != null)
            {
                string s = "You have picked: " + "\n";

                s += " Class name = " + e.GetType().Name + "\n";
                s += " Category = " + e.Category.Name + "\n";
                s += " Element id = " + e.Id + "\n" + "\n";
                
                // and, check its type info. 
                // 
                //Dim elemType As ElementType = elem.ObjectType '' this is obsolete. 
                ElementId elemTypeId = e.GetTypeId();
                ElementType elemType = (ElementType)m_rvtDoc.GetElement(elemTypeId);

                s += "Its ElementType:" + "\n";
                s += " Class name = " + elemType.GetType().Name + "\n";
                s += " Category = " + elemType.Category.Name + "\n";
                s += " Element type id = " + elemType.Id + "\n";


                TaskDialog.Show("Class name = ",s);
            }
            else
            {
                Debug.Print(e.Name + "equals null");

            }

        }
    }
}
