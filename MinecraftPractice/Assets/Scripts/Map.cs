using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    [Header("Chunk Data")]
    public GameObject chunkPrefab;
    public Dictionary<ChunkCoordinate, Chunk> chunks = new Dictionary<ChunkCoordinate, Chunk>();
    public Dictionary<ChunkCoordinate, Chunk> outOfViewChunks = new Dictionary<ChunkCoordinate, Chunk>();

    [Header("Player Data")]
    //public PlayerController player;
    public GameObject player;
    public Vector3 spawnPosition;
    public ChunkCoordinate currentCoordinate;

    [Header("Random Data")]
    public int seed = 1337;

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
        Random.InitState(seed);
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
                BuildChunk(new ChunkCoordinate(x, z));
            }
        }

        player.transform.position = spawnPosition;
        currentCoordinate = GetChunkCoordinate(spawnPosition);
    }

    public void BuildChunk(ChunkCoordinate coordinate)
    {
        GameObject chunkGO = Instantiate(chunkPrefab, new Vector3(coordinate.x, 0, coordinate.z), Quaternion.identity);
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.Initialize(coordinate);
        chunks.Add(coordinate, chunk);
    }

    public BlockType GetVoxel(int x, int y, int z)
    {
        // If outside the world, return air
        if (!IsVoxelInWorld(x, y, z))
        {
            return BlockType.Air;
        }

        // If bottom of chunk, return bedrock
        if (y == 0)
        {
            return BlockType.Bedrock;
        }

        // Basic Terrain Pass
        int terrainHeight = (int)(VoxelData.ChunkHeight * Noise.Get2DPerlin(x, z, 500, 0.25f));
        if (y <= terrainHeight)
        {
            return BlockType.Stone;
        }
        else
        {
            return BlockType.Air;
        }
    }

    public void UpdateMap()
    {
        ChunkCoordinate coordinate = GetChunkCoordinate(player.transform.position);
        if (coordinate.Equals(currentCoordinate)) return;

        int xStart = coordinate.x - VoxelData.ViewDistanceInChunks;
        int xEnd = coordinate.x + VoxelData.ViewDistanceInChunks;
        int zStart = coordinate.z - VoxelData.ViewDistanceInChunks;
        int zEnd = coordinate.z + VoxelData.ViewDistanceInChunks;
        for (int x = xStart ; x < xEnd ; x++)
        {
            for (int z = zStart; z < zEnd; z++)
            {
                ChunkCoordinate viewCoordinate = new ChunkCoordinate(x, z);
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
        List<ChunkCoordinate> removeCoordinates = new List<ChunkCoordinate>();
        foreach (KeyValuePair<ChunkCoordinate, Chunk> chunkPair in chunks)
        {
            ChunkCoordinate removeCoordinate = chunkPair.Key;
            if (Mathf.Abs(coordinate.x - removeCoordinate.x) > VoxelData.ViewDistanceInChunks ||
                Mathf.Abs(coordinate.z - removeCoordinate.z) > VoxelData.ViewDistanceInChunks)
            {
                Chunk removeChunk = chunkPair.Value;
                removeChunk.Toggle(false);
                outOfViewChunks[removeCoordinate] = removeChunk;
            }
        }

        foreach (ChunkCoordinate removeCoordinate in removeCoordinates)
        {
            chunks.Remove(removeCoordinate);
        }
        currentCoordinate = coordinate;
    }

    public ChunkCoordinate GetChunkCoordinate(Vector3 position)
    {
        int x = ((int)position.x + (VoxelData.ChunkWidth / 2)) / VoxelData.ChunkWidth;
        int z = ((int)position.z + (VoxelData.ChunkWidth / 2)) / VoxelData.ChunkWidth;
        return new ChunkCoordinate(x, z);
    }


    //-----------------------------------------------------------------------------------//
    //Check Functions
    //-----------------------------------------------------------------------------------//
    public bool IsChunkInWorld(ChunkCoordinate coordinate)
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
    //-----------------------------------------------------------------------------------//
}
