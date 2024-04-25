using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public Transform poolHolder { get; set; }
    [SerializeField] List<PoolData> pd;

    private Dictionary<PoolTag, GameObjectPool> pools = new Dictionary<PoolTag, GameObjectPool>();

    [System.Serializable]
    public class PoolData
    {
        public GameObject prefab;
        public int size;
        public int maxSize;
    }

    private void Awake()
    {
        instance = this;
        poolHolder = new GameObject("Pool").transform;
    }

    private void Start()
    {
        foreach (var data in pd)
        {
            var obj = data.prefab;
            var pool = new GameObjectPool(obj, poolHolder, data.size, data.maxSize);
            pools.Add(CheckPool(obj), pool);
        }
    }

    public GameObject SpawnGameObject(PoolTag tag, Vector3 position, Quaternion rotation)
    {
        return pools[tag].Get(position, rotation);
    }

    public void DespawnGameObject(GameObject obj,float time)
    {
        StartCoroutine(DespawnTime(obj,time));
    }

    public void DespawnGameObject(GameObject obj)
    {
        pools[CheckPool(obj)].Release(obj);
    }

    private PoolTag CheckPool(GameObject obj) => obj.GetComponent<IPoolingObject>().Tag;

    private IEnumerator DespawnTime(GameObject obj, float despawnTime)
    {
        yield return new WaitForSeconds(despawnTime);
        pools[CheckPool(obj)].Release(obj);
    }
}

public enum PoolTag
{
    BulletImpact,
    BulletTrail,
    BulletCasting,
}