using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Diagnostics.Tracing;
using UnityEngine.UI;

[System.Serializable]
public class RoomEntry
{
    public int roomId;
    public int globalDrafts;
    public int todayDrafts;
}

[System.Serializable] public class BoolEntry   { public string key; public bool value; }
[System.Serializable] public class IntEntry    { public string key; public int value; }

[System.Serializable]
public class AllEvents
{
    public List<BoolEntry> bools;
    public List<IntEntry> ints;
}

public class JSONImporter : MonoBehaviour
{
    public string jsonLocation;
    public string jsonPathDrafts;
    public string jsonPathEvents;

    public List<RoomEntry> rooms = new List<RoomEntry>();
    public AllEvents events;
    
    public Toggle AutoToggle;
    public GameObject WarningText;

    public GameObject Room46Object;
    public GameObject PlanetariumObject;
    public GameObject ConservatoryObject;
    public GameObject TunnelObject;
    public GameObject MechanariumObject;
    public GameObject ClosedExhibitObject;
    public GameObject LostAndFoundObject;
    public GameObject ThroneRoomObject;
    public GameObject TreasureTroveObject;
    public GameObject DovecoteObject;
    public GameObject TheKennelObject;
    public GameObject ClockTowerObject;
    public GameObject ClassroomObject;
    public GameObject DormitoryObject;
    public GameObject SolariumObject;
    public GameObject CasinoObject;
    public GameObject VestibuleObject;
    public GameObject ChessObject;

    void Awake()
    {
        jsonLocation = "C:/Program Files (x86)/Steam/steamapps/common/Blue Prince";
        jsonPathDrafts = jsonLocation + "/drafts.json";
        jsonPathEvents = jsonLocation + "/events.json";
    }

    void Start()
    {
        LoadDrafts();
        LoadEvents();

        InvokeRepeating("LoadDrafts", 0f, 0.5f);
        InvokeRepeating("LoadEvents", 0f, 0.5f);
    }

    public void LoadDrafts()
    {
        if (!AutoToggle.isOn)
            return;

        if (!File.Exists(jsonPathDrafts))
        {
            //Debug.LogWarning("drafts.json not found");
            WarningText.SetActive(true);
            return;
        }

        if (WarningText.activeSelf) WarningText.SetActive(false);

        string jsonDrafts = File.ReadAllText(jsonPathDrafts);
        
        string wrapped = "{\"rooms\":" + jsonDrafts + "}";
        RoomDataWrapper wrapper = JsonUtility.FromJson<RoomDataWrapper>(wrapped);
        rooms = wrapper.rooms;

        //Debug.Log($"Loaded {rooms.Count} rooms");

        RoomItem[] roomItems = FindObjectsByType<RoomItem>(FindObjectsSortMode.None);
        foreach (RoomItem item in roomItems)
        {
            RoomEntry match = rooms.Find(r => r.roomId == item.roomId);
            if (match != null)
                item.UpdateData(match);
        }
    }

    public void LoadEvents()
    {
        if (!AutoToggle.isOn)
            return;

        if (!File.Exists(jsonPathEvents))
        {
            //Debug.LogWarning("events.json not found");
            WarningText.SetActive(true);
            return;
        }

        if (WarningText.activeSelf) WarningText.SetActive(false);

        string jsonEvents = File.ReadAllText(jsonPathEvents);
        events = JsonUtility.FromJson<AllEvents>(jsonEvents);

        BoolEntry room46 = events.bools.Find(x => x.key == "Room 46 Reached");
        Room46Object.GetComponent<RoomItem>().Update46(room46.value);

        BoolEntry addedPlanetarium = events.bools.Find(x => x.key == "Planetarium Added");
        PlanetariumObject.GetComponent<RoomItem>().UpdateAddedToPool(addedPlanetarium.value);

        BoolEntry addedConservatory = events.bools.Find(x => x.key == "Conservatory Added");
        ConservatoryObject.GetComponent<RoomItem>().UpdateAddedToPool(addedConservatory.value);

        BoolEntry addedTunnel = events.bools.Find(x => x.key == "Tunnel Added");
        TunnelObject.GetComponent<RoomItem>().UpdateAddedToPool(addedTunnel.value);

        BoolEntry addedMechanarium = events.bools.Find(x => x.key == "Mechanarium Added");
        MechanariumObject.GetComponent<RoomItem>().UpdateAddedToPool(addedMechanarium.value);

        BoolEntry addedClosedExhibit = events.bools.Find(x => x.key == "Closed Exhibit Added");
        ClosedExhibitObject.GetComponent<RoomItem>().UpdateAddedToPool(addedClosedExhibit.value);

        BoolEntry addedLostAndFound = events.bools.Find(x => x.key == "Lost&Found Added");
        LostAndFoundObject.GetComponent<RoomItem>().UpdateAddedToPool(addedLostAndFound.value);

        BoolEntry addedThroneRoom = events.bools.Find(x => x.key == "Throne Room Added");
        ThroneRoomObject.GetComponent<RoomItem>().UpdateAddedToPool(addedThroneRoom.value);

        BoolEntry addedTreasureTrove = events.bools.Find(x => x.key == "Treasure Trove Added");
        TreasureTroveObject.GetComponent<RoomItem>().UpdateAddedToPool(addedTreasureTrove.value);

        BoolEntry addedDovecote = events.bools.Find(x => x.key == "Dovecote Added");
        DovecoteObject.GetComponent<RoomItem>().UpdateAddedToPool(addedDovecote.value);

        BoolEntry addedTheKennel = events.bools.Find(x => x.key == "The Kennel Added");
        TheKennelObject.GetComponent<RoomItem>().UpdateAddedToPool(addedTheKennel.value);

        BoolEntry addedClockTower = events.bools.Find(x => x.key == "Clock Tower Added");
        ClockTowerObject.GetComponent<RoomItem>().UpdateAddedToPool(addedClockTower.value);

        BoolEntry addedClassroom = events.bools.Find(x => x.key == "Classroom Added");
        ClassroomObject.GetComponent<RoomItem>().UpdateAddedToPool(addedClassroom.value);

        BoolEntry addedDormitory = events.bools.Find(x => x.key == "Dormitory Added");
        DormitoryObject.GetComponent<RoomItem>().UpdateAddedToPool(addedDormitory.value);

        BoolEntry addedSolarium = events.bools.Find(x => x.key == "Solarium Added");
        SolariumObject.GetComponent<RoomItem>().UpdateAddedToPool(addedSolarium.value);

        BoolEntry addedCasino = events.bools.Find(x => x.key == "Casino Added");
        CasinoObject.GetComponent<RoomItem>().UpdateAddedToPool(addedCasino.value);

        BoolEntry addedVestibule = events.bools.Find(x => x.key == "Vestibule Added");
        VestibuleObject.GetComponent<RoomItem>().UpdateAddedToPool(addedVestibule.value);

        IntEntry currentChess = events.ints.Find(x => x.key == "Chess Power");
        ChessObject.GetComponent<KeyChess>().ChangeSprite(currentChess.value);
    }

    public void UpdatePath(string path)
    {
        jsonLocation = path;
        jsonPathDrafts = jsonLocation + "/drafts.json";
        jsonPathEvents = jsonLocation + "/events.json";

        //Debug.Log(jsonPathDrafts);
        //Debug.Log(jsonPathEvents);
    }
}

[System.Serializable]
public class RoomDataWrapper
{
    public List<RoomEntry> rooms;
}