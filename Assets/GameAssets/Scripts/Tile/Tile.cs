using UnityEngine;

public class Tile : MonoBehaviour
{
    public GameObject Owner;
    private void Start ()
    {
        if (Owner != null)
        {
            Owner.GetComponentInChildren<BlockTile>().isStatic = true;
        }
    }
    public bool AddOwner ( GameObject Who )
    {
        if (Owner == null)
        {
            Owner = Who;
            Owner.GetComponentInChildren<BlockTile>().OccupiedTarget = gameObject;
            return true;
        }
        else
        {
            return false;
        }
    }
}
