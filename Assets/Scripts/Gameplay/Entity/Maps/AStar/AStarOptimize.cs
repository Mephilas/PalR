using Mathd;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AStarOptimize
{
    Vector3d StartPoint;
    Vector3d EndPoint;
    Map Map;

    List<AStarGrid> CheckList = new();
    Dictionary<Vector3d, AStarGrid> AllGrid = new();

    /// <summary>
    /// ����A*����·�߹յ�
    /// </summary>
    /// <param name="aStarList"></param>
    /// <returns></returns>
    public List<Vector3d> InflectionPointCalcByAStar(List<AStarGrid> aStarGridList)
    {

        int EndIndex = aStarGridList.Count - 1;
        List<Vector3d> result = new();
        result.Add(aStarGridList[EndIndex].Pos);
        for (; EndIndex > 0;)
        {
            for (int i = 0; i < EndIndex; i++)
            {
                if (!CheckObsBy2Point(aStarGridList[i].Pos, aStarGridList[EndIndex].Pos))
                {
                    EndIndex = i;
                    result.Add(aStarGridList[EndIndex].Pos);
                    break;
                }
            }
        }
        return result;
    }


    public bool CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint)
    {
        List<Vector3d> result = new();
        StartPoint = startPoint;
        EndPoint = endPoint;
        CheckList.Clear();
        AllGrid.Clear();

        //����ֱ�߼��
        AStarGrid startGrid = new(startPoint, endPoint);
        startGrid.H = 0;
        CheckList.Add(startGrid);
        AllGrid.Add(StartPoint, startGrid);
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


        Vector3d tempPos = startGrid.Pos;
        for (; ; )
        {
            Vector3d tempCheck1 = tempPos + new Vector3d(tempX, 0, 0);
            Vector3d tempCheck2 = tempPos + new Vector3d(0, 0, tempZ);
            if (tempPos == endPoint)
            {

            }
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
