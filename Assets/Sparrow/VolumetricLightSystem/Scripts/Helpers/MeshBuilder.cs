// 
// Copyright (c) 2023 Off The Beaten Track UG
// All rights reserved.
// 
// Maintainer: Jens Bahr
// 

using System.Collections.Generic;
using UnityEngine;

namespace Sparrow.VolumetricLight.Helpers
{
    /*
     * Creation of mesh primitives for light volumes
     */
    public static class MeshBuilder
    {
        public static Mesh Box(float depth, float width, float height)
        {
            Vector3[] vertices = new Vector3[8];
            Vector2[] uvs = new Vector2[vertices.Length];

            vertices[0] = new Vector3(width / 2, height / 2, 0);
            vertices[1] = new Vector3(-width / 2, height / 2, 0);
            vertices[2] = new Vector3(-width / 2, -height / 2, 0);
            vertices[3] = new Vector3(width / 2, -height / 2, 0);
            vertices[4] = new Vector3(width / 2, -height / 2, depth);
            vertices[5] = new Vector3(-width / 2, -height / 2, depth);
            vertices[6] = new Vector3(-width / 2, height / 2, depth);
            vertices[7] = new Vector3(width / 2, height / 2, depth);

            uvs[0] = new Vector2(0, 0);
            uvs[1] = new Vector2(0, 0.25f);
            uvs[2] = new Vector2(0, 0.5f);
            uvs[3] = new Vector2(0, 0.75f);
            uvs[4] = new Vector2(1, 0);
            uvs[5] = new Vector2(1, 0.25f);
            uvs[6] = new Vector2(1, 0.5f);
            uvs[7] = new Vector2(1, 0.75f);

            int[] triangles =
            {
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                2, 4, 5,
                1, 2, 5,
                1, 5, 6,
                0, 7, 4,
                0, 4, 3,
                5, 4, 7,
                5, 7, 6,
                0, 6, 7,
                0, 1, 6
            };

            //finalizing
            Mesh mesh = new Mesh {name = "Box"};
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);

            mesh.RecalculateNormals();

            return mesh;
        }

        public static Mesh HollowCylinder(float length, float radius, float baseRadius, int circleVertices = 32, int lengthVertices = 8)
        {
            List<Vector3> vertices = new List<Vector3>(lengthVertices * circleVertices);
            List<int> triangles = new List<int>((lengthVertices - 1) * circleVertices * 6);
            List<Vector2> uvs = new List<Vector2>(vertices.Capacity);
            List<Vector3> normals = new List<Vector3>(vertices.Capacity);

            float angleStep = 360f / circleVertices;
            float lengthStep = length / (lengthVertices - 1);
            for (int ri = 0; ri < lengthVertices; ri++) //ring index, previous ring index
            {
                int rvi = ri * circleVertices;
                int prvi = (ri - 1) * circleVertices;
                float ringRadius = Mathf.Lerp(radius, baseRadius,(lengthStep * ri) / length);//previous ring vertex index

                Vector3 ringOffset = ri * lengthStep * Vector3.forward;

                for (int i = 0; i < circleVertices; i++)
                {
                    Vector3 vec =
                        new Vector3(
                            Mathf.Sin(Mathf.Deg2Rad * angleStep * i),
                            Mathf.Cos(Mathf.Deg2Rad * angleStep * i),
                            0) * ringRadius;

                    vertices.Add(vec + ringOffset);
                    uvs.Add(new Vector2(lengthStep * ri / length, circleVertices / (float) i));
                    normals.Add(vec.normalized);

                    //on first ring, skip triangle gen
                    if (ri == 0) continue;

                    int vi = rvi + i;
                    int pvi = prvi + i; //previous vertex index

                    triangles.Add(pvi);
                    triangles.Add(rvi + (i + 1) % circleVertices);
                    triangles.Add(vi);

                    triangles.Add(prvi + (i + 1) % circleVertices);
                    triangles.Add(rvi + (i + 1) % circleVertices);
                    triangles.Add(pvi);
                }
            }

            //finalizing
            Mesh mesh = new Mesh {name = "Hollow Cylinder"};
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.SetNormals(normals);

            return mesh;
        }

