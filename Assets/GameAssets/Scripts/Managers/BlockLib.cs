using UnityEngine;
using System.Collections.Generic;
public enum BlockType
{
    Default,
    Rectangular,
    L_shaped,
    T_shaped,
}
[System.Serializable]
public class TheBlock
{
    public BlockType blockType;
    public GameObject BlockPref;
}
public class BlockLib : MonoBehaviour
{
    public TheBlock [] _blocks;

    public TheBlock GetBlock ( BlockType blockType )
    {
        if (_blocks == null) { return null; }

        for(int i = 0; i < _blocks.Length; i++)
        {
            if(_blocks[i].blockType == blockType)
            {
                return _blocks[i];
            }
        }
        return null;
    }

    public GameObject GetBlockPref(BlockType blockType )
    {
        TheBlock block = GetBlock(blockType);
        return block?.BlockPref;
    }
}
