using UnityEngine;
using UnityEngine.UI;

public class JrManager : MonoBehaviour
{
    public Toggle JrToggle;
    public GameObject StudioAdditions;
    public GameObject FoundFloorplans;
    public GameObject StudioArchived;
    public GameObject FoundArchived;

    public void ToggleJrMode()
    {
        if (JrToggle.isOn)
        {
            StudioAdditions.SetActive(false);
            FoundFloorplans.SetActive(false);
            StudioArchived.SetActive(true);
            FoundArchived.SetActive(true);
        }
        else
        {
            StudioAdditions.SetActive(true);
            FoundFloorplans.SetActive(true);
            StudioArchived.SetActive(false);
            FoundArchived.SetActive(false);
        }
    }
}


