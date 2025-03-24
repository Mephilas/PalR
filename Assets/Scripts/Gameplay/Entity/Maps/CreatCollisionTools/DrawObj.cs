using CreatCollisionTools;
using Mathd;
using MathSelf;
using System;
using System.Collections.Generic;
using ToolSelf;
using UnityEngine;
using UnityEngine.UIElements;

public class DrawObj : MonoBehaviour
{
    public Transform CheckPoint;

    public Vector3 Angle2 = new(30, 30, 0);
    public Vector3d Angle = new(30, 30, 0);

    Map m_GridMap = new();
    Map m_TransMap = new();
    Map m_TestMap = new();
    ObsCreater m_ObjCreater = new();
    public GameObject m_Father;

    bool flag = false;

    double BianChang = 0.32 / Math.Sqrt(2);

    private void Start()
    {

    }
    void Update()
    {
        Angle = Angle2;
        //地图信息转换
        if (Input.GetKeyDown(KeyCode.L))
        {
            List<Vector3d> localVertices = new();
            localVertices.Add(new(-BianChang / 2, 0, -BianChang / 2));
            localVertices.Add(new(-BianChang / 2, 0, BianChang / 2));
            localVertices.Add(new(BianChang / 2, 0, BianChang / 2));
            localVertices.Add(new(BianChang / 2, 0, -BianChang / 2));

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Obstacles2D obstacle0 = new(localVertices, new(j * BianChang, 0, i * BianChang));
                    m_GridMap.Obstacles2Ds.Add(obstacle0);
                }
            }

            m_TransMap = new Map();

            m_TransMap = MapCoordinateTransformation.MapTrans(m_GridMap, Angle);
            flag = true;

            m_TestMap = new();
            //localVertices.Clear();
            //for (int i = 0; i < 4; i++)
            //{
            //    //ToolM.WorldToGridVec(m_TransMap.Obstacles2Ds[0].WorldVertices[i], new Vector3d(90, 0, 0) - Angle, BianChang);
            //    localVertices.Add(ToolM.WorldToGridVec(m_TransMap.Obstacles2Ds[0].WorldVertices[i], new Vector3d(90, 0, 0), Angle, BianChang));
            //}

            //ToolM.WorldToGridVec(new Vector3d(1,0,0), new Vector3d(90, 0, 0), Angle, BianChang);
            //for (int i = 0; i < localVertices.Count; i++)
            //{
            //    Debug.DrawLine(localVertices[i], localVertices[(i + 1) % 4], Color.red);
            //}

        }

        //碰撞体生成
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    //删除所有子物体
        //    Transform tempTransform;
        //    for (int i = 0; i < m_Father.transform.childCount; i++)
        //    {
        //        tempTransform = m_Father.transform.GetChild(i);
        //        Destroy(tempTransform);
        //    }

        //    //创造碰撞体
        //    for (int i = 0; i < m_GridMap.Obstacles2Ds.Count; i++)
        //    {
        //        m_ObjCreater.CreatCollision(m_TransMap.Obstacles2Ds[i], m_Father);
        //    }
        //}

        //画线
        if (flag)
        {
            for (int i = 0; i < m_TransMap.Obstacles2Ds.Count; i++)
            {
                if (IsPointInCameraView(m_TransMap.Obstacles2Ds[i].Position, Camera.main))
                {
                    for (int j = 0; j < m_TransMap.Obstacles2Ds[i].WorldVertices.Count; j++)
                    {
                        Debug.DrawLine(m_TransMap.Obstacles2Ds[i].WorldVertices[j], m_TransMap.Obstacles2Ds[i].WorldVertices[(j + 1) % m_TransMap.Obstacles2Ds[i].WorldVertices.Count], Color.blue, Time.unscaledDeltaTime);
                    }
                    for (int j = 0; j < m_GridMap.Obstacles2Ds[i].WorldVertices.Count; j++)
                    {
                        Debug.DrawLine(m_GridMap.Obstacles2Ds[i].WorldVertices[j], m_GridMap.Obstacles2Ds[i].WorldVertices[(j + 1) % m_GridMap.Obstacles2Ds[i].WorldVertices.Count], Color.white, Time.unscaledDeltaTime);
                    }
                }
            }
        }

    }

    bool IsPointInCameraView(Vector3 worldPos, Camera cam)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);
        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1 &&
               viewportPos.z > 0; // 确保点在相机前方
    }
}
