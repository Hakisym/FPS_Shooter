using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet
{
    public BulletManager bulletManager;
    public float time;
    public Vector3 initialPosition;
    public Vector3 initialVelocity;
    public bool hasTracer;

    private GameObject tracer;
    private float bulletDrop;

    public Bullet(Vector3 _position, Vector3 _velocity, float _bulletDrop, bool _hasTracer = false)
    {
        initialPosition = _position;
        initialVelocity = _velocity;
        time = 0.0f;
        bulletDrop = _bulletDrop;
        hasTracer = _hasTracer;
    }

    public void Initialize(BulletManager manager)
    {
        bulletManager = manager;
        if (hasTracer)
            //bullet trail
            tracer = GameObject.Instantiate(bulletManager.tracer, initialPosition, Quaternion.identity, bulletManager._prefabHolder);
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

    public Vector3 GetPosition()
    {
        // p + v*t + 0.5*g*t*t
        Vector3 gravity = Vector3.down * bulletDrop;
        return (initialPosition) + (initialVelocity * time) + (0.5f * time * time * gravity);
    }

    public void RaycastSegment(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        //Raycast
        RaycastHit hit;
        if (Physics.Raycast(start, direction, out hit, distance, bulletManager.whatIsShootable))
        {
            UpdateTracer(hit.point);
            time = bulletManager.maxLifetime;
        }
        else
        {
            UpdateTracer(end);
        }
    }
}
