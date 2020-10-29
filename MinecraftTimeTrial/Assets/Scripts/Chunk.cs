using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    public const int width = 16;
    public const int height = 64;
    public BlockType[,,] blocks = new BlockType[width, height, width];

    private void Start()
    {
        //CreateChunk();
        CreateFace();
    }

    public void CreateFace()
    {
        int vertexIndex = 0;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i < 6; i++)
        {
            int triangleIndex = VoxelData.triangles[0, i];
            vertices.Add(VoxelData.vertices[triangleIndex]);
            triangles.Add(vertexIndex);
            vertexIndex += 1;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    
    public void CreateChunk()
    {
        Mesh mesh = new Mesh();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (blocks[x, y, z] != BlockType.Air)
                    {
                        Vector3 blockCoordinate = new Vector3(x, y, z);


                    }
                }
            }            
        }
    }
}
