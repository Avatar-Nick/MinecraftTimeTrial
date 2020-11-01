using UnityEngine;

public static class Noise
{
    public static float Get2DPerlin(Vector2 coordinate, float offset, float scale)
    {
        return Mathf.PerlinNoise((coordinate.x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (coordinate.y + 0.1f) / VoxelData.ChunkWidth * scale + offset);
    }

    public static bool Get3DPerlin(Vector3 coordinate, float offset, float scale, float threshold)
    {
        float x = (coordinate.x + offset + 0.1f) * scale;
        float y = (coordinate.y + offset + 0.1f) * scale;
        float z = (coordinate.z + offset + 0.1f) * scale;

        float XY = Mathf.PerlinNoise(x, y);
        float YZ = Mathf.PerlinNoise(y, z);
        float ZX = Mathf.PerlinNoise(z, x);
        float YX = Mathf.PerlinNoise(y, x);
        float ZY = Mathf.PerlinNoise(z, y);
        float XZ = Mathf.PerlinNoise(x, z);

        if ((XY + YZ + ZX + YX + ZY + XZ) / 6f > threshold)
        {
            return true;
        }
        return false;
    }
}
