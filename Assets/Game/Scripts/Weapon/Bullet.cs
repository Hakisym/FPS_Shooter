using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    public float time;
    public Vector3 initialPosition;
    public Vector3 initialVelocity;
    public bool hasTracer;
    private ParticleSystem tracer;

    public Bullet(Vector3 _position, Vector3 _velocity, ParticleSystem _tracer = null, Transform _prefabHolder = null)
    {
        initialPosition = _position;
        initialVelocity = _velocity;
        time = 0.0f;
        hasTracer = _tracer;
        if (hasTracer) AddTracer(_tracer, _position, _prefabHolder);
    }

    public void AddTracer(ParticleSystem _tracer, Vector3 position, Transform holder)
    {
        //bullet trail
        tracer = GameObject.Instantiate(_tracer, position, Quaternion.identity, holder);
    }

    public void UpdateTracer(Vector3 newPos)
    {
        //update tracer position
        if (hasTracer) tracer.transform.position = newPos;
    }

    public void OnDestroy()
    {
        //destroy bullet tracer after certain frame
        //if (hasTracer) GameObject.Destroy(tracer.gameObject, Time.deltaTime * 5f);
    }

    public Vector3 GetPosition(float bulletDrop)
    {
        // p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return (initialPosition) + (initialVelocity * time) + (0.5f * time * time * gravity);
    }
}
