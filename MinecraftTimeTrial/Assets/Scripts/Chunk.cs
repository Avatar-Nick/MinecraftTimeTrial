using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [Header("Chunk Data")]
    public Coordinate coordinate;

    [Header("Chunk Graphics")]
    public MeshRenderer meshRenderer;
    public MeshFilter meshFilter;

    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();

    public BlockType[,,] blocks = new BlockType[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];    

    public void Initialize(Coordinate coordinate)
    {
        this.coordinate = coordinate;
        name = string.Format("Chunk {0}", coordinate);
        transform.SetParent(Map.instance.transform);
        transform.position = new Vector3(coordinate.x * VoxelData.ChunkWidth, 0f, coordinate.z * VoxelData.ChunkWidth);

        PopulateVoxelMap();
        BuildChunk();
        CreateMesh();
    }

    public void PopulateVoxelMap()
    {
        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {
                    blocks[x, y, z] = Map.instance.GetVoxel(x + (int)transform.position.x, y + (int)transform.position.y, z + (int)transform.position.z);
                }
            }
        }
    }

    public void BuildChunk()
    {
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
    }

    public void BuildBlock(Vector3 coordinate)
    {
        BlockType blockType = blocks[(int)coordinate.x, (int)coordinate.y, (int)coordinate.z];
        if (blockType == BlockType.Air) return;

        for (int face = 0; face < 6; face++)
        {
            // Only draw face if there are no voxels adjacent to this face
            if (!CheckVoxel(coordinate + VoxelData.faceChecks[face]))
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

    public bool CheckVoxel(Vector3 coordinate)
    {
        int x = (int)coordinate.x;
        int y = (int)coordinate.y;
        int z = (int)coordinate.z;

        if(!IsVoxelInChunk(x, y, z))
        {
            BlockType mapVoxel = Map.instance.GetVoxel(x + (int)transform.position.x, y + (int)transform.position.y, z + (int)transform.position.z);
            return mapVoxel != BlockType.Air;
        }

        return blocks[(int)coordinate.x, (int)coordinate.y, (int)coordinate.z] != BlockType.Air;
    }

    public void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    public void AddTexture(BlockType blockType, int faceIndex)
    {        
        Block block = TextureData.blocks[blockType];
        Face face = block.GetFace(faceIndex);
        uvs.AddRange(face.uvs);
    }

    public void Toggle(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
        {
            return false;
        }
        return true;
    }
}
