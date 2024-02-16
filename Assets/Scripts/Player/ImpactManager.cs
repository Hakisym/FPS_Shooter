using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    public float mass = 3.0f; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private CharacterController character;

    private void Start()
    {
        character = GetComponent<CharacterController>();
    }

    // Call this function to add an impact force:
    public void AddImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // Reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    private void Update()
    {
        // Apply the impact force:
        if (impact.magnitude > 0.2f)
            character.Move(impact * Time.deltaTime);

        // Consume the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
    }
}
