using Mathd;
using System.Collections.Generic;

public class Obstacles2D
{
    List<Vector3d> worldVertices;
    public List<Vector3d> WorldVertices
    {
        get
        {
            return worldVertices;
        }
        set
        {
            worldVertices = new List<Vector3d>(value);
        }
    }

    List<Vector3d> localVertices;
    public List<Vector3d> LocalVertices
    {
        get
        {
            return localVertices;
        }
        set
        {
            localVertices = new List<Vector3d>(value);
            SetWorldVertices();
        }
    }

    Vector3d position;
    public Vector3d Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
            SetWorldVertices();
        }
    }

    public Obstacles2D()
    {
        LocalVertices = new List<Vector3d>();
        Position = new Vector3d();
    }

    public Obstacles2D(Obstacles2D obstacles2D)
    {
        LocalVertices = new List<Vector3d>(obstacles2D.localVertices);
        Position = obstacles2D.Position;
    }

    public Obstacles2D(List<Vector3d> localVertices, Vector3d position)
    {
        if (localVertices == null)
        {
            LocalVertices = new List<Vector3d>();
        }
        else
        {
            LocalVertices = new List<Vector3d>(localVertices);
        }
        Position = position;
    }

    void SetWorldVertices()
    {
        List<Vector3d> res = new List<Vector3d>();
        for (int i = 0; i < LocalVertices.Count; i++)
        {
            res.Add(LocalVertices[i] + Position);
        }
        WorldVertices = res;
    }
}

