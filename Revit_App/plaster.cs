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
    using Autodesk.Revit.DB.Architecture;
    using Autodesk.Revit.DB.Visual;

    [Transaction(TransactionMode.Manual)]
    internal class plaster : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            using (Transaction t = new Transaction(doc, "creatuing plaster"))
            {
                t.Start();
                //all walls in the project of class wall
                var walls = new FilteredElementCollector(doc).OfClass(typeof(Wall)).ToList();

                // get the room 
                var room = doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "Select a room").ElementId);

                //get wall height
                double wall_height = Convert.ToDouble(doc.GetElement(uidoc.Selection.PickObject(ObjectType.Element, "Select a wall to get the height of the plaster for the room").ElementId).get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsDouble());
                
                //get level id
                var levelid = room.LevelId;
                
                //wall type
                var wall_type = new FilteredElementCollector(doc).OfClass(typeof(WallType)).FirstOrDefault(x => x.Name == "plaster").Id;
                
                //get room boundary
                var room_op = new SpatialElementBoundaryOptions();
                
                var roomboundaty = (room as Room).GetBoundarySegments(room_op);
                var room_height = room.get_Parameter(BuiltInParameter.ROOM_HEIGHT);
                var plasterwalls= new List<Wall>();


                // creat walls and floors 
                foreach (var ilistOfLines in roomboundaty)
                {
                  
                    foreach (var line in ilistOfLines)
                    {
                        plasterwalls.Add(Wall.Create(doc, line.GetCurve()
                                                            .CreateOffset(UnitUtils
                                                            .ConvertToInternalUnits(-0.01, UnitTypeId.Meters),
                                                             XYZ.BasisZ), wall_type, levelid, 
                                                            wall_height,
                                                            0,
                                                            false,
                                                            false));
                        
                    }
                }
                foreach (var ilistOfLines in roomboundaty)
                {
                    
                    var basewalllevel = plasterwalls[0].LevelId;
                    var topWallLevel = Level.Create(doc, (plasterwalls[0].get_Parameter(BuiltInParameter.WALL_TOP_EXTENSION_DIST_PARAM).AsDouble()) + wall_height);

                    var roof_type = new FilteredElementCollector(doc).OfClass(typeof(RoofType)).FirstOrDefault(x => x.Name == "plaster").Id;
                    var roof = Floor.Create(doc, (ilistOfLines) as IList<CurveLoop> , roof_type, topWallLevel.Id);
                    t.Commit();
                }
                
                //creating the Roof Plaster from the room boundary
            

            }

            return Result.Succeeded;
        }
        
    }

}