        // using https://lindenreid.wordpress.com/2017/11/07/procedural-sphere-ellipsoid-tutorial/
        public static Mesh Sphere(float radius, int resolution = 8)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector3> normals = new List<Vector3>();

            float theta = Mathf.PI / resolution;
            float phi = Mathf.PI * 2 / resolution;

            //TOP CAP
            vertices.Add(Vector3.up * radius);
            normals.Add(Vector3.up);

            //middle
            for (int stack = 1; stack < resolution; stack++)
            {
                float stackRadius = Mathf.Sin(theta * stack) * radius;
                for (int slice = 0; slice < resolution; slice++)
                {
                    Vector3 pos = new Vector3(Mathf.Cos(phi * slice) * stackRadius,
                        Mathf.Cos(theta * stack) * radius,
                        Mathf.Sin(phi * slice) * stackRadius);
                    vertices.Add(pos);
                    normals.Add(pos.normalized);
                }
            }

            //bot cap
            vertices.Add(Vector3.down * radius);
            normals.Add(Vector3.down);

            //top cap
            for (int slice = 0; slice < resolution - 1; slice++)
            {
                triangles.Add(0);
                triangles.Add(slice + 2);
                triangles.Add(slice + 1);
            }

            triangles.Add(0);
            triangles.Add(1);
            triangles.Add(resolution);


            //middle
            for (int stack = 0; stack < resolution - 2; stack++)
            {
                int t1;
                int t2;
                int t3;
                int t4;
                for (int slice = 0; slice < resolution - 1; slice++)
                {
                    t1 = 1 + slice + (resolution * stack);
                    t2 = t1 + 1;
                    t3 = 1 + slice + (resolution * (stack + 1));
                    t4 = t3 + 1;

                    triangles.Add(t1);
                    triangles.Add(t2);
                    triangles.Add(t4);

                    triangles.Add(t1);
                    triangles.Add(t4);
                    triangles.Add(t3);
                }

                //last
                t1 = resolution * (stack + 1);
                t2 = 1 + (resolution * stack);
                t3 = resolution * (stack + 2);
                t4 = 1 + (resolution * (stack + 1));

                triangles.Add(t1);
                triangles.Add(t2);
                triangles.Add(t4);

                triangles.Add(t1);
                triangles.Add(t4);
                triangles.Add(t3);
            }

            //bottom 
            int lvi = vertices.Count - 1;
            for (int slice = 0; slice < resolution - 1; slice++)
            {
                int t2 = (resolution - 2) * resolution + slice + 1;
                int t3 = (resolution - 2) * resolution + slice + 2;
                triangles.Add(lvi);
                triangles.Add(t2);
                triangles.Add(t3);
            }

            triangles.Add(lvi);
            triangles.Add((resolution - 1) * resolution);
            triangles.Add((resolution - 2) * resolution + 1);


            //finalizing
            Mesh mesh = new Mesh {name = "Sphere"};
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetNormals(normals);

            //mesh.RecalculateNormals();

            return mesh;
        }

        public static Mesh HollowCone(float length, float angle, int circleVertices = 32, int lengthVertices = 8)
        {
            // ASA: a = angle at point, s = length, b = right angle at base
            // known: angle c = 360-90-a
            // rule : a/sina = b/sinb = c/sinc;
            float c = 180f - 90 - angle;
            float clength = length / Mathf.Sin(c * Mathf.Deg2Rad);

            // clength = x / sin (angle) == clength * sin(angle) = x
            float baseRadius = clength * Mathf.Sin(angle * Mathf.Deg2Rad);

            Mesh cylinder = HollowCylinder(length, float.Epsilon, baseRadius, circleVertices, lengthVertices);

            //finalizing
            cylinder.name = "Hollow Cone";
            //cylinder.SetVertices(vertices);

            return cylinder;
        }
    }
}