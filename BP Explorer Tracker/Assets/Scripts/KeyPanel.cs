using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeyPanel : MonoBehaviour
{
    public Toggle JrToggle;
    public GameObject RoomGrid;
    public GameObject Counters;
    public GameObject ChessIcon;
    public GameObject Room46Icon;

    public TMP_Text DayCounter;
    public TMP_Text RoomCounterLarge;
    public TMP_Text RoomCounterSmall;

    public Sprite Sprite1;
    public Sprite Sprite2;
    public Sprite Sprite3;

    // Update is called once per frame
    void Update()
    {
        if (Counters.activeSelf)
        {
            int roomTotal = 110;
            if (JrToggle.isOn)
                roomTotal = 94;

            int roomCount = 0;
            foreach (Transform child1 in RoomGrid.transform)
            {
                foreach (Transform child2 in child1.transform)
                {
                    foreach (Transform child3 in child2.transform)
                    {
                        if (child3.gameObject.activeInHierarchy
                            && child3.name != "ArchivedRoom" 
                            && !child3.GetComponentInChildren<Toggle>().isOn)
                        {
                            roomCount++;
                        }
                    }
                }
            }
            RoomCounterLarge.text = "Rooms:\n" + roomCount.ToString() + " / " + roomTotal.ToString();
            RoomCounterSmall.text = "Rooms:\n" + roomCount.ToString() + " / " + roomTotal.ToString();
        }
    }

    public void UpdateKeyPanel()
    {
        int iconCount = 0;
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy)
                iconCount++;
            Debug.Log("Child: " + child.name + " - Active: " + child.gameObject.activeInHierarchy);
        }

        if (iconCount == 1)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(306f, 306f);
            GetComponent<Image>().sprite = Sprite1;
        }
        else if (iconCount == 2)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(587f, 306f);
            GetComponent<Image>().sprite = Sprite2;
        }
        else if (iconCount >= 3)
        {
            GetComponent<RectTransform>().sizeDelta = new Vector2(868f, 306f);
            GetComponent<Image>().sprite = Sprite3;
        }
        else
            GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 306f);
    }
}
