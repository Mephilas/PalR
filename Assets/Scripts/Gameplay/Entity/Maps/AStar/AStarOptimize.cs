using Mathd;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AStarOptimize
{
    Vector3d StartPoint;
    Vector3d EndPoint;
    Map Map = new();

    List<AStarGrid> CheckList = new();
    Dictionary<Vector3d, AStarGrid> AllGrid = new();

    /// <summary>
    /// ����A*����·�߹յ�
    /// </summary>
    /// <param name="aStarList"></param>
    /// <returns></returns>
    public List<Vector3d> InflectionPointCalcByAStar(List<Vector3d> aStarList, Map map)
    {
        Map = map;
        bool isDown = false;
        List<Vector3d> InflectionPointList = new();
        Vector3d EndPos = aStarList[^1];
        InflectionPointList.Add(EndPos);
        Vector3d lastObsPos = Vector3d.zero;
        for (; ; )
        {
            for (int i = 0; i < aStarList.Count; i++)
            {
                Vector3d tempVec = CheckObsBy2Point(aStarList[i], EndPos);
                if (tempVec == aStarList[i])
                {
                    //���û���ϰ����ȡ��һ���ϰ����λ�ü���յ�λ��
                    //��ȡ��һ�������ϰ���ʱ��Ѱ·��ʼ����
                    if (i == 0)
                    {
                        InflectionPointList.Add(aStarList[i]);
                        isDown = true;
                    }
                    else
                    {
                        Vector3d trueDir = EndPos - aStarList[i];
                        Vector3d lastDir = EndPos - aStarList[i - 1];
                        Vector3d resultTrun;
                        Vector3d Left;
                        Vector3d Right;

                        double tempX = 0;
                        double tempZ = 0;
                        //�����������ߵĹյ�����
                        if (lastDir.x > 0)
                        {
                            tempZ = 1;
                        }
                        else if (lastDir.x < 0)
                        {
                            tempZ = -1;
                        }
                        if (lastDir.z > 0)
                        {
                            tempX = 1;
                        }
                        else if (lastDir.z < 0)
                        {
                            tempX = -1;
                        }
                        Left = new Vector3d(-tempX, 0, tempZ);
                        Right = new Vector3d(tempX, 0, -tempZ);
                        if (lastDir.x == 0)
                        {
                            Left = new Vector3d(-tempX, 0, tempX);
                            Right = new Vector3d(tempX, 0, tempX);
                        }
                        if (lastDir.z == 0)
                        {
                            Left = new Vector3d(tempZ, 0, tempZ);
                            Right = new Vector3d(tempZ, 0, -tempZ);
                        }
                        Left = lastObsPos + Left;
                        Right = lastObsPos + Right;
                        if (Vector3d.Cross(lastDir, trueDir).y > 0)
                        {
                            resultTrun = Left;
                        }
                        else
                        {
                            resultTrun = Right;
                        }
                        InflectionPointList.Add(resultTrun);
                        EndPos = resultTrun;
                    }
                    break;
                }
                else
                {
                    lastObsPos = tempVec;
                }
            }
            if (isDown)
            {
                break;
            }
        }
        List<Vector3d> result = new();
        result.Add(InflectionPointList[0]);
        for (int i = InflectionPointList.Count - 1; i > 0; i--)
        {
            result.AddRange(CalcPathList(InflectionPointList[i], InflectionPointList[i - 1]));
        }
        return result;
    }

    public List<Vector3d> InflectionPointCalcByAStar2(List<Vector3d> aStarList, Map map)
    {
        Map = map;
        List<Vector3d> InflectionPointList = new(aStarList);
        int index = 0;
        for (; ; )
        {
            if (index == aStarList.Count - 1)
            {
                break;
            }
            for (int j = aStarList.Count - 1; j > index; j--)
            {
                Vector3d tempVec = CheckObsBy2Point(aStarList[index], aStarList[j]);
                if (tempVec == aStarList[index])
                {
                    //���û����ײ�ɹ�������ֱ��
                    //�����ж���ֱ���Ƿ���ԭ����ֱ����ȫ��ͬ

                    List<Vector3d> tempList = CalcPathList(aStarList[index], aStarList[j]);
                    if (Check2LineSame(tempList, aStarList.GetRange(index, j - index)))
                    {
                        //�����ȫһ�����������һ������ж�
                        index++;
                    }
                    else
                    {
                        //��������߲�ƥ��˵����Ҫ�Ż���·
                        //�����Ż���·
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            aStarList[index + i] = tempList[i];
                        }
                        //�޸�����
                        index = j;
                    }
                    break;
                }
                else
                {
                    //�����ײ�ˣ���������������һ����
                    continue;
                }
            }
        }

        return aStarList;
    }

    /// <summary>
    /// ��������֮���ֱ��·��
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public List<Vector3d> CalcPathList(Vector3d startPoint, Vector3d endPoint)
    {
        List<Vector3d> resultList = new();
        Vector3d targetDir = (endPoint - startPoint).normalized;
        int tempX;
        int tempZ;
        if (targetDir.x > 0)
        {
            tempX = 1;
        }
        else if (targetDir.x < 0)
        {
            tempX = -1;
        }
        else
        {
            tempX = 0;
        }
        if (targetDir.z > 0)
        {
            tempZ = 1;
        }
        else if (targetDir.z < 0)
        {
            tempZ = -1;
        }
        else
        {
            tempZ = 0;
        }

        Vector3d tempPos = startPoint;
        for (; ; )
        {
            resultList.Add(tempPos);
            Vector3d tempCheck1 = tempPos + new Vector3d(tempX, 0, 0);
            Vector3d tempCheck2 = tempPos + new Vector3d(0, 0, tempZ);

            if (tempCheck1 == endPoint || tempCheck2 == endPoint)
            {
                break;
            }

            double dir1 = Vector3d.Dot((endPoint - tempCheck1).normalized, targetDir);
            double dir2 = Vector3d.Dot((endPoint - tempCheck2).normalized, targetDir);

            if (dir1 > dir2)
            {
                tempPos = tempCheck1;
            }
            else if (dir1 < dir2)
            {
                tempPos = tempCheck2;
            }
            else
            {
                //��������ĸ���б����ͬ
                //�Ƚ���Զ
                double dis1 = (tempCheck1 - endPoint).magnitude;
                double dis2 = (tempCheck2 - endPoint).magnitude;
                if (dis1 < dis2)
                {
                    tempPos = tempCheck1;
                }
                else if (dis2 < dis1)
                {
                    tempPos = tempCheck2;
                }
                else
                {
                    //������ͬ
                    //�Ⱥ����
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        tempPos = tempCheck1;
                    }
                    else
                    {
                        tempPos = tempCheck2;
                    }
                }
            }
        }
        return resultList;
    }

    /// <summary>
    /// ������ϰ���򷵻��ϰ��������
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <returns></returns>
    public Vector3d CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint)
    {
        Vector3d targetDir = (endPoint - startPoint).normalized;
        int tempX;
        int tempZ;
        if (targetDir.x > 0)
        {
            tempX = 1;
        }
        else if (targetDir.x < 0)
        {
            tempX = -1;
        }
        else
        {
            tempX = 0;
        }
        if (targetDir.z > 0)
        {
            tempZ = 1;
        }
        else if (targetDir.z < 0)
        {
            tempZ = -1;
        }
        else
        {
            tempZ = 0;
        }

        Vector3d tempPos = startPoint;
        for (; ; )
        {
            Vector3d tempCheck1 = tempPos + new Vector3d(tempX, 0, 0);
            Vector3d tempCheck2 = tempPos + new Vector3d(0, 0, tempZ);

            if (tempCheck1 == endPoint || tempCheck2 == endPoint)
            {
                break;
            }

            double dir1 = Vector3d.Dot((endPoint - tempCheck1).normalized, targetDir);
            double dir2 = Vector3d.Dot((endPoint - tempCheck2).normalized, targetDir);

            if (dir1 > dir2)
            {
                //dir1������ԭб��
                if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                {
                    tempPos = tempCheck1;
                }
                else
                {
                    return tempCheck1;
                }
            }
            else if (dir1 < dir2)
            {
                //dir2������ԭб��
                if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                {
                    tempPos = tempCheck2;
                }
                else
                {
                    return tempCheck2;
                }
            }
            else
            {
                //��������ĸ���б����ͬ
                //�Ƚ���Զ
                double dis1 = (tempCheck1 - endPoint).magnitude;
                double dis2 = (tempCheck2 - endPoint).magnitude;
                if (dis1 < dis2)
                {
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        tempPos = tempCheck1;
                    }
                    else
                    {
                        return tempCheck1;
                    }
                }
                else if (dis2 < dis1)
                {
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                    {
                        tempPos = tempCheck2;
                    }
                    else
                    {
                        return tempCheck2;
                    }
                }
                else
                {
                    //������ͬ
                    //�Ⱥ����
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        tempPos = tempCheck1;
                    }
                    else if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                    {
                        tempPos = tempCheck2;
                    }
                    else
                    {
                        return tempCheck1;
                    }
                }
            }
        }
        return startPoint;
    }

    public bool Check2LineSame(List<Vector3d> line1, List<Vector3d> line2)
    {
        if (line1.Count != line2.Count)
        {
            Debug.LogError("���������߲�һ������");
            return false;
        }
        for (int i = 0; i < line1.Count; i++)
        {
            if (line1[i] != line2[i])
            {
                return false;
            }
        }
        return true;
    }
}
