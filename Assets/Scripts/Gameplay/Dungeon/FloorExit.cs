using System;
using UnityEngine;

public class FloorExit : MonoBehaviour
{
    public event Action OnPlayerEnteredExit;

    private bool hasPlayerEntered;

    private void OnEnable()
    {
        hasPlayerEntered = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasPlayerEntered)
        {
            return;
        }

        if (!other.TryGetComponent<PlayerResources>(out PlayerResources playerResources))
        {
            return;
        }

        hasPlayerEntered = true;

        OnPlayerEnteredExit?.Invoke();

        Debug.Log("Player entered the FloorExit.",this);
    }
}