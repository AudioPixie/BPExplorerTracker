using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public Toggle jrToggle;
    public Toggle autoToggle;
    public Toggle gamePassToggle;
    public BPSaveDataReader saveDataReader;
    public TMP_InputField bgColor;
    public TMP_Dropdown saveFileSelect;
    public GameObject RoomGrid;
    public Button RunResetButton;

    public Toggle CounterToggle;
    public Toggle ChessToggle;
    public Toggle Room46Toggle;

    public bool isLoading;

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

        isLoading = true;

        if (PlayerPrefs.HasKey("Save File Path")) //save data file path
        {
            saveDataReader.SetSaveDirectory(PlayerPrefs.GetString("Save File Path"));
        }

        LoadSettings();
        
        isLoading = false;
    }

    private void Start()
    {
        RunResetButton.onClick.Invoke();
    }

    public void SaveSettings()
    {
        if (isLoading == false)
        {
            PlayerPrefs.SetInt("Jr On", jrToggle.isOn ? 1: 0);
            PlayerPrefs.SetInt("Auto On", autoToggle.isOn ? 1: 0);
            PlayerPrefs.SetString("Save File Path", saveDataReader.SaveDirectory);
            PlayerPrefs.SetString("BG Color", bgColor.text);
            PlayerPrefs.SetInt("Save Slot", saveFileSelect.value);
            PlayerPrefs.SetInt("Gamepass On", gamePassToggle.isOn ? 1: 0);
            PlayerPrefs.SetInt("Counter On", CounterToggle.isOn ? 1: 0);
            PlayerPrefs.SetInt("Chess On", ChessToggle.isOn ? 1: 0);
            PlayerPrefs.SetInt("Room46 On", Room46Toggle.isOn ? 1: 0);
        }
    }

    public void LoadSettings()
    {
        jrToggle.isOn = PlayerPrefs.GetInt("Jr On") != 0;
        autoToggle.isOn = PlayerPrefs.GetInt("Auto On") != 0;
        saveDataReader.SetSaveDirectory(PlayerPrefs.GetString("Save File Path"));
        bgColor.text = PlayerPrefs.GetString("BG Color");
        saveFileSelect.value = PlayerPrefs.GetInt("Save Slot");
        saveDataReader.SetSaveSlot(saveFileSelect);
        gamePassToggle.isOn = PlayerPrefs.GetInt("Gamepass On") != 0;
        CounterToggle.isOn = PlayerPrefs.GetInt("Counter On") != 0;
        ChessToggle.isOn = PlayerPrefs.GetInt("Chess On") != 0;
        Room46Toggle.isOn = PlayerPrefs.GetInt("Room46 On") != 0;
    }

    public void SaveManualDefault()
    {
        foreach (Transform child1 in RoomGrid.transform)
        {
            foreach (Transform child2 in child1.transform)
            {
                foreach (Transform child3 in child2.transform)
                {
                    if (child3.name != "ArchivedRoom")
                    {
                        RoomItem roomItem = child3.GetComponent<RoomItem>();
                        Toggle toggle = child3.GetComponent<Toggle>();

                        PlayerPrefs.SetInt("DefaultRoomState_" + roomItem.roomId, toggle.isOn ? 1: 0);

                        if (roomItem.offSprite != null)
                        {
                            PlayerPrefs.SetInt("DefaultRoomAdded_" + roomItem.roomId, roomItem.image.sprite == roomItem.onSprite ? 1: 0);
                        }
                    }
                }
            }
        }
    }

    public void LoadManualDefault()
    {
        foreach (Transform child1 in RoomGrid.transform)
        {
            foreach (Transform child2 in child1.transform)
            {
                foreach (Transform child3 in child2.transform)
                {
                    if (child3.name != "ArchivedRoom")
                    {
                        RoomItem roomItem = child3.GetComponent<RoomItem>();
                        Toggle toggle = child3.GetComponent<Toggle>();

                        toggle.isOn = PlayerPrefs.GetInt("DefaultRoomState_" + roomItem.roomId) != 0;
                        // Debug.Log("Room ID: " + roomItem.roomId + " - Changed toggle to: " + PlayerPrefs.GetInt("DefaultRoomState_" + roomItem.roomId));

                        if (roomItem.offSprite != null)
                        {
                            if (PlayerPrefs.GetInt("DefaultRoomAdded_" + roomItem.roomId) != 0)
                            {
                                roomItem.image.sprite = roomItem.onSprite;
                            }
                            else
                            {
                                roomItem.image.sprite = roomItem.offSprite;
                            }
                        }

                        roomItem.MatchToggle();
                        // Debug.Log("Room ID: " + roomItem.roomId + " - Final state: " + toggle.isOn + ", sprite: " + roomItem.image.sprite.name);
                    }
                }
            }
        }
    }

    public void DeleteManualDefault()
    {
        foreach (Transform child1 in RoomGrid.transform)
        {
            foreach (Transform child2 in child1.transform)
            {
                foreach (Transform child3 in child2.transform)
                {
                    if (child3.name != "ArchivedRoom")
                    {
                        RoomItem roomItem = child3.GetComponent<RoomItem>();
                        PlayerPrefs.DeleteKey("DefaultRoomState_" + roomItem.roomId);
                        if (roomItem.offSprite != null)
                            PlayerPrefs.DeleteKey("DefaultRoomAdded_" + roomItem.roomId);
                    }
                }
            }
        }
    }
}
