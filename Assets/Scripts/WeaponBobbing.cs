using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBobbing : MonoBehaviour
{
    public float effectIntensity = 0.03f;
    public float effectIntensityX = 0.01f;
    public float effectSpeed = 9f;
    public float effectSpeedIdle = 0.5f;
    public float smoothTime = 0.1f;
    public PlayerBehaviour pb;

    private Vector3 originalPosition;
    private float sinTime;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        // Change bobbing speed with movement
        if (inputVector.magnitude > 0f)
        {
            sinTime += Time.deltaTime * effectSpeed;
        }
        else
        {
            sinTime += Time.deltaTime * effectSpeedIdle;
        }

        // Calculate bobbing position 
        float sinAmountY = -Mathf.Abs(Mathf.Sin(sinTime) * effectIntensity * pb.currentAimEffectMultiplier);
        // Change horizontal bobbing with movement
        float sinAmountX = Mathf.Cos(sinTime) * (inputVector.magnitude > 0f ? effectIntensityX * pb.currentAimEffectMultiplier : 0f);

        Vector3 finalPosition = originalPosition + Vector3.up * sinAmountY + Vector3.right * sinAmountX;

        // Smooth towards final position
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref velocity, smoothTime);
    }
}
