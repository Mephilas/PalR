using Mathd;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class AStarOptimize2
{
    Map Map = new();
    List<OptimizePoint> result = new();
    int count = 0;

    public List<OptimizePoint> Init(List<Vector3d> aStarList, Map map)
    {
        index = 0;
        Map = map;
        result.Clear();
        for (int i = 0; i < aStarList.Count; i++)
        {
            result.Add(new(i, aStarList[i]));
        }
        count = aStarList.Count;
        //NextStep();
        return result;
    }
    int index = 0;
    List<Vector3d> obsPos = new();
    public List<OptimizePoint> NextStep(ref List<Vector3d> tempList, ref List<Vector3d> lastObsPos)
    {
        obsPos.Clear();
        //��������յ�
        if (index == count - 1)
        {
            index = 0;
            return result;
        }

        for (int i = count - 1; i > index; i--)
        {
            lastObsPos.Clear();
            //�����Ŀ������յ㣬���ü����˷��ؾ����ˡ�
            if (i <= result[index].TargetIndex)
            {
                index++;
                break;
            }
            if (!CheckObsBy2Point(result[index].Pos, result[i].Pos, ref tempList, ref lastObsPos))
            {
                //����ͨ
                obsPos = new(lastObsPos);
                continue;
            }
            else
            {
                //��ͨ��
                //�����ж��Ƿ�����Ч�߶�
                Vector3d tempInf1 = new(0, 1, 0);
                Vector3d tempInf2 = new(0, 1, 0);
                //1.�����������µĹյ�
                //�жϸ������
                if (obsPos.Count > 0)
                {
                    if (i == count - 1 && index != 0)
                    {
                        //���߶�β�����յ���ͷ�������,����ʹ�ú���
                        tempInf1 = CalcInflectionPoint(result[i].Pos, result[index - 1].Pos, result[index].Pos, obsPos[0]);
                        tempInf2 = CalcInflectionPoint(result[i].Pos, result[index - 1].Pos, result[index].Pos, obsPos[^1]);
                    }
                    else if (i != count - 1)
                    {
                        tempInf1 = CalcInflectionPoint(result[index].Pos, result[i + 1].Pos, result[i].Pos, obsPos[0]);
                        tempInf2 = CalcInflectionPoint(result[index].Pos, result[i + 1].Pos, result[i].Pos, obsPos[^1]);
                    }
                    lastObsPos.Clear();
                    lastObsPos.Add(tempInf1);
                    lastObsPos.Add(tempInf2);
                }

                //�жϵ��Ƿ���Ŀ���
                if (result[index].TargetIndex != -1)
                {
                    //����߱�Ŀ��㣬˵�������Ѿ���ʼ��
                    //2.�ж����߶��Ƿ�������߶���ͬһ��
                    if (result[index].InflectionPos1 == tempInf1 || result[index].InflectionPos2 == tempInf1 || result[index].InflectionPos1 == tempInf2 || result[index].InflectionPos2 == tempInf2)
                    {
                        index++;
                        //��ͬһ���߶Σ�ֱ������
                        break;
                    }
                }
                //����ɸ�������߶Σ��������ݴ���
                //�µ��߶�Ҫ������������
                //1.�������߶�ͷ���ľ��߶�Ŀ���͹յ�
                for (int j = index - 1; j >= 0; j--)
                {
                    if (result[j].InflectionPos1 == result[index].InflectionPos1)
                    {
                        result[j].InflectionPos1 = result[index].Pos;
                        result[j].InflectionPos2 = result[index].Pos;
                        result[j].TargetIndex = index;
                    }
                    else
                    {
                        break;
                    }
                }

                //��������������û�йյ�����ɣ���������߶ξ���ֱ��ͨ���յ���߶λ��ߵ�δ��ʼ��,�����߶ε���һ�����߶�ֱ����ֵ����
                if (tempInf1.y == 1 && tempInf2.y == 1)
                {
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        result[index + j].Pos = tempList[j];
                        result[index + j].TargetIndex = i;
                    }
                }
                else
                {
                    int InfIndex1 = -1;
                    int InfIndex2 = -1;
                    //2.�������߶ιյ�֮ǰ�Ĳ���
                    //2.1�ҵ��յ������߶��е�����
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        if (tempList[j] == tempInf1)
                        {
                            InfIndex1 = j;
                        }
                        if (tempList[j] == tempInf2)
                        {
                            InfIndex2 = j;
                        }
                        if (InfIndex1 != -1 && InfIndex2 != -1)
                        {
                            break;
                        }
                    }

                    if (InfIndex1 == -1 || InfIndex2 == -1)
                    {
                        Debug.Log(InfIndex1);
                        Debug.Log(InfIndex2);

                        Debug.Log(tempList.Count);
                    }


                    //2.2�����߶�ǰ���ε�����
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        result[index + j].Pos = tempList[j];
                        if (j < InfIndex1 || j < InfIndex2)
                        {
                            //����ǰ��ε�����
                            result[index + j].InflectionPos1 = tempList[InfIndex1];
                            result[index + j].InflectionPos2 = tempList[InfIndex2];
                            result[index + j].TargetIndex = InfIndex2 + index;
                        }
                        else
                        {
                            //������ε�����
                            result[index + j].TargetIndex = i;
                        }
                    }
                }
                //�ü��������
                index++;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// �������֮���Ƿ���ͨ
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="gridList"></param>
    /// <param name="obsPos"></param>
    /// <returns></returns>
    public bool CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint, ref List<Vector3d> gridList, ref List<Vector3d> obsPos)
    {
        gridList = new();
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
        gridList.Add(startPoint);
        for (; ; )
        {
            Vector3d tempCheck1 = tempPos + new Vector3d(tempX, 0, 0);
            Vector3d tempCheck2 = tempPos + new Vector3d(0, 0, tempZ);

            if (tempCheck1 == endPoint || tempCheck2 == endPoint)
            {
                gridList.Add(endPoint);
                break;
            }

            double dir1 = Vector3d.Dot((endPoint - tempCheck1).normalized, targetDir);
            double dir2 = Vector3d.Dot((endPoint - tempCheck2).normalized, targetDir);

            if (dir1 > dir2)
            {
                //dir1������ԭб��
                if (Map.Obstacles2Ds.ContainsKey(tempCheck1))
                {
                    obsPos.Add(tempCheck1);
                }
                tempPos = tempCheck1;
                gridList.Add(tempCheck1);
            }
            else if (dir1 < dir2)
            {
                //dir2������ԭб��
                if (Map.Obstacles2Ds.ContainsKey(tempCheck2))
                {
                    obsPos.Add(tempCheck2);
                }
                tempPos = tempCheck2;
                gridList.Add(tempCheck2);
            }
            else
            {
                //��������ĸ���б����ͬ
                //�Ƚ����
                double dis1 = (tempCheck1 - endPoint).magnitude;
                double dis2 = (tempCheck2 - endPoint).magnitude;
                if (dis1 < dis2)
                {
                    if (Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        obsPos.Add(tempCheck1);
                    }
                    tempPos = tempCheck1;
                    gridList.Add(tempCheck1);
                }
                else if (dis2 < dis1)
                {
                    if (Map.Obstacles2Ds.ContainsKey(tempCheck2))
                    {
                        obsPos.Add(tempCheck2);
                    }
                    tempPos = tempCheck2;
                    gridList.Add(tempCheck2);
                }
                else
                {
                    //������ͬ
                    //�Ⱥ����
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        tempPos = tempCheck1;
                        gridList.Add(tempCheck1);
                    }
                    else if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                    {
                        tempPos = tempCheck2;
                        gridList.Add(tempCheck2);
                    }
                    else
                    {
                        tempPos = tempCheck1;
                        obsPos.Add(tempCheck1);
                        gridList.Add(tempCheck1);
                    }
                }
            }
        }
        if (obsPos.Count == 0)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    /// <summary>
    /// ����յ�
    /// </summary>
    /// <param name="checkPos"></param>
    /// <param name="lastPos"></param>
    /// <param name="truePos"></param>
    /// <param name="obsPos"></param>
    /// <returns></returns>
    public Vector3d CalcInflectionPoint(Vector3d checkPos, Vector3d lastPos, Vector3d truePos, Vector3d obsPos)
    {
        Vector3d trueDir = truePos - checkPos;
        Vector3d lastDir = lastPos - checkPos;
        Vector3d Left;
        Vector3d Right;

        double tempX = 0;
        double tempZ = 0;

        if (lastDir.x < 0)
        {
            tempZ = -1;
        }
        else if (lastDir.x > 0)
        {
            tempZ = 1;
        }
        if (lastDir.z > 0)
        {
            tempX = -1;
        }
        else if (lastDir.z < 0)
        {
            tempX = 1;
        }

        Left = new(tempX, 0, tempZ);
        Right = new(-tempX, 0, -tempZ);
        if (lastDir.x == 0)
        {
            Left = new(tempX, 0, tempX);
            Right = new(-tempX, 0, tempX);
        }
        if (lastDir.z == 0)
        {
            Left = new(-tempZ, 0, tempZ);
            Right = new(-tempZ, 0, -tempZ);
        }
        Left = Left + obsPos;
        Right = Right + obsPos;
        if (Vector3d.Cross(lastDir, trueDir).y > 0)
        {
            return Right;
        }
        else
        {
            return Left;
        }
    }
}

public class OptimizePoint
{
    public int Index = -1;
    public Vector3d Pos;
    public int TargetIndex = -1;
    public Vector3d InflectionPos1 = new(0, 1, 0);
    public Vector3d InflectionPos2 = new(0, 1, 0);

    public OptimizePoint(int index, Vector3d pos)
    {
        Index = index;
        Pos = pos;
    }
}