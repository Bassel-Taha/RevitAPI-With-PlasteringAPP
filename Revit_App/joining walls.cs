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
    internal class testing : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var uidoc = commandData.Application.ActiveUIDocument;
            var doc = uidoc.Document;
            
            //starting the transaction 
            using (Transaction t = new Transaction(doc, "joining walls"))
            {
                t.Start();
                
                var allwalls = new FilteredElementCollector(doc).OfClass(typeof(Wall)).WhereElementIsNotElementType().ToList();
                var plasterwalls = new List<Element>();
                var walls = new List<Element>();
                foreach (var wall1  in allwalls)
                {
                    if (wall1.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsValueString() == "plaster")
                    {
                        plasterwalls.Add(wall1);
                    }
                    else
                    {
                        walls.Add(wall1);
                    }
                }
                #region testing the join method using faces 
                // plaster walls faces
                foreach (var plasterwall in plasterwalls)
                {
                    var plasterwall_geometry = plasterwall.get_Geometry(new Options());
                    foreach (var plasterwall_geo in plasterwall_geometry)
                    {
                        var plasterfaces = (plasterwall_geo as Solid).Faces;
                        foreach (var plasterface in plasterfaces)
                        {
                            //walls faces 
                            foreach (var wall in walls)
                            {
                                var wallgeometry = wall.get_Geometry(new Options());
                                foreach (var wallgeo in wallgeometry)
                                {
                                    var wall_faces = (wallgeo as Solid).Faces;
                                    foreach (var wall_face in wall_faces)
                                    {
                                        var intersectionresult = (wall_face as Face).Intersect(plasterface as Face);
                                        if (intersectionresult == FaceIntersectionFaceResult.Intersecting)
                                        {
                                            try { JoinGeometryUtils.JoinGeometry(doc, plasterwall, wall);}
                                            catch (Exception e)
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
                #endregion




                t.Commit();
            }
            
            return Result.Succeeded;
        }
    }
}
