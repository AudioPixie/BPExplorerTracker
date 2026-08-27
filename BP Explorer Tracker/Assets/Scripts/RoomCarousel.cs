using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomCarousel : MonoBehaviour
{
    public float scrollSpeed = 40f;
    public float tileWidth = 120f;
    public float padding = 20f;
    public Transform roomGrid;

    public GameObject newDayTile;

    private List<RectTransform> tiles = new List<RectTransform>();
    private int roomIndex = 0;
    private RoomItem[] allRooms;
    private Toggle[] allToggles;

    private bool queuedNewDayTile = false;
    private int dayNumber = 0;

    Canvas rootCanvas;

    void Start()
    {
        rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        allRooms = roomGrid.GetComponentsInChildren<RoomItem>();
        allToggles = roomGrid.GetComponentsInChildren<Toggle>();
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
        GameObject tile;
        if (queuedNewDayTile)
        {
            tile = Instantiate(newDayTile);
            tile.transform.SetParent(transform, false);
            TMP_Text newDayText = tile.GetComponentInChildren<TMP_Text>();
            if (newDayText)
            {
                if (dayNumber == 0)
                {
                    newDayText.text = "Day One";
                }
                else
                {
                    // For whatever reason, the day number in the save data is one less than the day number in game.
                    newDayText.text = "Day " + (dayNumber + 1);
                }
            }
            queuedNewDayTile = false;
        }
        else
        {
            Sprite sprite = GetNextUndraftedSprite();
            if (sprite == null) return;

            tile = new GameObject("Tile", typeof(RectTransform), typeof(Image));
            tile.GetComponent<Image>().sprite = sprite;
        }

        tile.transform.SetParent(transform, false);

        RectTransform rt = tile.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0.5f);
        rt.anchorMax = new Vector2(0, 0.5f);
        rt.pivot = new Vector2(0, 0.5f);
        rt.sizeDelta = new Vector2(tileWidth, tileWidth);
        rt.anchoredPosition = new Vector2(SpawnX, 0);

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

    public void QueueNewDayTile(int day)
    {
        dayNumber = day;
        queuedNewDayTile = true;
    }
}