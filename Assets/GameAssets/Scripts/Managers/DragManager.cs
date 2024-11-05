using UnityEngine;

public class DragManager : MonoBehaviour
{
    public Camera TheCamera;
    public Block target;
    public Vector3 Offset = new Vector3(0 , 2 , 2);
    Plane m_Plane;
    Transform Prev_Pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_Plane = new Plane(Vector3.up , 0);
    }

    // Update is called once per frame
    void Update ()
    {
        //if (Input.GetKey(KeyCode.Mouse0))
        //{
        //    Ray ray = TheCamera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;
        //    if (!target)
        //    {
        //        if (Physics.Raycast(ray , out hit))
        //        {
        //            if (hit.transform.GetComponentInParent<Block>())
        //            {
        //                Debug.Log("Hit a block!");
        //                target = hit.transform.GetComponentInParent<Block>();
        //                target.IsDrag = true;
        //                target.ToggleColliders(false);
        //                Prev_Pos = target.GetComponentInParent<BlockTarget>().transform;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        float enter = 0.0f;
        //        if (m_Plane.Raycast(ray , out enter))
        //        {
        //            //Get the point that is clicked
        //            Vector3 hitPoint = ray.GetPoint(enter);

        //            //Move your cube GameObject to the point where you clicked
        //            target.transform.position = hitPoint + Offset;
        //        }
        //    }
        //}
        //else
        //{
        //    if (target)
        //    {
        //        //place
        //        target.IsDrag = false;
        //    }
        //}
    }
}
