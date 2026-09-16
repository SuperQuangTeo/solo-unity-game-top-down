using UnityEngine;
using UnityEngine.Tilemaps;

public class RoomMarkerTilemap : MonoBehaviour
{
    [Header("Marker Tilemap")]
    [SerializeField] private Tilemap markerTilemap;

    [SerializeField] private TilemapRenderer markerTilemapRenderer;

    [Header("Marker Tiles")]
    [SerializeField] private TileBase floorExitMarkerTile;
    [SerializeField] private TileBase itemRewardMarkerTile;
    [SerializeField] private TileBase smallPickupMarkerTile;

    [Header("Runtime Visual")]
    [SerializeField] private bool hideMarkersAtRuntime = true;

    private void Awake()
    {
        if (hideMarkersAtRuntime && markerTilemapRenderer != null)
        {
            markerTilemapRenderer.enabled = false;
        }
    }

    public bool TryGetFloorExitWorldPosition(out Vector3 floorExitWorldPosition)
    {
        return TryGetMarkerWorldPosition(floorExitMarkerTile, out floorExitWorldPosition);
    }

    public bool TryGetItemRewardWorldPosition(out Vector3 itemRewardWorldPosition)
    {
        return TryGetMarkerWorldPosition(itemRewardMarkerTile, out itemRewardWorldPosition);
    }

    public bool TryGetSmallPickupWorldPosition(out Vector3 smallPickupWorldPosition)
    {
        return TryGetMarkerWorldPosition(smallPickupMarkerTile, out smallPickupWorldPosition);
    }

    private bool TryGetMarkerWorldPosition(TileBase markerTile, out Vector3 worldPosition)
    {
        worldPosition = Vector3.zero;

        if (markerTilemap == null)
        {
            return false;
        }

        if (markerTile == null)
        {
            return false;
        }

        foreach (Vector3Int cellPosition in markerTilemap.cellBounds.allPositionsWithin)
        {
            TileBase currentTile = markerTilemap.GetTile(cellPosition);

            if (currentTile != markerTile)
            {
                continue;
            }

            worldPosition = markerTilemap.GetCellCenterWorld(cellPosition);

            return true;
        }
        return false;
    }
}