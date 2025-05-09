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
    /// ���A*·��
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
                    Debug.LogError("Ѱ·ʧ�ܣ�");
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
        //�ȼ�鵱ǰ�б��Ƿ��б����ĸ���
        if (CheckList.Count == 0)
        {
            return true;
        }
        //ȡ����һ�����������
        AStarGrid checkGrid = CheckList[0];
        CheckList.RemoveAt(0);

        //��鵱ǰ��Ҫ����ĵ��Ƿ��Ѿ����յ���
        if (checkGrid.Pos == EndPoint)
        {
            return true;
        }

        Vector3d tempVec;
        //ȡ�Ϸ�
        tempVec = checkGrid.Pos + new Vector3d(0, 0, 1);
        //����Ϸ�û���ϰ���
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        //ȡ��
        tempVec = checkGrid.Pos + new Vector3d(-1, 0, 0);
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        //ȡ�·�
        tempVec = checkGrid.Pos + new Vector3d(0, 0, -1);
        if (!Map.Obstacles2Ds.ContainsKey(tempVec))
        {
            AddNewGrid(tempVec, checkGrid);
        }
        //ȡ�ҷ�
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
        //��������ֵ
        if (CheckList.Contains(aStarGrid))
        {
            //��ɾ��
            for (int i = 0; i < CheckList.Count; i++)
            {
                if (CheckList[i] == aStarGrid)
                {
                    CheckList.RemoveAt(i);
                }
            }
            //Ȼ��ѡ������˳��
            int index = 0;
            //�����ܴ�������
            for (; index < CheckList.Count; index++)
            {
                if (CheckList[index].F > aStarGrid.F)
                {
                    break;
                }
                else if (CheckList[index].F == aStarGrid.F)
                {
                    //�������ȼ�����
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
