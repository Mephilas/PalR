using CreatCollisionTools;
using Mathd;
using MathSelf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawObj : MonoBehaviour
{
    public Camera m_Cam;
    public Transform m_Cube;

    public Vector3d m_Position;

    Map m_Map;
    Map m_Map2;
    ObsCreater m_ObjCreater;
    public GameObject m_Father;

    private void Awake()
    {
        m_Map = new Map();
        m_ObjCreater = new ObsCreater();
    }

    void Start()
    {
        List<Vector3d> localVertices = new List<Vector3d>();
        double aaa = 0.32 / Math.Sqrt(2);
        localVertices.Add(new Vector3d(-aaa / 2, 0, -aaa / 2));
        localVertices.Add(new Vector3d(-aaa / 2, 0, aaa / 2));
        localVertices.Add(new Vector3d(aaa / 2, 0, aaa / 2));
        localVertices.Add(new Vector3d(aaa / 2, 0, -aaa / 2));

        for (int i = -50; i < 50; i++)
        {
            for (int j = -50; j < 50; j++)
            {
                Obstacles2D obstacle0 = new Obstacles2D(localVertices, new Vector3d(j * aaa, 0, i * aaa));
                m_Map.Obstacles2Ds.Add(obstacle0);
            }
        }

        m_Map2 = new Map(m_Map);
    }
    bool flag = false;
    void Update()
    {
        Debug.DrawLine(new Vector3d(-100, 0, 0), new Vector3d(100, 0, 0), Color.green);
        Debug.DrawLine(new Vector3d(0, 0, -100), new Vector3d(0, 0, 100), Color.green);

        Vector3d vec = m_Cam.transform.eulerAngles;
        Vector3d vec2 = new Vector3d(90 - MathM.RadianAndDegreeMeasure(Math.Acos(0.15 / 0.32), false), 45, 0);

        //地图信息转换
        if (Input.GetKeyDown(KeyCode.L))
        {
            m_Map2 = MapCoordinateTransformation.MapTrans(m_Map, vec);
            flag = true;
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
            for (int i = 0; i < m_Map.Obstacles2Ds.Count; i++)
            {
                m_ObjCreater.CreatCollision(m_Map2.Obstacles2Ds[i], m_Father);
            }
        }

        Vector3 a = new Vector3(0, 0, 1);
        m_Cam.transform.position = m_Cube.transform.position - (Quaternion.Euler(vec) * a.normalized * 5);

        //画线
        if (flag)
        {
            for (int i = 0; i < m_Map2.Obstacles2Ds.Count; i++)
            {
                for (int j = 0; j < m_Map2.Obstacles2Ds[i].WorldVertices.Count; j++)
                {
                    Debug.DrawLine(m_Map2.Obstacles2Ds[i].WorldVertices[j], m_Map2.Obstacles2Ds[i].WorldVertices[(j + 1) % m_Map2.Obstacles2Ds[i].WorldVertices.Count], Color.blue);
                }
            }
        }
    }
}
