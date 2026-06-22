using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class PathChanger : MonoBehaviour
{
    public TMP_InputField inputField;
    public JSONImporter jsonImporter;

    public void InputToPath()
    {
        jsonImporter.UpdatePath(inputField.text);
    }

    public void ResetPath()
    {
        jsonImporter.UpdatePath("C:/Program Files (x86)/Steam/steamapps/common/Blue Prince");
    }

    public void ClearSelection()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }
}
