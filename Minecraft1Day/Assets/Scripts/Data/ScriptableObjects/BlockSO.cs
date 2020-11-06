using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockSO", menuName = "MinecraftTimeTrial/BlockSO")]
public class BlockSO : ScriptableObject
{
    public BlockType blockType;
    public Sprite sprite;
}
