using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class VersionChecker : MonoBehaviour
{
    private static VersionChecker instance;

    public static VersionChecker Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<VersionChecker>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(VersionChecker).Name);
                    instance = singletonObject.AddComponent<VersionChecker>();
                }
            }
            return instance;
        }
    }

    public string remoteVersionUrl;
    public string localVersion;
    public string docsURL;
    public string latestReleaseURL;

    public Button UpdateResult;

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
        remoteVersionUrl = "https://raw.githubusercontent.com/AudioPixie/BPExplorerTracker/main/version.txt";
        docsURL = "https://github.com/AudioPixie/BPExplorerTracker?tab=readme-ov-file#bpexplorertracker";
        latestReleaseURL = "https://github.com/AudioPixie/BPExplorerTracker/releases";

        localVersion = Application.version;

        UpdateResult.interactable = false;
        
        CheckForUpdate();
    }

    public void CheckForUpdate()
    {
        UpdateResult.interactable = false;
        UpdateResult.GetComponentInChildren<TMP_Text>().text = "<color=#D2B22F>Loading...</color>";

        StartCoroutine(CheckVersionCoroutine(remoteVersionUrl));
    }

    IEnumerator CheckVersionCoroutine(string url)
    {
        // Fetch remote version
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                UpdateResult.interactable = false;
                UpdateResult.GetComponentInChildren<TMP_Text>().text = "<color=#D95841>Connection failed, please try again.</color>";
            }
            else
            {
                string remoteVersion = webRequest.downloadHandler.text.Trim();
                CompareVersions(localVersion, remoteVersion);
            }
        }
    }

    private void CompareVersions(string localVersion, string remoteVersion)
    {
        if (localVersion == remoteVersion)
        {
            UpdateResult.interactable = false;
            UpdateResult.GetComponentInChildren<TMP_Text>().text = "<color=#247FD9>Version is up to date</color>";
        }
        else
        {
            UpdateResult.interactable = true;
            UpdateResult.GetComponentInChildren<TMP_Text>().text = "<color=#D2B22F>Version " + remoteVersion + " available <u>here</u></color>";
        }
    }

    public void OpenDocs()
    {
        Application.OpenURL(docsURL);
    }

    public void OpenLatestVersion()
    {
        Application.OpenURL(latestReleaseURL);
    }
}