using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    [Header("Chunk Data")]
    public GameObject chunkPrefab;
    public Dictionary<Coordinate, Chunk> chunks = new Dictionary<Coordinate, Chunk>();
    public Dictionary<Coordinate, Chunk> outOfViewChunks = new Dictionary<Coordinate, Chunk>();

    [Header("Player Data")]
    //public PlayerController player;
    public GameObject player;
    public Vector3 spawnPosition;
    public Coordinate currentCoordinate;

    private void Awake()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    private void Start()
    {
        spawnPosition = new Vector3(VoxelData.MapSizeInChunks * VoxelData.ChunkWidth / 2, VoxelData.ChunkHeight + 2, VoxelData.MapSizeInChunks * VoxelData.ChunkWidth / 2);
        GenerateMap();        
    }

    private void Update()
    {
        UpdateMap();
    }

    public void GenerateMap()
    {
        int start = (VoxelData.MapSizeInChunks / 2) - VoxelData.ViewDistanceInChunks;
        int end = (VoxelData.MapSizeInChunks / 2) + VoxelData.ViewDistanceInChunks;
        for (int x = start; x < end; x++)
        {
            for (int z = start; z < end; z++)
            {
                BuildChunk(new Coordinate(x, z));
            }
        }

        player.transform.position = spawnPosition;
        currentCoordinate = GetChunkCoordinate(spawnPosition);

    }

    public void BuildChunk(Coordinate coordinate)
    {
        GameObject chunkGO = Instantiate(chunkPrefab, new Vector3(coordinate.x, 0, coordinate.z), Quaternion.identity);
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.Initialize(coordinate);
        chunks.Add(coordinate, chunk);
    }

    public BlockType GetVoxel(int x, int y, int z)
    {
        if (!IsVoxelInWorld(x, y, z))
        {
            return BlockType.Air;
        }

        if (y == VoxelData.ChunkHeight - 1)
        {
            return BlockType.Grass;
        }
        else if (y == VoxelData.ChunkHeight - 2)
        {
            return BlockType.Dirt;
        }
        else if (y > 0 && y < VoxelData.ChunkHeight - 2)
        {
            return BlockType.Stone;
        }
        else if (y == 0)
        {
            return BlockType.Bedrock;
        }
        else
        {
            Debug.LogError("Map.cs - GetVoxel(): Out Of Range");
            return BlockType.Bedrock;
        } 
    }

    public void UpdateMap()
    {
        Coordinate coordinate = GetChunkCoordinate(player.transform.position);
        if (coordinate.Equals(currentCoordinate)) return;

        int xStart = coordinate.x - VoxelData.ViewDistanceInChunks;
        int xEnd = coordinate.x + VoxelData.ViewDistanceInChunks;
        int zStart = coordinate.z - VoxelData.ViewDistanceInChunks;
        int zEnd = coordinate.z + VoxelData.ViewDistanceInChunks;
        for (int x = xStart ; x < xEnd ; x++)
        {
            for (int z = zStart; z < zEnd; z++)
            {
                Coordinate viewCoordinate = new Coordinate(x, z);
                if (IsChunkInWorld(viewCoordinate))
                {
                    chunks.TryGetValue(viewCoordinate, out Chunk chunk);
                    outOfViewChunks.TryGetValue(viewCoordinate, out Chunk outOfViewChunk);
                    if (chunk == null && outOfViewChunk == null) 
                    {
                        BuildChunk(viewCoordinate);
                    }
                    if (outOfViewChunk != null)
                    {
                        outOfViewChunk.Toggle(true);
                        chunks[viewCoordinate] = outOfViewChunk;
                        outOfViewChunks.Remove(viewCoordinate);
                    }
                }
            }
        }

        // Remove Chunks that are far away
        List<Coordinate> removeCoordinates = new List<Coordinate>();
        foreach (KeyValuePair<Coordinate, Chunk> chunkPair in chunks)
        {
            Coordinate removeCoordinate = chunkPair.Key;
            if (Mathf.Abs(coordinate.x - removeCoordinate.x) > VoxelData.ViewDistanceInChunks ||
                Mathf.Abs(coordinate.z - removeCoordinate.z) > VoxelData.ViewDistanceInChunks)
            {
                Chunk removeChunk = chunkPair.Value;
                removeChunk.Toggle(false);
                outOfViewChunks[removeCoordinate] = removeChunk;
            }
        }

        foreach (Coordinate removeCoordinate in removeCoordinates)
        {
            chunks.Remove(removeCoordinate);
        }
        currentCoordinate = coordinate;
    }

    public Coordinate GetChunkCoordinate(Vector3 position)
    {
        int x = ((int)position.x + (VoxelData.ChunkWidth / 2)) / VoxelData.ChunkWidth;
        int z = ((int)position.z + (VoxelData.ChunkWidth / 2)) / VoxelData.ChunkWidth;
        return new Coordinate(x, z);
    }

    public bool IsChunkInWorld(Coordinate coordinate)
    {
        if (coordinate.x > 0 && coordinate.x < VoxelData.MapSizeInChunks &&
            coordinate.z > 0 && coordinate.z < VoxelData.MapSizeInChunks)
        {
            return true;
        }
        return false;
    }

    public bool IsVoxelInWorld(int x, int y, int z)
    {
        if (x >= 0 && x < VoxelData.MapSizeInVoxels &&
            y >= 0 && y < VoxelData.ChunkHeight &&
            z >= 0 && z < VoxelData.MapSizeInVoxels)
        {
            return true;
        }
        return false;
    }
}
