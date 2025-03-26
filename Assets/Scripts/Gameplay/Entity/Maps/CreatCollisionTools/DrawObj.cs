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
        for (int i = 0; i < m_SelectMap.Obstacles2Ds.Count; i++)
        {
            ObsSelected.Add(m_SelectMap.Obstacles2Ds[i].GirdPosition);
        }
    }
    void Update()
    {
        //��������
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
        for (int i = 0; i < m_SelectMap.Obstacles2Ds.Count; i++)
        {
            for (int j = 0; j < m_SelectMap.Obstacles2Ds[i].WorldVertices.Count; j++)
            {
                Debug.DrawLine(m_SelectMap.Obstacles2Ds[i].WorldVertices[j], m_SelectMap.Obstacles2Ds[i].WorldVertices[(j + 1) % m_SelectMap.Obstacles2Ds[i].WorldVertices.Count], Color.red);
            }
            Debug.DrawLine(m_SelectMap.Obstacles2Ds[i].WorldVertices[0], m_SelectMap.Obstacles2Ds[i].WorldVertices[2], Color.red);
            Debug.DrawLine(m_SelectMap.Obstacles2Ds[i].WorldVertices[1], m_SelectMap.Obstacles2Ds[i].WorldVertices[3], Color.red);
        }

        m_MouseMap.Obstacles2Ds.Clear();
        m_MouseMap.Obstacles2Ds.Add(new(localVertices, ToolM.GetWorldPosByGrid(mousePoint), mousePoint));
        m_MouseMap = MapCoordinateTransformation.MapTrans(m_MouseMap);
        for (int j = 0; j < m_MouseMap.Obstacles2Ds[0].WorldVertices.Count; j++)
        {
            Debug.DrawLine(m_MouseMap.Obstacles2Ds[0].WorldVertices[j], m_MouseMap.Obstacles2Ds[0].WorldVertices[(j + 1) % m_MouseMap.Obstacles2Ds[0].WorldVertices.Count], Color.blue);
        }

        //��ײ������
        if (Input.GetKeyDown(KeyCode.K))
        {
            //ɾ������������
            Transform tempTransform;
            for (int i = 0; i < m_Father.transform.childCount; i++)
            {
                tempTransform = m_Father.transform.GetChild(i);
                Destroy(tempTransform);
            }

            //������ײ��
            for (int i = 0; i < m_SelectMap.Obstacles2Ds.Count; i++)
            {
                m_ObjCreater.CreatCollision(m_SelectMap.Obstacles2Ds[i], m_Father);
            }
        }

        //�����ͼ
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_SelectMap.SaveMap();
        }
    }
}
