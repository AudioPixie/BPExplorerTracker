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
        if (!AutoToggle.isOn)
        {
            foreach (Transform child1 in RoomGrid.transform)
            {
                foreach (Transform child2 in child1.transform)
                {
                    foreach (Transform child3 in child2.transform)
                    {
                        RoomItem roomItem = child3.GetComponent<RoomItem>();
                        Image image = child3.GetComponent<Image>();
                        Toggle toggle = child3.GetComponent<Toggle>();

                        if (roomItem.offSprite != null)
                        {
                            image.sprite = roomItem.offSprite;
                        }
                        
                        if (roomItem.roomId == 1 || roomItem.roomId == 45)
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

            Chess.GetComponent<KeyChess>().currentIndex = 0;
            Chess.GetComponent<KeyChess>().ChangeSprite(0);
        }
    }

    public void ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
