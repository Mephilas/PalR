using Mathd;
using System;
using UnityEngine;

public class AStarGrid
{
    public Vector3d GridPoint = Vector3d.zero;
    private double h = 0;//ʵ�ʴ���
    private double g = 0;//Ԥ������
    private double f = 0;//�ܴ���

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

    public AStarGrid(double h, Vector3d gridPoint, Vector3d endPoint)
    {
        GridPoint = gridPoint;
        H = h;
        G = Math.Abs(endPoint.x - GridPoint.x) + Math.Abs(endPoint.z - GridPoint.z);
    }
}
