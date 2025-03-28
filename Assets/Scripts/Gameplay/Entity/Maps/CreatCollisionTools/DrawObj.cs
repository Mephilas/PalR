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
    Map m_MouseMap = new();
    Map m_SelectMap = new();
    HashSet<Vector3d> ObsSelected = new();
    ObsCreater m_ObjCreater = new();
    public GameObject m_Father;

    List<Vector3d> localVertices = new();
    private void Start()
    {
        localVertices.Add(new(-Map.GridSize / 2, 0, -Map.GridSize / 2));
        localVertices.Add(new(-Map.GridSize / 2, 0, Map.GridSize / 2));
        localVertices.Add(new(Map.GridSize / 2, 0, Map.GridSize / 2));
        localVertices.Add(new(Map.GridSize / 2, 0, -Map.GridSize / 2));

        m_SelectMap.LoadMap();
        foreach (var v in m_SelectMap.Obstacles2Ds)
        {
            ObsSelected.Add(v.GirdPosition);
        }
    }
    void Update()
    {
        //交互格子
        Vector3d mousePosition = Input.mousePosition;
        mousePosition.z = 1f;
        Vector3d mousePoint = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePoint.y = 0;
        mousePoint = ToolM.WorldToGridVec(mousePoint);

        if (Input.GetMouseButton(0))
        {
            ObsSelected.Add(mousePoint);
        }
        if (Input.GetMouseButton(1))
        {
            ObsSelected.Remove(mousePoint);
        }
        m_SelectMap.Obstacles2Ds.Clear();
        foreach (var v in ObsSelected)
        {
            Obstacles2D obstacle0 = new(localVertices, ToolM.GetWorldPosByGrid(v), v);
            m_SelectMap.Obstacles2Ds.Add(obstacle0);
        }
        m_SelectMap = MapCoordinateTransformation.MapTrans(m_SelectMap);
        foreach (var v in m_SelectMap.Obstacles2Ds)
        {
            for (int j = 0; j < v.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.WorldVertices[j], v.WorldVertices[(j + 1) % v.WorldVertices.Count], Color.red);
            }
            Debug.DrawLine(v.WorldVertices[0], v.WorldVertices[2], Color.red);
            Debug.DrawLine(v.WorldVertices[1], v.WorldVertices[3], Color.red);
        }

        m_MouseMap.Obstacles2Ds.Clear();
        m_MouseMap.Obstacles2Ds.Add(new(localVertices, ToolM.GetWorldPosByGrid(mousePoint), mousePoint));
        m_MouseMap = MapCoordinateTransformation.MapTrans(m_MouseMap);

        foreach (var v in m_MouseMap.Obstacles2Ds)
        {
            for (int j = 0; j < v.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.WorldVertices[j], v.WorldVertices[(j + 1) % v.WorldVertices.Count], Color.blue);
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
            foreach (var v in m_SelectMap.Obstacles2Ds)
            {
                m_ObjCreater.CreatCollision(v, m_Father);
            }
        }

        //保存地图
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_SelectMap.SaveMap();
        }
    }
}
