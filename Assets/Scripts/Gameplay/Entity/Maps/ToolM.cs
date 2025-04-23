using CreatCollisionTools;
using Mathd;
using MathSelf;
using System.Net;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

namespace ToolSelf
{
    public static class ToolM
    {
        /// <summary>
        /// double三维向量转float
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vector3[] Vector3dArrayToNor(Vector3d[] value)
        {
            Vector3[] res = new Vector3[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                res[i] = value[i];
            }
            return res;
        }

        /// <summary>
        /// 获取旋转矩阵（角度制）
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <param name="flag">旋转顺序</param>
        /// <returns></returns>
        public static Matrix4x4d GetRotateMatrix(Vector3d angle, bool flag)
        {
            Matrix4x4d resVec;

            angle.x = MathM.RadianAndDegreeMeasure(angle.x, true);
            angle.y = MathM.RadianAndDegreeMeasure(angle.y, true);
            angle.z = MathM.RadianAndDegreeMeasure(angle.z, true);

            Matrix4x4d MatrixX = MathM.GetRotateMatrixX(angle.x);
            Matrix4x4d MatrixY = MathM.GetRotateMatrixY(angle.y);
            Matrix4x4d MatrixZ = MathM.GetRotateMatrixZ(angle.z);

            if (flag)
            {
                resVec = MatrixY * MatrixX * MatrixZ;

            }
            else
            {
                resVec = MatrixZ * MatrixX * MatrixY;
            }

            return resVec;
        }

        /// <summary>
        /// 获取旋转矩阵（角度制）
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <param name="flag">旋转顺序</param>
        /// <returns></returns>
        public static Matrix4x4d GetRotateMatrixI(Vector3d angle, bool flag)
        {
            Matrix4x4d resVec;

            angle.x = MathM.RadianAndDegreeMeasure(angle.x, true);
            angle.y = MathM.RadianAndDegreeMeasure(angle.y, true);
            angle.z = MathM.RadianAndDegreeMeasure(angle.z, true);

            Matrix4x4d MatrixX = MathM.GetRotateMatrixX(-angle.x);
            Matrix4x4d MatrixY = MathM.GetRotateMatrixY(-angle.y);
            Matrix4x4d MatrixZ = MathM.GetRotateMatrixZ(-angle.z);

            if (flag)
            {
                resVec = MatrixZ * MatrixX * MatrixY;

            }
            else
            {
                resVec = MatrixY * MatrixX * MatrixZ;
            }

            return resVec;
        }


        /// <summary>
        /// 相机平面点和格子平面点映射计算
        /// </summary>
        /// <param name="CheckPoint">相机平面点</param>
        /// <param name="DefaultAngle">默认相机的角度</param>
        /// <returns>返回格子坐标</returns>
        public static Vector3d WorldToGridVec(Vector3d CheckPoint)
        {
            Matrix4x4d WorldGridMI = ToolM.GetRotateMatrixI(MapCoordinateTransformation.DefaultAngle - MapCoordinateTransformation.CameraAngle, false);//格子平面映射到相机平面矩阵的逆
            Matrix4x4d DefaultM = ToolM.GetRotateMatrix(MapCoordinateTransformation.DefaultAngle, false);//默认相机视角下的旋转矩阵，主要用于计算相机平面和格子平面的法线
            Vector3d res = CheckPoint;
            //res += new Vector3d(-0.08f, 0, 0.035f);
            res.z /= 1.2;


            //这里的旋转矩阵之所以不取Y轴的旋转，是因为Y轴的旋转对于平面来说不会影响到对穿点之间的距离。
            Matrix4x4d Camera2GridHeightM = ToolM.GetRotateMatrix(new(MapCoordinateTransformation.CameraAngle.x, 0, MapCoordinateTransformation.CameraAngle.z), false);
            Vector3d NorCamera = DefaultM * new Vector3d(0, 0, 1);
            Vector3d NorGrid = (Camera2GridHeightM * DefaultM * new Vector3d(0, 1, 0)).normalized;
            double CheckPointLengthNormal = Vector3d.Dot(res, NorGrid);//检查点在格子法线上的映射长度
            double MappingLengthNormals = Vector3d.Dot(NorCamera, NorGrid);//两个平面法线的映射长度
            res += CheckPointLengthNormal / MappingLengthNormals * NorCamera;
            res = WorldGridMI * res;
            res.y = 0;
            res += new Vector3d(Map.GridSize / 2, 0, Map.GridSize / 2);
            if (res.x >= 0)
            {
                res.x = (int)(res.x / Map.GridSize);
            }
            else
            {
                res.x = ((int)(res.x / Map.GridSize) - 1);
            }

            if (res.z >= 0)
            {
                res.z = (int)(res.z / Map.GridSize);
            }
            else
            {
                res.z = ((int)(res.z / Map.GridSize) - 1);
            }

            return res;
        }

        public static Vector3d GetWorldPosByGrid(Vector3d value)
        {
            Vector3d res = new(value.x * Map.GridSize, 0, value.z * Map.GridSize);
            return res;
        }

        /// <summary>
        /// 判断某个点是否处于正交相机视野里
        /// </summary>
        /// <param name="worldPos"></param>
        /// <param name="cam"></param>
        /// <returns></returns>
        public static bool IsPointInCameraView(Vector3d worldPos, Camera cam)
        {
            Vector3d viewportPos = cam.WorldToViewportPoint(worldPos);
            return viewportPos.x >= 0 && viewportPos.x <= 1 &&
                   viewportPos.y >= 0 && viewportPos.y <= 1 &&
                   viewportPos.z > 0; // 确保点在相机前方
        }
    
        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <returns></returns>
        public static double GetTileDistance(Vector3d pos1, Vector3d pos2)
        {
            double result=0;
            result = Math.Abs(pos1.x - pos2.x) + Math.Abs(pos1.z - pos2.z);
            return result;
        }
        /// <summary>
        /// 欧式距离
        /// </summary>
        /// <returns></returns>
        public static double GetEuclideanDistance(Vector3d pos1, Vector3d pos2)
        {
            return (pos1 - pos2).magnitude;
        }
    }
}