using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Slicer
{
    public enum MeshSide
    {
        Positive = 0,
        Negative = 1,
    }

    public class SliceMetaData
    {
        //general
        private readonly List<Vector3> _pointsAlongPlane;
        private Plane _plane;
        private Mesh _mesh;
        //positiveSide
        private Mesh _positiveMesh;
        private List<Vector3> _positiveSideVertices;
        private List<int> _positiveSideTriangles;
        private List<Vector2> _positiveSideUvs;
        private List<Vector3> _positiveSideNormals;
        //negativeSide
        private Mesh _negativeMesh;
        private List<Vector3> _negativeSideVertices;
        private List<int> _negativeSideTriangles;
        private List<Vector2> _negativeSideUvs;
        private List<Vector3> _negativeSideNormals;

        private void SetMeshSide(MeshSide side)
        {
            if (MeshSide.Positive == side)
            {
                _positiveMesh.vertices = _positiveSideVertices.ToArray();
                _positiveMesh.triangles = _positiveSideTriangles.ToArray();
                _positiveMesh.normals = _positiveSideNormals.ToArray();
                _positiveMesh.uv = _positiveSideUvs.ToArray();
            }
            else
            {
                _negativeMesh.vertices = _negativeSideVertices.ToArray();
                _negativeMesh.triangles = _negativeSideTriangles.ToArray();
                _negativeMesh.normals = _negativeSideNormals.ToArray();
                _negativeMesh.uv = _negativeSideUvs.ToArray();
            }
        }

        public Mesh PositiveMesh
        {
            get
            {
                if (_positiveMesh == null) 
                {
                    _positiveMesh = new Mesh();
                }
                SetMeshSide(MeshSide.Positive);
                return _positiveMesh;
            }   
        }
        
        public Mesh NegativeMesh
        {
            get
            {
                if (_negativeMesh == null)
                {
                    _negativeMesh = new Mesh();
                }
                SetMeshSide(MeshSide.Negative);
                return _negativeMesh;
            }
        }

        public SliceMetaData(Plane plane, Mesh mesh)
        {
            _positiveSideVertices = new List<Vector3>();
            _positiveSideTriangles = new List<int>();
            _positiveSideUvs = new List<Vector2>();
            _positiveSideNormals = new List<Vector3>();
            _negativeSideVertices = new List<Vector3>();
            _negativeSideUvs = new List<Vector2>();
            _negativeSideTriangles = new List<int>();
            _negativeSideNormals = new List<Vector3>();
            _pointsAlongPlane = new List<Vector3>();
            _plane = plane;
            _mesh = mesh;

            ComputeNewMeshes();

        }

        private void AddTrianglesNormalAndUvs(MeshSide side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            if (side == MeshSide.Positive)
            {
                AddTrianglesNormalsAndUvs(ref _positiveSideVertices, ref _positiveSideTriangles, ref _positiveSideNormals, ref _positiveSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
            else
            {
                AddTrianglesNormalsAndUvs(ref _negativeSideVertices, ref _negativeSideTriangles, ref _negativeSideNormals, ref _negativeSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
        }

        private void AddTrianglesNormalsAndUvs(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            int tri1Index = vertices.IndexOf(vertex1);

            if (addFirst)
            {
                ShiftTriangleIndeces(ref triangles);
            }

            if (tri1Index > -1 && shareVertices)
            {
                triangles.Add(tri1Index);
            }
            else
            {
                if (normal1 == null)
                {
                    normal1 = ComputeNormal(vertex1, vertex2, vertex3);
                }

                int? i = null;
                if (addFirst)
                {
                    i = 0;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex1, (Vector3)normal1, uv1, i);
            }

            int tri2Index = vertices.IndexOf(vertex2);

            if (tri2Index > -1 && shareVertices)
            {
                triangles.Add(tri2Index);
            }
            else
            {
                if (normal2 == null)
                {
                    normal2 = ComputeNormal(vertex2, vertex3, vertex1);
                }

                int? i = null;
                if (addFirst)
                {
                    i = 1;
                }


                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex2, (Vector3)normal2, uv2, i);
            }

            int tri3Index = vertices.IndexOf(vertex3);

            if (tri3Index > -1 && shareVertices)
            {
                triangles.Add(tri3Index);
            }
            else
            {
                if (normal3 == null)
                {
                    normal3 = ComputeNormal(vertex3, vertex1, vertex2);
                }

                int? i = null;
                if (addFirst)
                {
                    i = 2;
                }


                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex3, (Vector3)normal3, uv3, i);
            }
        }

        private void AddVertNormalUv(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
        {
            if (index != null)
            {
                int i = (int)index;
                vertices.Insert(i, vertex);
                uvs.Insert(i, uv);
                normals.Insert(i, normal);
                triangles.Insert(i, i);
            }
            else
            {
                vertices.Add(vertex);
                normals.Add(normal);
                uvs.Add(uv);
                triangles.Add(vertices.IndexOf(vertex));
            }
        }

        private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 side1 = vertex2 - vertex1;
            Vector3 side2 = vertex3 - vertex1;

            Vector3 normal = Vector3.Cross(side1, side2);

            return normal;
        }

        private void ShiftTriangleIndeces(ref List<int> triangles)
        {
            for (int j = 0; j < triangles.Count; j += 3)
            {
                triangles[j] += +3;
                triangles[j + 1] += 3;
                triangles[j + 2] += 3;
            }
        }

        private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
        {
            float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection);
            uv = InterpolateUvs(vertex1Uv, vertex2Uv, distance);
            return pointOfIntersection;
        }

        private float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
        {
            Ray ray = new Ray(vertex1, (vertex2 - vertex1));
            _plane.Raycast(ray, out float distance);
            pointOfintersection = ray.GetPoint(distance);
            return distance;
        }

        private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
        {
            Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
            return uv;
        }

        private void ComputeNewMeshes()
        {
            int[] meshTriangles = _mesh.triangles;
            Vector3[] meshVerts = _mesh.vertices;
            Vector3[] meshNormals = _mesh.normals;
            Vector2[] meshUvs = _mesh.uv;

            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Vector3 vert1 = meshVerts[meshTriangles[i]];
                int vert1Index = Array.IndexOf(meshVerts, vert1);
                Vector2 uv1 = meshUvs[vert1Index];
                Vector3 normal1 = meshNormals[vert1Index];
                var vert1Side = _plane.GetSide(vert1);

                Vector3 vert2 = meshVerts[meshTriangles[i + 1]];
                int vert2Index = Array.IndexOf(meshVerts, vert2);
                Vector2 uv2 = meshUvs[vert2Index];
                Vector3 normal2 = meshNormals[vert2Index];
                var vert2Side = _plane.GetSide(vert2);

                Vector3 vert3 = meshVerts[meshTriangles[i + 2]];
                int vert3Index = Array.IndexOf(meshVerts, vert3);
                Vector2 uv3 = meshUvs[vert3Index];
                Vector3 normal3 = meshNormals[vert3Index];
                var vert3Side = _plane.GetSide(vert3);

                if (vert1Side == vert2Side && vert2Side == vert3Side)
                {
                    MeshSide side = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
                    AddTrianglesNormalAndUvs(side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
                }
                else
                {
                    Vector3 intersection1;
                    Vector3 intersection2;

                    Vector2 intersection1Uv;
                    Vector2 intersection2Uv;

                    MeshSide side1 = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
                    MeshSide side2 = (vert1Side) ? MeshSide.Negative : MeshSide.Positive;

                    if (vert1Side == vert2Side)
                    {            
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert3, uv3, vert1, uv1, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, vert2, null, uv2, intersection1, null, intersection1Uv, false, false);
                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, false, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, false, false);
                    }
                    else if (vert1Side == vert3Side)
                    {                  
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, vert3, null, uv3, false, false);
                        AddTrianglesNormalAndUvs(side1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, vert3, null, uv3, false, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, intersection2, null, intersection2Uv, false, false);
                    }
                    else
                    {                  
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert3, uv3, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, false, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, vert3, null, uv3, false, false);
                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, false, false);
                    }

                    _pointsAlongPlane.Add(intersection1);
                    _pointsAlongPlane.Add(intersection2);
                }
            }

            JoinPointsAlongPlane();
        }
        private void JoinPointsAlongPlane()
        {
            Vector3 halfway = GetHalfwayPoint(out float distance);

            for (int i = 0; i < _pointsAlongPlane.Count; i += 2)
            {
                Vector3 firstVertex;
                Vector3 secondVertex;

                firstVertex = _pointsAlongPlane[i];
                secondVertex = _pointsAlongPlane[i + 1];

                Vector3 normal3 = ComputeNormal(halfway, secondVertex, firstVertex);
                normal3.Normalize();

                var direction = Vector3.Dot(normal3, _plane.normal);

                if (direction > 0)
                {
                    AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                }
                else
                {
                    AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                }
            }
        }
        private Vector3 GetHalfwayPoint(out float distance)
        {
            if (_pointsAlongPlane.Count > 0)
            {
                Vector3 firstPoint = _pointsAlongPlane[0];
                Vector3 furthestPoint = Vector3.zero;
                distance = 0f;

                foreach (Vector3 point in _pointsAlongPlane)
                {
                    float currentDistance = 0f;
                    currentDistance = Vector3.Distance(firstPoint, point);

                    if (currentDistance > distance)
                    {
                        distance = currentDistance;
                        furthestPoint = point;
                    }
                }

                return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
            }
            else
            {
                distance = 0;
                return Vector3.zero;
            }
        }

    }
}