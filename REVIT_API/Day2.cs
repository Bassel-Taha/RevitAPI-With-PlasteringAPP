using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;


namespace REVIT_API
{
    using Autodesk.Revit.ApplicationServices;
    using Autodesk.Revit.UI.Events;
    [Transaction(TransactionMode.Manual)]
    public class Day2 : IExternalCommand
    {
        #region getting elements info
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        //{
        //    // Get application and document objects
        //    UIApplication uiApp = commandData.Application;
        //    UIDocument uiDoc = uiApp.ActiveUIDocument;
        //    Application app = uiApp.Application;
        //    Document doc = uiDoc.Document;
        //    // Element ID
        //    var elms = uiDoc.Selection.PickObject(ObjectType.Element, "select obj to get its ID");
        //    // Get element
        //    Element elm = uiDoc.Document.GetElement(elms.ElementId);
        //    // Get element type
        //    ElementId typeId = elm.GetTypeId();
        //    ElementType type = doc.GetElement(typeId) as ElementType;
        //    // Get element level
        //    ElementId levelId = elm.LevelId;
        //    Level levelName = doc.GetElement(levelId) as Level;
        //    // Show element Info
        //    TaskDialog.Show(
        //        "Element ID",
        //        "Element ID " + elms.ElementId.ToString() + " \n" + "Element Type " + type.Name.ToString() + "\n"
        //        + "Element Name " + elm.Name.ToString() + "\n" + "Element Category " + elm.Category.Name.ToString()
        //        + "\n" + "Element Level " + levelName.Name.ToString());
        #endregion

        #region filter elementrs
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        //{
        //    UIDocument uidoc = commandData.Application.ActiveUIDocument;
        //    Document doc = uidoc.Document;
        //    FilteredElementCollector collector = new FilteredElementCollector(doc);
        //    var wallsFilter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
        //    var collection = collector.WherePasses(wallsFilter).WhereElementIsNotElementType().ToElements();
        //    TaskDialog.Show("the number of columns in the view ", collection.Count.ToString());


        //    return Result.Succeeded;
        //}
        #endregion

        #region delete element
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        //{
        //    UIDocument uidoc = commandData.Application.ActiveUIDocument;
        //    Document doc = uidoc.Document;
        //    var pickedElement = uidoc.Selection.PickObject(ObjectType.Element, "select element to delete it ");

        //    doc.Delete(pickedElement.ElementId);

        //    return Result.Succeeded;
        //}
        #endregion

        #region creat wall
        //public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        //{
        //    UIDocument uidoc = commandData.Application.ActiveUIDocument;   
        //    Document doc = commandData.Application.ActiveUIDocument.Document;
        //    Line curve = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 10, 0));
        //    Line curve2 = Line.CreateBound(new XYZ(0, 0, 0), new XYZ(10, 0, 0));

        //    //IList<Curve> curves = new List<Curve>();
        //    //curves.Add(curve);

        //    ElementId level = new FilteredElementCollector(doc)
        //                          .OfCategory(BuiltInCategory.OST_Levels)
        //                          .WhereElementIsNotElementType()
        //                          .FirstOrDefault(e => e.Name == "L1")
        //                           .Id;
        //    //ElementId wallTypeId = new FilteredElementCollector(doc)
        //    //                                    .OfCategory(BuiltInCategory.OST_Walls)
        //    //                                    .WhereElementIsElementType()
        //    //                                    .FirstOrDefault(e => e.Name == "Generic - 200mm")
        //    //                                     .Id;

        //    using(Transaction t = new Transaction(doc, "create wall"))
        //    {
        //        t.Start();
        //        var walls = Wall.Create(doc, curve , level, false);
        //        t.Commit();
        //    }
        //    return Result.Succeeded;
        //}


        #endregion

        #region creat walls with floor

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = commandData.Application.ActiveUIDocument.Document;

            List<XYZ> points = new List<XYZ>() { new XYZ(-10, 10, 0),
                                                   new XYZ(10,10,0) ,
                                                   new XYZ(15,0,0) ,
                                                   new XYZ(10,-10,0) ,
                                                   new XYZ(-10,-10,0)};
            IList<Curve> lines = new List<Curve>()
                                    {
                                        Line.CreateBound(points[0], points[1]),
                                        Arc.Create(points[1], points[3], points[2]),
                                        Line.CreateBound(points[3], points[4]),
                                        Line.CreateBound(points[4], points[0])
                                    };

            var levelid = new FilteredElementCollector(doc)
                                    .OfCategory(BuiltInCategory.OST_Levels)
                                    .WhereElementIsNotElementType()
                                    .FirstOrDefault(e => e.Name == "L1")
                                    .Id;
            var floortype = new FilteredElementCollector(doc)
                                                        .OfCategory(BuiltInCategory.OST_Floors)
                                                        .WhereElementIsElementType()
                                                        .FirstOrDefault(e => e.Name == "Concrete 100mm")
                                                        .Id;
            var line1 = Line.CreateBound(new XYZ(20, 20, 0), new XYZ(35, 35, 0));


            IList<CurveLoop> curveLoops = new List<CurveLoop>() { CurveLoop.CreateViaOffset(CurveLoop.Create(lines), UnitUtils.ConvertToInternalUnits(-100, UnitTypeId.Millimeters), XYZ.BasisZ) };
            using (Transaction t = new Transaction(doc, "creating walls"))
            {
                t.Start();
                foreach (var line in lines)
                {
                    Wall.Create(doc, line, levelid, false);
                }
                t.Commit();

            }
            using (Transaction t2 = new Transaction(doc, "creating floor"))
            {
                t2.Start();
                Floor.Create(doc, curveLoops, floortype, levelid);
                t2.Commit();
            }
            return Result.Succeeded;
        }



        #endregion
        #region set/get parameters 

        //write the code later and if you forgot watch the last segment of the vedio of iti day 2

        #endregion

    }
}
