using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class DropPool : MonoBehaviour
{
    [Header("Drop")]
    [SerializeField] private PooledDrop dropPrefab;

    [Header("Pool Settings")]
    [SerializeField] private int defaultCapacity = 10;

    [SerializeField] private int maxSize = 30;

    private ObjectPool<PooledDrop> dropPool;

    private readonly HashSet<PooledDrop> rentedDrops = new HashSet<PooledDrop>();

    private void Awake()
    {
        dropPool = new ObjectPool<PooledDrop>(
            CreateDrop,
            OnGetDrop,
            OnReleaseDrop,
            OnDestroyDrop,
            true,
            defaultCapacity,
            maxSize
        );
    }

    private PooledDrop CreateDrop()
    {
        PooledDrop createdDrop = Instantiate(
            dropPrefab,
            transform
        );

        createdDrop.SetOwnerPool(this);
        createdDrop.gameObject.SetActive(false);

        return createdDrop;
    }

    private void OnGetDrop(PooledDrop pooledDrop)
    {
        if (pooledDrop == null)
        {
            return;
        }

        rentedDrops.Add(pooledDrop);
        pooledDrop.ResetForReuse();
    }

    private void OnReleaseDrop(PooledDrop pooledDrop)
    {
        if (pooledDrop == null)
        {
            return;
        }

        pooledDrop.gameObject.SetActive(false);
        pooledDrop.transform.SetParent(transform);
    }

    private void OnDestroyDrop(PooledDrop pooledDrop)
    {
        if (pooledDrop == null)
        {
            return;
        }

        Destroy(pooledDrop.gameObject);
    }

    public PooledDrop GetDrop(Vector3 spawnPosition)
    {
        PooledDrop pooledDrop = dropPool.Get();
        pooledDrop.transform.position = spawnPosition;

        pooledDrop.gameObject.SetActive(true);
        return pooledDrop;
    }

    public void ReleaseDrop(PooledDrop pooledDrop)
    {
        if (pooledDrop == null)
        {
            return;
        }
        if (!rentedDrops.Remove(pooledDrop))
        {
            return;
        }

        dropPool.Release(pooledDrop);
    }

    public void ReleaseAllRentedDrops()
    {
        List<PooledDrop> dropsToRelease = new List<PooledDrop>(rentedDrops);

        foreach (PooledDrop pooledDrop in dropsToRelease)
        {
            ReleaseDrop(pooledDrop);
        }
    }
}