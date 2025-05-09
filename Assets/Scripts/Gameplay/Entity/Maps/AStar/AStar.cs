using Mathd;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    Vector3d StartPoint;
    Vector3d EndPoint;
    Map Map;
    List<AStarGrid> CheckList = new();
    Dictionary<Vector3d, AStarGrid> AllGrid = new();

    /// <summary>
    /// 获得A*路线
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public List<Vector3d> AStarCalc(Vector3d startPoint, Vector3d endPoint, Map map)
    {
        int count = 0;
        List<Vector3d> result = new();
        StartPoint = startPoint;
        EndPoint = endPoint;
        Map = map;
        CheckList.Clear();
        AllGrid.Clear();
        AStarGrid startGrid = new(StartPoint, EndPoint)
        {
            H = 0
        };
        CheckList.Add(startGrid);
        AllGrid.Add(StartPoint, startGrid);
        for (; count < 10000;)
        {
            count++;
            if (NextStep())
            {
                if (!AllGrid.ContainsKey(EndPoint))
                {
                    Debug.LogError("寻路失败！");
                    break;
                }
                AStarGrid tempGrid = AllGrid[EndPoint];
                for (; ; )
                {
                    result.Add(tempGrid.Pos);
                    if (tempGrid.LastGrid == null)
                    {
                        break;
                    }
                    tempGrid = tempGrid.LastGrid;
                }
                break;
            }
        }
        return result;
    }

    public bool NextStep()
    {
        //先检查当前列表是否还有遍历的格子
        if (CheckList.Count == 0)
        {
            return true;
        }
        //取出第一个格子来检查
        AStarGrid checkGrid = CheckList[0];
        CheckList.RemoveAt(0);

        //检查当前需要查验的点是否已经是终点了
        if (checkGrid.Pos == EndPoint)
        {
            return true;
        }

        Vector3d tempVec;
        //取上方
        tempVec = checkGrid.Pos + new Vector3d(0, 0, 1);
        //如果上方没有障碍物
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        //取左方
        tempVec = checkGrid.Pos + new Vector3d(-1, 0, 0);
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        //取下方
        tempVec = checkGrid.Pos + new Vector3d(0, 0, -1);
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        //取右方
        tempVec = checkGrid.Pos + new Vector3d(1, 0, 0);
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        return false;
    }

    public AStarGrid AddNewGrid(Vector3d gridPos, AStarGrid lastGrid)
    {
        AStarGrid tempASG;
        if (!AllGrid.ContainsKey(gridPos))
        {
            tempASG = new(gridPos, EndPoint);
            AllGrid.Add(gridPos, tempASG);
            CheckList.Add(tempASG);
        }
        else
        {
            tempASG = AllGrid[gridPos];
        }

        if (tempASG.SetLastGrid(lastGrid))
        {
            SordInsertGridToList(tempASG);
        }
        return AllGrid[gridPos];
    }

    public void SordInsertGridToList(AStarGrid aStarGrid)
    {
        //如果有这个值
        if (CheckList.Contains(aStarGrid))
        {
            //先删除
            for (int i = 0; i < CheckList.Count; i++)
            {
                if (CheckList[i] == aStarGrid)
                {
                    CheckList.RemoveAt(i);
                }
            }
            //然后选择排入顺序
            int index = 0;
            //根据总代价排序
            for (; index < CheckList.Count; index++)
            {
                if (CheckList[index].F > aStarGrid.F)
                {
                    break;
                }
                else if (CheckList[index].F == aStarGrid.F)
                {
                    //根据优先级排序
                    if (CheckList[index].F >= aStarGrid.F)
                    {
                        break;
                    }
                }
            }
            CheckList.Insert(index, aStarGrid);
        }
    }
}
