using Mathd;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    Vector3d StartPoint;
    Vector3d EndPoint;
    Map Map;
    bool isDown = false;

    List<AStarGrid> CheckList = new();
    Dictionary<Vector3d, AStarGrid> AllGrid = new();
    public List<Vector3d> AStarCalc(Vector3d startPoint, Vector3d endPoint, Map map)
    {
        isDown = false;
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
        for (; ; )
        {
            if (isDown)
            {
                AStarGrid tempGrid = AllGrid[EndPoint];
                for (; ; )
                {
                    result.Add(tempGrid.Pos);
                    Debug.Log(tempGrid.Pos);
                    if (tempGrid.LastGrid == null)
                    {
                        return result;
                    }
                    tempGrid = tempGrid.LastGrid;
                }
            }
            NextStep();
        }
    }

    public void NextStep()
    {
        //�ȼ�鵱ǰ�б��Ƿ��б����ĸ���
        if (CheckList.Count == 0)
        {
            isDown = true;
            Debug.Log("��·���ߣ�");
            return;
        }
        //ȡ����һ�����������
        AStarGrid checkGrid = CheckList[0];
        CheckList.RemoveAt(0);

        //��鵱ǰ��Ҫ����ĵ��Ƿ��Ѿ����յ���
        if (checkGrid.Pos == EndPoint)
        {
            isDown = true;
            Debug.Log("�ҵ��յ��ˣ�");
            return;
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
