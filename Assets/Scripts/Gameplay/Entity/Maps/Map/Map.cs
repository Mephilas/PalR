using Mathd;
using System;
using System.Collections.Generic;
using System.IO;
using ToolSelf;
using UnityEngine;

public class Map
{
    public static double GridSize = 0.32 / Math.Sqrt(2);
    List<Obstacles2D> obstacles2Ds;
    public List<Obstacles2D> Obstacles2Ds
    {
        get
        {
            return obstacles2Ds;
        }
        set
        {
            obstacles2Ds = new List<Obstacles2D>(value);
        }
    }

    public Map()
    {
        Obstacles2Ds = new List<Obstacles2D>();
    }
    public Map(Map map)
    {
        Obstacles2Ds = map.Obstacles2Ds;
    }

    public void SaveMap()
    {
        using BinaryWriter writer = new(File.Open(Application.persistentDataPath + "/MapEditor", FileMode.Create));
        for (int i = 0; i < obstacles2Ds.Count; i++)
        {
            writer.Write((int)(obstacles2Ds[i].GirdPosition.x));
            writer.Write((int)(obstacles2Ds[i].GirdPosition.z));
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
            obstacles2Ds.Add(obs);
        }
    }
}

