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
    }
}
