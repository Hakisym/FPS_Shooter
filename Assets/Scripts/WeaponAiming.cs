using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAiming : MonoBehaviour
{
    public Vector3 aimPosition;
    public Vector3 aimRotation;
    public float smoothTime;
    public float smoothRotation;
    public PlayerBehaviour pb;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        bool isAiming = Input.GetMouseButton(1);
        Vector3 finalPosition;
        Quaternion finalRotation;

        if (isAiming)
        {
            finalPosition = aimPosition;
            finalRotation = Quaternion.Euler(aimRotation);
            pb.isAiming = true;
        }
        else
        {
            finalPosition = originalPosition;
            finalRotation = originalRotation;
            pb.isAiming = false;
        }

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, finalPosition, ref velocity, smoothTime);
        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation, Time.deltaTime * smoothRotation);

    }
}
