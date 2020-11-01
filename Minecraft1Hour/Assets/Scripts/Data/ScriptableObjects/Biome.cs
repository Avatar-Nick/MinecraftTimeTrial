using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Biome", menuName = "MinecraftTimeTrial/Biome")]
public class Biome : ScriptableObject
{
    public string biomeName;

    public int solidGroundHeight;
    public int terrainHeight;
    public float terrainScale;

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
