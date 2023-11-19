using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI.Events;

namespace REVIT_API_Day_3
{
    [Transaction(TransactionMode.Manual)]
    public class DAY3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;

            #region element location and moving and rotating using "elementtransformutils" class
            //var wall = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "Select a wall")) as Wall;
            ////get the center point of un element 
            //var wallcurve = (wall.Location as LocationCurve).Curve;
            //var wall_center_point = wallcurve.Evaluate(0.5, true);
            //using(Transaction t = new Transaction(doc, "Move wall"))
            //{
            //    t.Start();
            //    //move element
            //    ElementTransformUtils.MoveElement(doc, wall.Id, new XYZ(35, 30, 0));

            //    // rotate element
            //    ElementTransformUtils.RotateElement(doc, wall.Id, Line.CreateUnbound(wall_center_point, XYZ.BasisZ), 45 * Math.PI / 180);

            //    t.Commit();
            //}

            #endregion

            #region geometry and faces abd solids and area of faces

            //var wall = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "select wall"));
            //Options wall_option = new Options();
            //wall_option.DetailLevel.Equals(ViewDetailLevel.Undefined);
            //var wall_geometery = wall.get_Geometry(wall_option);
            //var num_of_faces = 0;
            //var area_of_faces = 0.0;
            //foreach (var geo in wall_geometery)
            //{
            //    if (geo is Solid)
            //    {
            //        var wall_solid = geo as Solid;
            //        foreach (Face face in wall_solid.Faces)
            //        {
            //            num_of_faces++;
            //            TaskDialog.Show("properties of the selected element", $"num of faces = {num_of_faces} \n area of faces = {UnitUtils.ConvertFromInternalUnits(face.Area , UnitTypeId.SquareMeters )}");
            //            area_of_faces+= face.Area;
            //        }
            //        TaskDialog.Show("properties of the selected element", $"the whole area of the surfaces of the elemets = {UnitUtils.ConvertFromInternalUnits(area_of_faces, UnitTypeId.SquareMeters)}");
            //    }
            //}
            #endregion

            #region creat new level and plan and sheets


            using (Transaction t = new Transaction(doc , "making levels, sheets and plans"))
            {
                t.Start();

                //creat levels

                 var levelHeadID= new FilteredElementCollector (doc)
                                .OfCategory(BuiltInCategory.OST_Levels)
                                .WhereElementIsElementType()
                                .FirstOrDefault(x => x.Name == "8mm Head").Id;

                Level level5 = Level.Create(doc, 5);
                level5.Name = "level 5";
                

                var levelid = new FilteredElementCollector(doc).
                          OfCategory(BuiltInCategory.OST_Levels)
                          .WhereElementIsNotElementType()
                          .FirstOrDefault(x => x.Name == "level 5");

                var plantypeid = new FilteredElementCollector(doc)
                                .OfClass(typeof(ViewFamilyType))
                                .WhereElementIsElementType()
                                .Cast<ViewFamilyType>()
                                .FirstOrDefault(x => x.ViewFamily == ViewFamily.FloorPlan).Id;

                var sheetid = new FilteredElementCollector(doc)
                              .OfCategory(BuiltInCategory.OST_TitleBlocks)
                              .First();
                

                //creat viewplan 

                 var plan = ViewPlan.Create(doc, plantypeid, levelid.Id);

                //creat sheet

                var sheet = ViewSheet.Create(doc, sheetid.Id);
                sheet.Name = "newsheet for learing";
                var boundarybox = sheet.Outline;
                var caterOfView = new XYZ((boundarybox.Max.U + boundarybox.Min.U) / 2, (boundarybox.Max.V + boundarybox.Min.V) / 2 , 0);
                var viewport = Viewport.Create(doc, sheet.Id, plan.Id, caterOfView);
                t.Commit();
            }

           #endregion

                return Result.Succeeded;
            }
    }
}
