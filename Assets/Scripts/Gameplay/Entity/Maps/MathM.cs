using Mathd;
using System;
using System.Numerics;

namespace MathSelf
{
    public static class MathM
    {
        /// <summary>
        /// 三维向量去维度化(保持和中心点共面)
        /// </summary>
        /// <param name="origin">中心点</param>
        /// <param name="direction">剔除维度方向</param>
        /// <param name="value">待降维向量</param>
        /// <returns></returns>
        public static Vector3d Vector3DDimensionalityReduction(Vector3d origin, Vector3d direction, Vector3d value)
        {
            Vector3d res;

            direction = direction.normalized;
            res = value - Vector3d.Dot(value - origin, direction) * direction;

            return res;
        }

        /// <summary>
        /// 弧度角度互换
        /// </summary>
        /// <returns></returns>
        public static double RadianAndDegreeMeasure(double angle, bool isAngle)
        {
            if (isAngle)
            {
                return angle / 180f * Math.PI;
            }
            else
            {
                return angle / Math.PI * 180f;
            }
        }

        /// <summary>
        /// 左手系X轴旋转矩阵
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix4x4d GetRotateMatrixX(double angle)
        {
            Matrix4x4d Matrix = new Matrix4x4d();
            Matrix.SetRow(0, new Vector4d(1, 0, 0, 0));
            Matrix.SetRow(1, new Vector4d(0, Math.Cos(angle), -Math.Sin(angle), 0));
            Matrix.SetRow(2, new Vector4d(0, Math.Sin(angle), Math.Cos(angle), 0));
            Matrix.SetRow(3, new Vector4d(0, 0, 0, 1));
            return Matrix;
        }

        /// <summary>
        /// 左手系Y轴旋转矩阵
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix4x4d GetRotateMatrixY(double angle)
        {
            Matrix4x4d Matrix = new Matrix4x4d();
            Matrix.SetRow(0, new Vector4d(Math.Cos(angle), 0, Math.Sin(angle), 0));
            Matrix.SetRow(1, new Vector4d(0, 1, 0, 0));
            Matrix.SetRow(2, new Vector4d(-Math.Sin(angle), 0, Math.Cos(angle), 0));
            Matrix.SetRow(3, new Vector4d(0, 0, 0, 1));
            return Matrix;
        }

        /// <summary>
        /// 左手系Z轴旋转矩阵
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static Matrix4x4d GetRotateMatrixZ(double angle)
        {
            Matrix4x4d Matrix = new Matrix4x4d();
            Matrix.SetRow(0, new Vector4d(Math.Cos(angle), -Math.Sin(angle), 0, 0));
            Matrix.SetRow(1, new Vector4d(Math.Sin(angle), Math.Cos(angle), 0, 0));
            Matrix.SetRow(2, new Vector4d(0, 0, 1, 0));
            Matrix.SetRow(3, new Vector4d(0, 0, 0, 1));
            return Matrix;
        }

        /// <summary>
        /// 世界坐标转换格子坐标
        /// </summary>
        /// <param name="CheckPoint"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3d WorldToGridVec(Vector3d CheckPoint, Vector3d q)
        {
            Vector3d res = new();
            return res;
        }
    }
}