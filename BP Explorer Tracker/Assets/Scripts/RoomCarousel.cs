using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomCarousel : MonoBehaviour
{
    public float scrollSpeed = 40f;
    public float tileWidth = 120f;
    public float padding = 20f;
    public Transform roomGrid;

    private List<RectTransform> tiles = new List<RectTransform>();
    private int roomIndex = 0;
    private RoomItem[] allRooms;
    private Toggle[] allToggles;

    Canvas rootCanvas;

    void Start()
    {
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        allRooms = roomGrid.GetComponentsInChildren<RoomItem>();
        allToggles = roomGrid.GetComponentsInChildren<Toggle>();
        
        SpawnTile();
    }

    float SpawnX => ((RectTransform)rootCanvas.transform).rect.width - 280f;

    void Update()
    {
        RectTransform rightmost = null;
        List<RectTransform> toDestroy = new List<RectTransform>();

        foreach (RectTransform tile in tiles)
        {
            Vector2 pos = tile.anchoredPosition;
            pos.x -= scrollSpeed * Time.deltaTime;
            tile.anchoredPosition = pos;

            if (pos.x + tileWidth < 0)
                toDestroy.Add(tile);
            else if (rightmost == null || pos.x > rightmost.anchoredPosition.x)
                rightmost = tile;
        }

        foreach (RectTransform tile in toDestroy)
        {
            tiles.Remove(tile);
            Destroy(tile.gameObject);
        }

        if (rightmost == null || rightmost.anchoredPosition.x + tileWidth + padding < SpawnX)
            SpawnTile();
    }

    void SpawnTile()
    {
        Sprite sprite = GetNextUndraftedSprite();
        if (sprite == null) return;

        GameObject tile = new GameObject("Tile", typeof(RectTransform), typeof(Image));
        tile.transform.SetParent(transform, false);

        RectTransform rt = tile.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(tileWidth, tileWidth);
        rt.anchoredPosition = new Vector2(SpawnX, 0);

        tile.GetComponent<Image>().sprite = sprite;
        tiles.Add(rt);
    }

    Sprite GetNextUndraftedSprite()
    {
        if (allRooms == null || allRooms.Length == 0) return null;

        int checkedCount = 0;
        while (checkedCount < allRooms.Length)
        {
            roomIndex = roomIndex % allRooms.Length;
            RoomItem item = allRooms[roomIndex];
            Toggle toggle = allToggles[roomIndex];
            roomIndex++;
            checkedCount++;

            if (item != null 
                && toggle != null
                && toggle.isOn
                && item.roomId != 2
                && item.roomId != 45
                && item.roomId != 46)
                return item.GetComponent<Image>().sprite;
        }

        return null;
    }

}