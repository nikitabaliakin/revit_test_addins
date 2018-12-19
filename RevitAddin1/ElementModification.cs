using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitAddin1
{
    [Transaction(TransactionMode.Manual)]
    class ElementModification : IExternalCommand
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

            // save picked wall into variable 
            Wall currentWall = (Wall)PickObject(rvtUIDoc);
           

            Level level1 = (Level)FindElement(m_rvtDoc, typeof(Level), "Level 2", null);
            if (level1 != null)
            {
                using (Transaction tr = new Transaction(m_rvtDoc))
                {
                    tr.Start("Modify wall to lvl2");
                    currentWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).
                    Set(level1.Id);
                    tr.Commit();
                }
                // Top Constraint 
                TaskDialog.Show("Wall Change", "Level was changed to Level 2");
            }

            

                            
            return Result.Succeeded;
        }

        public Element PickObject(UIDocument rvtUIDoc)
        {
            Reference refPick = rvtUIDoc.Selection.PickObject(ObjectType.Element, "Pick an element"); 
            
            Element element = m_rvtDoc.GetElement(refPick);
            if (element != null)
            {
                return element;
            }
            else
            {
                throw new Exception("No element picked");
            }
        }

        // retrieving a specific parameter individually. 
        public Parameter RetrieveParameter(Element elem, string header)
        {
           
            Parameter param =
             elem.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM);
            if (param != null){
                return param;
                }else{
                throw new Exception("No such parameter");
            }
            
        }

        public void ModifyElementPropertiesWall(Element elem)
        {
            // Constant to this function. 
            // this is for wall. e.g., "Basic Wall: Exterior - Brick on CMU" 
            // you can modify this to fit your need. 
            // 
            const string wallFamilyName = "Basic Wall";
            const string wallTypeName = "Exterior - Brick on CMU";
            const string wallFamilyAndTypeName =
                wallFamilyName + ": " + wallTypeName;

            // for simplicity, we assume we can only modify a wall 
            if (!(elem is Wall))
            {
                TaskDialog.Show("Revit Intro Lab",
                    "Sorry, I only know how to modify a wall. Please select a wall.");
                return;
            }

            Wall aWall = (Wall) elem;

            // keep the message to the user.
            string msg = "Wall changed: " + "\n" + "\n";

            // (1) change its family type to a different one. 
            // (You can enhance this to import symbol if you want.)  

            Element newWallType = FindFamilyType(m_rvtDoc,
                typeof(WallType), wallFamilyName, wallTypeName, null);

            if (newWallType != null)
            {
                aWall.WallType = (WallType) newWallType;
                msg = msg + "Wall type to: " + wallFamilyAndTypeName + "\n";
            }
            // (2) change its parameters. 
            // as a way of exercise, let's constrain the top of the wall 
            // to the level1 and set an offset. 

            // find the level 1 using the helper function we defined in the lab3. 
            Level level1 = (Level) FindElement(
                m_rvtDoc, typeof(Level), "Level 1", null);
            if (level1 != null)
            {
                aWall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level1.Id);
                // Top Constraint 
                msg += "Top Constraint to: Level 1" + "\n";
            }
        }

        public static Element FindFamilyType(Document rvtDoc, Type targetType, string targetFamilyName,
            string targetTypeName, Nullable<BuiltInCategory> targetCategory)
        {
            // first, narrow down to the elements of the given type and category
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // parse the collection for the given names 
            // using LINQ query here. 
            var targetElems = from element in collector
                where element.Name.Equals(targetTypeName) && element
                          .get_Parameter(BuiltInParameter.SYMBOL_FAMILY_NAME_PARAM).AsString().Equals(targetFamilyName)
                select element;

            // put the result as a list of element fo accessibility. 
            IList<Element> elems = targetElems.ToList();

            // return the result. 
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }

        // find a list of element with given Class, Name and Category (optional). 

        public static IList<Element> FindElements(Document rvtDoc, Type targetType, string targetName,
            Nullable<BuiltInCategory> targetCategory)
        {
            // first, narrow down to the elements of the given type and category 
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);
            if (targetCategory.HasValue)
            {
                collector.OfCategory(targetCategory.Value);
            }

            // parse the collection for the given names 
            // using LINQ query here. 
            var elems = from element in collector where element.Name.Equals(targetName) select element;

            // put the result as a list of element for accessibility. 
            return elems.ToList();
        }

        // ------------------------------------------------------------------ 
        // Helper function: searches elements with given Class, Name and 
        // Category (optional), 
        // and returns the first in the elements found. 
        // This gets handy when trying to find, for example, Level. 
        // e.g., FindElement(m_rvtDoc, GetType(Level), "Level 1") 

        public static Element FindElement(Document rvtDoc, Type targetType, string targetName,
            Nullable<BuiltInCategory> targetCategory)
        {
            // find a list of elements using the overloaded method. 
            IList<Element> elems = FindElements(rvtDoc, targetType, targetName, targetCategory);

            // return the first one from the result. 
            if (elems.Count > 0)
            {
                return elems[0];
            }

            return null;
        }



    }
}
