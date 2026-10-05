using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PathChanger : MonoBehaviour
{
    public TMP_InputField inputField;
    public BPSaveDataReader saveDataReader;

    public void InputToPath()
    {
        saveDataReader.SetSaveDirectory(inputField.text);
    }

    public void ResetPath()
    {
        saveDataReader.ResetSaveDirectory();
    }

    public void ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
