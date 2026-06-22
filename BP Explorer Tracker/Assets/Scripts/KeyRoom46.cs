using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class KeyRoom46 : MonoBehaviour
{
    private Toggle toggle;
    private Image image;

    public Toggle Room46Toggle;
    public Toggle AutoToggle;

    void Awake()
    {
        toggle = GetComponent<Toggle>();
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (Room46Toggle.isOn)
        {
            toggle.isOn = false;
            image.color = new Color(1, 1, 1, 0.12f);
        }
        else
        {
            toggle.isOn = true;
            image.color = new Color(1, 1, 1, 1);
        }
    }

    public void Toggle46(bool isOn)
    {
        if (!AutoToggle.isOn)
        {
            if (isOn)
                Room46Toggle.isOn = false;
            else
                Room46Toggle.isOn = true;
        }
    }
}
