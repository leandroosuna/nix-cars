using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.MediaFoundation.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nix_cars.Components.Collisions
{
    public static class CollisionHelper
    {
        public static bool isTriangleItersectingOBB(CollisionTriangle triangle, OrientedBoundingBox obb)
        {

            triangle.v[0] = obb.ToOBBSpace(triangle.v[0]);
            triangle.v[1] = obb.ToOBBSpace(triangle.v[1]);
            triangle.v[2] = obb.ToOBBSpace(triangle.v[2]);

            // Now test against axis-aligned box (since we're in OBB space)
            BoundingBox localAABB = new BoundingBox(-obb.Extents, obb.Extents);

            return IsTriangleIntersectingAABB(triangle, localAABB);
        }

        public static bool IsTriangleIntersectingAABB(CollisionTriangle triangle, BoundingBox aabb)
        {
            // Step 1: Check if the triangle’s points are inside the AABB
            if (IsPointInAABB(triangle.v[0], aabb) ||
                IsPointInAABB(triangle.v[1], aabb) ||
                IsPointInAABB(triangle.v[2], aabb))
            {
                return true;
            }

            // Step 2: Check for overlap between triangle and AABB on all axes
            Vector3[] aabbAxes = { Vector3.UnitX, Vector3.UnitY, Vector3.UnitZ };

            // Test overlap on AABB's X, Y, Z axes
            foreach (var axis in aabbAxes)
            {
                if (!OverlapsOnAxis(triangle, aabb, axis))
                    return false; // Separation found
            }

            // Step 3: Check triangle's normal as a separating axis
            Vector3 triangleNormal = triangle.GetNormal();
            if (!OverlapsOnAxis(triangle, aabb, triangleNormal))
                return false; // Separation found

            // Step 4: Check cross products of triangle edges and AABB axes
            Vector3[] triangleEdges = { triangle.v[1] - triangle.v[0], triangle.v[2] - triangle.v[1], triangle.v[0] - triangle.v[2] };

            foreach (var edge in triangleEdges)
            {
                foreach (var axis in aabbAxes)
                {
                    Vector3 crossAxis = Vector3.Cross(edge, axis);
                    if (!OverlapsOnAxis(triangle, aabb, crossAxis))
                        return false; // Separation found
                }
            }

            // If no separation axis is found, the triangle and AABB are intersecting
            return true;
        }

        private static bool IsPointInAABB(Vector3 point, BoundingBox aabb)
        {
            return (point.X >= aabb.Min.X && point.X <= aabb.Max.X &&
                    point.Y >= aabb.Min.Y && point.Y <= aabb.Max.Y &&
                    point.Z >= aabb.Min.Z && point.Z <= aabb.Max.Z);
        }

        private static bool OverlapsOnAxis(CollisionTriangle triangle, BoundingBox aabb, Vector3 axis)
        {
            // Project the AABB onto the axis
            (float minAABB, float maxAABB) = ProjectAABBOnAxis(aabb, axis);

            // Project the triangle onto the same axis
            (float minTriangle, float maxTriangle) = ProjectTriangleOnAxis(triangle, axis);

            // Check if the projections overlap
            return !(minTriangle > maxAABB || maxTriangle < minAABB);
        }

        private static (float, float) ProjectAABBOnAxis(BoundingBox aabb, Vector3 axis)
        {
           // Project all 8 corners of the AABB onto the axis and get min/max values
            Vector3[] corners = aabb.GetCorners();
            float min = Vector3.Dot(corners[0], axis);
            float max = min;

            for (int i = 1; i < corners.Length; i++)
            {
                float projection = Vector3.Dot(corners[i], axis);
                min = Math.Min(min, projection);
                max = Math.Max(max, projection);
            }

            return (min, max);
        }

        private static (float, float) ProjectTriangleOnAxis(CollisionTriangle triangle, Vector3 axis)
        {
            // Project each vertex of the triangle onto the axis
            float p1 = Vector3.Dot(triangle.v[0], axis);
            float p2 = Vector3.Dot(triangle.v[1], axis);
            float p3 = Vector3.Dot(triangle.v[2], axis);

            float min = Math.Min(p1, Math.Min(p2, p3));
            float max = Math.Max(p1, Math.Max(p2, p3));

            return (min, max);
        }

        public static List<CollisionTriangle> mapTriangles = new List<CollisionTriangle>();
        public static List<CollisionTriangle> mapWallTriangles = new List<CollisionTriangle>();
        public static List<CollisionTriangle> mapFloorTriangles = new List<CollisionTriangle>();

        public static List<BoundingSphere>[] boundingSpheresMP;
        public static void BuildMapCollider(Model model)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                boundingSpheresMP = new List<BoundingSphere>[mesh.MeshParts.Count];

                Matrix transform = CreateTransform(mesh.ParentBone);
                int index = 0;
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    boundingSpheresMP[index] = ExtractMeshPart(meshPart, transform);
                    index++;
                }
            }
        }
        public static Matrix CreateTransform(ModelBone bone)
        {
            if (bone == null)
                return Matrix.Identity;

            return bone.Transform * CreateTransform(bone.Parent);
        }
        public static float MapScale = 0.02f;
        static uint id = 0;
        public static List<BoundingSphere> ExtractMeshPart(ModelMeshPart meshPart, Matrix transform)
        {
            List<BoundingSphere> boundingSpheres = new List<BoundingSphere>();
            VertexDeclaration declaration = meshPart.VertexBuffer.VertexDeclaration;
            VertexElement[] vertexElements = declaration.GetVertexElements();
            VertexElement vertexPosition = new VertexElement();

            foreach (VertexElement vert in vertexElements)
            {
                if (vert.VertexElementUsage == VertexElementUsage.Position && vert.VertexElementFormat == VertexElementFormat.Vector3)
                {
                    vertexPosition = vert;

                    Vector3[] allVertex = new Vector3[meshPart.NumVertices];
                    meshPart.VertexBuffer.GetData(
                                    meshPart.VertexOffset * declaration.VertexStride + vertexPosition.Offset,
                                    allVertex,
                                    0,
                                    meshPart.NumVertices,
                                    declaration.VertexStride);

                    short[] indices = new short[meshPart.PrimitiveCount * 3];
                    meshPart.IndexBuffer.GetData(meshPart.StartIndex * 2, indices, 0, meshPart.PrimitiveCount * 3);

                    for (int i = 0; i != allVertex.Length; ++i)
                    {
                        Vector3.Transform(ref allVertex[i], ref transform, out allVertex[i]);
                    }

                    for (int i = 0; i < indices.Length; i += 3)
                    {
                        CollisionTriangle triangle = new CollisionTriangle();
                        triangle.v[0] = allVertex[indices[i]];
                        triangle.v[1] = allVertex[indices[i + 1]];
                        triangle.v[2] = allVertex[indices[i + 2]];

                        triangle.v[0] *= MapScale;
                        triangle.v[1] *= MapScale;
                        triangle.v[2] *= MapScale;

                        triangle.id = id;
                        id++;

                        var center = (triangle.v[0] + triangle.v[1] + triangle.v[2]) / 3;
                        var d0 = Vector3.DistanceSquared(center, triangle.v[0]);
                        var d1 = Vector3.DistanceSquared(center, triangle.v[1]);
                        var d2 = Vector3.DistanceSquared(center, triangle.v[2]);

                        var radius = Math.Max(Math.Max(d0, d1), d2);

                        boundingSpheres.Add(new BoundingSphere(center, radius));

                        //mapTriangles.Add(triangle);

                        if(Math.Abs(Vector3.Dot(triangle.GetNormal(), Vector3.Up)) > .6f)
                        {
                            mapFloorTriangles.Add(triangle);
                        }
                        else
                        {
                            mapWallTriangles.Add(triangle);
                        }
                    }
                }
            }

            

            return boundingSpheres;
        }
        static Vector3 tempV3;
        public static Vector3 Vec3Avg(CollisionTriangle t)
        {
            tempV3.X = (t.v[0].X + t.v[1].X + t.v[2].X) / 3;
            tempV3.Y = (t.v[0].Y + t.v[1].Y + t.v[2].Y) / 3;
            tempV3.Z = (t.v[0].Z + t.v[1].Z + t.v[2].Z) / 3;

            return tempV3;

        }
    }

    public class CollisionTriangle
    {
        public Vector3[] v;
        public uint id;
        public CollisionTriangle()
        {
            v = new Vector3[3];
        }
        public Vector3 GetNormal()
        {
            Vector3 edge1 = v[1] - v[0];
            Vector3 edge2 = v[2] - v[0];
            return Vector3.Normalize(Vector3.Cross(edge1, edge2));
        }

        public void GetPlane(out Vector3 normal, out float D)
        {
            normal = GetNormal();
            D = -Vector3.Dot(normal, v[0]);
        }
    }
}
