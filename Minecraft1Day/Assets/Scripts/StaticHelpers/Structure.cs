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

        int leavesWidth = 4 + (int)(Map.instance.random.NextDouble() * 6);
        int leavesHeight = 4 + (int)(Map.instance.random.NextDouble() * 6);

        int shift = 0;
        for (int y = 0; y < leavesHeight; y++)
        {
            for (int x = -(leavesWidth / 2) + (shift / 2); x < (leavesWidth / 2) - (shift / 2); x++)
            {
                for (int z = -(leavesWidth / 2) + (shift / 2); z < (leavesWidth / 2) - (shift / 2); z++)
                {
                    int leavesPercent = (int)Map.instance.random.NextDouble();
                    if (leavesPercent < 0.8f)
                    {
                        queue.Enqueue(new VoxelMod(BlockType.Leaves, new Vector3(coordinate.x + x, coordinate.y + height + y, coordinate.z + z)));
                    }
                }
            }
            shift += 1;
        }        
    }
}
