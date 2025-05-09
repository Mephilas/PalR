using Mathd;
using System.Collections.Generic;

public class AStarOptimize
{
    Map Map = new();
    List<OptimizePoint> result = new();
    int count = 0;
    List<Vector3d> obsPos = new();
    List<Vector3d> lastObsPos = new();
    List<Vector3d> listGrid = new();
    List<Vector3d> listResult = new();

    public List<Vector3d> Init(List<Vector3d> aStarList, Map map)
    {
        Map = map;
        result.Clear();
        listResult.Clear();
        for (int i = 0; i < aStarList.Count; i++)
        {
            result.Add(new(i, aStarList[i]));
        }
        count = aStarList.Count;
        for (int i = 0; i < 4; i++)
        {
            NextStep();
        }
        for (int i = 0; i < count; i++)
        {
            listResult.Add(result[i].Pos);
        }
        return listResult;
    }

    /// <summary>
    /// ��һ�ε������Ż�·����λ�á�
    /// </summary>
    public void NextStep()
    {
        for (int index = 0; index < count - 1;)
        {
            obsPos.Clear();
            for (int i = count - 1; i > index; i--)
            {
                lastObsPos.Clear();
                //�����Ŀ������յ㣬���ü����˷��ؾ����ˡ�
                if (i <= result[index].TargetIndex)
                {
                    index++;
                    break;
                }
                if (!CheckObsBy2Point(result[index].Pos, result[i].Pos, ref listGrid, ref lastObsPos))
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
                        tempInf1 = CalcInflectionPoint(result[index].Pos, result[i + 1].Pos, result[i].Pos, obsPos[0]);
                        tempInf2 = CalcInflectionPoint(result[index].Pos, result[i + 1].Pos, result[i].Pos, obsPos[^1]);
                    }

                    //�жϵ��Ƿ���Ŀ���
                    if (result[index].TargetIndex != -1)
                    {
                        //����߱�Ŀ��㣬˵�������Ѿ���ʼ��
                        //2.�ж����߶��Ƿ�������߶���ͬһ��
                        if (result[index].InflectionPos1 == tempInf1 || result[index].InflectionPos2 == tempInf1 || result[index].InflectionPos1 == tempInf2 || result[index].InflectionPos2 == tempInf2)
                        {
                            index++;
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
                        for (int j = 0; j < listGrid.Count; j++)
                        {
                            result[index + j].Pos = listGrid[j];
                            result[index + j].TargetIndex = i;
                        }
                    }
                    else
                    {
                        int InfIndex1 = -1;
                        int InfIndex2 = -1;
                        //2.�������߶ιյ�֮ǰ�Ĳ���
                        //2.1�ҵ��յ������߶��е�����
                        for (int j = 0; j < listGrid.Count; j++)
                        {
                            if (listGrid[j] == tempInf1)
                            {
                                InfIndex1 = j;
                            }
                            if (listGrid[j] == tempInf2)
                            {
                                InfIndex2 = j;
                            }
                            if (InfIndex1 != -1 && InfIndex2 != -1)
                            {
                                break;
                            }
                        }

                        //2.2�����߶�ǰ���ε�����
                        for (int j = 0; j < listGrid.Count; j++)
                        {
                            result[index + j].Pos = listGrid[j];
                            if (j < InfIndex1 || j < InfIndex2)
                            {
                                //����ǰ��ε�����
                                result[index + j].InflectionPos1 = listGrid[InfIndex1];
                                result[index + j].InflectionPos2 = listGrid[InfIndex2];
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
        }
    }

    /// <summary>
    /// �������֮���Ƿ���ͨ
    /// </summary>
    /// <param name="startPoint">���</param>
    /// <param name="endPoint">�յ�</param>
    /// <param name="gridList">·��</param>
    /// <param name="obsPos">�ϰ����б�</param>
    /// <returns></returns>
    public bool CheckObsBy2Point(Vector3d startPoint, Vector3d endPoint, ref List<Vector3d> gridList, ref List<Vector3d> obsPos)
    {
        gridList.Clear();
        obsPos.Clear();
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

        Vector3d checkPoint = startPoint;
        gridList.Add(startPoint);
        for (; ; )
        {
            Vector3d checkPos1 = checkPoint + new Vector3d(tempX, 0, 0);
            Vector3d checkPos2 = checkPoint + new Vector3d(0, 0, tempZ);

            if (checkPos1 == endPoint || checkPos2 == endPoint)
            {
                gridList.Add(endPoint);
                break;
            }

            double dir1 = Vector3d.Dot((endPoint - checkPos1).normalized, targetDir);
            double dir2 = Vector3d.Dot((endPoint - checkPos2).normalized, targetDir);

            if (dir1 > dir2)
            {
                //dir1������ԭб��
                if (Map.Obstacles2Ds.ContainsKey(checkPos1))
                {
                    obsPos.Add(checkPos1);
                }
                checkPoint = checkPos1;
                gridList.Add(checkPos1);
            }
            else if (dir1 < dir2)
            {
                //dir2������ԭб��
                if (Map.Obstacles2Ds.ContainsKey(checkPos2))
                {
                    obsPos.Add(checkPos2);
                }
                checkPoint = checkPos2;
                gridList.Add(checkPos2);
            }
            else
            {
                //��������ĸ���б����ͬ
                //�Ƚ����
                double dis1 = (checkPos1 - endPoint).magnitude;
                double dis2 = (checkPos2 - endPoint).magnitude;
                if (dis1 < dis2)
                {
                    if (Map.Obstacles2Ds.ContainsKey(checkPos1))
                    {
                        obsPos.Add(checkPos1);
                    }
                    checkPoint = checkPos1;
                    gridList.Add(checkPos1);
                }
                else if (dis2 < dis1)
                {
                    if (Map.Obstacles2Ds.ContainsKey(checkPos2))
                    {
                        obsPos.Add(checkPos2);
                    }
                    checkPoint = checkPos2;
                    gridList.Add(checkPos2);
                }
                else
                {
                    //������ͬ
                    //�Ⱥ����
                    if (!Map.Obstacles2Ds.ContainsKey(checkPos1))
                    {
                        checkPoint = checkPos1;
                        gridList.Add(checkPos1);
                    }
                    else if (!Map.Obstacles2Ds.ContainsKey(checkPos2))
                    {
                        checkPoint = checkPos2;
                        gridList.Add(checkPos2);
                    }
                    else
                    {
                        checkPoint = checkPos1;
                        obsPos.Add(checkPos1);
                        gridList.Add(checkPos1);
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
    /// <param name="checkPos">������</param>
    /// <param name="lastPos">���һ�α��赲�ļ���</param>
    /// <param name="truePos">�״���ͨ����</param>
    /// <param name="obsPos">�ϰ�������</param>
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