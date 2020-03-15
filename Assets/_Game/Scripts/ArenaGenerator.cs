using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArenaGenerator : MonoBehaviour
{
    [SerializeField] ArenaParameters Arena = null;

    [Header("Side Terrain")]
    [SerializeField] Transform SideTerrainPrefab = null;
    [SerializeField, Range(0, 4)] int FillSteps = 3;
    [SerializeField] float SideTerrainOriginalSize = 20f;
    [SerializeField] float SideTerrainScaleY = 0.1f;

    [Header("Items")]
    [SerializeField] Transform[] Items = null;
    [SerializeField] float ItemsPerUnit = 0.01f;
    [SerializeField] float RaycastHeight = 10f;
    [SerializeField] LayerMask RaycastLayerMask = 0;
    [SerializeField] float ItemScale = 0.3f;

    [Header("Bounds")]
    [SerializeField] Transform BoundsPrefab = null;

    [Space]
    [SerializeField] Transform Plane = null;

    [SerializeField, Range(1f, 1.5f)] float PlaneExtent = 1.1f;

    void Start()
    {
        SetupArena();
    }

    void SetupArena()
    {
        SetupGround();
        SetupBounds();
        SetupItems();
    }

    void SetupGround()
    {
        Plane.position = Vector3.zero;
        Plane.localScale = new Vector3(Arena.Size.x, Arena.Size.y, 1) * PlaneExtent;

        float RandomInverse() => Random.value > 0.5f ? -1f : 1f;

        for (int x = -FillSteps; x <= FillSteps; x++)
        {
            for (int y = -FillSteps; y <= FillSteps; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                Vector3 pos = new Vector3(x * Arena.Size.x, 0f, y * Arena.Size.y);
                int rotationIndex = Random.Range(0, 4);
                Quaternion rotation = Quaternion.Euler(0, rotationIndex * 90, 0);
                var terrainObject = Instantiate(SideTerrainPrefab, pos, rotation, transform);

                bool isSameSide = rotationIndex % 2 == 0;
                Vector3 scale = new Vector3(
                    RandomInverse() * (isSameSide ? Arena.Size.x : Arena.Size.y), 
                    Mathf.Min(Arena.Size.x, Arena.Size.y) * SideTerrainScaleY,
                    RandomInverse() * (isSameSide ? Arena.Size.y : Arena.Size.x));
                terrainObject.localScale = scale / SideTerrainOriginalSize;
            }
        }
    }

    void SetupBounds()
    {
        SpawnBound(-1, 0);
        SpawnBound(1, 0);
        SpawnBound(0, -1);
        SpawnBound(0, 1);
    }

    void SpawnBound(int xShift, int yShift)
    {
        Vector3 pos = new Vector3(Arena.Size.x * xShift, 0f, Arena.Size.y * yShift) / 2f;
        Quaternion rotation = Quaternion.Euler(0f, xShift == 0 ? 0f : 90f, 0f);
        var bound = Instantiate(BoundsPrefab, pos, rotation, transform);

        Vector3 scale = new Vector3(xShift == 0 ? Arena.Size.x : Arena.Size.y, 1f, 1f);
        bound.localScale = scale;
    }

    void SetupItems()
    {
        int mapSideSteps = 2 * FillSteps + 1;
        float totalArea = Arena.Size.x * Arena.Size.y * Mathf.Pow(mapSideSteps, 2);
        int totalItems = Mathf.FloorToInt(totalArea * ItemsPerUnit);

        Vector2 halfArenaSize = Arena.Size / 2;
        Vector2 halfMapSize = Arena.Size * mapSideSteps / 2;
        for (int i = 0; i < totalItems; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-halfMapSize.x, halfMapSize.x), 
                0f, 
                Random.Range(-halfMapSize.y, halfMapSize.y));
            if (Arena.IsInside(position))
                continue;
            
            Quaternion rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            var prefab = Items[Random.Range(0, Items.Length)];
            var item = Instantiate(prefab, position, rotation, transform);
            item.localScale = new Vector3(ItemScale, ItemScale, ItemScale);

            if (Physics.Raycast(
                position + new Vector3(0f, RaycastHeight, 0f),
                Vector3.down,
                out RaycastHit hit,
                2 * RaycastHeight,
                RaycastLayerMask))
            {
                
            }
            else
            {
                Debug.LogError($"Can't find terrain at {position}");
            }
        }
    }
}
