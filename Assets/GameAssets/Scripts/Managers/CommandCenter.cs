using UnityEngine;

public class CommandCenter : MonoBehaviour
{
    public static CommandCenter Instance ;
    public ColorLib ColorLib_;
    public BlockLib BlockLib_;

    private void Awake ()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
}
