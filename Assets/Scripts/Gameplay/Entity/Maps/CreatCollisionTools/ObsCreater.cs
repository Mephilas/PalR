using Mathd;
using ToolSelf;
using UnityEngine;

namespace CreatCollisionTools
{
    public class ObsCreater
    {
        Obstacles2D m_Obstacles2D;//平面障碍物
        GameObject m_Creat;//生成的碰撞体

        Mesh mesh;

        Vector3d[] points;
        Vector3d[] vertices;
        int[] triangles;
        Vector3d[] normals;

        public void CreatCollision(Obstacles2D obstacles2D, GameObject father)
        {
            m_Obstacles2D = obstacles2D;
            mesh = new Mesh();
            m_Creat = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CollisionPrefab"), father.transform) ;
            m_Creat.name = string.Format("{0}_{1}", obstacles2D.GirdPosition.x, obstacles2D.GirdPosition.z);
            m_Creat.transform.position = obstacles2D.Position;
            SetPoints();
            SetVertices();
            SetTriangles();
            SetNormals();

            mesh.vertices = ToolM.Vector3dArrayToNor(vertices);
            mesh.triangles = triangles;
            mesh.normals = ToolM.Vector3dArrayToNor(normals);

            m_Creat.GetComponent<MeshFilter>().mesh = mesh;
            m_Creat.GetComponent<MeshCollider>().sharedMesh = mesh;
        }

        void SetNormals()
        {
            normals = new Vector3d[triangles.Length];
            for (int i = 0; i < triangles.Length / 3; i++)
            {
                normals[i * 3] = Vector3.Cross(vertices[i * 3 + 1] - vertices[i * 3], vertices[i * 3 + 2] - vertices[i * 3 + 1]).normalized;
                normals[i * 3 + 1] = normals[i * 3];
                normals[i * 3 + 2] = normals[i * 3];
            }
        }

        void SetPoints()
        {
            points = new Vector3d[m_Obstacles2D.LocalVertices.Count * 2];

            for (int i = 0; i < m_Obstacles2D.LocalVertices.Count; i++)
            {
                points[i * 2] = m_Obstacles2D.LocalVertices[i];
                points[i * 2 + 1] = new Vector3d(points[i * 2].x, points[i * 2].y + 1, points[i * 2].z);
            }
        }

        void SetVertices()
        {
            vertices = new Vector3d[m_Obstacles2D.WorldVertices.Count * 12 - 12];

            for (int i = 0; i < m_Obstacles2D.WorldVertices.Count; i++)
            {
                vertices[i * 6] = points[i * 2];
                vertices[i * 6 + 1] = points[(i * 2 + 2) % points.Length];
                vertices[i * 6 + 2] = points[(i * 2 + 1) % points.Length];
                vertices[i * 6 + 3] = points[(i * 2 + 2) % points.Length];
                vertices[i * 6 + 4] = points[(i * 2 + 3) % points.Length];
                vertices[i * 6 + 5] = points[(i * 2 + 1) % points.Length];
            }

            for (int i = 0; i < m_Obstacles2D.WorldVertices.Count - 2; i++)
            {
                vertices[m_Obstacles2D.WorldVertices.Count * 6 + i * 6] = points[1];
                vertices[m_Obstacles2D.WorldVertices.Count * 6 + i * 6 + 1] = points[(i * 2 + 3) % points.Length];
                vertices[m_Obstacles2D.WorldVertices.Count * 6 + i * 6 + 2] = points[(i * 2 + 5) % points.Length];
                vertices[m_Obstacles2D.WorldVertices.Count * 6 + i * 6 + 3] = points[0];
                vertices[m_Obstacles2D.WorldVertices.Count * 6 + i * 6 + 4] = points[(i * 2 + 4) % points.Length];
                vertices[m_Obstacles2D.WorldVertices.Count * 6 + i * 6 + 5] = points[(i * 2 + 2) % points.Length];
            }
        }

        void SetTriangles()
        {
            triangles = new int[m_Obstacles2D.WorldVertices.Count * 12 - 12];

            for (int i = 0; i < triangles.Length; i++)
            {
                triangles[i] = i;
            }
        }
    }
}