using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Colors
{
    Default,
    Yellow,
    Blue,
    Pink,
    Green,
    Orange,
    Red,
    Purple
}
[System.Serializable]
public class TheColor
{
    public Colors whichColor;
    public Material BlockMat;
}
public class ColorLib : MonoBehaviour
{
    public TheColor [] _Colors;

    public TheColor GetColorLib ( Colors whichColor )
    {
        if (_Colors == null) { return null; }

        for (int i = 0 ; i < _Colors.Length ; i++)
        {
            if (_Colors [i].whichColor == whichColor)
            {
                return _Colors [i];
            }
        }
        return null;  // If not found, return null
    }

    public Material GetMaterial(Colors which )
    {
        for(int i= 0 ; i < _Colors.Length ;i++)
        {
            if( _Colors [i].whichColor == which)
            {
                return _Colors [i].BlockMat;
            }
        }
        return null;
    }
}
