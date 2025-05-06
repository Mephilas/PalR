using Mathd;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AStarOptimize2
{
    Map Map = new();
    List<OptimizeGrid> result = new();

    public void Init(List<Vector3d> aStarList, Map map)
    {
        Map = map;
        result.Clear();
        for (int i = 0; i < aStarList.Count; i++)
        {
            result.Add(new(aStarList[i], new(0, 0, 0), false));
        }
    }

    public List<OptimizeGrid> NextStep()
    {
        int index = 0;
        for (; ; )
        {
            //如果遍历到了最后一个点，则跳出循环
            if (index == result.Count - 1)
            {
                break;
            }
            for (int j = result.Count - 1; j > index; j--)
            {
                //如果当前点是线段点且点的目标点就是当前遍历点
                if (result[index].HasTarget && result[index].TargetPos == result[j].Pos)
                {
                    //如果完全一致，则判断下一个点
                    index++;
                    break;
                }
                List<OptimizeGrid> tempList = new();
                //如果两点之间存在障碍物，则跳至下一个遍历点
                if (!CheckObsBy2Point(result[index].Pos, result[j].Pos, ref tempList))
                {
                    //有障碍物，下一个遍历点
                    continue;
                }
                else
                {
                    //如果没有障碍物，此时的情况就是崭新的新线段
                    //首先刷新新的线段到旧路径中去
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        result[index + i] = tempList[i];
                    }
                    index++;
                    break;
                }
            }
        }

        //List<Vector3d> tempList2 = new();
        //for (int i = 0; i < result.Count; i++)
        //{
        //    tempList2.Add(result[i].Pos);
        //}
        return result;
    }

    public bool CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint, ref List<OptimizeGrid> gridList)
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
                        return false;
                    }
                }
            }
        }
        return true;
    }

    public bool Check2LineSame(List<Vector3d> line1, List<Vector3d> line2)
    {
        if (line1.Count != line2.Count)
        {
            Debug.LogError("错误！两条线不一样长！");
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

public class OptimizeGrid
{
    public Vector3d Pos;
    public Vector3d TargetPos;
    public bool HasTarget = false;

    public OptimizeGrid(Vector3d pos, Vector3d targetPos, bool hasTarget)
    {
        Pos = pos;
        TargetPos = targetPos;
        HasTarget = hasTarget;
    }
}
