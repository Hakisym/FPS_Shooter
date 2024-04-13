using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    public float time;
    public Vector3 initialPosition;
    public Vector3 initialVelocity;
    public bool hasTracer;
    private TrailRenderer tracer;

    public Bullet(Vector3 position, Vector3 velocity, bool hasTracer, WeaponController wc)
    {
        initialPosition = position;
        initialVelocity = velocity;
        time = 0.0f;
        this.hasTracer = hasTracer;
        if (hasTracer) AddTracer(wc.bulletTrail, position, wc.prefabHolder);
    }

    public void AddTracer(TrailRenderer tr, Vector3 position, Transform holder)
    {
        //bullet trail
        tracer = GameObject.Instantiate(tr, position, Quaternion.identity, holder);
        tracer.AddPosition(position);
    }

    public void UpdateTracer(Vector3 newPos)
    {
        //update tracer position
        if (hasTracer) tracer.transform.position = newPos;
    }

    public void OnDestroy()
    {
        //destroy bullet tracer after certain frame
        if (hasTracer) GameObject.Destroy(tracer.gameObject, Time.deltaTime * 5f);
    }

    public Vector3 GetPosition(float bulletDrop)
    {
        // p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return (initialPosition) + (initialVelocity * time) + (0.5f * time * time * gravity);
    }
}
