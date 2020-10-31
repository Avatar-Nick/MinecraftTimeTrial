using UnityEngine;

public static class Noise
{
    public static float Get2DPerlin(int x, int y, float offset, float scale)
    {
        return Mathf.PerlinNoise((x + 0.1f) / VoxelData.ChunkWidth * scale + offset, (y + 0.1f) / VoxelData.ChunkWidth * scale + offset);
    }
}
