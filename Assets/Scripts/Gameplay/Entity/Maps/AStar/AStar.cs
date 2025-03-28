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

    List<AStarGrid> listReady = new();
    public void AStarCalc(Vector3d startPoint, Vector3d endPoint, Map map)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
        Map = map;
        listReady.Add(new(0, StartPoint, EndPoint));
        for (; ; )
        {
            if (isDown)
            {
                break;
            }
            NextStep();
        }
    }

    public void NextStep()
    {
        
    }

    public void AddNewSeekGrid(AStarGrid aStarGrid)
    {
        //if ()
        //{

        //}
    }
}
