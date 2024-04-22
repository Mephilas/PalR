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
        public static Vector3d Angle;
        public static Vector3d DefaultAngle = new Vector3d(90, 0, 0);
        public static Vector3d WorldCenter = new Vector3d(0, 0, 0);

        public static Map MapTrans(Map map, Vector3d camAngle)
        {
            Map res = new Map();
            Angle = camAngle;

            Vector3d dir = (ToolM.GetRotateMatrix(DefaultAngle,true)).MultiplyVector(new Vector3d(0, 0, 1));

            for (int i = 0; i < map.Obstacles2Ds.Count; i++)
            {
                List<Vector3d> tempVertices = new List<Vector3d>();
                for (int j = 0; j < map.Obstacles2Ds[i].LocalVertices.Count; j++)
                {
                    Vector3d point = CoordinateTrans(map.Obstacles2Ds[i].LocalVertices[j]);
                    point = MathM.Vector3DDimensionalityReduction(WorldCenter, dir, point);
                    tempVertices.Add(point);
                }
                Vector3d positon = CoordinateTrans(map.Obstacles2Ds[i].Position);
                positon = MathM.Vector3DDimensionalityReduction(WorldCenter, dir, positon);
                Obstacles2D obstacles2D = new Obstacles2D(tempVertices, positon);
                res.Obstacles2Ds.Add(obstacles2D);
            }
            return res;
        }

        static Vector3d CoordinateTrans(Vector3d value)
        {
            Vector3d resVec;
            resVec = ToolM.GetRotateMatrix(-Angle, false).MultiplyVector(value);
            resVec = ToolM.GetRotateMatrix(DefaultAngle, true).MultiplyVector(resVec);
            return resVec;
        }
    }
}
