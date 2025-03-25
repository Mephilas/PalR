using CreatCollisionTools;
using Mathd;
using MathSelf;
using System;
using System.Collections.Generic;
using ToolSelf;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawObj : MonoBehaviour
{
    public Transform CheckPoint;
    public Transform GridCenter;

    public Vector3 Angle2 = new(30, 45, 0);
    public Vector3d CameraAngle = new(30, 45, 0);

    Map m_GridMap = new();
    Map m_CameraMap = new();
    Map m_SelectMap = new();
    HashSet<Vector3d> ObsSelected = new();
    ObsCreater m_ObjCreater = new();
    public GameObject m_Father;

    bool flag = false;

    double GridSize = 0.32 / Math.Sqrt(2);

    Vector3d DefaultAngle = new(90, 0, 0);

    List<Vector3d> localVertices = new();
    private void Start()
    {
        localVertices.Add(new(-GridSize / 2, 0, -GridSize / 2));
        localVertices.Add(new(-GridSize / 2, 0, GridSize / 2));
        localVertices.Add(new(GridSize / 2, 0, GridSize / 2));
        localVertices.Add(new(GridSize / 2, 0, -GridSize / 2));

        ObsSelected.Clear();
    }
    void Update()
    {
        //这里根据相机绘制格子范围
        Vector3d CameraPos = Camera.main.transform.position;
        CameraPos.y = 0;
        Vector3d CameraGridPos = ToolM.WorldToGridVec(CameraPos, DefaultAngle, CameraAngle, GridSize);

        m_CameraMap.Obstacles2Ds.Clear();
        for (int i = (int)CameraGridPos.x - 10; i < CameraGridPos.x + 10; i++)
        {
            for (int j = (int)CameraGridPos.z - 10; j < CameraGridPos.z + 10; j++)
            {
                Obstacles2D obstacle0 = new(localVertices, ToolM.GetWorldPosByGrid(new(i, 0, j), GridSize));
                m_CameraMap.Obstacles2Ds.Add(obstacle0);
            }
        }
        m_CameraMap = MapCoordinateTransformation.MapTrans(m_CameraMap, CameraAngle);
        for (int i = 0; i < m_CameraMap.Obstacles2Ds.Count; i++)
        {
            for (int j = 0; j < m_CameraMap.Obstacles2Ds[i].WorldVertices.Count; j++)
            {
                Debug.DrawLine(m_CameraMap.Obstacles2Ds[i].WorldVertices[j], m_CameraMap.Obstacles2Ds[i].WorldVertices[(j + 1) % m_CameraMap.Obstacles2Ds[i].WorldVertices.Count], Color.green);
            }
        }

        //左键交互格子
        if (Input.GetMouseButton(0))
        {
            Vector3d mousePosition = Input.mousePosition;
            mousePosition.z = 1f;
            Vector3d mousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
            mousePoint.y = 0;
            ObsSelected.Add(ToolM.GetWorldPosByGrid(ToolM.WorldToGridVec(mousePoint, DefaultAngle, CameraAngle, GridSize), GridSize));
            CheckPoint.transform.position = mousePoint;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3d mousePosition = Input.mousePosition;
            mousePosition.z = 1f;
            Vector3d mousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
            mousePoint.y = 0;
            ObsSelected.Remove(ToolM.GetWorldPosByGrid(ToolM.WorldToGridVec(mousePoint, DefaultAngle, CameraAngle, GridSize), GridSize));
        }
        m_SelectMap.Obstacles2Ds.Clear();
        foreach (var v in ObsSelected)
        {
            Obstacles2D obstacle0 = new(localVertices, v);
            m_SelectMap.Obstacles2Ds.Add(obstacle0);
        }
        m_SelectMap = MapCoordinateTransformation.MapTrans(m_SelectMap, CameraAngle);
        for (int i = 0; i < m_SelectMap.Obstacles2Ds.Count; i++)
        {
            for (int j = 0; j < m_SelectMap.Obstacles2Ds[i].WorldVertices.Count; j++)
            {
                Debug.DrawLine(m_SelectMap.Obstacles2Ds[i].WorldVertices[j], m_SelectMap.Obstacles2Ds[i].WorldVertices[(j + 1) % m_SelectMap.Obstacles2Ds[i].WorldVertices.Count], Color.red);
            }
        }

        //碰撞体生成
        if (Input.GetKeyDown(KeyCode.K))
        {
            //删除所有子物体
            Transform tempTransform;
            for (int i = 0; i < m_Father.transform.childCount; i++)
            {
                tempTransform = m_Father.transform.GetChild(i);
                Destroy(tempTransform);
            }

            //创造碰撞体
            for (int i = 0; i < m_SelectMap.Obstacles2Ds.Count; i++)
            {
                m_ObjCreater.CreatCollision(m_SelectMap.Obstacles2Ds[i], m_Father);
            }
        }
    }


}
