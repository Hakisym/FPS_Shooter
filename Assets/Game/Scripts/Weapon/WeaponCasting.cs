using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCasting : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Force X")]
    public float minimumXForce;
    public float maximumXForce;
    [Header("Force Y")]
    public float minimumYForce;
    public float maximumYForce;
    [Header("Force Z")]
    public float minimumZForce;
    public float maximumZForce;
    [Header("Rotation Force")]
    public float minimumRotation;
    public float maximumRotation;
    [Header("Despawn Time")]
    public float despawnTime;

    [Header("Audio")]
    public AudioClip[] castingSounds;
    public AudioSource audioSource;

    [Header("Spin Settings")]
    [Tooltip("How fast the casting spins over time")]
    public float speed = 2500.0f;

    //Launch the casing at start
    public void Initialize(Vector3 velocity)
    {
        rb.velocity = velocity;

        //Random rotation of the casing
        rb.AddRelativeTorque(
            Random.Range(minimumRotation, maximumRotation), //X Axis
            Random.Range(minimumRotation, maximumRotation), //Y Axis
            Random.Range(minimumRotation, maximumRotation)  //Z Axis
            * Time.deltaTime);

        //Random direction the casing will be ejected in
        rb.AddRelativeForce(
            Random.Range(minimumXForce, maximumXForce), //X Axis
            Random.Range(minimumYForce, maximumYForce),  //Y Axis
            Random.Range(minimumZForce, maximumZForce)); //Z Axis
    }

    private void Start()
    {
        //Destroy the casing after set amount of seconds
        Destroy(gameObject, despawnTime);
        //Set random rotation at start
        transform.rotation = Random.rotation;
        //Wait for random time before playing sound clip
        float randomTime = Random.Range(0.25f, 0.85f);
        Invoke(nameof(PlaySound), randomTime);
    }

    private void FixedUpdate()
    {
        //Spin the casing based on speed value
        transform.Rotate(Vector3.right, speed * Time.deltaTime);
        transform.Rotate(Vector3.down, speed * Time.deltaTime);
    }

    //Play the random casing sound
    private void PlaySound()
    {
        audioSource.clip = castingSounds
            [Random.Range(0, castingSounds.Length)];
        audioSource.Play();
    }
}
