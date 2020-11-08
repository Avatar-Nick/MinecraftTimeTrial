using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : MonoBehaviour
{
    [Header("Chunk Data")]
    public bool[,] water = new bool[VoxelData.ChunkWidth, VoxelData.ChunkWidth];

    [Header("Chunk Graphics")]
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    private void Start()
    {
        transform.localPosition = new Vector3(0, Map.instance.biome.waterHeight, 0);
    }

    public void BuildWater(BlockType[,,] blocks)
    {
        ClearMesh();
        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {
                water[x, z] = IsWater(blocks, x, z);
                if (water[x, z])
                {
                    vertices.Add(new Vector3(x, 0, z));
                    vertices.Add(new Vector3(x, 0, z + 1));
                    vertices.Add(new Vector3(x + 1, 0, z + 1));
                    vertices.Add(new Vector3(x + 1, 0, z));

                    vertices.Add(new Vector3(x, 0, z));
                    vertices.Add(new Vector3(x, 0, z + 1));
                    vertices.Add(new Vector3(x + 1, 0, z + 1));
                    vertices.Add(new Vector3(x + 1, 0, z));

                    uvs.AddRange(VoxelData.waterUVs);
                    uvs.AddRange(VoxelData.waterUVs);
                    int verticesCount = vertices.Count - 8;

                    triangles.Add(verticesCount);
                    triangles.Add(verticesCount + 1);
                    triangles.Add(verticesCount + 2);
                    triangles.Add(verticesCount);
                    triangles.Add(verticesCount + 2);
                    triangles.Add(verticesCount + 3);
                    triangles.Add(verticesCount + 7);
                    triangles.Add(verticesCount + 6);
                    triangles.Add(verticesCount + 4);
                    triangles.Add(verticesCount + 6);
                    triangles.Add(verticesCount + 5);
                    triangles.Add(verticesCount + 4);
                }                
            }
        }
        CreateMesh();
    }

    public bool IsWater(BlockType[,,] blocks, int x, int z)
    {
        int y = VoxelData.ChunkHeight - 1;

        //Decrease y from the height of the chunk until you read the ground
        while (y > 0 && blocks[x, y, z] == BlockType.Air)
        {
            y -= 1;
        }

        if (y + 1 < Map.instance.biome.waterHeight)
        {
            return true;
        }
        return false;
    }

    public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshRenderer.receiveShadows = true;
    }

    public void ClearMesh()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }
}
