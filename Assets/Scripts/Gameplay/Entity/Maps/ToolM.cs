using Mathd;
using MathSelf;
using UnityEngine;

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
        /// 世界坐标转换格子坐标
        /// </summary>
        /// <param name="CheckPoint"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public static Vector3d WorldToGridVec(Vector3d CheckPoint, Vector3d DefaultAngle, Vector3d CameraAngle, double BianChang)
        {
            Matrix4x4d mI = ToolM.GetRotateMatrixI(DefaultAngle - CameraAngle, false);
            Vector3d res = CheckPoint;
            //res += new Vector3d(-0.08f, 0, 0.035f);
            //res.z /= 1.2;

            Matrix4x4d m = ToolM.GetRotateMatrix(-CameraAngle, false);
            Vector3d tempNor = (m * new Vector3d(0, 1, 0)).normalized;
            double norLength = Vector3d.Dot(new Vector3d(0, 1, 0), tempNor);
            double CheckPointLength = Vector3d.Dot(res, tempNor);
            res -= CheckPointLength / norLength * new Vector3d(0, 1, 0);


            res = mI * res;

            Debug.Log(res);

            //vec += new Vector3d(BianChang / 2, 0, BianChang / 2);

            //if (vec.x >= 0)
            //{
            //    res.x = (int)(vec.x / BianChang) * BianChang;
            //}
            //else
            //{
            //    res.x = ((int)(vec.x / BianChang) - 1) * BianChang;
            //}

            //if (vec.z >= 0)
            //{
            //    res.z = (int)(vec.z / BianChang);
            //}
            //else
            //{
            //    res.z = (int)(vec.z / BianChang) - 1;
            //}

            return res;
        }
    }
}
