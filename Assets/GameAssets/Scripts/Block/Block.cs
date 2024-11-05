using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    bool IsDrag = false;
    public bool IsAlreadyLifted = false;
    public bool IsPlaced = false;
    BlockTile [] blockTiles;

    private void Start ()
    {
        blockTiles = GetComponentsInChildren<BlockTile>();
    }

    public void Drag ( Vector3 Offset )
    {
        //Debug.Log("Starting Dragging");

        // Disable colliders on block tiles to prevent interaction while dragging
        foreach (var tile in blockTiles)
        {
            tile.GetComponent<Collider>().enabled = false;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray , out hit))
        {
            //Debug.Log($"Hit: {hit.transform.name}");
            transform.position = ( hit.point - Offset ) + new Vector3(0 , 1 , 0);
        }
        else
        {
            //Debug.Log("No object hit by raycast");
        }

        // Clear OccupiedTarget references
        foreach (var tile in blockTiles)
        {
            if (tile.OccupiedTarget != null)
            {
                tile.OccupiedTarget.GetComponent<Tile>().Owner = null;
                tile.OccupiedTarget = null;
            }
        }

        IsPlaced = false;
        IsDrag = true;
    }

    public void FinishedDrag ()
    {
        Debug.Log("Finishing Drag");

        if (CheckCorrect())
        {
            foreach (var tile in blockTiles)
            {
                tile.GetComponentInChildren<Collider>().enabled = true;
            }
        }

        if (GetComponentInParent<Level>().CanCheckOver())
        {
            if (GetComponentInParent<Level>().CheckSuccess())
            {
                Debug.Log("Placement successful");
            }
            else
            {
                Debug.Log("Placement failed, clearing tiles");
                foreach (var tile in blockTiles)
                {
                    if (tile.OccupiedTarget != null)
                    {
                        var tileComponent = tile.OccupiedTarget.GetComponent<Tile>();
                        if (tileComponent != null)
                        {
                            tileComponent.Owner = null;
                        }
                        tile.OccupiedTarget = null;
                    }
                }

            }
        }
        for (int i = 0 ; i < blockTiles.Length ; i++)
        {
            blockTiles [i].GetComponent<Collider>().enabled = true;
        }
        IsDrag = false;
    }

    public bool CheckCorrect ()
    {
        int correctCount = 0;
        Vector3 offset = Vector3.zero;
        foreach (var tile in blockTiles)
        {
            Transform hitTransform = tile.GetSingleHit();
            if (hitTransform != null && hitTransform.GetComponentInParent<Tile>() is Tile tileComponent)
            {
                if (tileComponent.AddOwner(tile.gameObject))
                {
                    correctCount++;
                    offset = transform.InverseTransformPoint(tileComponent.transform.position) - tile.transform.localPosition;
                }
            }
            else
            {
                Debug.Log("No tile detected for block tile during CheckCorrect");
            }
        }

        if (correctCount == blockTiles.Length)
        {
            IsPlaced = true;
            offset.y = 0;
            Vector3 targetPos = transform.position + new Vector3(0 , -1 , 0) + offset;
            transform.DOMove(targetPos , 0.1f);
            return true;
        }
        else
        {
            Debug.Log("Not all block tiles are correctly positioned");
            foreach (var tile in blockTiles)
            {
                if (tile.OccupiedTarget != null)
                {
                    // Set the Owner property of the Tile component to null
                    var tileComponent = tile.OccupiedTarget.GetComponent<Tile>();
                    if (tileComponent != null)
                    {
                        tileComponent.Owner = null;
                    }

                    // Clear the OccupiedTarget reference
                    tile.OccupiedTarget = null;
                }
            }

            return false;
        }
    }
}
