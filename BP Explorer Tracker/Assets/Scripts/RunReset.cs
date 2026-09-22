using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class RunReset : MonoBehaviour
{
    public GameObject RoomGrid;
    public Toggle AutoToggle;
    public GameObject Chess;
    
    public void ResetTracker()
    {
        //Debug.Log("Resetting tracker...");
        if (!AutoToggle.isOn)
        {
            if (PlayerPrefs.HasKey("DefaultRoomState_1"))
            {
                SaveManager.Instance.LoadManualDefault();
            }
            else
            {
                // Debug.Log("No default template found. Resetting to default state.");
                foreach (Transform child1 in RoomGrid.transform)
                {
                    foreach (Transform child2 in child1.transform)
                    {
                        foreach (Transform child3 in child2.transform)
                        {
                            if (child3.name != "ArchivedRoom")
                            {
                                RoomItem roomItem = child3.GetComponent<RoomItem>();
                                Image image = child3.GetComponent<Image>();
                                Toggle toggle = child3.GetComponent<Toggle>();
                                
                                Debug.Log("No default template");
                                if (roomItem.offSprite != null)
                                {
                                    image.sprite = roomItem.offSprite;
                                }
                                
                                if (roomItem.roomId == 2 || roomItem.roomId == 45)
                                {
                                    toggle.isOn = false;
                                    roomItem.MatchToggle();
                                }
                                else
                                {
                                    toggle.isOn = true;
                                    roomItem.MatchToggle();
                                }
                            }
                        }
                    }
                }
            }

            Chess.GetComponent<KeyChess>().currentIndex = 0;
            Chess.GetComponent<KeyChess>().ChangeSprite(0);
        }
    }

    public void ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
