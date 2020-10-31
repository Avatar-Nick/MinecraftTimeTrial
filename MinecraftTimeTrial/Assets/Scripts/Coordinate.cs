using System;

[Serializable]
public struct Coordinate
{
    public int x;
    public int z;

    public Coordinate(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
    public override string ToString()
    {
        return String.Format("({0}, {1})", x, z);
    }
}