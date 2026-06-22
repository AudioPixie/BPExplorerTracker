using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class RoomItem : MonoBehaviour, IPointerClickHandler
{
    public Toggle AutoToggle;
    private RoomCarousel carousel;

    private Toggle toggle;
    private Image image;

    public int roomId;
    public int totalDrafts;

    private RoomEntry data;
    
    public Sprite onSprite;
    public Sprite offSprite;

    private int zeroCycleCount = 0;
    private const int zeroCyclesRequired = 3;
    private RoomEntry pendingResetData;

    void Awake()
    {
        //totalDrafts = 0;\
        carousel = GetComponentInParent<RoomCarousel>();
        toggle = GetComponent<Toggle>();
        image = GetComponent<Image>();
        onSprite = GetComponent<Image>().sprite;
        image.color = new Color(1, 1, 1, 1f);
    }

    public void UpdateData(RoomEntry newData)
    {
        bool isReset = newData.globalDrafts == 0 && newData.todayDrafts == 0;
        bool hadData = data != null && (data.globalDrafts > 0 || data.todayDrafts > 0);

        // stops glitching at start of day
        // everything was getting set to 0 for a second during loading (caused flashing)
        // if turning back on, waits for a few cycles before accepting to fix
        if (isReset && hadData)
        {
            zeroCycleCount++;
            pendingResetData = newData;

            if (zeroCycleCount >= zeroCyclesRequired)
            {
                zeroCycleCount = 0;
                pendingResetData = null;
                data = newData;
                OnDataUpdated();
            }
        }
        else
        {
            zeroCycleCount = 0;
            pendingResetData = null;
            data = newData;
            OnDataUpdated();
        }
    }

    private void OnDataUpdated()
    {
        //Debug.Log($"{data.roomId}: {data.globalDrafts} global, {data.todayDrafts} today");
        totalDrafts = data.globalDrafts + data.todayDrafts;
        
        if (totalDrafts > 0
        || roomId == 2
        || roomId == 45
        || roomId == 46)
        {
            image.color = new Color(1, 1, 1, 0.12f);
            toggle.isOn = false;
        }

        else
        {
            image.color = new Color(1, 1, 1, 1f);
            toggle.isOn = true;
        }

    }

    public void Update46(bool has46)
    {
        if (has46)
        {
            image.color = new Color(1, 1, 1, 0.12f);
            toggle.isOn = false;
        }
        else
        {
            image.color = new Color(1, 1, 1, 1f);
            toggle.isOn = true;
        }
    }

    public void MatchToggle()
    {
        if (toggle.isOn)
            image.color = new Color(1, 1, 1, 1f);
        else 
            image.color = new Color(1, 1, 1, 0.12f);;
    }

    public void UpdateAddedToPool(bool isAdded)
    {
        if (isAdded || toggle.isOn == false)
            image.sprite = onSprite;
        else
            image.sprite = offSprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right 
            && offSprite != null
            && AutoToggle != null
            && AutoToggle.isOn == false )
        {
            if (image.sprite == onSprite) 
                image.sprite = offSprite;
            else 
                image.sprite = onSprite;
        }
    }
}
