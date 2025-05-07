using Mathd;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class AStarOptimize
{
    Map Map = new();
    List<OptimizePoint> result = new();

    public List<OptimizePoint> Init(List<Vector3d> aStarList, Map map)
    {
        Map = map;
        result.Clear();
        for (int i = 0; i < aStarList.Count; i++)
        {
            result.Add(new(aStarList[i], new(0, 0, 0), false));
        }
        NextStep();
        return result;
    }

    public void NextStep()
    {
        for (; ; )
        {
            bool isDown = true;
            int index = 0;
            for (; ; )
            {
                //��������������һ���㣬������ѭ��
                if (index == result.Count - 1)
                {
                    break;
                }
                for (int j = result.Count - 1; j > index; j--)
                {
                    //�����ǰ�����߶ε��ҵ��Ŀ�����ǵ�ǰ������
                    if (result[index].HasTarget && result[index].TargetPos == result[j].Pos)
                    {
                        //�����ȫһ�£����ж���һ����
                        index++;
                        break;
                    }
                    List<OptimizePoint> tempList = new();
                    //�������֮������ϰ����������һ��������
                    if (!CheckObsBy2Point(result[index].Pos, result[j].Pos, ref tempList))
                    {
                        //���ϰ����һ��������
                        continue;
                    }
                    else
                    {
                        //���û���ϰ����ʱ���������ո�µ����߶�
                        //����ˢ���µ��߶ε���·����ȥ
                        for (int i = 0; i < tempList.Count; i++)
                        {
                            result[index + i] = tempList[i];
                        }
                        index++;
                        isDown = false;
                        break;
                    }
                }
            }
            if (isDown)
            {
                break;
            }
        }
    }

    public bool CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint, ref List<OptimizePoint> gridList)
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
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
