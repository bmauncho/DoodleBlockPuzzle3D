using System.Collections.Generic;
using UnityEngine;

public class BlockTile : MonoBehaviour
{
    public bool IsDrag;
    public Transform [] RayPoints;
    [HideInInspector] public GameObject OccupiedTarget;
    public bool isStatic;

    private void OnMouseDown ()
    {
        //Debug.Log("Mouse Down on BlockTile");
        IsDrag = true;
    }

    private void OnMouseUp ()
    {
        IsDrag = false;
        GetComponentInParent<Block>().FinishedDrag();
        //Debug.Log("Mouse Up on BlockTile");
    }

    private void Update ()
    {
        if (IsDrag)
        {
            GetComponentInParent<Block>().Drag(transform.localPosition);
        }
    }

    public Transform GetSingleHit ()
    {
        RaycastHit hit;
        foreach (var point in RayPoints)
        {
            if (Physics.Raycast(point.position , Vector3.down , out hit , 20f))
            {
                //Debug.Log($"Raycast hit: {hit.transform.name}");
                return hit.transform;
            }
        }

       // Debug.Log("No hits detected by GetSingleHit raycasts");
        return null;
    }

    public List<Transform> GetHit ()
    {
        List<Transform> hitList = new List<Transform>();
        RaycastHit hit;
        foreach (var point in RayPoints)
        {
            if (Physics.Raycast(point.position , Vector3.down , out hit , 5f))
            {
               // Debug.Log($"Raycast detected object: {hit.transform.name}");
                hitList.Add(hit.transform);
            }
        }
        return hitList;
    }
}
