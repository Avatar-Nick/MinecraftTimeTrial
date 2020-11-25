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
    public int treeZoneHeight = 100;
    public float treeZoneScale = 1.3f;
    [Range(0.1f, 1f)]
    public float treeZoneThreshold = 0.6f;

    public float treePlacementScale = 15f;
    [Range(0.1f, 1f)]
    public float treePlacementThreshold = 0.8f;

    public int maxTreeHeight = 12;
    public int minTreeHeight = 5;

    [Header("Water")]
    public int waterHeight = 100;

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
