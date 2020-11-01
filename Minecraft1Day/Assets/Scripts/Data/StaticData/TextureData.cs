using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureData
{
    public static readonly float TextureAtlasSizeInBlocks = 4.0f;

    public static float NormalizedBlockTextureSize
    {
        get { return 1 / (float)TextureAtlasSizeInBlocks; }
    }

    public static Dictionary<FaceType, Face> faces = new Dictionary<FaceType, Face>()
    {
        { FaceType.Dirt, new Face(FaceType.Dirt, 1, 3) },
        { FaceType.GrassSide, new Face(FaceType.GrassSide, 2, 3) },
        { FaceType.Grass, new Face(FaceType.Grass, 3, 2) },
        { FaceType.Leaves, new Face(FaceType.Leaves, 0, 3) },
        { FaceType.TreeSide, new Face(FaceType.TreeSide, 1, 2) },
        { FaceType.TreeTop, new Face(FaceType.TreeTop, 2, 2) },
        { FaceType.Stone, new Face(FaceType.Stone, 0, 3) },
        { FaceType.Sand, new Face(FaceType.Sand, 2, 1) },
        { FaceType.Bedrock, new Face(FaceType.Bedrock, 1, 1) },
    };


    public static Dictionary<BlockType, Block> blocks = new Dictionary<BlockType, Block>()
    {
        { BlockType.Dirt, new Block(BlockType.Dirt, FaceType.Dirt) },
        { BlockType.Grass, new Block(BlockType.Grass, FaceType.Grass, FaceType.GrassSide, FaceType.Dirt) },
        { BlockType.Leaves, new Block(BlockType.Leaves, FaceType.Leaves) },
        { BlockType.Trunk, new Block(BlockType.Trunk, FaceType.TreeTop, FaceType.TreeSide, FaceType.TreeTop) },
        { BlockType.Stone, new Block(BlockType.Stone, FaceType.Stone) },
        { BlockType.Sand, new Block(BlockType.Sand, FaceType.Sand) },
        { BlockType.Bedrock, new Block(BlockType.Bedrock, FaceType.Bedrock) }
    };
}
