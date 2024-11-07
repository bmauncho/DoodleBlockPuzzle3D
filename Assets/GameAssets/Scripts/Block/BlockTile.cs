using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTile : MonoBehaviour
{
    private Vector3 initialMousePosition;
    public Transform [] RayPoints;
    [HideInInspector] public GameObject OccupiedTarget;
    public bool IsDragging;
    public bool isStatic;
    public bool IsClicked;
    public bool IsDoubleclicked;
    public Material ErrorMat;
    public Material FlipMat;
    Vector3 StartPos;
    Vector3 UpPos;
    float WaveTimestamp;
    float UsedTimestamp;
    public float ClickResetTime = .25f;
    private float mouseDownTime;
    private float dragThreshold = 10f;
    private bool isMouseHeld = false;
    private const float holdThreshold = 0.1f;
    private bool actionTriggered = false;
    public GameObject StarBurst;
    public GameObject LandingSmoke;
    public bool clockwiserotation = false;
    public bool counterclockwiserotation = false;

    private int clickCount = 0;
    private float clickResetDelay = 0.2f; // Delay to reset the click count
    private Coroutine clickCoroutine;

    private void Start ()
    {
        StartPos = transform.localPosition;
    }

    private void OnMouseDown ()
    {
        initialMousePosition = Input.mousePosition;
        IsDragging = false;
        IsClicked = false;
        mouseDownTime = Time.time;
        isMouseHeld = true;
        actionTriggered = false;
        clockwiserotation = false;
        counterclockwiserotation = false;
    }

    private void OnMouseDrag ()
    {
        float distance = Vector3.Distance(initialMousePosition , Input.mousePosition);

        if (distance > dragThreshold)
        {
            IsDragging = true;
        }
    }

    private void OnMouseUp ()
    {
        if (!IsDragging)
        {
            clickCount++;

            // If a coroutine is already running, stop it to reset the delay
            if (clickCoroutine != null)
            {
                StopCoroutine(clickCoroutine);
            }

            // Start the coroutine to check click count after a delay
            clickCoroutine = StartCoroutine(HandleClicks());
        }
        else
        {
            IsDragging = false;
            GetComponentInParent<Block>().FinishedDrag();
        }

        isMouseHeld = false;
    }

    private IEnumerator HandleClicks ()
    {
        yield return new WaitForSeconds(clickResetDelay);

        if (clickCount == 1)
        {
            // Single-click detected
            IsClicked = true;
            IsDoubleclicked = false;
            Clicked();
        }
        else if (clickCount == 2)
        {
            // Double-click detected
            IsDoubleclicked = true;
            IsClicked = false;
            DoubleClicked();
        }

        // Reset the click count and coroutine reference
        clickCount = 0;
        clickCoroutine = null;
    }


    void Clicked ()
    {
        if (!clockwiserotation)
        {
            clockwiserotation = true;
            GetComponentInParent<Block>().Rotate(true); // Clockwise rotation
        }
    }

    void DoubleClicked ()
    {
        Debug.Log("Double-clicked on BlockTile!");
        if (!counterclockwiserotation)
        {
            counterclockwiserotation = true;
            GetComponentInParent<Block>().Rotate(false); // Counterclockwise rotation
        }
    }

    private void Update ()
    {
        if (isMouseHeld && !actionTriggered && Time.time - mouseDownTime > holdThreshold)
        {
            TriggerAction();
            actionTriggered = true;
        }

        if (IsDragging)
        {
            GetComponentInParent<Block>().Drag(transform.localPosition);
        }
        else if (IsClicked)
        {
            ClickResetTime -= Time.deltaTime;
            if (ClickResetTime <= 0)
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


    void TriggerAction ()
    {
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
            Debug.DrawRay(point.position , Vector3.down * 20f , Color.red , 0.1f);

            if (Physics.Raycast(point.position , Vector3.down , out hit , 20f))
            {
                return hit.transform;
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
