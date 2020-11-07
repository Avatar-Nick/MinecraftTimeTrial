using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelMod
{
    public BlockType blockType;
    public Vector3 coordinate;

    public VoxelMod() { }

    public VoxelMod(BlockType blockType, Vector3 coordinate)
    {
        this.blockType = blockType;
        this.coordinate = coordinate;
    }
}
