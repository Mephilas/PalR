using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;

public class Map
{
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
}

