using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour
{
    Tile [] Tiles;
    List<BlockTile> Rearrangedtiles = new List<BlockTile>();
    private void Start ()
    {
        Tiles = GetComponentsInChildren<Tile>();
    }
    public void OnBlockPlacement ()
    {
        // Check if all blocks are placed correctly
        if (CheckSuccess())
        {
            Debug.Log("Level Completed!");
        }
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
            WaveAction();
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
    public void WaveAction ()
    {
        StartCoroutine(ShowWave());
    }
    IEnumerator ShowWave ()
    {
        Rearrangedtiles.Clear();
        BlockTile [] blockTiles = Object.FindObjectsByType<BlockTile>(FindObjectsSortMode.None);
        List<BlockTile> theBlockTiles = new List<BlockTile>();
        float Startx = 10;
        Transform StartTile = null;
        for (int i = 0 ; i < Tiles.Length ; i++)
        {
            theBlockTiles.Add(blockTiles [i]);
            if (Tiles [i].transform.position.x <= Startx)
            {
                Startx = Tiles [i].transform.position.x;
                StartTile = Tiles [i].transform;
            }

        }

        while (theBlockTiles.Count > 0)
        {
            float MinDist = 10;
            int Which = 0;
            for (int i = 0 ; i < theBlockTiles.Count ; i++)
            {
                float Dist = Vector3.Distance(StartTile.position , theBlockTiles [i].transform.position);
                if (Dist <= MinDist)
                {
                    Which = i;
                    MinDist = Dist;
                }
            }
            Rearrangedtiles.Add(theBlockTiles [Which]);
            theBlockTiles.RemoveAt(Which);

        }

        bool IsDone = false;
        int Active = 0;
        yield return new WaitForSeconds(0.6f);
        IsDone = false;
        Active = 0;
        Tile [] FloorTiles = GetComponentsInChildren<Tile>();
        while (!IsDone)
        {
            float T = 0.1f;
            if (Active > FloorTiles.Length - 1)
            {
                IsDone = true;
            }
            else
            {
                FloorTiles [Active].Owner.GetComponent<BlockTile>().Wave();
                FloorTiles [Active].Owner.GetComponent<BlockTile>().ShowCorrect();
                //CashUi

                Active += 1;


            }
            yield return new WaitForSeconds(T);
        }
    }
}
