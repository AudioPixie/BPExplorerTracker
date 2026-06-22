using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public Toggle autoToggle;
    public JSONImporter jSONImporter;
    public TMP_InputField bgColor;

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
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("Auto On") && !PlatformManager.Instance.IsMac()) //automatic tracking toggle
            autoToggle.isOn = (PlayerPrefs.GetInt("Auto On") != 0);
        if (PlayerPrefs.HasKey("BG Color")) //background color
            bgColor.text = (PlayerPrefs.GetString("BG Color"));
        if (PlayerPrefs.HasKey("File Path") && !PlatformManager.Instance.IsMac()) //mod file path
            jSONImporter.UpdatePath(PlayerPrefs.GetString("File Path"));
    }

    public void Save()
    {
        if (!PlatformManager.Instance.IsMac())
        {
            PlayerPrefs.SetInt("Auto On", (autoToggle.isOn ? 1: 0));
            PlayerPrefs.SetString("File Path", jSONImporter.jsonLocation);
        }
        PlayerPrefs.SetString("BG Color", bgColor.text);
    }

    public void Load()
    {
        if (!PlatformManager.Instance.IsMac())
        {
            autoToggle.isOn = (PlayerPrefs.GetInt("Auto On") != 0);
            jSONImporter.UpdatePath(PlayerPrefs.GetString("File Path"));
        }
        bgColor.text = (PlayerPrefs.GetString("BG Color"));
    }
}
