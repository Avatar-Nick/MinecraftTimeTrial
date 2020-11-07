using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure
{
    public static void CreateTree(Vector3 coordinate, Queue<VoxelMod> queue, int minTrunkHeight, int maxTrunkHeight)
    {
        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(coordinate.x, coordinate.z), 0, 1.0f));
        if (height < minTrunkHeight)
        {
            height = minTrunkHeight;
        }
        
        for (int i = 1; i < height; i++)
        {
            queue.Enqueue(new VoxelMod(BlockType.Wood, new Vector3(coordinate.x, coordinate.y + i, coordinate.z)));
        }
        
        for (int x = -3; x < 4; x++)
        {
            for (int y = 0; y < 7; y++)
            {
                for (int z = -4; z < 4; z++)
                {
                    queue.Enqueue(new VoxelMod(BlockType.Leaves, new Vector3(coordinate.x + x, coordinate.y + height + y, coordinate.z + z)));
                }
            }
        }
    }
}
