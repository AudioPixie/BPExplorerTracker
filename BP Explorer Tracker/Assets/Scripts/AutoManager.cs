using UnityEngine;
using UnityEngine.UI;

public class AutoManager : MonoBehaviour
{
    public GameObject RoomGrid;

    public void RoomButtonsInteractable(bool autoOn)
    {
        foreach (Transform child1 in RoomGrid.transform)
        {
            foreach (Transform child2 in child1.transform)
            {
                foreach (Transform child3 in child2.transform)
                {
                    Toggle toggle = child3.GetComponentInChildren<Toggle>();
                    toggle.interactable = !autoOn;
                }
            }
        }
    }
}
