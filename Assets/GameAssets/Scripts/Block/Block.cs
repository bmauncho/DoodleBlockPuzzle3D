using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Block : MonoBehaviour
{
    Level level;
    Vector3 StartPos;
    Transform PrevPos;
    public bool IsDrag = false;
    public bool IsPlaced = false;
    BlockTile [] blockTiles;
    public bool IsClicked;

    private void Start ()
    {
        PrevPos = GetComponentInParent<BlockTarget> ().transform;
        blockTiles = GetComponentsInChildren<BlockTile>();
        level = GetComponentInParent(typeof(Level)) as Level;
    }

    private void Update ()
    {
        IsClicked = CheckClick();
    }

    public void Rotate (bool isClockwise)
    {
        if (isClockwise)
        {
            Debug.Log("Rotate clockwise");
            //StartCoroutine(Rotation(true));  
        }
        else
        {
            Debug.Log("Rotate  counterclockwise");
            //StartCoroutine(Rotation(false));
        }
        
    }
    
    IEnumerator Rotation(bool isClockwise )
    {
        Rotator rotator = transform.GetComponentInChildren<Rotator>();
        if (isClockwise)
        {

            Quaternion newRot = rotator.transform.rotation * Quaternion.Euler(0 , 90 , 0);
            rotator.transform.DORotateQuaternion(newRot , 0.25f);

        }
        else
        {
            Quaternion newRot = rotator.transform.rotation * Quaternion.Euler(0 , -90 , 0);
            rotator.transform.DORotateQuaternion(newRot , 0.25f);

        }
        yield return null;
    }
   
    public void Drag ( Vector3 Offset )
    {
        IsPlaced = false;
        IsDrag = true;
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
    }

    public void Bounce()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray , out hit))
        {
            Vector3 ScaleOffset = new Vector3(.2f , .2f , .2f);
            transform.DOPunchScale(ScaleOffset , .25f , 10 , 1);
        }
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
            IsPlaced = true;
            Debug.Log("Placement successful");
            if (transform.GetComponentInParent<BlockTarget>()?.TheOwner)
            {
                transform.GetComponentInParent<BlockTarget>().TheOwner = null;
                transform.SetParent(level.transform);
            }

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
        transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
        Vector3 Offset = new Vector3(0 , .5f , 0);
        StartPos = new Vector3(transform.position.x , StartPos.y+Offset.y  , transform.position.z);
        if (transform.GetComponentInParent<BlockTarget>()?.TheOwner)
        {
            transform.DOMove(PrevPos.position , 0.5f);
        }
        else
        {
            transform.DOMove(StartPos , .5f);
            
        }
        

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
        Vector3 cumulativeOffset = Vector3.zero;

        for (int i = 0 ; i < blockTiles.Length ; i++)
        {
            Transform hitTransform = blockTiles [i].GetSingleHit();

            if (hitTransform != null && hitTransform.GetComponentInParent<Tile>() is Tile tileComponent)
            {
                if (tileComponent.AddOwner(blockTiles [i].gameObject))
                {
                    correctCount++;

                    Vector3 localTilePos = transform.InverseTransformPoint(hitTransform.GetComponentInParent<Tile>().transform.position);
                    Vector3 localBlockTilePos = blockTiles [i].transform.localPosition;

                    // Accumulate the offset in local space
                    Vector3 localOffset = localTilePos - localBlockTilePos;
                    cumulativeOffset += localOffset;
                }
            }
        }

        if (correctCount == blockTiles.Length)
        {
            IsPlaced = true;

            // Average offset for all tiles
            Vector3 finalOffset = cumulativeOffset / correctCount;

            // Convert the final offset back to world space and move the block
            Vector3 targetPos = transform.TransformPoint(finalOffset);


            transform.DOMove(targetPos , 0.1f)
                .OnComplete(() =>
                {
                    for (int i = 0 ; i < blockTiles.Length ; i++)
                    {
                        if (blockTiles [i].LandingSmoke)
                        {
                            blockTiles [i].LandingSmoke.GetComponent<ParticleSystem>().Play();
                        }
                    }

                    Camera.main.DOShakePosition(.2f , .1f , 1 , 45 , true , ShakeRandomnessMode.Harmonic);
                });

            return true;
        }
        else
        {
            // Reset ownership for incorrect placement
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

    public bool CheckClick ()
    {
        for (int i = 0; i < blockTiles.Length; i++)
        {
            if (blockTiles [i].IsClicked)
            {
                return true;
            }
        }
        return false;
    }
}
