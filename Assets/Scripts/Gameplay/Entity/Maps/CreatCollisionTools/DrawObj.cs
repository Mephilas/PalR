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
    Map m_AStarMap = new();
    HashSet<Vector3d> ObsSelected = new();
    ObsCreater m_ObjCreater = new();
    public GameObject m_Father;
    AStar aStar = new();

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
            ObsSelected.Add(v.Value.GirdPosition);
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
            m_SelectMap.Obstacles2Ds.Add(v, obstacle0);
        }
        m_SelectMap = MapCoordinateTransformation.MapTrans(m_SelectMap);
        foreach (var v in m_SelectMap.Obstacles2Ds)
        {
            for (int j = 0; j < v.Value.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.Value.WorldVertices[j], v.Value.WorldVertices[(j + 1) % v.Value.WorldVertices.Count], Color.red);
            }
            Debug.DrawLine(v.Value.WorldVertices[0], v.Value.WorldVertices[2], Color.red);
            Debug.DrawLine(v.Value.WorldVertices[1], v.Value.WorldVertices[3], Color.red);
        }

        m_MouseMap.Obstacles2Ds.Clear();
        m_MouseMap.Obstacles2Ds.Add(mousePoint, new(localVertices, ToolM.GetWorldPosByGrid(mousePoint), mousePoint));
        m_MouseMap = MapCoordinateTransformation.MapTrans(m_MouseMap);

        foreach (var v in m_MouseMap.Obstacles2Ds)
        {
            for (int j = 0; j < v.Value.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.Value.WorldVertices[j], v.Value.WorldVertices[(j + 1) % v.Value.WorldVertices.Count], Color.blue);
            }
        }

        //碰撞体生成
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("DrawObj:生成碰撞体");
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
                m_ObjCreater.CreatCollision(v.Value, m_Father);
            }
        }

        //保存地图
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("DrawObj:保存地图");
            m_SelectMap.SaveMap();
        }

        ///寻路测试
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("DrawObj:寻路测试");
            //清楚上次寻路结果
            m_AStarMap.Obstacles2Ds.Clear();
            List<Vector3d> tempWay = aStar.AStarCalc(new(0, 0, 0), mousePoint, m_SelectMap);
            for (int i = 0; i < tempWay.Count; i++)
            {
                Obstacles2D obstacle0 = new(localVertices, ToolM.GetWorldPosByGrid(tempWay[i]), tempWay[i]);
                m_AStarMap.Obstacles2Ds.Add(tempWay[i], obstacle0);
            }
            m_AStarMap = MapCoordinateTransformation.MapTrans(m_AStarMap);
        }
        ///结果绘制
        foreach (var v in m_AStarMap.Obstacles2Ds)
        {
            for (int j = 0; j < v.Value.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.Value.WorldVertices[j], v.Value.WorldVertices[(j + 1) % v.Value.WorldVertices.Count], Color.green);
            }
        }
    }
}
