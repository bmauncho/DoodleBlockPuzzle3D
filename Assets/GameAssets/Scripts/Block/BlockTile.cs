using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTile : MonoBehaviour
{
    public bool IsDrag;
    public Transform [] RayPoints;
    [HideInInspector] public GameObject OccupiedTarget;
    public bool isStatic;
    public Material ErrorMat;
    public Material FlipMat;
    Vector3 StartPos;
    Vector3 UpPos;
    float WaveTimestamp;
    float UsedTimestamp;
    private void Start ()
    {
        StartPos = transform.localPosition;
    }

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
        else
        {
            if (WaveTimestamp < Time.time)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition , StartPos , 100 * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation , Quaternion.Euler(Vector3.zero) , 10 * Time.deltaTime);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition , StartPos , 10 * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation , Quaternion.Euler(new Vector3(0 , 0 , -30)) , 10 * Time.deltaTime);
            }
        }
    }
    public void Wave ()
    {
        if (UsedTimestamp > Time.time)
        {
            return;
        }
        UsedTimestamp = Time.time + .5f;

        transform.eulerAngles = new Vector3(0 , 0 , 120);
        WaveTimestamp = Time.time + 0.3f;
        transform.localPosition += new Vector3(0 , 1.5f , 0);
        StartCoroutine(ChangeColor());

    }
    IEnumerator ChangeColor ()
    {
        MeshRenderer [] Meshes = GetComponentsInChildren<MeshRenderer>();
        Material [] TheMat = new Material [Meshes.Length];
        for (int i = 0 ; i < Meshes.Length ; i++)
        {
            TheMat [i] = Meshes [i].material;
            Meshes [i].material = FlipMat;
        }
        yield return new WaitForSeconds(0.2f);
    }
    public void ShowCorrect ()
    {
        MeshRenderer [] Meshes = GetComponentsInChildren<MeshRenderer>();
        Material [] TheMat = new Material [Meshes.Length];
        for (int i = 0 ; i < Meshes.Length ; i++)
        {
            TheMat [i] = Meshes [i].material;
            Meshes [i].material = FlipMat;
        }
    }

    public Transform GetSingleHit ()
    {
        RaycastHit hit;
        foreach (var point in RayPoints)
        {
            // Draw a ray from each RayPoint downward with a length of 20 units
            Debug.DrawRay(point.position , Vector3.down * 20f , Color.red , 0.1f);

            // Cast the ray
            if (Physics.Raycast(point.position , Vector3.down , out hit , 20f))
            {
                return hit.transform; // Returns the first hit
            }
        }
        return null;
    }

    public List<Transform> GetHit ()
    {
        List<Transform> hitList = new List<Transform>();
        RaycastHit hit;
        foreach (var point in RayPoints)
        {
            if (Physics.Raycast(point.position , Vector3.down , out hit , 20f))
            {
                hitList.Add(hit.transform);
            }
        }
        return hitList;
    }

}
