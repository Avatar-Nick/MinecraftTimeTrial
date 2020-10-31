using UnityEngine;

public static class VoxelData
{
    public static readonly int ChunkWidth = 16;
    public static readonly int ChunkHeight = 64;
    public static readonly int MapSizeInChunks = 100;
    public static readonly int ViewDistanceInChunks = 1;

    public static int MapSizeInVoxels
    {
        get { return MapSizeInChunks * ChunkWidth; }
    }

    public static readonly Vector3[] vertices = new Vector3[8]
    {
        new Vector3(0, 0, 0),
        new Vector3(1, 0, 0),
        new Vector3(1, 1, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1),
        new Vector3(1, 1, 1),
        new Vector3(0, 1, 1),
    };

    public static readonly int[,] triangles = new int[6, 4]
    {
        { 0, 3, 1, 2 }, // Front
        { 5, 6, 4, 7 }, // Back
        { 3, 7, 2, 6 }, // Top 
        { 1, 5, 0, 4 }, // Bottom
        { 4, 7, 0, 3 }, // Left
        { 1, 2, 5, 6 }, // Right
    };

    public static readonly Vector2[] uvs = new Vector2[4]
    {
        new Vector2(0, 0),
        new Vector2(0, 1),
        new Vector2(1, 0),
        new Vector2(1, 1)
    };

    public static readonly Vector3[] faceChecks = new Vector3[6]
    {
        new Vector3( 0,  0, -1),
        new Vector3( 0,  0,  1),
        new Vector3( 0,  1,  0),
        new Vector3( 0, -1,  0),
        new Vector3(-1,  0,  0),
        new Vector3( 1,  0,  0),
    };
}
