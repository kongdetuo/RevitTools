using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Autodesk.Revit.DB;

namespace RevitCodePad.Extensions
{
    public static class RevitDBExtensions
    {
        public static IEnumerable<T> GetElements<T>(this Document document)
            where T : Element
        {
            return new FilteredElementCollector(document)
                .OfClass(typeof(T))
                .OfType<T>();
        }

        public static IEnumerable<Element> GetElements(this Document document, IEnumerable<ElementId> elementIds)
        {
            return elementIds
                .Select(id => document.GetElement(id))
                .ToList();
        }

        public static Point3D ToPoint3D(this XYZ xyz)
        {
            return new Point3D(xyz.X, xyz.Y, xyz.Z);
        }

        public static GeometryModel3D ToModel3D(this Element element)
        {
            try
            {
                var option = new Options()
                {
                    DetailLevel = ViewDetailLevel.Fine,
                };
                var geometryElement = element.get_Geometry(option);

                var solids = geometryElement
                    .OfType<Solid>()
                    .ToList();
                if (solids.Count == 0)
                {
                    var geometryInstance = geometryElement
                        .OfType<GeometryInstance>()
                        .FirstOrDefault();
                    if (geometryElement != null)
                    {

                        solids = geometryInstance
                            .GetSymbolGeometry(geometryInstance.Transform)
                            .OfType<Solid>()
                            .ToList();
                    }
                }

                if (solids.Count == 0)
                    return null;

                var faces = solids
                    .SelectMany(solid => solid.Faces.Cast<Face>())
                    .ToList();
                var geometryGroup = new GeometryGroup();
                var geometry = new MeshGeometry3D();
                var points = new List<Point3D>();
                foreach (var face in faces)
                {
                    var baseIndex = geometry.Positions.Count;
                    var triangulate = face.Triangulate();
                    foreach (var point in triangulate.Vertices)
                    {
                        geometry.Positions.Add(point.ToPoint3D());
                    }

                    for (int i = 0; i < triangulate.NumTriangles; i++)
                    {
                        var triangle = triangulate.get_Triangle(i);
                        geometry.TriangleIndices.Add((int)triangle.get_Index(0) + baseIndex);
                        geometry.TriangleIndices.Add((int)triangle.get_Index(1) + baseIndex);
                        geometry.TriangleIndices.Add((int)triangle.get_Index(2) + baseIndex);
                    }
                }

                var model = new GeometryModel3D();
                model.Geometry = geometry;
                model.Material = new DiffuseMaterial(Brushes.Red);
                return model;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
