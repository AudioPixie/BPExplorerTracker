using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour
{
    public Image CanvasImage;
    public Camera MainCamera;

    private static BackgroundManager instance;

    public static BackgroundManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<BackgroundManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(BackgroundManager).Name);
                    instance = singletonObject.AddComponent<BackgroundManager>();
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

    public void ApplyColorFromHex(string hex)
    {
        // Convert HTML to Color
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
        {
            CanvasImage.color = color;
            MainCamera.backgroundColor = color;
        }
    }
}
