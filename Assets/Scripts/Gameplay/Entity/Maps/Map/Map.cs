using Mathd;
using System;
using System.Collections.Generic;
using System.IO;
using ToolSelf;
using UnityEngine;

public class Map
{
    public static double GridSize = 0.32 / Math.Sqrt(2);
    Dictionary<Vector3d, Obstacles2D> obstacles2Ds = new();
    public Dictionary<Vector3d, Obstacles2D> Obstacles2Ds
    {
        get
        {
            return obstacles2Ds;
        }
        set
        {
            obstacles2Ds = new Dictionary<Vector3d, Obstacles2D>(value);
        }
    }

    public void SaveMap()
    {
        using BinaryWriter writer = new(File.Open(Application.persistentDataPath + "/MapEditor", FileMode.Create));
        foreach (var v in obstacles2Ds)
        {
            writer.Write((int)(v.Key.x));
            writer.Write((int)(v.Key.z));
        }
    }

    public void LoadMap()
    {
        string path = Application.persistentDataPath + "/MapEditor";
        if (!File.Exists(path))
        {
            return;
        }
        List<Vector3d> localVertices = new();
        localVertices.Add(new(-Map.GridSize / 2, 0, -Map.GridSize / 2));
        localVertices.Add(new(-Map.GridSize / 2, 0, Map.GridSize / 2));
        localVertices.Add(new(Map.GridSize / 2, 0, Map.GridSize / 2));
        localVertices.Add(new(Map.GridSize / 2, 0, -Map.GridSize / 2));

        FileStream fs = File.Open(path, FileMode.Open);
        using BinaryReader reader = new(fs);
        Vector3d tempPos = Vector3d.zero;
        for (int i = 0; i < fs.Length / 8; i++)
        {
            tempPos.x = reader.ReadInt32();
            tempPos.z = reader.ReadInt32();
            Obstacles2D obs = new(localVertices, ToolM.GetWorldPosByGrid(tempPos), tempPos);
            obstacles2Ds.Add(tempPos,obs);
        }
    }
}

