
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

    public void Enable(Transform player)
    {
        transform.position = player.position + player.forward * lookOffset.z + Vector3.up * lookOffset.y;
        transform.rotation = Quaternion.LookRotation(player.forward);

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
