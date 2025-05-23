using CreatCollisionTools;
using Mathd;
using MathSelf;
using System;
using System.Collections.Generic;
using System.Linq;
using ToolSelf;
using UnityEngine;

public class DrawObj : MonoBehaviour
{
    Map m_MouseMap = new();
    Map m_SelectMap = new();
    Map m_AStarMap = new();
    Map m_AStarOptimize = new();
    Vector3d startAStar = new();
    HashSet<Vector3d> ObsSelected = new();
    ObsCreater m_ObjCreater = new();
    public GameObject m_Father;
    AStar aStar = new();
    AStarOptimize aStarOptimize = new();

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
    List<Vector3d> tempWay = new();
    List<Vector3d> tempWay2 = new();
    void Update()
    {
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
                Debug.DrawLine(v.Value.WorldVertices[j], v.Value.WorldVertices[(j + 1) % v.Value.WorldVertices.Count], Color.green);
            }
        }

        //生成碰撞体
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("DrawObj:生成碰撞体");
            Transform tempTransform;
            for (int i = 0; i < m_Father.transform.childCount; i++)
            {
                tempTransform = m_Father.transform.GetChild(i);
                Destroy(tempTransform);
            }

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
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("DrawObj:设置寻路起点");
            startAStar = mousePoint;
        }

        ///寻路测试
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("DrawObj:寻路测试");
            m_AStarMap.Obstacles2Ds.Clear();
            m_AStarOptimize.Obstacles2Ds.Clear();
            //tempWay = aStar.AStarCalc(new Vector3d(88, 0, 227), new Vector3d(128, 0, 279), m_SelectMap);
            tempWay = aStar.AStarCalc(startAStar, mousePoint, m_SelectMap);
            tempWay2 = new(aStarOptimize.Init(tempWay, m_SelectMap));
            for (int i = 0; i < tempWay.Count; i++)
            {
                Obstacles2D obstacle0 = new(localVertices, ToolM.GetWorldPosByGrid(tempWay[i]), tempWay[i]);
                m_AStarMap.Obstacles2Ds.Add(tempWay[i], obstacle0);
            }
            m_AStarMap = MapCoordinateTransformation.MapTrans(m_AStarMap);

            for (int i = 0; i < tempWay2.Count; i++)
            {
                Obstacles2D obstacle0 = new(localVertices, ToolM.GetWorldPosByGrid(tempWay2[i]), tempWay2[i]);
                m_AStarOptimize.Obstacles2Ds.Add(tempWay2[i], obstacle0);
            }
            m_AStarOptimize = MapCoordinateTransformation.MapTrans(m_AStarOptimize);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {

        }

        foreach (var v in m_AStarMap.Obstacles2Ds)
        {
            Debug.DrawLine(v.Value.WorldVertices[0], v.Value.WorldVertices[2], Color.cyan);
            Debug.DrawLine(v.Value.WorldVertices[1], v.Value.WorldVertices[3], Color.cyan);
            for (int j = 0; j < v.Value.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.Value.WorldVertices[j], v.Value.WorldVertices[(j + 1) % v.Value.WorldVertices.Count], Color.cyan);
            }
        }

        foreach (var v in m_AStarOptimize.Obstacles2Ds)
        {
            for (int j = 0; j < v.Value.WorldVertices.Count; j++)
            {
                Debug.DrawLine(v.Value.WorldVertices[j], v.Value.WorldVertices[(j + 1) % v.Value.WorldVertices.Count], Color.purple);
            }
        }
    }
}