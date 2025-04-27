using Mathd;
using System;
using ToolSelf;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class AStarGrid
{
    public Vector3d Pos = Vector3d.zero;//格子位置
    public AStarGrid LastGrid;//来自于哪个格子
    private double h = 99999;//实际代价
    private double g = 0;//预估代价
    private double f = 99999;//总代价
    private double p = 0;//优先度

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
        //这里决定预估代价的计算方式
        G = ToolM.GetTileDistance(endPos, pos);
        //这里决定优先级计算方式
        //P = Math.Abs(Math.Abs(endPos.x - Pos.x) - Math.Abs(endPos.z - Pos.z));
        P = 0;
    }
}
