using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public Toggle autoToggle;
    public Toggle gamePassToggle;
    public BPSaveDataReader saveDataReader;
    public TMP_InputField bgColor;
    public TMP_Dropdown saveFileSelect;

    private static SaveManager instance;

    public static SaveManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<SaveManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(SaveManager).Name);
                    instance = singletonObject.AddComponent<SaveManager>();
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        // Ensure only one instance exists, even if multiple scripts try to create it.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Moved this to happen on awake so that it can set the save directory before the BPSaveDataReader would try to find the initial path
        if (PlayerPrefs.HasKey("Auto On")) //automatic tracking toggle
            autoToggle.isOn = (PlayerPrefs.GetInt("Auto On") != 0);
        if (PlayerPrefs.HasKey("BG Color")) //background color
            bgColor.text = (PlayerPrefs.GetString("BG Color"));
        if (PlayerPrefs.HasKey("Save File Path")) //save data file path
            saveDataReader.SetSaveDirectory(PlayerPrefs.GetString("File Path"));
        if (PlayerPrefs.HasKey("Save Slot"))
        {
            saveFileSelect.value = PlayerPrefs.GetInt("Save Slot");
            saveDataReader.SetSaveSlot(saveFileSelect);
        }
        if (PlayerPrefs.HasKey("Gamepass On")) //gamepass toggle
            gamePassToggle.isOn = (PlayerPrefs.GetInt("Gamepass On") != 0);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("Auto On", (autoToggle.isOn ? 1: 0));
        PlayerPrefs.SetString("Save File Path", saveDataReader.SaveDirectory);
        PlayerPrefs.SetString("BG Color", bgColor.text);
        PlayerPrefs.SetInt("Save Slot", saveFileSelect.value);
        PlayerPrefs.SetInt("Gamepass On", (gamePassToggle.isOn ? 1: 0));

    }

    public void Load()
    {
        autoToggle.isOn = (PlayerPrefs.GetInt("Auto On") != 0);
        saveDataReader.SetSaveDirectory(PlayerPrefs.GetString("Save File Path"));
        bgColor.text = (PlayerPrefs.GetString("BG Color"));
        saveFileSelect.value = PlayerPrefs.GetInt("Save Slot");
        saveDataReader.SetSaveSlot(saveFileSelect);
        gamePassToggle.isOn = (PlayerPrefs.GetInt("Gamepass On") != 0);

    }
}
