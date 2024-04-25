
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathCamera : MonoBehaviour
{
    public float lookDistance = 5;
    public Vector3 lookOffset;

    public Camera Camera { get; private set; }
    public AudioListener AudioListener { get; private set; }

    private Transform target;

    private void Awake()
    {
        Camera = GetComponent<Camera>();
        AudioListener = GetComponent<AudioListener>();

        Disable();
    }

    private void Update()
    {
        if (target)
        {
            transform.LookAt(target);
        }
    }

    public void Enable(Actor self,Ragdoll ragdoll)
    {
        Vector3 insideUnitSpherePosition = Random.insideUnitSphere * lookDistance;
        insideUnitSpherePosition.y = 0;

        transform.position = self.transform.position + insideUnitSpherePosition + lookOffset;

        target = ragdoll.transform.GetComponentInChildren<Rigidbody>().transform;

        Camera.enabled = true;
        AudioListener.enabled = true;
    }

    public void Disable()
    {
        target = null;

        Camera.enabled = false;
        AudioListener.enabled = false;
    }
}
