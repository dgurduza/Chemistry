using ChemistryProg.Data;
using ChemistryProg.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace ChemistryProg.MakeSphere
{
    public static class ExtensionToMesh
    {
        private const double border_thickness = 0.05;
        #region Transformation

        // Apply a transformation Matrix3D or transformation class.
        public static void ApplyTransformation(this MeshGeometry3D mesh, Matrix3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);

            Vector3D[] normals = mesh.Normals.ToArray();
            transformation.Transform(normals);
            mesh.Normals = new Vector3DCollection(normals);
        }

        public static void ApplyTransformation(this MeshGeometry3D mesh, Transform3D transformation)
        {
            Point3D[] points = mesh.Positions.ToArray();
            transformation.Transform(points);
            mesh.Positions = new Point3DCollection(points);

            Vector3D[] normals = mesh.Normals.ToArray();
            transformation.Transform(normals);
            mesh.Normals = new Vector3DCollection(normals);
        }

        #endregion Transformation

        #region Merging

        // Merge a mesh into this one.
        // Do not copy texture coordinates or normals.
        public static void Merge(this MeshGeometry3D mesh, MeshGeometry3D other)
        {
            // Copy the positions. Save their new indices in an indices array.
            int index = mesh.Positions.Count;
            int[] indices = new int[other.Positions.Count];
            for (int i = 0; i < other.Positions.Count; i++)
            {
                mesh.Positions.Add(other.Positions[i]);
                indices[i] = index++;
            }

            // Copy the triangles.
            for (int t = 0; t < other.TriangleIndices.Count; t++)
            {
                int i = other.TriangleIndices[t];
                mesh.TriangleIndices.Add(indices[i]);
            }
        }

        #endregion Merging

        #region PointSharing

        // If the point is already in the dictionary, return its index in the mesh.
        // If the point isn't in the dictionary, create it in the mesh and add its
        // index to the dictionary.
        private static int PointIndex(this MeshGeometry3D mesh,
            Point3D point, Dictionary<Point3D, int> pointDict = null)
        {
            // See if the point already exists.
            if ((pointDict != null) && (pointDict.ContainsKey(point)))
            {
                // The point is already in the dictionary. Return its index.
                return pointDict[point];
            }

            // Create the point.
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point);

            // Add the point's index to the dictionary.
            if (pointDict != null) pointDict.Add(point, index);

            // Return the index.
            return index;
        }

        // If the point is already in the dictionary, return its index in the mesh.
        // If the point isn't in the dictionary, create it and its texture coordinates
        // in the mesh and add its index to the dictionary.
        private static int PointIndex(this MeshGeometry3D mesh,
            Point3D point, Point textureCoord,
            Dictionary<Point3D, int> pointDict = null)
        {
            // See if the point already exists.
            if ((pointDict != null) && (pointDict.ContainsKey(point)))
            {
                // The point is already in the dictionary. Return its index.
                return pointDict[point];
            }

            // Create the point.
            int index = mesh.Positions.Count;
            mesh.Positions.Add(point);

            // Add the point's texture coordinates.
            mesh.TextureCoordinates.Add(textureCoord);

            // Add the point's index to the dictionary.
            if (pointDict != null) pointDict.Add(point, index);

            // Return the index.
            return index;
        }

        #endregion PointSharing

        #region Polygon

        // Add a simple polygon with no texture coordinates, smoothing, or wireframe.
        public static void AddPolygon(this MeshGeometry3D mesh,
            HashSet<Edges> edges, double thickness, params Point3D[] points)
        {
            mesh.AddPolygon(pointDict: null, points: points, edges: edges, thickness: thickness);
        }
        public static void AddPolygon(this MeshGeometry3D mesh, params Point3D[] points)
        {
            mesh.AddPolygon(pointDict: null, points: points);
        }

        // Add a polygon.
        public static void AddPolygon(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> pointDict = null,
            HashSet<Edges> edges = null, double thickness = 0.1,
            Point[] textureCoords = null, params Point3D[] points)
        {
            if (edges != null)
            {
                // Make a wireframe polygon.
                mesh.AddPolygonEdges(edges, thickness, points);
            }
            else
            {
                // Make a wireframe polygon.
                mesh.AddPolygonTriangles(pointDict, textureCoords, points);
            }
        }

        // Make a polygon's triangles.
        public static void AddPolygonTriangles(this MeshGeometry3D mesh,
            Dictionary<Point3D, int> pointDict = null,
            Point[] textureCoords = null, params Point3D[] points)
        {
            // Make a point dictionary.
            if (pointDict == null) pointDict = new Dictionary<Point3D, int>();

            // Get the first two point indices.
            int indexA, indexB, indexC;

            Point3D roundedA = points[0].Round();
            if (textureCoords == null)
                indexA = mesh.PointIndex(roundedA, pointDict);
            else
                indexA = mesh.PointIndex(roundedA, textureCoords[0], pointDict);

            Point3D roundedC = points[1].Round();
            if (textureCoords == null)
                indexC = mesh.PointIndex(roundedC, pointDict);
            else
                indexC = mesh.PointIndex(roundedC, textureCoords[1], pointDict);

            // Make triangles.
            Point3D roundedB;
            for (int i = 2; i < points.Length; i++)
            {
                indexB = indexC;
                roundedB = roundedC;

                // Get the next point.
                roundedC = points[i].Round();
                if (textureCoords == null)
                    indexC = mesh.PointIndex(points[i].Round(), pointDict);
                else
                    indexC = mesh.PointIndex(points[i].Round(), textureCoords[i], pointDict);

                // If two of the points are the same, skip this triangle.
                if ((roundedA != roundedB) &&
                    (roundedB != roundedC) &&
                    (roundedC != roundedA))
                {
                    mesh.TriangleIndices.Add(indexA);
                    mesh.TriangleIndices.Add(indexB);
                    mesh.TriangleIndices.Add(indexC);
                }
            }
        }

        // Add a regular polygon with optional texture coordinates.
        public static void AddRegularPolygon(this MeshGeometry3D mesh,
            int numSides, Point3D center, Vector3D vx, Vector3D vy,
            Dictionary<Point3D, int> pointDict = null,
            HashSet<Edges> edges = null, double thickness = 0.1,
            Point[] textureCoords = null)
        {
            // Generate the points.
            Point3D[] points = G3.MakePolygonPoints(numSides, center, vx, vy);

            // Make the polygon.
            mesh.AddPolygon(pointDict, edges, thickness, textureCoords, points);
        }

        #endregion Polygon

        #region Models

        // Make a model with a diffuse brush.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, Brush brush)
        {
            Material material = new DiffuseMaterial(brush);
            return new GeometryModel3D(mesh, material);
        }

        // Make a model with a texture brush.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, string uri)
        {
            ImageBrush brush = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri(uri, UriKind.Relative))
            };
            return mesh.MakeModel(brush);
        }

        // Make a model with a material group.
        public static GeometryModel3D MakeModel(this MeshGeometry3D mesh, MaterialGroup material)
        {
            return new GeometryModel3D(mesh, material);
        }

        #endregion Models

        #region Parallelogram

        // Add a parallelogram defined by a corner point and two edge vectors.
        // Texture coordinates and the point dictionary are optional.
        public static void AddParallelogram(this MeshGeometry3D mesh,
            Point3D corner, Vector3D v1, Vector3D v2,
            Point[] textureCoords = null,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Find the parallelogram's corners.
            Point3D[] points =
            {
                corner,
                corner + v1,
                corner + v1 + v2,
                corner + v2,
            };

            // Make it.
            mesh.AddPolygon(points: points, textureCoords: textureCoords,
                edges: edges, thickness: thickness);
        }

        #endregion Parallelogram

        #region Boxes

        // Add a parallelepiped defined by a corner point and three edge vectors.
        // The vectors should have more or less the orientation of the X, Y, and Z axes.
        // The corner point should be the back, lower, left corner
        // analogous to the smallest X, Y, and Z coordinates.
        // Texture coordinates are optional.
        // Points are shared on each face and not between faces.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            Point[] textureCoords = null,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            mesh.AddBox(corner, vx, vy, vz,
                textureCoords, textureCoords, textureCoords,
                textureCoords, textureCoords, textureCoords,
                edges, thickness);
        }

        // Add a parallelepiped with different texture coordinates for each face.
        public static void AddBox(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            Point[] frontCoords, Point[] leftCoords, Point[] rightCoords,
            Point[] backCoords, Point[] topCoords, Point[] bottomCoords,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            mesh.AddParallelogram(corner + vz, vx, vy, frontCoords, edges, thickness);        // Front
            mesh.AddParallelogram(corner, vz, vy, leftCoords, edges, thickness);              // Left
            mesh.AddParallelogram(corner + vx + vz, -vz, vy, rightCoords, edges, thickness);  // Right
            mesh.AddParallelogram(corner + vx, -vx, vy, backCoords, edges, thickness);        // Back
            mesh.AddParallelogram(corner + vy + vz, vx, -vz, topCoords, edges, thickness);    // Top
            mesh.AddParallelogram(corner, vx, vz, bottomCoords, edges, thickness);            // Bottom
        }

        // Add a parallelepiped with wrapped texture coordinates.
        public static void AddBoxWrapped(this MeshGeometry3D mesh,
            Point3D corner, Vector3D vx, Vector3D vy, Vector3D vz,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Get texture coordinates for the pieces.
            Point[] frontCoords =
            {
                new Point(0.25, 0.75),
                new Point(0.50, 0.75),
                new Point(0.50, 0.50),
                new Point(0.25, 0.50),
            };
            Point[] leftCoords =
            {
                new Point(0.00, 0.25),
                new Point(0.00, 0.50),
                new Point(0.25, 0.50),
                new Point(0.25, 0.25),
            };
            Point[] rightCoords =
            {
                new Point(0.75, 0.50),
                new Point(0.75, 0.25),
                new Point(0.50, 0.25),
                new Point(0.50, 0.50),
            };
            Point[] backCoords =
            {
                new Point(0.50, 0.00),
                new Point(0.25, 0.00),
                new Point(0.25, 0.25),
                new Point(0.50, 0.25),
            };
            Point[] topCoords =
            {
                new Point(0.25, 0.50),
                new Point(0.50, 0.50),
                new Point(0.50, 0.25),
                new Point(0.25, 0.25),
            };
            Point[] bottomCoords =
            {
                new Point(0.25, 1.00),
                new Point(0.50, 1.00),
                new Point(0.50, 0.75),
                new Point(0.25, 0.75),
            };

            // Add a point to use all texture coordinates in the area (0, 0) - (1, 1).
            mesh.Positions.Add(new Point3D());
            mesh.TextureCoordinates.Add(new Point(1, 1));

            // Add the box.
            mesh.AddBox(corner, vx, vy, vz,
                frontCoords, leftCoords, rightCoords,
                backCoords, topCoords, bottomCoords,
                edges, thickness);
        }

        #endregion Boxes

        #region Axes

        // Make models for the coordinate axes.
        public static void AddXAxis(Model3DGroup group,
            double length = 4, double thickness = 0.1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(thickness / 2) -
                D3.YVector(thickness / 2) -
                D3.ZVector(thickness / 2);
            mesh.AddBox(origin,
                D3.XVector(length), D3.YVector(thickness), D3.ZVector(thickness));
            group.Children.Add(mesh.MakeModel(Brushes.Red));
        }

        public static void AddYAxis(Model3DGroup group,
            double length = 4, double thickness = 0.1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(thickness / 2) -
                D3.YVector(thickness / 2) -
                D3.ZVector(thickness / 2);
            mesh.AddBox(origin,
                D3.XVector(thickness), D3.YVector(length), D3.ZVector(thickness));
            group.Children.Add(mesh.MakeModel(Brushes.Green));
        }

        public static void AddZAxis(Model3DGroup group,
            double length = 4, double thickness = 0.1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(thickness / 2) -
                D3.YVector(thickness / 2) -
                D3.ZVector(thickness / 2);
            mesh.AddBox(origin,
                D3.XVector(thickness), D3.YVector(thickness), D3.ZVector(length));
            group.Children.Add(mesh.MakeModel(Brushes.Blue));
        }

        // Make a cube at the origin.
        public static void AddOrigin(Model3DGroup group,
            double cubeThickness = 0.102)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            Point3D origin = D3.Origin -
                D3.XVector(cubeThickness / 2) -
                D3.YVector(cubeThickness / 2) -
                D3.ZVector(cubeThickness / 2);
            mesh.AddBox(origin,
                D3.XVector(cubeThickness),
                D3.YVector(cubeThickness),
                D3.ZVector(cubeThickness));
            group.Children.Add(mesh.MakeModel(Brushes.Black));
        }

        // Make X, Y, and Z axes, and the origin cube.
        public static void AddAxes(Model3DGroup group,
            double length = 4, double thickness = 0.1,
            double cubeThickness = 0.102)
        {
            AddXAxis(group, length, thickness);
            AddYAxis(group, length, thickness);
            AddZAxis(group, length, thickness);
            AddOrigin(group, cubeThickness);
        }

        #endregion Axes

        #region Pyramids

        // Add a pyramid defined by a center point, a polygon, and an axis vector.
        // The polygon should be oriented toward its axis.
        public static void AddPyramid(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            bool smoothSides = false, HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Find the apex.
            Point3D apex = center + axis;

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            int numPoints = polygon.Length;
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], apex);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(pointDict, edges, thickness, null, bottom);
        }

        // Add a frustum.
        // Length is the length measured along the axis.
        public static void AddFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis, double length,
            bool smoothSides = false, HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Find the length ratio.
            double ratio = length / axis.Length;

            // See where the apex would be.
            Point3D apex = center + axis;

            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                Vector3D vector = apex - polygon[i];
                vector *= ratio;
                top[i] = polygon[i] + vector;
            }
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        // Add a frustum where the top is determined by a plane of intersection.
        // The plane is determined by the point planePt and the normal vector n.
        public static void AddFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            Point3D planePt, Vector3D n, bool smoothSides = false,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // See where the apex would be.
            Point3D apex = center + axis;

            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // Get the vector from the point to the apex.
                Vector3D vector = apex - polygon[i];

                // See where this vector intersects the plane.
                top[i] = IntersectPlaneLine(polygon[i], vector, planePt, n);
            }
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        // Find the intersection of a plane and a line.
        // The line is given by point linePt and vector v.
        // The plane is given by point planePt and normal vector n.
        private static Point3D IntersectPlaneLine(Point3D linePt, Vector3D v,
            Point3D planePt, Vector3D n, bool smoothSides = false)
        {
            // Get the equation for the plane.
            // For information on getting the plane equation, see:
            // http://www.songho.ca/math/plane/plane.html
            double A = n.X;
            double B = n.Y;
            double C = n.Z;
            double D = -(A * planePt.X + B * planePt.Y + C * planePt.Z);

            // Find the intersection parameter t.
            // For information on finding the intersection, see:
            // http://www.ambrsoft.com/TrigoCalc/Plan3D/PlaneLineIntersection_.htm
            double t = -(A * linePt.X + B * linePt.Y + C * linePt.Z + D) /
                (A * v.X + B * v.Y + C * v.Z);

            // Find the point of intersection.
            return linePt + t * v;
        }

        #endregion Pyramids

        #region Cones

        // These methods delegate their work to pyramid and frustum methods.
        private static void AddCone(this MeshGeometry3D mesh, Point3D end_point,
             Vector3D axis, double radius1, double radius2, int num_sides)
        {
            // Get two vectors perpendicular to the axis.
            Vector3D top_v1;
            if ((axis.Z < -0.01) || (axis.Z > 0.01))
                top_v1 = new Vector3D(axis.Z, axis.Z, -axis.X - axis.Y);
            else
                top_v1 = new Vector3D(-axis.Y - axis.Z, axis.X, axis.X);
            Vector3D top_v2 = Vector3D.CrossProduct(top_v1, axis);

            Vector3D bot_v1 = top_v1;
            Vector3D bot_v2 = top_v2;

            // Make the vectors have length radius.
            top_v1 *= (radius1 / top_v1.Length);
            top_v2 *= (radius1 / top_v2.Length);

            bot_v1 *= (radius2 / bot_v1.Length);
            bot_v2 *= (radius2 / bot_v2.Length);

            // Make the top end cap.
            double theta = 0;
            double dtheta = 2 * Math.PI / num_sides;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                AddTriangle(mesh, end_point, p1, p2);
            }

            // Make the bottom end cap.
            Point3D end_point2 = end_point + axis;
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point2 +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                AddTriangle(mesh, end_point2, p2, p1);
            }

            // Make the sides.
            theta = 0;
            for (int i = 0; i < num_sides; i++)
            {
                Point3D p1 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p3 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;
                theta += dtheta;
                Point3D p2 = end_point +
                    Math.Cos(theta) * top_v1 +
                    Math.Sin(theta) * top_v2;
                Point3D p4 = end_point + axis +
                    Math.Cos(theta) * bot_v1 +
                    Math.Sin(theta) * bot_v2;

                AddTriangle(mesh, p1, p3, p2);
                AddTriangle(mesh, p2, p3, p4);
            }
        }

        public static void AddConeFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis, double length,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            mesh.AddFrustum(center, polygon, axis, length, true, edges, thickness);
        }
        public static void AddConeFrustum(this MeshGeometry3D mesh,
            Point3D center, Point3D[] polygon, Vector3D axis,
            Point3D planePt, Vector3D n,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            mesh.AddFrustum(center, polygon, axis, planePt, n, true, edges, thickness);
        }

        #endregion Cones

        #region Cylinders

        // Add a cylinder defined by a center point, a polygon, and an axis vector.
        // The cylinder should be oriented toward its axis.
        public static void AddCylinder(this MeshGeometry3D mesh,
            Point3D[] polygon, Vector3D axis, bool smoothSides = false,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                top[i] = polygon[i] + axis;
            }
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    polygon[i], polygon[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            Array.Copy(polygon, bottom, numPoints);
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        // Add a cylinder defined by a polygon, two axis, and two cutting planes.
        public static void AddCylinder(this MeshGeometry3D mesh,
            Point3D[] polygon, Vector3D axis,
            Point3D topPlanePt, Vector3D topN,
            Point3D bottomPlanePt, Vector3D bottomN,
            bool smoothSides = false,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Make the top.
            int numPoints = polygon.Length;
            Point3D[] top = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // See where this vector intersects the top cutting plane.
                top[i] = IntersectPlaneLine(polygon[i], axis, topPlanePt, topN);
            }
            mesh.AddPolygon(points: top, edges: edges, thickness: thickness);

            // Make the bottom.
            Point3D[] bottom = new Point3D[numPoints];
            for (int i = 0; i < polygon.Length; i++)
            {
                // See where this vector intersects the bottom cutting plane.
                bottom[i] = IntersectPlaneLine(polygon[i], axis, bottomPlanePt, bottomN);
            }

            // If we should smooth the sides, make the point dictionary.
            Dictionary<Point3D, int> pointDict = null;
            if (smoothSides) pointDict = new Dictionary<Point3D, int>();

            // Make the sides.
            for (int i = 0; i < polygon.Length; i++)
            {
                int i1 = (i + 1) % numPoints;
                mesh.AddPolygon(pointDict, edges, thickness, null,
                    bottom[i], bottom[i1], top[i1], top[i]);
            }

            // Make the bottom.
            Array.Reverse(bottom);
            mesh.AddPolygon(points: bottom, edges: edges, thickness: thickness);
        }

        #endregion Cylinders

        #region Spheres

        // Add a sphere without texture coordinates.
        public static void AddSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numTheta, int numPhi,
            bool smooth = false,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Make a point dictionary if needed.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // Generate the points.
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = Math.PI / numPhi;
            double theta = 0;
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D[] points =
                    {
                        G3.SpherePoint(center, radius, theta, phi),
                        G3.SpherePoint(center, radius, theta, phi + dphi),
                        G3.SpherePoint(center, radius, theta + dtheta, phi + dphi),
                        G3.SpherePoint(center, radius, theta + dtheta, phi),
                    };

                    // Make the polygon.
                    mesh.AddPolygon(pointDict: pointDict,
                        edges: edges, thickness: thickness, points: points.Reverse().ToArray());
                    //@mesh.AddPolygon(pointDict: pointDict, points: points);

                    phi += dphi;
                }
                theta += dtheta;
            }
        }

        // Add a sphere with texture coordinates.
        public static void AddTexturedSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numTheta, int numPhi,
            bool smooth = false)
        {
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = Math.PI / numPhi;
            double theta = 0;
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D point1 = G3.SpherePoint(center, radius, theta, phi).Round();
                    Point3D point2 = G3.SpherePoint(center, radius, theta, phi + dphi).Round();
                    Point3D point3 = G3.SpherePoint(center, radius, theta + dtheta, phi + dphi).Round();
                    Point3D point4 = G3.SpherePoint(center, radius, theta + dtheta, phi).Round();

                    // Find this piece's texture coordinates.
                    Point coords1 = new Point((double)t / numTheta, (double)p / numPhi);
                    Point coords2 = new Point((double)t / numTheta, (double)(p + 1) / numPhi);
                    Point coords3 = new Point((double)(t + 1) / numTheta, (double)(p + 1) / numPhi);
                    Point coords4 = new Point((double)(t + 1) / numTheta, (double)p / numPhi);

                    // Find this piece's normals.
                    Vector3D normal1 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta, phi).Round();
                    Vector3D normal2 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta, phi + dphi).Round();
                    Vector3D normal3 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta + dtheta, phi + dphi).Round();
                    Vector3D normal4 = (Vector3D)G3.SpherePoint(D3.Origin, 1, theta + dtheta, phi).Round();

                    // Make the first triangle.
                    int index = mesh.Positions.Count;
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point2);
                    if (smooth) mesh.Normals.Add(normal2);
                    mesh.TextureCoordinates.Add(coords2);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    // Make the second triangle.
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.Positions.Add(point4);
                    if (smooth) mesh.Normals.Add(normal4);
                    mesh.TextureCoordinates.Add(coords4);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    phi += dphi;
                }
                theta += dtheta;
            }
        }

        #endregion Spheres

        #region Tori

        // Make a torus without texture coordinates.
        public static void AddTorus(this MeshGeometry3D mesh,
            Point3D center, double R, double r, int numTheta, int numPhi,
            bool smooth = false,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Make a point dictionary if needed.
            Dictionary<Point3D, int> pointDict = null;
            if (smooth) pointDict = new Dictionary<Point3D, int>();

            // Generate the points.
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = 2 * Math.PI / numPhi;
            double theta = 0;
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D[] points =
                    {
                        G3.TorusPoint(center, R, r, theta + dtheta, phi),
                        G3.TorusPoint(center, R, r, theta + dtheta, phi + dphi),
                        G3.TorusPoint(center, R, r, theta, phi + dphi),
                        G3.TorusPoint(center, R, r, theta, phi),
                    };

                    // Make the polygon.
                    mesh.AddPolygon(pointDict: pointDict, points: points,
                        edges: edges, thickness: thickness);

                    phi += dphi;
                }
                theta += dtheta;
            }
        }

        // Add a textured torus.
        public static void AddTexturedTorus(this MeshGeometry3D mesh,
            Point3D center, double R, double r, int numTheta, int numPhi,
            bool smooth = false)
        {
            double dtheta = 2 * Math.PI / numTheta;
            double dphi = 2 * Math.PI / numPhi;
            double theta = Math.PI;         // Puts the texture's top/bottom on the inside.
            for (int t = 0; t < numTheta; t++)
            {
                double phi = 0;
                for (int p = 0; p < numPhi; p++)
                {
                    // Find this piece's points.
                    Point3D point1 = G3.TorusPoint(center, R, r, theta, phi).Round();
                    Point3D point2 = G3.TorusPoint(center, R, r, theta + dtheta, phi).Round();
                    Point3D point3 = G3.TorusPoint(center, R, r, theta + dtheta, phi + dphi).Round();
                    Point3D point4 = G3.TorusPoint(center, R, r, theta, phi + dphi).Round();

                    // Find this piece's normals.
                    Vector3D normal1 = G3.TorusNormal(D3.Origin, R, r, theta, phi);
                    Vector3D normal2 = G3.TorusNormal(D3.Origin, R, r, theta + dtheta, phi);
                    Vector3D normal3 = G3.TorusNormal(D3.Origin, R, r, theta + dtheta, phi + dphi);
                    Vector3D normal4 = G3.TorusNormal(D3.Origin, R, r, theta, phi + dphi);

                    // Find this piece's texture coordinates.
                    Point coords1 = new Point(1 - (double)p / numPhi, 1 - (double)t / numTheta);
                    Point coords2 = new Point(1 - (double)p / numPhi, 1 - (double)(t + 1) / numTheta);
                    Point coords3 = new Point(1 - (double)(p + 1) / numPhi, 1 - (double)(t + 1) / numTheta);
                    Point coords4 = new Point(1 - (double)(p + 1) / numPhi, 1 - (double)t / numTheta);

                    // Make the first triangle.
                    int index = mesh.Positions.Count;
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point2);
                    if (smooth) mesh.Normals.Add(normal2);
                    mesh.TextureCoordinates.Add(coords2);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    // Make the second triangle.
                    mesh.Positions.Add(point1);
                    if (smooth) mesh.Normals.Add(normal1);
                    mesh.TextureCoordinates.Add(coords1);

                    mesh.Positions.Add(point3);
                    if (smooth) mesh.Normals.Add(normal3);
                    mesh.TextureCoordinates.Add(coords3);

                    mesh.Positions.Add(point4);
                    if (smooth) mesh.Normals.Add(normal4);
                    mesh.TextureCoordinates.Add(coords4);

                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);
                    mesh.TriangleIndices.Add(index++);

                    phi += dphi;
                }
                theta += dtheta;
            }

            // Add texture coordinates 1.01 to prevent "seams."
            mesh.Positions.Add(new Point3D());
            mesh.TextureCoordinates.Add(new Point(1.01, 1.01));
        }

        #endregion Tori

        #region Platonic Solids

        // Make a tetrahedron without texture coordinates or smoothing.
        public static void AddTetrahedron(this MeshGeometry3D mesh, bool centered = true,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Get the points.
            G3.TetrahedronPoints(out Point3D A, out Point3D B, out Point3D C, out Point3D D, centered);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, B, C);
            mesh.AddPolygon(edges, thickness, A, C, D);
            mesh.AddPolygon(edges, thickness, A, D, B);
            mesh.AddPolygon(edges, thickness, D, C, B);
        }
        public static void VerifyTetrahedron()
        {
            // Get the points.
            G3.TetrahedronPoints(out Point3D A, out Point3D B, out Point3D C, out Point3D D, true);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D);

            // Verify the faces.
            G3.VerifyPolygon(A, B, C);
            G3.VerifyPolygon(A, C, D);
            G3.VerifyPolygon(A, D, B);
            G3.VerifyPolygon(D, C, B);
        }

        // Make a cube without texture coordinates or smoothing.
        public static void AddCube(this MeshGeometry3D mesh,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Get the points.
            G3.CubePoints(out Point3D A, out Point3D B, out Point3D C, out Point3D D,
                out Point3D E, out Point3D F, out Point3D G, out Point3D H);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, B, C, D);
            mesh.AddPolygon(edges, thickness, A, D, H, E);
            mesh.AddPolygon(edges, thickness, A, E, F, B);
            mesh.AddPolygon(edges, thickness, G, C, B, F);
            mesh.AddPolygon(edges, thickness, G, F, E, H);
            mesh.AddPolygon(edges, thickness, G, H, D, C);
        }
        public static void VerifyCube()
        {
            // Get the points.
            G3.CubePoints(out Point3D A, out Point3D B, out Point3D C, out Point3D D,
                out Point3D E, out Point3D F, out Point3D G, out Point3D H);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D, E, F, G, H);

            // Verify the faces.
            G3.VerifyPolygon(A, B, C, D);
            G3.VerifyPolygon(A, D, H, E);
            G3.VerifyPolygon(A, E, F, B);
            G3.VerifyPolygon(G, C, B, F);
            G3.VerifyPolygon(G, F, E, H);
            G3.VerifyPolygon(G, H, D, C);
        }

        // Make an octahedron without texture coordinates or smoothing.
        public static void AddOctahedron(this MeshGeometry3D mesh,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Get the points.
            G3.OctahedronPoints(out Point3D A, out Point3D B, out Point3D C, out Point3D D, out Point3D E, out Point3D F);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, B, C);
            mesh.AddPolygon(edges, thickness, A, C, D);
            mesh.AddPolygon(edges, thickness, A, D, E);
            mesh.AddPolygon(edges, thickness, A, E, B);
            mesh.AddPolygon(edges, thickness, F, B, E);
            mesh.AddPolygon(edges, thickness, F, C, B);
            mesh.AddPolygon(edges, thickness, F, D, C);
            mesh.AddPolygon(edges, thickness, F, E, D);
        }
        public static void VerifyOctahedron()
        {
            // Get the points.
            Point3D A, B, C, D, E, F;
            G3.OctahedronPoints(out A, out B, out C, out D, out E, out F);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D);

            // Verify the faces.
            G3.VerifyPolygon(A, B, C);
            G3.VerifyPolygon(A, C, D);
            G3.VerifyPolygon(A, D, E);
            G3.VerifyPolygon(A, E, B);
            G3.VerifyPolygon(F, B, E);
            G3.VerifyPolygon(F, C, B);
            G3.VerifyPolygon(F, D, C);
            G3.VerifyPolygon(F, E, D);
        }

        // Make a dodecahedron without texture coordinates or smoothing.
        public static void AddDodecahedron(this MeshGeometry3D mesh,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T;
            G3.DodecahedronPoints(
                out A, out B, out C, out D, out E,
                out F, out G, out H, out I, out J,
                out K, out L, out M, out N, out O,
                out P, out Q, out R, out S, out T);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, E, D, C, B, A);
            mesh.AddPolygon(edges, thickness, A, B, G, K, F);
            mesh.AddPolygon(edges, thickness, A, F, O, J, E);
            mesh.AddPolygon(edges, thickness, E, J, N, I, D);
            mesh.AddPolygon(edges, thickness, D, I, M, H, C);
            mesh.AddPolygon(edges, thickness, C, H, L, G, B);
            mesh.AddPolygon(edges, thickness, K, P, T, O, F);
            mesh.AddPolygon(edges, thickness, O, T, S, N, J);
            mesh.AddPolygon(edges, thickness, N, S, R, M, I);
            mesh.AddPolygon(edges, thickness, M, R, Q, L, H);
            mesh.AddPolygon(edges, thickness, L, Q, P, K, G);
            mesh.AddPolygon(edges, thickness, P, Q, R, S, T);
        }
        public static void VerifyDodecahedron()
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T;
            G3.DodecahedronPoints(
                out A, out B, out C, out D, out E,
                out F, out G, out H, out I, out J,
                out K, out L, out M, out N, out O,
                out P, out Q, out R, out S, out T);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T);

            // Verify the faces.
            G3.VerifyPolygon(E, D, C, B, A);
            G3.VerifyPolygon(A, B, G, K, F);
            G3.VerifyPolygon(A, F, O, J, E);
            G3.VerifyPolygon(E, J, N, I, D);
            G3.VerifyPolygon(D, I, M, H, C);
            G3.VerifyPolygon(C, H, L, G, B);
            G3.VerifyPolygon(K, P, T, O, F);
            G3.VerifyPolygon(O, T, S, N, J);
            G3.VerifyPolygon(N, S, R, M, I);
            G3.VerifyPolygon(M, R, Q, L, H);
            G3.VerifyPolygon(L, Q, P, K, G);
            G3.VerifyPolygon(P, Q, R, S, T);
        }

        // Make an icosahedron without texture coordinates or smoothing.
        public static void AddIcosahedron(this MeshGeometry3D mesh,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out L);

            // Make the faces.
            mesh.AddPolygon(edges, thickness, A, C, B);
            mesh.AddPolygon(edges, thickness, A, D, C);
            mesh.AddPolygon(edges, thickness, A, E, D);
            mesh.AddPolygon(edges, thickness, A, F, E);
            mesh.AddPolygon(edges, thickness, A, B, F);
            mesh.AddPolygon(edges, thickness, D, K, C);
            mesh.AddPolygon(edges, thickness, C, K, J);
            mesh.AddPolygon(edges, thickness, C, J, B);
            mesh.AddPolygon(edges, thickness, B, J, I);
            mesh.AddPolygon(edges, thickness, B, I, F);
            mesh.AddPolygon(edges, thickness, F, I, H);
            mesh.AddPolygon(edges, thickness, F, H, E);
            mesh.AddPolygon(edges, thickness, E, H, G);
            mesh.AddPolygon(edges, thickness, E, G, D);
            mesh.AddPolygon(edges, thickness, D, G, K);
            mesh.AddPolygon(edges, thickness, L, J, K);
            mesh.AddPolygon(edges, thickness, L, I, J);
            mesh.AddPolygon(edges, thickness, L, H, I);
            mesh.AddPolygon(edges, thickness, L, G, H);
            mesh.AddPolygon(edges, thickness, L, K, G);
        }
        public static void VerifyIcosahedron()
        {
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K, L;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out L);

            // Verify the points.
            G3.VerifyPoints(A, B, C, D, E, F, G, H, I, J, K, L);

            // Verify the faces.
            G3.VerifyPolygon(A, C, B);
            G3.VerifyPolygon(A, D, C);
            G3.VerifyPolygon(A, E, D);
            G3.VerifyPolygon(A, F, E);
            G3.VerifyPolygon(A, B, F);
            G3.VerifyPolygon(D, K, C);
            G3.VerifyPolygon(C, K, J);
            G3.VerifyPolygon(C, J, B);
            G3.VerifyPolygon(B, J, I);
            G3.VerifyPolygon(B, I, F);
            G3.VerifyPolygon(F, I, H);
            G3.VerifyPolygon(F, H, E);
            G3.VerifyPolygon(E, H, G);
            G3.VerifyPolygon(E, G, D);
            G3.VerifyPolygon(D, G, K);
            G3.VerifyPolygon(L, J, K);
            G3.VerifyPolygon(L, I, J);
            G3.VerifyPolygon(L, H, I);
            G3.VerifyPolygon(L, G, H);
            G3.VerifyPolygon(L, K, G);
        }

        #endregion Platonic Solids

        #region Wireframe

        // Make a thin line segment.
        public static void AddSegment(this MeshGeometry3D mesh,
            double thickness, Point3D point1, Point3D point2)
        {
            // Get a vector between the points.
            Vector3D v = point2 - point1;

            // Get perpendicular vectors.
            Vector3D vz, vx;
            double angle = Vector3D.AngleBetween(v, D3.YVector());
            if ((angle > 10) && (angle < 170))
                vz = Vector3D.CrossProduct(v, D3.YVector());
            else
                vz = Vector3D.CrossProduct(v, D3.ZVector());
            vx = Vector3D.CrossProduct(v, vz);

            // Give the perpendicular vectors length thickness.
            vx *= thickness / vx.Length;
            vz *= thickness / vz.Length;

            // Make the box.
            mesh.AddBox(point1 - vx / 2 - vz / 2, vx, v, vz);
        }

        // Add a wireframe edge to this mesh.
        public static void AddEdge(this MeshGeometry3D mesh,
            HashSet<Edges> edges, double thickness, Point3D point1, Point3D point2)
        {
            // If the points are the same, skip it.
            if (point1 == point2) return;

            // See if the edge is already in the HashSet.
            Edges edge = new Edges(point1, point2);
            if (edges.Contains(edge)) return;

            // Add the edge.
            edges.Add(edge);
            mesh.AddSegment(thickness, point1, point2);
        }

        // Add a polygon's wireframe to this mesh.
        public static void AddPolygonEdges(this MeshGeometry3D mesh,
            HashSet<Edges> edges, double thickness, params Point3D[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                int i1 = (i + 1) % points.Length;
                mesh.AddEdge(edges, thickness, points[i], points[i1]);
            }
        }

        // Convert a mesh into a new mesh containing a wireframe.
        public static MeshGeometry3D ToWireframe(this MeshGeometry3D mesh, double thickness)
        {
            // Make a dictionary of edges.
            HashSet<Edges> edges = new HashSet<Edges>();

            // Make the wireframe pieces.
            MeshGeometry3D result = new MeshGeometry3D();
            for (int i = 0; i < mesh.TriangleIndices.Count; i += 3)
            {
                Point3D point1 = mesh.Positions[mesh.TriangleIndices[i]];
                Point3D point2 = mesh.Positions[mesh.TriangleIndices[i + 1]];
                Point3D point3 = mesh.Positions[mesh.TriangleIndices[i + 2]];
                result.AddPolygonEdges(edges, thickness, point1, point2, point3);
            }
            return result;
        }

        #endregion Wireframe

        #region Geodesic Sphere

        // Add a geodesic sphere.
        public static void AddGeodesicSphere(this MeshGeometry3D mesh,
            Point3D center, double radius, int numDivisions,
            HashSet<Edges> edges = null, double thickness = 0.1)
        {
            // Create an icosahedron.
            // Get the points.
            Point3D A, B, C, D, E, F, G, H, I, J, K;
            G3.IcosahedronPoints(
                out A, out B, out C, out D, out E, out F,
                out G, out H, out I, out J, out K, out Point3D L);

            // Scale the icosahedron to the proper radius and center it.
            double scale = radius / G3.IcosahedronCircumradius();
            ScaleTransform3D scaleT = new ScaleTransform3D(scale, scale, scale);
            TranslateTransform3D translateT = new TranslateTransform3D(center.X, center.Y, center.Z);
            Transform3DGroup groupT = new Transform3DGroup();
            groupT.Children.Add(scaleT);
            groupT.Children.Add(translateT);
            A = groupT.Transform(A);
            B = groupT.Transform(B);
            C = groupT.Transform(C);
            D = groupT.Transform(D);
            E = groupT.Transform(E);
            F = groupT.Transform(F);
            G = groupT.Transform(G);
            H = groupT.Transform(H);
            I = groupT.Transform(I);
            J = groupT.Transform(J);
            K = groupT.Transform(K);
            L = groupT.Transform(L);

            // Make the icosahedron's faces.
            List<Triangles> triangles = new List<Triangles>
            {
                new Triangles(A, C, B),
                new Triangles(A, D, C),
                new Triangles(A, E, D),
                new Triangles(A, F, E),
                new Triangles(A, B, F),
                new Triangles(D, K, C),
                new Triangles(C, K, J),
                new Triangles(C, J, B),
                new Triangles(B, J, I),
                new Triangles(B, I, F),
                new Triangles(F, I, H),
                new Triangles(F, H, E),
                new Triangles(E, H, G),
                new Triangles(E, G, D),
                new Triangles(D, G, K),
                new Triangles(L, J, K),
                new Triangles(L, I, J),
                new Triangles(L, H, I),
                new Triangles(L, G, H),
                new Triangles(L, K, G)
            };

            // Subdivide the faces as desired.
            List<Triangles> newTriangles = new List<Triangles>();
            foreach (Triangles triangle in triangles)
            {
                // Subdivide this triangle and add the results to newTriangles.
                newTriangles.AddRange(triangle.DivideGeodesic(center, radius, numDivisions));
            }

            // Create the geodesic sphere.
            foreach (Triangles triangle in newTriangles)
            {
                mesh.AddPolygon(edges, thickness, triangle.Points.ToArray());
            }
        }

        #endregion Geodesic Sphere

        #region Axis
        public static MeshGeometry3D XAxisArrow(double length)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), new Point3D(length, 0, 0),
                new Vector3D(0, 1, 0), 0.5);
            return mesh;
        }


        internal static MeshGeometry3D XAxisArrow(Params cellParams, double length = 1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), cellParams.ToCartesian(new Point3D(length, 0, 0)),
                new Vector3D(0, 1, 0), 0.5);
            return mesh;
        }
        public static MeshGeometry3D YAxisArrow(double length)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), new Point3D(0, length, 0),
                new Vector3D(1, 0, 0), 0.5);
            return mesh;
        }

        public static MeshGeometry3D YAxisArrow(Params cellParams, double length = 1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), cellParams.ToCartesian(new Point3D(0, length, 0)),
                new Vector3D(1, 0, 0));
            return mesh;
        }
        public static MeshGeometry3D ZAxisArrow(double length)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), new Point3D(0, 0, length),
                new Vector3D(0, 1, 0));
            return mesh;
        }

        public static MeshGeometry3D ZAxisArrow(Params cellParams, double length = 1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.AddArrow(new Point3D(0, 0, 0), cellParams.ToCartesian(new Point3D(0, 0, length)),
                new Vector3D(0, 1, 0));
            return mesh;
        }

        public static void AddArrow(this MeshGeometry3D mesh,
           Point3D point1, Point3D point2, Vector3D up,
           double cone_length = 0.7, double cone_radius = 0.2)
        {
            // Make the shaft.
            AddSegment(mesh, point1, point2, 0.05, true);

            // Get a unit vector in the direction of the segment.
            Vector3D v = point2 - point1;
            v.Normalize();
            mesh.AddCone(point2, v * cone_length, cone_radius, 0, 20);

            /* v.Normalize();

            // Get a perpendicular unit vector in the plane of the arrowhead.
            Vector3D perp = Vector3D.CrossProduct(v, up);
            perp.Normalize();

            // Calculate the arrowhead end points.
            Vector3D v1 = ScaleVector(-v + perp, barb_length);
            Vector3D v2 = ScaleVector(-v - perp, barb_length);

            // Draw the arrowhead.
            AddSegment(mesh, point2, point2 + v1, up, 0.05);
            AddSegment(mesh, point2, point2 + v2, up, 0.05);*/
        }
        // Give the mesh a diffuse material.
        public static GeometryModel3D SetMaterial(this MeshGeometry3D mesh, Brush brush, bool set_back_material)
        {
            DiffuseMaterial material = new DiffuseMaterial(brush);
            GeometryModel3D model = new GeometryModel3D(mesh, material);
            if (set_back_material) model.BackMaterial = material;
            return model;
        }


        // Make a thin rectangular prism between the two points.
        // If extend is true, extend the segment by half the
        // thickness so segments with the same end points meet nicely.
        // If up is missing, create a perpendicular vector to use.
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, double thickness, bool extend)
        {
            // Find an up vector that is not colinear with the segment.
            // Start with a vector parallel to the Y axis.
            Vector3D up = new Vector3D(0, 1, 0);

            // If the segment and up vector point in more or less the
            // same direction, use an up vector parallel to the X axis.
            Vector3D segment = point2 - point1;
            segment.Normalize();
            if (Math.Abs(Vector3D.DotProduct(up, segment)) > 0.9)
                up = new Vector3D(1, 0, 0);

            // Add the segment.
            AddSegment(mesh, point1, point2, up, thickness, extend);
        }

        public static void AddSegment(MeshGeometry3D mesh,
           Point3D point1, Point3D point2, double thickness)
        {
            AddSegment(mesh, point1, point2, thickness, false);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness)
        {
            AddSegment(mesh, point1, point2, up, thickness, false);
        }
        public static void AddSegment(MeshGeometry3D mesh,
            Point3D point1, Point3D point2, Vector3D up, double thickness,
            bool extend)
        {
            // Get the segment's vector.
            Vector3D v = point2 - point1;

            if (extend)
            {
                // Increase the segment's length on both ends by thickness / 2.
                Vector3D n = ScaleVector(v, thickness / 2.0);
                point1 -= n;
                point2 += n;
            }

            // Get the scaled up vector.
            Vector3D n1 = ScaleVector(up, thickness / 2.0);

            // Get another scaled perpendicular vector.
            Vector3D n2 = Vector3D.CrossProduct(v, n1);
            n2 = ScaleVector(n2, thickness / 2.0);

            // Make a skinny box.
            // p1pm means point1 PLUS n1 MINUS n2.
            Point3D p1pp = point1 + n1 + n2;
            Point3D p1mp = point1 - n1 + n2;
            Point3D p1pm = point1 + n1 - n2;
            Point3D p1mm = point1 - n1 - n2;
            Point3D p2pp = point2 + n1 + n2;
            Point3D p2mp = point2 - n1 + n2;
            Point3D p2pm = point2 + n1 - n2;
            Point3D p2mm = point2 - n1 - n2;

            // Sides.
            AddTriangle(mesh, p1pp, p1mp, p2mp);
            AddTriangle(mesh, p1pp, p2mp, p2pp);

            AddTriangle(mesh, p1pp, p2pp, p2pm);
            AddTriangle(mesh, p1pp, p2pm, p1pm);

            AddTriangle(mesh, p1pm, p2pm, p2mm);
            AddTriangle(mesh, p1pm, p2mm, p1mm);

            AddTriangle(mesh, p1mm, p2mm, p2mp);
            AddTriangle(mesh, p1mm, p2mp, p1mp);

            // Ends.
            AddTriangle(mesh, p1pp, p1pm, p1mm);
            AddTriangle(mesh, p1pp, p1mm, p1mp);

            AddTriangle(mesh, p2pp, p2mp, p2mm);
            AddTriangle(mesh, p2pp, p2mm, p2pm);
        }

        // Add a triangle to the indicated mesh.
        // Do not reuse points so triangles don't share normals.
        private static void AddTriangle(MeshGeometry3D mesh, Point3D point1, Point3D point2, Point3D point3)
        {
            // Create the points.
            int index1 = mesh.Positions.Count;
            mesh.Positions.Add(point1);
            mesh.Positions.Add(point2);
            mesh.Positions.Add(point3);

            // Create the triangle.
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1++);
            mesh.TriangleIndices.Add(index1);
        }

        // Set the vector's length.
        public static Vector3D ScaleVector(Vector3D vector, double length)
        {
            double scale = length / vector.Length;
            return new Vector3D(
                vector.X * scale,
                vector.Y * scale,
                vector.Z * scale);
        }

        public static MeshGeometry3D Border(Params cellParams, int x = 1, int y = 1, int z = 1)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    for (int k = 0; k < z; k++)
                    {
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 0)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 0, k + 0)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 1, j + 1, k + 1)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 0, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 1, j + 1, k + 1)),
                            cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 1, j + 1, k + 1)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 1, k + 0)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 1, k + 0)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 1)),
                            cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 1)),
                            cellParams.ToCartesian(new Point3D(i + 0, j + 1, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 0, j + 0, k + 1)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 0, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 1, j + 0, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 0, k + 1)));
                        mesh.AddSegment(border_thickness, cellParams.ToCartesian(new Point3D(i + 1, j + 0, k + 0)),
                            cellParams.ToCartesian(new Point3D(i + 1, j + 1, k + 0)));
                    }
                }
            }

            return mesh;
        }

        public static Model3D CreateTextLabel3D(
       string text,
       Brush textColor,
       bool isDoubleSided,
       double height,
       Point3D basePoint,
       bool isBasePointCenterPoint,
       Vector3D vectorOver,
       Vector3D vectorUp)
        {
            // First we need a textbox containing the text of our label
            TextBlock textblock = new TextBlock(new Run(text));
            textblock.Foreground = textColor; // setting the text color
            textblock.FontFamily = new FontFamily("Arial"); // setting the font to be used
                                                            // Now use that TextBox as the brush for a material
            DiffuseMaterial mataterialWithLabel = new DiffuseMaterial();
            // Allows the application of a 2-D brush, 
            // like a SolidColorBrush or TileBrush, to a diffusely-lit 3-D model. 
            // we are creating the brush from the TextBlock    
            mataterialWithLabel.Brush = new VisualBrush(textblock);
            //calculation of text width (assumming that characters are square):
            double width = text.Length * height;
            // we need to find the four corners
            // p0: the lower left corner;  p1: the upper left
            // p2: the lower right; p3: the upper right
            Point3D p0 = basePoint;
            // when the base point is the center point we have to set it up in different way
            if (isBasePointCenterPoint)
                p0 = basePoint - width / 2 * vectorOver - height / 2 * vectorUp;
            Point3D p1 = p0 + vectorUp * 1 * height;
            Point3D p2 = p0 + vectorOver * width;
            Point3D p3 = p0 + vectorUp * 1 * height + vectorOver * width;
            // we are going to create object in 3D now:
            // this object will be painted using the (text) brush created before
            // the object is rectangle made of two triangles (on each side).
            MeshGeometry3D mg_RestangleIn3D = new MeshGeometry3D();
            mg_RestangleIn3D.Positions = new Point3DCollection();
            mg_RestangleIn3D.Positions.Add(p0);    // 0
            mg_RestangleIn3D.Positions.Add(p1);    // 1
            mg_RestangleIn3D.Positions.Add(p2);    // 2
            mg_RestangleIn3D.Positions.Add(p3);    // 3
                                                   // when we want to see the text on both sides:
            if (isDoubleSided)
            {
                mg_RestangleIn3D.Positions.Add(p0);    // 4
                mg_RestangleIn3D.Positions.Add(p1);    // 5
                mg_RestangleIn3D.Positions.Add(p2);    // 6
                mg_RestangleIn3D.Positions.Add(p3);    // 7
            }
            mg_RestangleIn3D.TriangleIndices.Add(0);
            mg_RestangleIn3D.TriangleIndices.Add(3);
            mg_RestangleIn3D.TriangleIndices.Add(1);
            mg_RestangleIn3D.TriangleIndices.Add(0);
            mg_RestangleIn3D.TriangleIndices.Add(2);
            mg_RestangleIn3D.TriangleIndices.Add(3);
            // when we want to see the text on both sides:
            if (isDoubleSided)
            {
                mg_RestangleIn3D.TriangleIndices.Add(4);
                mg_RestangleIn3D.TriangleIndices.Add(5);
                mg_RestangleIn3D.TriangleIndices.Add(7);
                mg_RestangleIn3D.TriangleIndices.Add(4);
                mg_RestangleIn3D.TriangleIndices.Add(7);
                mg_RestangleIn3D.TriangleIndices.Add(6);
            }
            // texture coordinates must be set to
            // stretch the TextBox brush to cover 
            // the full side of the 3D label.
            mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 1));
            mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 0));
            mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 1));
            mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 0));
            // when the label is double sided:
            if (isDoubleSided)
            {
                mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 1));
                mg_RestangleIn3D.TextureCoordinates.Add(new Point(1, 0));
                mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 1));
                mg_RestangleIn3D.TextureCoordinates.Add(new Point(0, 0));
            }
            return new GeometryModel3D(mg_RestangleIn3D, mataterialWithLabel);
        }

        #endregion

    }
}
