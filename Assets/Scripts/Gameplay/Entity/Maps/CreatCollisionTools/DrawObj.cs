using CreatCollisionTools;
using Mathd;
using MathSelf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawObj : MonoBehaviour
{
    public Vector3d Angle = new(30, 45, 0);

    Map m_GridMap = new();
    Map m_TransMap = new();
    ObsCreater m_ObjCreater = new();
    public GameObject m_Father;

    bool flag = false;
    void Update()
    {

        //地图信息转换
        if (Input.GetKeyDown(KeyCode.L))
        {
            List<Vector3d> localVertices = new();
            double aaa = 0.32 / Math.Sqrt(2);
            localVertices.Add(new(-aaa / 2, 0, -aaa / 2));
            localVertices.Add(new(-aaa / 2, 0, aaa / 2));
            localVertices.Add(new(aaa / 2, 0, aaa / 2));
            localVertices.Add(new(aaa / 2, 0, -aaa / 2));

            for (int i = -200; i < 200; i++)
            {
                for (int j = -200; j < 200; j++)
                {
                    Obstacles2D obstacle0 = new(localVertices, new(j * aaa, 0, i * aaa));
                    m_GridMap.Obstacles2Ds.Add(obstacle0);
                }
            }

            m_TransMap = new Map(m_GridMap);

            m_TransMap = MapCoordinateTransformation.MapTrans(m_GridMap, Angle);
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
            for (int i = 0; i < m_GridMap.Obstacles2Ds.Count; i++)
            {
                m_ObjCreater.CreatCollision(m_TransMap.Obstacles2Ds[i], m_Father);
            }
        }

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
