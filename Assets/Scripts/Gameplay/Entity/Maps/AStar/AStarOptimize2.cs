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
        //检查点就是终点
        if (index == count - 1)
        {
            index = 0;
            return result;
        }

        for (int i = count - 1; i > index; i--)
        {
            lastObsPos.Clear();
            //检查点的目标点是终点，则不用继续了返回就行了。
            if (i <= result[index].TargetIndex)
            {
                index++;
                break;
            }
            if (!CheckObsBy2Point(result[index].Pos, result[i].Pos, ref tempList, ref lastObsPos))
            {
                //连不通
                obsPos = new(lastObsPos);
                continue;
            }
            else
            {
                //连通了
                //优先判断是否是有效线段
                Vector3d tempInf1 = new(0, 1, 0);
                Vector3d tempInf2 = new(0, 1, 0);
                //1.计算各种情况下的拐点
                //判断各种情况
                if (obsPos.Count > 0)
                {
                    if (i == count - 1 && index != 0)
                    {
                        //新线段尾部在终点且头不在起点,反向使用函数
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

                //判断点是否有目标点
                if (result[index].TargetIndex != -1)
                {
                    //检查点具备目标点，说明检查点已经初始化
                    //2.判断新线段是否和已有线段是同一条
                    if (result[index].InflectionPos1 == tempInf1 || result[index].InflectionPos2 == tempInf1 || result[index].InflectionPos1 == tempInf2 || result[index].InflectionPos2 == tempInf2)
                    {
                        index++;
                        //是同一条线段，直接跳过
                        break;
                    }
                }
                //经过筛查是新线段，进行数据处理
                //新的线段要处理三段数据
                //1.处理新线段头部的旧线段目标点和拐点
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

                //分两种情况，如果没有拐点的生成，则表明新线段就是直接通向终点的线段或者点未初始化,整条线段当作一整条线段直连赋值即可
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
                    //2.处理新线段拐点之前的部分
                    //2.1找到拐点在新线段中的索引
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


                    //2.2处理线段前后半段的数据
                    for (int j = 0; j < tempList.Count; j++)
                    {
                        result[index + j].Pos = tempList[j];
                        if (j < InfIndex1 || j < InfIndex2)
                        {
                            //处理前半段的数据
                            result[index + j].InflectionPos1 = tempList[InfIndex1];
                            result[index + j].InflectionPos2 = tempList[InfIndex2];
                            result[index + j].TargetIndex = InfIndex2 + index;
                        }
                        else
                        {
                            //处理后半段的数据
                            result[index + j].TargetIndex = i;
                        }
                    }
                }
                //该检查点检查完毕
                index++;
                break;
            }
        }
        return result;
    }

    /// <summary>
    /// 检查两点之间是否连通
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
                //dir1更贴合原斜率
                if (Map.Obstacles2Ds.ContainsKey(tempCheck1))
                {
                    obsPos.Add(tempCheck1);
                }
                tempPos = tempCheck1;
                gridList.Add(tempCheck1);
            }
            else if (dir1 < dir2)
            {
                //dir2更贴合原斜率
                if (Map.Obstacles2Ds.ContainsKey(tempCheck2))
                {
                    obsPos.Add(tempCheck2);
                }
                tempPos = tempCheck2;
                gridList.Add(tempCheck2);
            }
            else
            {
                //两个方向的格子斜率相同
                //先近后错
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
                    //距离相同
                    //先横后竖
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
    /// 计算拐点
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