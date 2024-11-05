using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    public bool IsDrag = false;
    public bool isAlreadyLifted = false;

    public void ToggleColliders ( bool istrue )
    {
        Collider [] colliders = GetComponentsInChildren<Collider>();
        for (int i = 0 ; i < colliders.Length ; i++)
        {
            colliders [i].enabled = istrue;
        }

        if (istrue == false && !isAlreadyLifted)
        {
            isAlreadyLifted = true;
        }
    }

    public IEnumerator MoveToTarget ( Transform which , Transform _Target )
    {
        float Dist = Vector3.Distance(which.position , _Target.position);
        while ( Dist > 0.1f )
        {
            Dist = Vector3.Distance(which.position , _Target.position);
            which.transform.position = Vector3.Lerp(which.transform.position , _Target.position , 5 * Time.deltaTime);
            yield return new WaitForSeconds(0.001f);
        }
        which.transform.SetParent(_Target);
        which.transform.localPosition = Vector3.zero;
        ToggleColliders ( true );
    }
}
