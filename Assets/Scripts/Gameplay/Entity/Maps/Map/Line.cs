using Mathd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Line
{
    public Vector3d m_PointA;
    public Vector3d m_PointB;
    public Vector3d Dir { get; private set; }

    public Line(Vector3d pointA, Vector3d pointB)
    {
        m_PointA = pointA;
        m_PointB = pointB;
        Init();
    }

    public void Init()
    {
        Dir = m_PointB - m_PointA;
    }

    public Line Commutation()
    {
        return new Line(m_PointB, m_PointA);
    }
}

