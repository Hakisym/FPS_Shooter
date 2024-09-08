using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    private List<Bullet> bullets = new List<Bullet>();
    public Transform _prefabHolder;
    public float maxLifetime = 3.0f;
    public LayerMask whatIsShootable;
    public GameObject tracer;

    private void FixedUpdate()
    {
        UpdateBullets(Time.fixedDeltaTime);
    }

    private void Update()
    {
        DestroyBullets();
    }

    private void UpdateBullets(float deltaTime)
    {
        SimulateBullets(deltaTime);
        DestroyBullets();
    }

    private void DestroyBullets()
    {
        bullets.RemoveAll(bullet =>
        {
            if (bullet.time >= maxLifetime)
            {
                //Call event on destroy
                bullet.OnDestroy();
                return true;
            }
            return false;
        });
    }

    private void SimulateBullets(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            Vector3 p0 = bullet.GetPosition();
            bullet.time += deltaTime;
            Vector3 p1 = bullet.GetPosition();
            bullet.RaycastSegment(p0, p1);
        });
    }

    public void CreateBullet(Vector3 _position, Vector3 _velocity, float _bulletDrop, bool _hasTracer = false)
    {
        var bullet = new Bullet(_position, _velocity, _bulletDrop, _hasTracer);
        bullet.Initialize(this);
        bullets.Add(bullet);
    }
}
