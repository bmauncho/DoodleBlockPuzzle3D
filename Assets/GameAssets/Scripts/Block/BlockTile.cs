using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BlockTile : MonoBehaviour
{
    private Vector3 initialMousePosition;
    public Transform [] RayPoints;
    [HideInInspector] public GameObject OccupiedTarget;
    public bool IsDragging;
    public bool isStatic;
    public bool IsClicked;
    public Material ErrorMat;
    public Material FlipMat;
    Vector3 StartPos;
    Vector3 UpPos;
    float WaveTimestamp;
    float UsedTimestamp;
    public float ClickResetTime = .25f;
    private float mouseDownTime;
    private float dragThreshold = 10f; // Adjust this for click vs drag sensitivity
    private bool isMouseHeld = false;
    private const float holdThreshold = 0.1f; // Time threshold in seconds
    private bool actionTriggered = false;
    public GameObject StarBurst;
    public GameObject LandingSmoke;

    private void Start ()
    {
        StartPos = transform.localPosition;
    }

    private void OnMouseDown ()
    {
        //Debug.Log("Mouse Down on BlockTile");
        initialMousePosition = Input.mousePosition;
        IsDragging = false;
        IsClicked = false;
        mouseDownTime = Time.time;
        isMouseHeld = true;
        actionTriggered = false;
    }

    private void OnMouseDrag ()
    {
        float distance = Vector3.Distance(initialMousePosition , Input.mousePosition);

        if (distance > dragThreshold)
        {
            // Start dragging only if the movement exceeds the threshold
            IsDragging = true;
        }
    }

    private void OnMouseUp ()
    {
        if (!IsDragging)
        {
            IsClicked = true;
            Clicked();
        }
        else
        {
            IsDragging = false;
            GetComponentInParent<Block>().FinishedDrag();
        }
        isMouseHeld = false;
        Debug.Log("Mouse Up on BlockTile");
    }
    private void Update ()
    {
        if (isMouseHeld && !actionTriggered && Time.time - mouseDownTime > holdThreshold)
        {
            // Action to perform after holding for more than 0.3 seconds
            TriggerAction();
            actionTriggered = true;  // Prevent repeated actions
        }


        if (IsDragging)
        {
            GetComponentInParent<Block>().Drag(transform.localPosition);
        }
        else if (IsClicked)
        {
            GetComponentInParent<Block>().Rotate();
            ClickResetTime -= Time.deltaTime;
            if (ClickResetTime<=0)
            {
                IsClicked = false;
                ClickResetTime = .25f;
            }
            
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
    void Clicked ()
    {
        if (isMouseHeld && Time.time - mouseDownTime < holdThreshold)
        {
            // You can add an action here for a short press if desired
            Debug.Log("Mouse released too soon!");
        }
    }
    void TriggerAction ()
    {
        // Perform the action, e.g., rotate the object
       // Debug.Log("Mouse held down for more than 0.3 seconds - Action triggered");
        GetComponentInParent<Block>().Bounce();    
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
        if (StarBurst)
        {
            StarBurst.transform.GetComponent<ParticleSystem>().Play();
        }
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
