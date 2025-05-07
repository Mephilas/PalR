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
        //如果遍历到了最后一个点，则跳出循环
        if (result[index].TargetPos == result[^1].Pos)
        {
            index = 0;
            return result;
        }
        for (int j = result.Count - 1; j > index; j--)
        {
            tempList.Clear();
            Vector3d inflectionPoint = new();
            //如果当前点是线段点且点的目标点就是当前遍历点
            if (result[index].HasTarget && result[index].TargetPos == result[j].Pos)
            {
                //如果完全一致，则判断下一个点
                index++;

                tempList = result.GetRange(index, j - index);
                return result;
            }
            //如果两点之间存在障碍物，则跳至下一个遍历点
            if (!CheckObsBy2Point(result[index].Pos, result[j].Pos, ref tempList, ref inflectionPoint))
            {
                //有障碍物，下一个遍历点
                //这里要记录障碍点，如果下一条线连通了则要判断拐点位置
                tempHashSet.Clear();
                tempHashSet.Add(inflectionPoint + new Vector3d(1, 0, 1));
                tempHashSet.Add(inflectionPoint + new Vector3d(1, 0, -1));
                tempHashSet.Add(inflectionPoint + new Vector3d(-1, 0, 1));
                tempHashSet.Add(inflectionPoint + new Vector3d(-1, 0, -1));
                continue;
            }
            else
            {
                //如果没有障碍物，此时的情况就是崭新的新线段
                //如果线段已经有了拐点，并且新线段经过了拐点，则代表线段不合法
                if (result[index].HasInflection && tempHashSet.Contains(result[index].InflectionPoint))
                {
                    //线段不合法，下一个遍历点
                    continue;
                }
                //如果线段没有拐点，则代表线段直接合法
                //首先刷新新的线段到旧路径中去
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
                //dir1更贴合原斜率
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
                //dir2更贴合原斜率
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
                //两个方向的格子斜率相同
                //先近后错
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
                    //距离相同
                    //先横后竖
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
