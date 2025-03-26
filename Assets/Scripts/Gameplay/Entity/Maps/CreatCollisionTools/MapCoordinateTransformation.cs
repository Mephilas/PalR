using Mathd;
using MathSelf;
using ToolSelf;
using System.Collections.Generic;
using UnityEngine;
using System.Drawing;

namespace CreatCollisionTools
{
    public static class MapCoordinateTransformation
    {
        public static Vector3d CameraAngle = new(30, 45, 0);
        public static Vector3d DefaultAngle = new Vector3d(90, 0, 0);
        public static Vector3d WorldCenter = new Vector3d(0, 0, 0);

        public static Map MapTrans(Map map)
        {
            Map res = new Map();
            Vector3d dir = (ToolM.GetRotateMatrix(DefaultAngle, true)).MultiplyVector(new Vector3d(0, 0, 1));
            //Debug.Log(dir);

            for (int i = 0; i < map.Obstacles2Ds.Count; i++)
            {
                List<Vector3d> tempVertices = new List<Vector3d>();
                for (int j = 0; j < map.Obstacles2Ds[i].LocalVertices.Count; j++)
                {
                    Vector3d point = CoordinateTrans(map.Obstacles2Ds[i].LocalVertices[j]);
                    point = MathM.Vector3DDimensionalityReduction(WorldCenter, dir, point);
                    point.z *= 1.2f;
                    tempVertices.Add(point);
                }
                Vector3d positon = CoordinateTrans(map.Obstacles2Ds[i].Position);
                positon = MathM.Vector3DDimensionalityReduction(WorldCenter, dir, positon);
                positon.z *= 1.2f;
                positon -= new Vector3d(-0.08f, 0, 0.035f);
                Obstacles2D obstacles2D = new Obstacles2D(tempVertices, positon, map.Obstacles2Ds[i].GirdPosition);
                res.Obstacles2Ds.Add(obstacles2D);
            }
            return res;
        }

        static Vector3d CoordinateTrans(Vector3d value)
        {
            Vector3d resVec;
            //DefaultAngle是初始的相机视角，这个视角下默认格子为一条缝，所以旋转角要先加上相机的默认角，然后格子也跟着默认角旋转相同的角度，这样初始化视角就完成了。
            //然后-Angle之所以用负角度是因为现在模拟相机围绕Angle的角度绕着物体旋转，变相的就是物体自转但是反方向，所以是负值。
            //因为在这个项目里相机只围绕格子的Y轴和X轴旋转，所以GetRotateMatrix的旋转轴顺序使用先Y轴再X轴旋转，所以旋转轴的顺序采用Z-X-Y的矩阵，因为是左乘矩阵所以优先级从右往左。
            resVec = ToolM.GetRotateMatrix(DefaultAngle - CameraAngle, false).MultiplyVector(value);
            return resVec;
        }
    }
}
