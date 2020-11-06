using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map instance;

    [Header("World Data")]
    public int seed = 1337;
    public Biome biome;

    [Header("Chunk Data")]
    public GameObject chunkPrefab;
    public Dictionary<ChunkCoordinate, Chunk> chunks = new Dictionary<ChunkCoordinate, Chunk>();
    public Dictionary<ChunkCoordinate, Chunk> outOfViewChunks = new Dictionary<ChunkCoordinate, Chunk>();
    public List<ChunkCoordinate> generateChunks = new List<ChunkCoordinate>();

    [Header("Player Data")]
    public GameObject player;
    public Vector3 spawnPosition;
    public ChunkCoordinate currentCoordinate;

    //-----------------------------------------------------------------------------------//
    //Chunk Initialization and Update
    //-----------------------------------------------------------------------------------//
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
        spawnPosition = new Vector3(VoxelData.MapSizeInChunks * VoxelData.ChunkWidth / 2, VoxelData.ChunkHeight - 50, VoxelData.MapSizeInChunks * VoxelData.ChunkWidth / 2);
        GenerateMap();
    }

    private void Update()
    {
        UpdateMap();
    }
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Map Functions
    //-----------------------------------------------------------------------------------//
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

    public void UpdateMap(bool instant = false)
    {
        ChunkCoordinate coordinate = GetChunkMidCoordinate(player.transform.position);
        if (coordinate.Equals(currentCoordinate)) return;

        int xStart = coordinate.x - VoxelData.ViewDistanceInChunks;
        int xEnd = coordinate.x + VoxelData.ViewDistanceInChunks;
        int zStart = coordinate.z - VoxelData.ViewDistanceInChunks;
        int zEnd = coordinate.z + VoxelData.ViewDistanceInChunks;
        for (int x = xStart; x < xEnd; x++)
        {
            for (int z = zStart; z < zEnd; z++)
            {
                ChunkCoordinate viewCoordinate = new ChunkCoordinate(x, z);
                if (IsChunkInWorld(viewCoordinate))
                {
                    chunks.TryGetValue(viewCoordinate, out Chunk chunk);
                    outOfViewChunks.TryGetValue(viewCoordinate, out Chunk outOfViewChunk);
                    if (chunk == null && outOfViewChunk == null && !generateChunks.Contains(viewCoordinate))
                    {
                        if (instant)
                        {
                            BuildChunk(viewCoordinate);
                        }
                        else
                        {
                            generateChunks.Add(viewCoordinate);
                        }
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

        StartCoroutine(EfficientBuildChunks());
    }

    public void BuildChunk(ChunkCoordinate coordinate)
    {
        GameObject chunkGO = Instantiate(chunkPrefab, new Vector3(coordinate.x, 0, coordinate.z), Quaternion.identity);
        Chunk chunk = chunkGO.GetComponent<Chunk>();
        chunk.Initialize(coordinate);
        chunks.Add(coordinate, chunk);
    }
    private IEnumerator EfficientBuildChunks()
    {
        while (generateChunks.Count > 0)
        {
            BuildChunk(generateChunks[0]);
            generateChunks.RemoveAt(0);

            yield return new WaitForSeconds(VoxelData.ChunkBuildDelay);

        }
    }
    //-----------------------------------------------------------------------------------//

    //-----------------------------------------------------------------------------------//
    //Voxel Functions
    //-----------------------------------------------------------------------------------//
    public BlockType GetExistingVoxel(int x, int y, int z)
    {
        Vector3 position = new Vector3(x, y, z);
        ChunkCoordinate chunkCoordinate = GetChunkCoordinate(position);
        if (!IsChunkInWorld(chunkCoordinate) || y < 0 || y > VoxelData.ChunkHeight)
        {
            return BlockType.Air;
        }

        chunks.TryGetValue(chunkCoordinate, out Chunk chunk);
        if (chunk != null && chunk.initialized)
        {
            return chunk.GetVoxel(position);
        }

        return GetNewVoxel(x, y, z);
    }

    public BlockType GetNewVoxel(int x, int y, int z)
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

        // 1st Terrain Pass
        int terrainHeight = (int)(biome.terrainHeight * Noise.Get2DPerlin(new Vector2(x, z), 0, biome.terrainScale) + biome.solidGroundHeight);
        if (y == terrainHeight)
        {
            return BlockType.Grass;
        }
        else if (y < terrainHeight && y > terrainHeight - 4)
        {
            return BlockType.Dirt;
        }
        else if (y > terrainHeight)
        {
            return BlockType.Air;
        }

        // 2nd Pass
        foreach (Lode lode in biome.lodes)
        {
            if (y > lode.minHeight &&
                y < lode.maxHeight &&
                Noise.Get3DPerlin(new Vector3(x, y, z), lode.noiseOffset, lode.scale, lode.threshold))
            {
                return lode.blockType;
            }
        }

        return BlockType.Stone;
    }
    //-----------------------------------------------------------------------------------//


    //-----------------------------------------------------------------------------------//
    //Check Functions
    //-----------------------------------------------------------------------------------//
    public ChunkCoordinate GetChunkCoordinate(Vector3 position)
    {
        int x = (int)position.x / VoxelData.ChunkWidth;
        int z = (int)position.z / VoxelData.ChunkWidth;
        return new ChunkCoordinate(x, z);
    }


    public Chunk GetChunk(Vector3 position)
    {
        ChunkCoordinate chunkCoordinate = GetChunkCoordinate(position);
        chunks.TryGetValue(chunkCoordinate, out Chunk chunk);
        return chunk;
    }

    // Used for correct map view distance updating
    public ChunkCoordinate GetChunkMidCoordinate(Vector3 position)
    {
        int x = ((int)position.x + (VoxelData.ChunkWidth / 2)) / VoxelData.ChunkWidth;
        int z = ((int)position.z + (VoxelData.ChunkWidth / 2)) / VoxelData.ChunkWidth;
        return new ChunkCoordinate(x, z);
    }

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

    public bool IsSolid(float x, float y, float z)
    {
        BlockType blockType = GetExistingVoxel((int)x, (int)y, (int)z);
        return blockType != BlockType.Air;
        /*
        int xCheck = (int)x;
        int yCheck = (int)y;
        int zCheck = (int)z;

        int xChunk = xCheck / VoxelData.ChunkWidth;
        int zChunk = zCheck / VoxelData.ChunkWidth;
        
        chunks.TryGetValue(new ChunkCoordinate(xChunk, zChunk), out Chunk chunk);
        if (chunk == null)
        {
            return false;
        }

        xCheck -= (xChunk * VoxelData.ChunkWidth);
        zCheck -= (zChunk * VoxelData.ChunkWidth);

        BlockType blockType = chunk.blocks[xCheck, yCheck, zCheck];
        */
    }
    //-----------------------------------------------------------------------------------//
}
