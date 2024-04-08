using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool
{
    private GameObject prefab;
    private ObjectPool<GameObject> pool;
    private Transform holder;
    private int defaultSize;
    private int maxSize;

    public GameObjectPool(GameObject prefab,Transform holder, int defaultSize = 50, int maxSize = 100)
    {
        this.prefab = prefab;
        this.holder = holder;
        this.defaultSize = defaultSize;
        this.maxSize = maxSize;
        pool = new ObjectPool<GameObject>(
            OnCreatePoolItem,
            OnGetPoolItem,
            OnReleasePoolItem,
            OnDestroyPoolItem,
            true,
            defaultSize,
            maxSize
            );
    }

    public GameObject Get(Vector3 position, Quaternion rotation)
    {
        var obj = pool.Get();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (obj.TryGetComponent<IPoolingObject>(out IPoolingObject pobj))
        {
            pobj.OnObjectSpawn();
        }

        return obj;
    }

    public void Release(GameObject obj) => pool.Release(obj);

    GameObject OnCreatePoolItem() => GameObject.Instantiate(prefab,holder);
    void OnReleasePoolItem(GameObject obj) => obj.SetActive(false);
    void OnGetPoolItem(GameObject obj) => obj.SetActive(true);
    void OnDestroyPoolItem(GameObject obj) => GameObject.Destroy(obj);
}
