using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "MinecraftTimeTrial/Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;

    [Header("Trees")]
    public int treeZoneHeight = 65;
    public float treeZoneScale = 1.3f;
    [Range(0.1f, 1f)]
    public float treeZoneThreshold = 0.6f;

    public float treePlacementScale = 15f;
    [Range(0.1f, 1f)]
    public float treePlacementThreshold = 0.8f;

    public int maxTreeHeight = 12;
    public int minTreeHeight = 5;

    [Header("Water")]
    public int waterHeight = 60;
    public int sandDepth = 3;
    public float waterSandZoneThreshold = 0.7f;

    [Header("Stone")]
    public int stoneheight = 75;
    public float stoneZoneThreshold = 0.4f;

    [Header("Lodes")]
    public Lode[] lodes;
}

[Serializable]
public class Lode
{
    public string lodeName;
    public BlockType blockType;
    public int minHeight;
    public int maxHeight;
    public float scale;
    public float threshold;
    public float noiseOffset;
}
