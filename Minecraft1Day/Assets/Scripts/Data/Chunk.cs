using System;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Data")]
    public BlockType[,,] blocks = new BlockType[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];
    public ChunkCoordinate chunkCoordinate;
    public bool initialized = false;

    [Header("Chunk Graphics")]
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    
    [Header("Modifications")]
    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    [Header("Water")]
    public WaterChunk waterChunk;

    //-----------------------------------------------------------------------------------//
    //Chunk Initialization
    //-----------------------------------------------------------------------------------//
    public void Initialize(ChunkCoordinate coordinate)
    {
        this.chunkCoordinate = coordinate;
        name = string.Format("Chunk {0}", coordinate);
        transform.SetParent(Map.instance.transform);
        transform.position = new Vector3(coordinate.x * VoxelData.ChunkWidth, 0f, coordinate.z * VoxelData.ChunkWidth);

        InitializeChunkData();
        UpdateChunk();

        waterChunk.BuildWater(blocks);
    }

    public void InitializeChunkData()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    blocks[x, y, z] = Map.instance.GetNewVoxel(x + (int)transform.position.x, y + (int)transform.position.y, z + (int)transform.position.z);
                }
            }
        }
        initialized = true;
    }

    public void UpdateChunk()
    {
        while (modifications.Count > 0)
        {
            VoxelMod modification = modifications.Dequeue();
            Vector3 coordinate = modification.coordinate -= transform.position;
            blocks[(int)coordinate.x, (int)coordinate.y, (int)coordinate.z] = modification.blockType;
        }

        ClearMesh();
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    BuildBlock(new Vector3(x, y, z));                   
                }
            }
        }
        CreateMesh();
    }

    public void BuildBlock(Vector3 coordinate)
    {
        BlockType blockType = blocks[(int)coordinate.x, (int)coordinate.y, (int)coordinate.z];
        if (blockType == BlockType.Air) return;

        for (int face = 0; face < 6; face++)
        {
            // Only draw face if there are no voxels adjacent to this face
            if (IsAir(coordinate + VoxelData.faceChecks[face]))
            {
                vertices.Add(coordinate + VoxelData.vertices[VoxelData.triangles[face, 0]]);
                vertices.Add(coordinate + VoxelData.vertices[VoxelData.triangles[face, 1]]);
                vertices.Add(coordinate + VoxelData.vertices[VoxelData.triangles[face, 2]]);
                vertices.Add(coordinate + VoxelData.vertices[VoxelData.triangles[face, 3]]);

                AddTexture(blockType, face);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }            
        }        
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
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Update Functions
    //-----------------------------------------------------------------------------------//
    public void UpdateVoxel(Vector3 position, BlockType blockType)
    {
        int xCheck = (int)position.x - (int)transform.position.x;
        int yCheck = (int)position.y;
        int zCheck = (int)position.z - (int)transform.position.z;

        blocks[xCheck, yCheck, zCheck] = blockType;
        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);
        UpdateChunk();
    }

    public void UpdateSurroundingVoxels(int x, int y, int z)
    {
        Vector3 startPosition = new Vector3(x, y, z);
        for (int face = 0; face < 6; face++)
        {
            Vector3 newPosition = startPosition + VoxelData.faceChecks[face];
            if (!IsVoxelInChunk((int)newPosition.x, (int)newPosition.y, (int)newPosition.z))
            {
                Map.instance.GetChunk(newPosition + transform.position).UpdateChunk();
            }
        }
    }
    //-----------------------------------------------------------------------------------//


    //-----------------------------------------------------------------------------------//
    //Check Functions
    //-----------------------------------------------------------------------------------//
    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        return true;
    }

    public BlockType GetVoxel(Vector3 position)
    {
        int xCheck = (int)position.x;
        int yCheck = (int)position.y;
        int zCheck = (int)position.z;

        xCheck -= (int)transform.position.x;
        zCheck -= (int)transform.position.z;
        return blocks[xCheck, yCheck, zCheck];
    }

    public bool IsAir(Vector3 coordinate)
    {
        int x = (int)coordinate.x;
        int y = (int)coordinate.y;
        int z = (int)coordinate.z;
        if (!IsVoxelInChunk(x, y, z))
        {
            return !Map.instance.IsSolid(x + (int)transform.position.x, y + (int)transform.position.y, z + (int)transform.position.z);
        }

        return blocks[(int)coordinate.x, (int)coordinate.y, (int)coordinate.z] == BlockType.Air;
    }
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Graphics Functions
    //-----------------------------------------------------------------------------------//
    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    public void AddTexture(BlockType blockType, int faceIndex)
    {
        Block block = TextureData.blocks[blockType];
        Face face = block.GetFace(faceIndex);
        uvs.AddRange(face.uvs);
    }    
    //-----------------------------------------------------------------------------------//
}
