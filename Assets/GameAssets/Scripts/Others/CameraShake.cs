using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform camTransform;
    public float shakeDuration = 0f;
    public float shakeAmount = 0.7f;
    public float ShakeDur;
    public float decreaseFactor = 1.0f;
    public Vector3 originalPos;
    public bool StopShake;

    public void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    public void ShakeNow(float ShakeInt)
    {
        StopShake = false;
        originalPos = camTransform.localPosition;
        shakeDuration = ShakeInt;
    }

    public void ShakeCameraNow() 
    { ShakeNow(ShakeDur); }

    void Update()
    {
        if (shakeDuration > 0)
        {
            camTransform.localPosition = transform.localPosition + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;

        }
        else
        {
            if (!StopShake)
            {
                camTransform.localPosition = originalPos;
                StopShake = true;
            }
        }
    }
}