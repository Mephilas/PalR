using Mathd;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

public class AStarOptimize2
{
    Map Map = new();
    List<OptimizePoint> result = new();

    public List<OptimizePoint> Init(List<Vector3d> aStarList, Map map)
    {
        index = 0;

        Map = map;
        result.Clear();
        for (int i = 0; i < aStarList.Count; i++)
        {
            result.Add(new(aStarList[i], new(0, 0, 0), false));
        }
        //NextStep();
        return result;
    }
    int index = 0;
    //HashSet<Vector3d> tempHashSet = new();
    public List<OptimizePoint> NextStep(ref List<OptimizePoint> tempList, ref HashSet<Vector3d> tempHashSet)
    {
        tempHashSet.Clear();
        //��������������һ���㣬������ѭ��
        if (result[index].TargetPos == result[^1].Pos)
        {
            index = 0;
            return result;
        }
        for (int j = result.Count - 1; j > index; j--)
        {
            tempList.Clear();
            Vector3d inflectionPoint = new();
            //�����ǰ�����߶ε��ҵ��Ŀ�����ǵ�ǰ������
            if (result[index].HasTarget && result[index].TargetPos == result[j].Pos)
            {
                //�����ȫһ�£����ж���һ����
                index++;

                tempList = result.GetRange(index, j - index);
                return result;
            }
            //�������֮������ϰ����������һ��������
            if (!CheckObsBy2Point(result[index].Pos, result[j].Pos, ref tempList, ref inflectionPoint))
            {
                //���ϰ����һ��������
                //����Ҫ��¼�ϰ��㣬�����һ������ͨ����Ҫ�жϹյ�λ��
                tempHashSet.Clear();
                tempHashSet.Add(inflectionPoint + new Vector3d(1, 0, 1));
                tempHashSet.Add(inflectionPoint + new Vector3d(1, 0, -1));
                tempHashSet.Add(inflectionPoint + new Vector3d(-1, 0, 1));
                tempHashSet.Add(inflectionPoint + new Vector3d(-1, 0, -1));
                continue;
            }
            else
            {
                //���û���ϰ����ʱ���������ո�µ����߶�
                //����߶��Ѿ����˹յ㣬�������߶ξ����˹յ㣬������߶β��Ϸ�
                if (result[index].HasInflection && tempHashSet.Contains(result[index].InflectionPoint))
                {
                    //�߶β��Ϸ�����һ��������
                    continue;
                }
                //����߶�û�йյ㣬������߶�ֱ�ӺϷ�
                //����ˢ���µ��߶ε���·����ȥ
                for (int i = 0; i < tempList.Count; i++)
                {
                    result[index + i] = tempList[i];
                    if (tempHashSet.Count != 0)
                    {
                        Vector3d tempInflectionPoint = GetInflectionPoint(tempHashSet, tempList);
                        result[index + i].InflectionPoint = tempInflectionPoint;
                        result[index + i].HasInflection = true;
                    }
                }
                index++;
                return result;
            }
        }
        return result;
    }

    public bool CheckInflectionPointOnLine(Vector3d inflectionPoint, List<OptimizePoint> listNewLine)
    {
        for (int i = 0; i < listNewLine.Count; i++)
        {
            if (listNewLine[i].Pos == inflectionPoint)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3d GetInflectionPoint(HashSet<Vector3d> hashSetInfPoints, List<OptimizePoint> listNewLine)
    {
        for (int i = 0; i < listNewLine.Count; i++)
        {
            if (hashSetInfPoints.Contains(listNewLine[i].Pos))
            {
                return listNewLine[i].Pos;
            }
        }
        return Vector3d.zero;
    }

    public bool CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint, ref List<OptimizePoint> gridList, ref Vector3d inflectionPoint)
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
        gridList.Add(new(startPoint, endPoint, true));
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
                    gridList.Add(new(tempCheck1, endPoint, true));
                }
                else
                {
                    inflectionPoint = tempCheck1;
                    return false;
                }
            }
            else if (dir1 < dir2)
            {
                //dir2������ԭб��
                if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                {
                    tempPos = tempCheck2;
                    gridList.Add(new(tempCheck2, endPoint, true));
                }
                else
                {
                    inflectionPoint = tempCheck2;
                    return false;
                }
            }
            else
            {
                //��������ĸ���б����ͬ
                //�Ƚ����
                double dis1 = (tempCheck1 - endPoint).magnitude;
                double dis2 = (tempCheck2 - endPoint).magnitude;
                if (dis1 < dis2)
                {
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        tempPos = tempCheck1;
                        gridList.Add(new(tempCheck1, endPoint, true));
                    }
                    else
                    {
                        inflectionPoint = tempCheck1;
                        return false;
                    }
                }
                else if (dis2 < dis1)
                {
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                    {
                        tempPos = tempCheck2;
                        gridList.Add(new(tempCheck2, endPoint, true));
                    }
                    else
                    {
                        inflectionPoint = tempCheck2;
                        return false;
                    }
                }
                else
                {
                    //������ͬ
                    //�Ⱥ����
                    if (!Map.Obstacles2Ds.ContainsKey(tempCheck1))
                    {
                        tempPos = tempCheck1;
                        gridList.Add(new(tempCheck1, endPoint, true));
                    }
                    else if (!Map.Obstacles2Ds.ContainsKey(tempCheck2))
                    {
                        tempPos = tempCheck2;
                        gridList.Add(new(tempCheck2, endPoint, true));
                    }
                    else
                    {
                        inflectionPoint = tempCheck1;
                        return false;
                    }
                }
            }
        }
        return true;
    }
}

public class OptimizePoint
{
    public Vector3d Pos;
    public Vector3d TargetPos;
    public Vector3d InflectionPoint;
    public bool HasTarget = false;
    public bool HasInflection = false;

    public OptimizePoint(Vector3d pos, Vector3d targetPos, bool hasTarget)
    {
        Pos = pos;
        TargetPos = targetPos;
        HasTarget = hasTarget;
    }
    public void SetInflection(Vector3d inflectionPoint, bool hasInflection)
    {
        InflectionPoint = inflectionPoint;
        HasInflection = hasInflection;
    }
}

public class OptimizeLine
{
    List<Vector3d> listPoints = new();
    Vector3d startPoint;
    Vector3d endPoint;
}
