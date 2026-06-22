using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlatformManager : MonoBehaviour
{
    public Toggle AutoToggle;
    public Image AutoToggleCheck;
    public TMP_InputField FilePathInputField;
    public TMP_Text FilePathPlaceholder;
    public Button ApplyButton;
    public Button ResetButton;

    private static PlatformManager instance;

    public static PlatformManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<PlatformManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(PlatformManager).Name);
                    instance = singletonObject.AddComponent<PlatformManager>();
                }
            }
            return instance;
        }
    }

    void Awake()
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

        if (IsMac())
        {
            DisableMacOptions();
        }
    }
 
    public bool IsMac()
    {
        #if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return true;
        #else
            return false;
        #endif
    }

    private void DisableMacOptions()
    {
        AutoToggle.isOn = false;
        AutoToggle.interactable = false;
        AutoToggle.GetComponentInChildren<Text>().color = new Color (1, 1, 1, 0.2f);    
        AutoToggleCheck.color = new Color (0, 1, 0, 0.2f);

        FilePathInputField.interactable = false;
        FilePathPlaceholder.color = new Color (1, 1, 1, 0.2f);

        ApplyButton.interactable = false;
        ApplyButton.GetComponentInChildren<TMP_Text>().color = new Color (1, 1, 1, 0.2f);
        ResetButton.interactable = false;
        ResetButton.GetComponentInChildren<TMP_Text>().color = new Color (1, 1, 1, 0.2f);
    }

}
