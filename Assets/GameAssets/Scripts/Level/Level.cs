using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    public bool IsLevelStarted = false;
    public bool IsLevelFinished = false;
    public bool IsLevelPaused = false;

    Tile [] Tiles;
    private void Start ()
    {
        Tiles = GetComponentsInChildren<Tile>();
    }

    public bool CanCheckOver ()
    {
        int CorrectCount = 0;
        for (int i = 0 ; i < Tiles.Length ; i++)
        {
            if (Tiles [i].Owner != null)
            {
                CorrectCount += 1;
            }
        }
        if (CorrectCount == Tiles.Length)
        {
            return true;
        }
        return false;
    }
    public bool CheckSuccess ()
    {
        int CorrectCount = 0;
        for (int i = 0 ; i < Tiles.Length ; i++)
        {
            if (Tiles[i].Owner != null)
            {
                CorrectCount += 1;
            }
        }
        if (CorrectCount == Tiles.Length)
        {
            Debug.Log("Success");
            return true;
        }
        else
        {
            StartCoroutine(DelayRestart());
            return false;
        }
    }
    IEnumerator DelayRestart ()
    {
        yield return new WaitForSeconds(3);

    }
}
