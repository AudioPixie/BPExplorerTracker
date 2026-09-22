using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AutoManager : MonoBehaviour
{
    private Toggle toggle;
    public GameObject RoomGrid;

    public Toggle gamePassToggle;
    public TMP_Dropdown saveFileSelect;
    public Button forceReloadButton;
    public TMP_InputField filePathInput;
    public Button filePathApply;
    public Button filePathReset;
    public Button manualResetRun;
    public Button saveTemplate;
    public Button resetTemplate;

    public GameObject PanelDayText;
    public GameObject PanelRoomsLarge;
    public GameObject PanelRoomsSmall;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        SettingsInteractable(toggle.isOn);
        PanelDisplay(toggle.isOn);
    }

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

    public void SettingsInteractable(bool autoOn)
    {
        gamePassToggle.interactable = autoOn;
        saveFileSelect.interactable = autoOn;
        forceReloadButton.interactable = autoOn;
        filePathInput.interactable = autoOn;
        filePathApply.interactable = autoOn;
        filePathReset.interactable = autoOn;

        manualResetRun.interactable = !autoOn;
        saveTemplate.interactable = !autoOn;
        resetTemplate.interactable = !autoOn;
    }

    public void PanelDisplay(bool autoOn)
    {
        PanelDayText.SetActive(autoOn);
        PanelRoomsSmall.SetActive(autoOn);

        PanelRoomsLarge.SetActive(!autoOn);
    }
}
