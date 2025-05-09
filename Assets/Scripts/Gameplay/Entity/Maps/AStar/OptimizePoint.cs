using Mathd;

public class OptimizePoint
{
    public int Index = -1;
    public Vector3d Pos;
    public int TargetIndex = -1;
    public Vector3d InflectionPos1 = new(0, 1, 0);
    public Vector3d InflectionPos2 = new(0, 1, 0);

    public OptimizePoint(int index, Vector3d pos)
    {
        Index = index;
        Pos = pos;
    }
}