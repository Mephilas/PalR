using Mathd;
using System;
using ToolSelf;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AStarGrid
{
    public Vector3d Pos = Vector3d.zero;//����λ��
    public AStarGrid LastGrid;//�������ĸ�����
    private double h = 99999;//ʵ�ʴ���
    private double g = 0;//Ԥ������
    private double f = 99999;//�ܴ���
    private double p = 0;//���ȶ�

    public double H
    {
        get => h;
        set
        {
            h = value;
            F = H + G;
        }
    }
    public double G
    {
        get => g;
        set
        {
            g = value;
            F = H + G;
        }
    }
    public double F { get => f; set => f = value; }
    public double P { get => p; set => p = value; }

    public bool SetLastGrid(AStarGrid lastGrid)
    {
        double tempH = ToolM.GetTileDistance(lastGrid.Pos, Pos) + lastGrid.H;
        if (tempH < H)
        {
            H = tempH;
            LastGrid = lastGrid;
            return true;
        }
        return false;
    }
    public AStarGrid(Vector3d pos, Vector3d endPos)
    {
        Pos = pos;
        //�������Ԥ�����۵ļ��㷽ʽ
        G = ToolM.GetTileDistance(endPos, pos);
        //����������ȼ����㷽ʽ
        //P = Math.Abs(Math.Abs(endPos.x - Pos.x) - Math.Abs(endPos.z - Pos.z));
        P = 0;
    }
}
