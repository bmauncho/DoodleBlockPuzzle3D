using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    Level level;
    Transform PrevPos;
    bool IsDrag = false;
    public bool IsAlreadyLifted = false;
    public bool IsPlaced = false;
    BlockTile [] blockTiles;

    private void Start ()
    {
        PrevPos = GetComponentInParent<BlockTarget> ().transform;
        blockTiles = GetComponentsInChildren<BlockTile>();
        level = GetComponentInParent(typeof(Level)) as Level;
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
            Debug.Log($"Hit: {hit.transform.name}");
            transform.position = ( hit.point - Offset ) + new Vector3(0 , 2 , 0);
        }
        else
        {
            Debug.Log("No object hit by raycast");
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

    // Block.cs
    public void FinishedDrag ()
    {
        Debug.Log("Finishing Drag");

        // Re-enable colliders on block tiles after dragging
        foreach (var tile in blockTiles)
        {
            tile.GetComponent<Collider>().enabled = true;
        }

        // Check if placement is correct
        if (CheckCorrect())
        {
            Debug.Log("Placement successful");
            level.OnBlockPlacement();
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
            StartCoroutine(ErrorShow());
        }
    }

    IEnumerator ErrorShow ()
    {
        MeshRenderer [] Meshes = GetComponentsInChildren<MeshRenderer>();
        Material [] originalMaterials = new Material [Meshes.Length];
        float waitTime = 0.5f;

        // Change materials to indicate error
        for (int i = 0 ; i < Meshes.Length ; i++)
        {
            originalMaterials [i] = Meshes [i].material;
            Meshes [i].material = Meshes [i].GetComponentInParent<BlockTile>().ErrorMat;
        }

        // Shake block for error feedback
        transform.DOShakeRotation(0.5f , 20 , 10 , 45 , true);
        yield return new WaitForSeconds(waitTime);

        // Reset materials and move back to initial position
        for (int i = 0 ; i < Meshes.Length ; i++)
        {
            Meshes [i].material = originalMaterials [i];
        }
        transform.eulerAngles = Vector3.zero;
        transform.DOMove(PrevPos.position , 0.5f);

        // Re-enable colliders on block tiles after error show
        foreach (var tile in blockTiles)
        {
            tile.GetComponent<Collider>().enabled = true;
        }

        IsDrag = false;
        IsPlaced = false; // Ensure IsPlaced is set to false for incorrect placements
    
        yield return null;
        
    }


    public bool CheckCorrect ()
    {
        int correctCount = 0;
        foreach (var tile in blockTiles)
        {
            Transform hitTransform = tile.GetSingleHit();
            if (hitTransform != null && hitTransform.GetComponentInParent<Tile>() is Tile tileComponent)
            {
                if (tileComponent.AddOwner(tile.gameObject))
                {
                    correctCount++;
                }
            }
        }

        if (correctCount == blockTiles.Length)
        {
            IsPlaced = true;
            transform.DOMove(transform.position + new Vector3(0 , -2 , 0) , 0.1f);
            return true;
        }
        else
        {
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
            return false;
        }
    }
}
