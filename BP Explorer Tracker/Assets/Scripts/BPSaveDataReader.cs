using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

public class SaveSlotFields
{
    public Dictionary<string, bool> BoolEvents = new Dictionary<string, bool>();
    public Dictionary<string, int> IntEvents = new Dictionary<string, int>();
}

// This class is based off of Bacowl's work found here: https://github.com/BAC0WL/explorer-tracker-addon
// I'm basically just taking what is there, and making a unity version of it to work natively in here without needing to run the python script in addition.
public class BPSaveDataReader : MonoBehaviour
{
    // Serialized fields
    [SerializeField]
    private JSONImporter jsonImporter;

    [SerializeField]
    private GameObject LoadingText;


    // Const values
    private static readonly string[] DEFAULT_SAVE_DIRS = {
        System.Environment.GetEnvironmentVariable("USERPROFILE") + "/AppData/LocalLow/Dogubomb/BLUE PRINCE/storage",
        "~/Library/Application Support/com.Dogubomb.BluePrince/storage",
        "~/.config/unity3d/Dogubomb/BLUE PRINCE/storage"
    };

    private static readonly string SAVE_FILENAME = "MtHollyBlueprint.es3";

    // Magic decryption words and numbers from Bacowl that I don't fully understand
    private static readonly byte[] sbox =
    {
        0x63, 0x7c, 0x77, 0x7b, 0xf2, 0x6b, 0x6f, 0xc5,
        0x30, 0x01, 0x67, 0x2b, 0xfe, 0xd7, 0xab, 0x76,
        0xca, 0x82, 0xc9, 0x7d, 0xfa, 0x59, 0x47, 0xf0,
        0xad, 0xd4, 0xa2, 0xaf, 0x9c, 0xa4, 0x72, 0xc0,
        0xb7, 0xfd, 0x93, 0x26, 0x36, 0x3f, 0xf7, 0xcc,
        0x34, 0xa5, 0xe5, 0xf1, 0x71, 0xd8, 0x31, 0x15,
        0x04, 0xc7, 0x23, 0xc3, 0x18, 0x96, 0x05, 0x9a,
        0x07, 0x12, 0x80, 0xe2, 0xeb, 0x27, 0xb2, 0x75,
        0x09, 0x83, 0x2c, 0x1a, 0x1b, 0x6e, 0x5a, 0xa0,
        0x52, 0x3b, 0xd6, 0xb3, 0x29, 0xe3, 0x2f, 0x84,
        0x53, 0xd1, 0x00, 0xed, 0x20, 0xfc, 0xb1, 0x5b,
        0x6a, 0xcb, 0xbe, 0x39, 0x4a, 0x4c, 0x58, 0xcf,
        0xd0, 0xef, 0xaa, 0xfb, 0x43, 0x4d, 0x33, 0x85,
        0x45, 0xf9, 0x02, 0x7f, 0x50, 0x3c, 0x9f, 0xa8,
        0x51, 0xa3, 0x40, 0x8f, 0x92, 0x9d, 0x38, 0xf5,
        0xbc, 0xb6, 0xda, 0x21, 0x10, 0xff, 0xf3, 0xd2,
        0xcd, 0x0c, 0x13, 0xec, 0x5f, 0x97, 0x44, 0x17,
        0xc4, 0xa7, 0x7e, 0x3d, 0x64, 0x5d, 0x19, 0x73,
        0x60, 0x81, 0x4f, 0xdc, 0x22, 0x2a, 0x90, 0x88,
        0x46, 0xee, 0xb8, 0x14, 0xde, 0x5e, 0x0b, 0xdb,
        0xe0, 0x32, 0x3a, 0x0a, 0x49, 0x06, 0x24, 0x5c,
        0xc2, 0xd3, 0xac, 0x62, 0x91, 0x95, 0xe4, 0x79,
        0xe7, 0xc8, 0x37, 0x6d, 0x8d, 0xd5, 0x4e, 0xa9,
        0x6c, 0x56, 0xf4, 0xea, 0x65, 0x7a, 0xae, 0x08,
        0xba, 0x78, 0x25, 0x2e, 0x1c, 0xa6, 0xb4, 0xc6,
        0xe8, 0xdd, 0x74, 0x1f, 0x4b, 0xbd, 0x8b, 0x8a,
        0x70, 0x3e, 0xb5, 0x66, 0x48, 0x03, 0xf6, 0x0e,
        0x61, 0x35, 0x57, 0xb9, 0x86, 0xc1, 0x1d, 0x9e,
        0xe1, 0xf8, 0x98, 0x11, 0x69, 0xd9, 0x8e, 0x94,
        0x9b, 0x1e, 0x87, 0xe9, 0xce, 0x55, 0x28, 0xdf,
        0x8c, 0xa1, 0x89, 0x0d, 0xbf, 0xe6, 0x42, 0x68,
        0x41, 0x99, 0x2d, 0x0f, 0xb0, 0x54, 0xbb, 0x16
    };

    private static readonly byte[] invSbox =
    {
        0x52, 0x09, 0x6A, 0xD5, 0x30, 0x36, 0xA5, 0x38,
        0xBF, 0x40, 0xA3, 0x9E, 0x81, 0xF3, 0xD7, 0xFB,
        0x7C, 0xE3, 0x39, 0x82, 0x9B, 0x2F, 0xFF, 0x87,
        0x34, 0x8E, 0x43, 0x44, 0xC4, 0xDE, 0xE9, 0xCB,
        0x54, 0x7B, 0x94, 0x32, 0xA6, 0xC2, 0x23, 0x3D,
        0xEE, 0x4C, 0x95, 0x0B, 0x42, 0xFA, 0xC3, 0x4E,
        0x08, 0x2E, 0xA1, 0x66, 0x28, 0xD9, 0x24, 0xB2,
        0x76, 0x5B, 0xA2, 0x49, 0x6D, 0x8B, 0xD1, 0x25,
        0x72, 0xF8, 0xF6, 0x64, 0x86, 0x68, 0x98, 0x16,
        0xD4, 0xA4, 0x5C, 0xCC, 0x5D, 0x65, 0xB6, 0x92,
        0x6C, 0x70, 0x48, 0x50, 0xFD, 0xED, 0xB9, 0xDA,
        0x5E, 0x15, 0x46, 0x57, 0xA7, 0x8D, 0x9D, 0x84,
        0x90, 0xD8, 0xAB, 0x00, 0x8C, 0xBC, 0xD3, 0x0A,
        0xF7, 0xE4, 0x58, 0x05, 0xB8, 0xB3, 0x45, 0x06,
        0xD0, 0x2C, 0x1E, 0x8F, 0xCA, 0x3F, 0x0F, 0x02,
        0xC1, 0xAF, 0xBD, 0x03, 0x01, 0x13, 0x8A, 0x6B,
        0x3A, 0x91, 0x11, 0x41, 0x4F, 0x67, 0xDC, 0xEA,
        0x97, 0xF2, 0xCF, 0xCE, 0xF0, 0xB4, 0xE6, 0x73,
        0x96, 0xAC, 0x74, 0x22, 0xE7, 0xAD, 0x35, 0x85,
        0xE2, 0xF9, 0x37, 0xE8, 0x1C, 0x75, 0xDF, 0x6E,
        0x47, 0xF1, 0x1A, 0x71, 0x1D, 0x29, 0xC5, 0x89,
        0x6F, 0xB7, 0x62, 0x0E, 0xAA, 0x18, 0xBE, 0x1B,
        0xFC, 0x56, 0x3E, 0x4B, 0xC6, 0xD2, 0x79, 0x20,
        0x9A, 0xDB, 0xC0, 0xFE, 0x78, 0xCD, 0x5A, 0xF4,
        0x1F, 0xDD, 0xA8, 0x33, 0x88, 0x07, 0xC7, 0x31,
        0xB1, 0x12, 0x10, 0x59, 0x27, 0x80, 0xEC, 0x5F,
        0x60, 0x51, 0x7F, 0xA9, 0x19, 0xB5, 0x4A, 0x0D,
        0x2D, 0xE5, 0x7A, 0x9F, 0x93, 0xC9, 0x9C, 0xEF,
        0xA0, 0xE0, 0x3B, 0x4D, 0xAE, 0x2A, 0xF5, 0xB0,
        0xC8, 0xEB, 0xBB, 0x3C, 0x83, 0x53, 0x99, 0x61,
        0x17, 0x2B, 0x04, 0x7E, 0xBA, 0x77, 0xD6, 0x26,
        0xE1, 0x69, 0x14, 0x63, 0x55, 0x21, 0x0C, 0x7D
    };

    // More Bacowl magic numbers
    private static readonly byte[] rcon =
    {
        0x01, 0x02, 0x04, 0x08,
        0x10, 0x20, 0x40, 0x80,
        0x1B, 0x36
    };

    private static readonly byte[] _M9  = CreateMultiplicationTable(0x09);
    private static readonly byte[] _M11 = CreateMultiplicationTable(0x0B);
    private static readonly byte[] _M13 = CreateMultiplicationTable(0x0D);
    private static readonly byte[] _M14 = CreateMultiplicationTable(0x0E);

    private static readonly string DECRYPTION_KEY_STRING = "D#vnrl%TI_9q0euFPIx+wKRuNx%Aja2-AtuH1jtMSk2k%H1jXjUPor08QaeQE=p5l=LAIWaSYms-68SYVS0PPoWxgM1B8?8tirM+UGr=cp!5a3=B5tBsKYEUfqxN!H9DvRVkLW?6cMeZWxgov%OOXmfl9zRiqWPsXq95lEc4yax7hqf5m_i5ssn-OGgLA8LJu2ETibBi7DwLc-zQ4M9jRGIdV_izS_J_=3FA=rAo0HUiEr-HWYVnuK$OQUyaVMchXxf%EBo3A7Z-PXYm$6PPG%fJfWzV7M$L5he#y5cb?kVR67IfGzG$UzBcLhNMDhQFwQSEX59ZG7hP32q?6PgirmvGTd-45+7ZKyG$FrDHoNw7ceUhrxYdzYSHd0yRz0T_RR_R5$GZda%DDfCUPHIaVlIhMq4FEOzo?GL7wyXr9XD7SD_QGpjZh&NDwycjnBeOy2mmFazlOV5eR7jsiwYDde9jCOH&cOxeTody=iUEt|l7JCQ8IyX|0g3H&NO6DMveVqC9|OPkOZpO3DpM|||3LJ7PX40rZJXmLILu0UXU9hpM5";


    // Various consts for reading the save file 
    private Dictionary<string, int> ROOM_NAME_TO_ID = new Dictionary<string, int> {
        { "THE FOUNDATION",           1 },
        { "ENTRANCE HALL",            2 },
        { "SPARE ROOM",               3 },
        { "ROTUNDA",                  4 },
        { "PARLOR",                   5 },
        { "BILLIARD ROOM",            6 },
        { "GALLERY",                  7 },
        { "ROOM 8",                   8 },
        { "CLOSET",                   9 },
        { "WALK-IN CLOSET",           10 },
        { "ATTIC",                    11 },
        { "STOREROOM",                12 },
        { "NOOK",                     13 },
        { "GARAGE",                   14 },
        { "MUSIC ROOM",               15 },
        { "LOCKER ROOM",              16 },
        { "DEN",                      17 },
        { "WINE CELLAR",              18 },
        { "TROPHY ROOM",              19 },
        { "BALLROOM",                 20 },
        { "PANTRY",                   21 },
        { "RUMPUS ROOM",              22 },
        { "VAULT",                    23 },
        { "OFFICE",                   24 },
        { "DRAWING ROOM",             25 },
        { "STUDY",                    26 },
        { "LIBRARY",                  27 },
        { "CHAMBER OF MIRRORS",       28 },
        { "THE POOL",                 29 },
        { "DRAFTING STUDIO",          30 },
        { "UTILITY CLOSET",           31 },
        { "BOILER ROOM",              32 },
        { "PUMP ROOM",                33 },
        { "SECURITY",                 34 },
        { "WORKSHOP",                 35 },
        { "LABORATORY",               36 },
        { "SAUNA",                    37 },
        { "COAT CHECK",               38 },
        { "MAIL ROOM",                39 },
        { "FREEZER",                  40 },
        { "DINING ROOM",              41 },
        { "OBSERVATORY",              42 },
        { "CONFERENCE ROOM",          43 },
        { "AQUARIUM",                 44 },
        { "ANTECHAMBER",              45 },
        { "ROOM 46",                  46 },
        { "BEDROOM",                  47 },
        { "BOUDOIR",                  48 },
        { "GUEST BEDROOM",            49 },
        { "NURSERY",                  50 },
        { "SERVANT'S QUARTERS",       51 },
        { "BUNK ROOM",                52 },
        { "HER LADYSHIP'S CHAMBER",   53 },
        { "MASTER BEDROOM",           54 },
        { "HALLWAY",                  55 },
        { "WEST WING HALL",           56 },
        { "EAST WING HALL",           57 },
        { "CORRIDOR",                 58 },
        { "PASSAGEWAY",               59 },
        { "SECRET PASSAGE",           60 },
        { "FOYER",                    61 },
        { "GREAT HALL",               62 },
        { "TERRACE",                  63 },
        { "PATIO",                    64 },
        { "COURTYARD",                65 },
        { "CLOISTER",                 66 },
        { "VERANDA",                  67 },
        { "GREENHOUSE",               68 },
        { "MORNING ROOM",             69 },
        { "SECRET GARDEN",            70 },
        { "COMMISSARY",               71 },
        { "KITCHEN",                  72 },
        { "LOCKSMITH",                73 },
        { "SHOWROOM",                 74 },
        { "LAUNDRY ROOM",             75 },
        { "BOOKSHOP",                 76 },
        { "THE ARMORY",               77 },
        { "GIFT SHOP",                78 },
        { "LAVATORY",                 79 },
        { "CHAPEL",                   80 },
        { "MAID'S CHAMBER",           81 },
        { "ARCHIVES",                 82 },
        { "GYMNASIUM",                83 },
        { "DARKROOM",                 84 },
        { "WEIGHT ROOM",              85 },
        { "FURNACE",                  86 },
        { "DOVECOTE",                 87 },
        { "THE KENNEL",               88 },
        { "CLOCK TOWER",              89 },
        { "CLASSROOM",                90 },
        { "SOLARIUM",                 91 },
        { "DORMITORY",                92 },
        { "VESTIBULE",                93 },
        { "CASINO",                   94 },
        { "PLANETARIUM",              95 },
        { "MECHANARIUM",              96 },
        { "TREASURE TROVE",           97 },
        { "THRONE ROOM",              98 },
        { "TUNNEL",                   101 },
        { "CONSERVATORY",             100 },
        { "LOST & FOUND",             99 },
        { "CLOSED EXHIBIT",           102 },
        { "TOOLSHED",                 103 },
        { "BOMB SHELTER",             104 },
        { "SCHOOLHOUSE",              105 },
        { "SHRINE",                   106 },
        { "ROOT CELLAR",              107 },
        { "HOVEL",                    108 },
        { "TRADING POST",             109 },
        { "TOMB",                     110 },
        { "Slot Zero",                111 },
    };

    private List<string> EVENTS_BOOLS = new List<string> {
        "Planetarium Added",
        "Conservatory Added",
        "Tunnel Added",
        "Mechanarium Added",
        "Closed Exhibit Added",
        "Lost&Found Added",
        "Throne Room Added",
        "Treasure Trove Added",
        "Dovecote Added",
        "The Kennel Added",
        "Clock Tower Added",
        "Classroom Added",
        "Dormitory Added",
        "Solarium Added",
        "Casino Added",
        "Vestibule Added",
        "Room 46 Reached",
        "Trophy Explorers",
    };

    private List<string> EVENTS_INTS = new List<string> {
        "Chess Power",
    };

    private List<string> SAVE_SLOTS = new List<string> {
        "BluePrint",
        "BluePrint2",
        "BluePrint3",
        "BluePrint4",
    };

    Regex FIELD_RE = new Regex(
        @"""([^""]+)"":\{\s*""__type""\s*:\s*""([^""]+)""(.*?)\s*\}",
        RegexOptions.Singleline
    );

    Regex SLOT_RE = new Regex(
        @"""(BluePrint\d*)""\s*:.*?""objs""\s*:\s*\{(.*?)\},\s*""arrays""",
        RegexOptions.Singleline
    );

    Regex ARRAY_BLOCK_RE = new Regex(
        @"""([^""]+)""\s*:\[(.*?)\]",
        RegexOptions.Singleline
    );

    private Regex ARRAY_VALUE_RE = new Regex(
        @"""__type""\s*:\s*""([^""]+)""(.*?)\s*\}",
        RegexOptions.Singleline
    );

    private string saveDirectory = "";
    private string savePath = "";
    private string savePlainText = "";
    private string saveSlotToLoad = "BluePrint";

    private AllEvents events = new AllEvents();
    private List<RoomEntry> roomRecords = new List<RoomEntry>();
    private Thread saveProcessingThread;
    private bool shouldReloadData = false;

    FileSystemWatcher saveFileWatcher;

    void Start()
    {
        string initialSaveDirectory = GetSaveDirectory();
        SetSaveDirectory(initialSaveDirectory);
        CreateSaveFileWatcher();

        // Do first initial loading of save file
        StartProcessSaveThread();
    }

    void Update()
    {
        // Check if the save file loading thread is done and we should update the UI.
        if (shouldReloadData)
        {
            LoadData();
            shouldReloadData = false;
        }
    }

    void OnDestroy()
    {
        if (saveFileWatcher != null)
        {
            saveFileWatcher.Changed -= OnSaveFileUpdated;
            saveFileWatcher.Created -= OnSaveFileUpdated;
            saveFileWatcher.Deleted -= OnSaveFileUpdated;
            saveFileWatcher.Dispose();
        }
    }

    // Should probably do something to avoid duplicating all the stuff in jsonImporter, but I don't want to make too many changes to jsonImporter right now.
    public void LoadData()
    {
        RoomItem[] roomItems = FindObjectsByType<RoomItem>(FindObjectsSortMode.None);
        foreach (RoomItem item in roomItems)
        {
            RoomEntry match = roomRecords.Find(r => r.roomId == item.roomId);
            if (match != null)
                item.ForceUpdateData(match);
        }

        if (LoadingText)
        {
            LoadingText.SetActive(false);
        }

        if (events.bools == null) { return; }

        BoolEntry room46 = events.bools.Find(x => x.key == "Room 46 Reached");
        jsonImporter.Room46Object.GetComponent<RoomItem>().Update46(room46.value);

        BoolEntry addedPlanetarium = events.bools.Find(x => x.key == "Planetarium Added");
        jsonImporter.PlanetariumObject.GetComponent<RoomItem>().UpdateAddedToPool(addedPlanetarium.value);

        BoolEntry addedConservatory = events.bools.Find(x => x.key == "Conservatory Added");
        jsonImporter.ConservatoryObject.GetComponent<RoomItem>().UpdateAddedToPool(addedConservatory.value);

        BoolEntry addedTunnel = events.bools.Find(x => x.key == "Tunnel Added");
        jsonImporter.TunnelObject.GetComponent<RoomItem>().UpdateAddedToPool(addedTunnel.value);

        BoolEntry addedMechanarium = events.bools.Find(x => x.key == "Mechanarium Added");
        jsonImporter.MechanariumObject.GetComponent<RoomItem>().UpdateAddedToPool(addedMechanarium.value);

        BoolEntry addedClosedExhibit = events.bools.Find(x => x.key == "Closed Exhibit Added");
        jsonImporter.ClosedExhibitObject.GetComponent<RoomItem>().UpdateAddedToPool(addedClosedExhibit.value);

        BoolEntry addedLostAndFound = events.bools.Find(x => x.key == "Lost&Found Added");
        jsonImporter.LostAndFoundObject.GetComponent<RoomItem>().UpdateAddedToPool(addedLostAndFound.value);

        BoolEntry addedThroneRoom = events.bools.Find(x => x.key == "Throne Room Added");
        jsonImporter.ThroneRoomObject.GetComponent<RoomItem>().UpdateAddedToPool(addedThroneRoom.value);

        BoolEntry addedTreasureTrove = events.bools.Find(x => x.key == "Treasure Trove Added");
        jsonImporter.TreasureTroveObject.GetComponent<RoomItem>().UpdateAddedToPool(addedTreasureTrove.value);

        BoolEntry addedDovecote = events.bools.Find(x => x.key == "Dovecote Added");
        jsonImporter.DovecoteObject.GetComponent<RoomItem>().UpdateAddedToPool(addedDovecote.value);

        BoolEntry addedTheKennel = events.bools.Find(x => x.key == "The Kennel Added");
        jsonImporter.TheKennelObject.GetComponent<RoomItem>().UpdateAddedToPool(addedTheKennel.value);

        BoolEntry addedClockTower = events.bools.Find(x => x.key == "Clock Tower Added");
        jsonImporter.ClockTowerObject.GetComponent<RoomItem>().UpdateAddedToPool(addedClockTower.value);

        BoolEntry addedClassroom = events.bools.Find(x => x.key == "Classroom Added");
        jsonImporter.ClassroomObject.GetComponent<RoomItem>().UpdateAddedToPool(addedClassroom.value);

        BoolEntry addedDormitory = events.bools.Find(x => x.key == "Dormitory Added");
        jsonImporter.DormitoryObject.GetComponent<RoomItem>().UpdateAddedToPool(addedDormitory.value);

        BoolEntry addedSolarium = events.bools.Find(x => x.key == "Solarium Added");
        jsonImporter.SolariumObject.GetComponent<RoomItem>().UpdateAddedToPool(addedSolarium.value);

        BoolEntry addedCasino = events.bools.Find(x => x.key == "Casino Added");
        jsonImporter.CasinoObject.GetComponent<RoomItem>().UpdateAddedToPool(addedCasino.value);

        BoolEntry addedVestibule = events.bools.Find(x => x.key == "Vestibule Added");
        jsonImporter.VestibuleObject.GetComponent<RoomItem>().UpdateAddedToPool(addedVestibule.value);

        IntEntry currentChess = events.ints.Find(x => x.key == "Chess Power");
        jsonImporter.ChessObject.GetComponent<KeyChess>().ChangeSprite(currentChess.value);
    }

    public void SetSaveSlot(TMP_Dropdown saveSlot)
    {
        saveSlotToLoad = SAVE_SLOTS[saveSlot.value];
        Debug.Log("Save slot set to " + saveSlotToLoad);
        ForceRestartProcessSave();
    }

    private string GetSaveDirectory()
    {
        // TODO: Allow users to enter their own save data directory.
        foreach(string saveDirectory in DEFAULT_SAVE_DIRS)
        {
            FileAttributes attributes = File.GetAttributes(saveDirectory);
            if (attributes > 0)
            {
                return saveDirectory;
            }
        }
        return null;
    }

    private void SetSaveDirectory(string newSaveDirectory)
    {
        if (String.IsNullOrEmpty(newSaveDirectory))
        {
            Debug.Log("No save directory found");
            return;
        }
        saveDirectory = newSaveDirectory;
        savePath = newSaveDirectory + "/" + SAVE_FILENAME;
        if (!File.Exists(savePath))
        {
            Debug.Log("No save found");
            return;
        }

        if (saveFileWatcher != null)
        {
            saveFileWatcher.Path = savePath;
        }
    }

    // Want to explain why I'm using threading stuff here:
    // The actual process of loading the save file, decrypting it, and then reading it takes a bit (a few seconds at most usually).
    // Because Unity runs things on a single thread, this would mean that whenever we reloaded the save file, the tracker would lag the heck out for a second.
    // By doing all the save data reading on a thread, this means that the tracker can run on it's own smoothly and just update the UI once the thread has finished processing the save file.
    // There might be a cleaner/safer way to do this, but this is what I came up with! Open to suggestions though.
    private void StartProcessSaveThread()
    {
        Debug.Log("Started processing save");

        if (saveProcessingThread != null && saveProcessingThread.IsAlive)
        {
            return;
        }

        saveProcessingThread = new Thread(ProcessSave);
        saveProcessingThread.Start();
    }

    // When we pick a new save slot, we want to stop loading any old save data and start loading new data immediately.
    // Threads makes doing that a bit weird, so this is to try and immediately stop whatever save loading we are doing and start loading the new save after that.
    // This is a bit finnicky and causes some weird stuff to happen occasionally when switching slots, so I'm gonna try and find a better solution later.
    private void ForceRestartProcessSave()
    {
        if (saveProcessingThread != null && saveProcessingThread.IsAlive)
        {
            saveProcessingThread.Abort();
        }
        StartProcessSaveThread();
    }

    // Instead of repeatedly checking the save file, we can just set up a watcher to look at the save file and see when it's updated - then re-process the save file then.
    private void CreateSaveFileWatcher()
    {
        if (saveFileWatcher == null)
        {
            saveFileWatcher = new FileSystemWatcher();
        }
        saveFileWatcher.Path = saveDirectory;
        saveFileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
        saveFileWatcher.Filter = SAVE_FILENAME;
        saveFileWatcher.Changed += OnSaveFileUpdated;
        saveFileWatcher.Created += OnSaveFileUpdated;
        saveFileWatcher.Deleted += OnSaveFileUpdated;
        saveFileWatcher.EnableRaisingEvents = true;
    }

    private void OnSaveFileUpdated(object sender, FileSystemEventArgs e)
    {
        StartProcessSaveThread();
    }

    // Should avoid calling this on a main thread anywhere when possible, will lag out the tracker for a few seconds.
    private void ProcessSave()
    {
        // Small delay before we process the save, in case this was triggered by an update to the save file - let the game finish writing to it.
        Thread.Sleep(300);

        // Create a file stream to read the save file - make sure to specify the FileShare ReadWrite, 
        // so that we don't prevent the game from being able to read and write to it as well
        FileStream saveFile = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
        try
        {
            // put all the save data into a raw byte array.\
            byte[] raw = new byte[saveFile.Length];
            int bytesToRead = (int)saveFile.Length;
            int bytesRead = 0;
            while (bytesToRead > 0)
            {
                int bytesProcessed = saveFile.Read(raw, bytesRead, bytesToRead);
                if (bytesProcessed == 0)
                {
                    break;
                }
                bytesRead += bytesProcessed;
                bytesToRead -= bytesProcessed;
            }

            // This is where the decrypting starts.
            // I'll be honest, I don't understand a lot of this and basically copy-pasted what Bacowl had done, just converting the python code to C#.
            // I'll try my best to explain what bits I do understand though.

            // take the raw bytes - the first 16 bytes are the 'salt', and the rest is the cipher text that we need to decrypt.
            byte[] salt = new byte[16];
            Array.Copy(raw, 0, salt, 0, 16);
            byte[] ciphertext = new byte[saveFile.Length - 16];
            Array.Copy(raw, 16, ciphertext, 0, saveFile.Length - 16);

            // taking the salt and and turning it into a key
            byte[] key = DeriveKey(salt);
            byte[] rk = AesExpandKey(key);

            byte[] outBuffer = new byte[ciphertext.Length];

            // Previous ciphertext block starts as the salt.
            byte[] prev = new byte[16];
            Array.Copy(salt, prev, 16);

            // Go through the ciphertext 16 bytes at a time, decrypting it.
            for (int i = 0; i < ciphertext.Length; i += 16)
            {
                byte[] block = new byte[16];
                Array.Copy(ciphertext, i, block, 0, 16);

                byte[] decryptedBlock = AesDecryptBlock(block, rk);

                for (int j = 0; j < 16; j++)
                {
                    outBuffer[i + j] = (byte)(decryptedBlock[j] ^ prev[j]);
                }

                prev = block;
            }

            // Figure out how much padding is at the end
            int padLen = outBuffer[outBuffer.Length - 1];
            if (padLen < 1 || padLen > 16)
            {
                Debug.LogError("Invalid padding");
                return;
            }
            for (int i = outBuffer.Length - padLen; i < outBuffer.Length; i++)
            {
                if (outBuffer[i] != padLen)
                {
                    Debug.LogError("Invalid padding");
                    return;
                }
            }

            // Remove padding and decode UTF-8.
            int plaintextLength = outBuffer.Length - padLen;

            // This is now the whole save file written as plain text - if you want to see what that looks like, uncomment the line below it to have it write the save text to a file
            savePlainText = Encoding.UTF8.GetString(outBuffer, 0, plaintextLength);
            //LogSavefile(savePlainText);

            // From the save data, get the specific events we are looking for and the room draft counts - for only the save slot we care about.
            events = ParseSlotFields(savePlainText, saveSlotToLoad);
            roomRecords = ParseRoomRecords(savePlainText, saveSlotToLoad);

            // Setting this bool lets the main Unity thread know that we are done loading the save data, and can reload the tracker again.
            shouldReloadData = true;
        }
        finally
        {
            saveFile.Dispose();
        }
    }

    ///
    // Starting here is decrypting-related functions that I don't fully understand, basically just converted what Bacowl was doing to C#
    ///

    private byte[] DeriveKey(byte[] salt)
    {
        byte[] password = Encoding.UTF8.GetBytes(DECRYPTION_KEY_STRING);

        using var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100,
            HashAlgorithmName.SHA1
        );

        return pbkdf2.GetBytes(16);
    }

    private byte[] AesExpandKey(byte[] key)
    {
        byte[,] w = new byte[44, 4];

        for (int i = 0; i < 16; i++)
        {
            w[i / 4, i % 4] = key[i];
        }

        for (int i = 4; i < 44; i++)
        {
            byte[] t =
            {
                w[i - 1, 0],
                w[i - 1, 1],
                w[i - 1, 2],
                w[i - 1, 3]
            };

            if (i % 4 == 0)
            {
                t = new byte[]
                {
                    (byte)(sbox[t[1]] ^ rcon[(i / 4) - 1]),
                    sbox[t[2]],
                    sbox[t[3]],
                    sbox[t[0]]
                };
            }

            for (int j = 0; j < 4; j++)
            {
                w[i, j] = (byte)(w[i - 4, j] ^ t[j]);
            }
        }

        byte[] flat = new byte[176];

        for (int i = 0; i < 44; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                flat[i * 4 + j] = w[i, j];
            }
        }

        return flat;
    }

    private byte[] AesDecryptBlock(byte[] block, byte[] rk)
    {
        if (block == null || block.Length != 16)
            throw new ArgumentException("AES block must be exactly 16 bytes.", nameof(block));

        if (rk == null || rk.Length != 176)
            throw new ArgumentException("AES-128 expanded key must be 176 bytes.", nameof(rk));

        byte[] s = new byte[16];

        for (int i = 0; i < 16; i++)
        {
            s[i] = (byte)(block[i] ^ rk[160 + i]);
        }

        for (int rnd = 9; rnd >= 1; rnd--)
        {
            byte s0  = s[0];
            byte s4  = s[4];
            byte s8  = s[8];
            byte s12 = s[12];

            byte s1  = s[1];
            byte s5  = s[5];
            byte s9  = s[9];
            byte s13 = s[13];

            byte s2  = s[2];
            byte s6  = s[6];
            byte s10 = s[10];
            byte s14 = s[14];

            byte s3  = s[3];
            byte s7  = s[7];
            byte s11 = s[11];
            byte s15 = s[15];

            s = new byte[]
            {
                s0,  s13, s10, s7,
                s4,  s1,  s14, s11,
                s8,  s5,  s2,  s15,
                s12, s9,  s6,  s3
            };

            for (int i = 0; i < 16; i++)
            {
                s[i] = invSbox[s[i]];
            }

            int baseIndex = rnd * 16;

            for (int i = 0; i < 16; i++)
            {
                s[i] = (byte)(s[i] ^ rk[baseIndex + i]);
            }

            byte[] ns = new byte[16];

            for (int c = 0; c < 4; c++)
            {
                int i = c * 4;

                byte a0 = s[i];
                byte a1 = s[i + 1];
                byte a2 = s[i + 2];
                byte a3 = s[i + 3];

                ns[i] =
                    (byte)(_M14[a0] ^
                        _M11[a1] ^
                        _M13[a2] ^
                        _M9[a3]);

                ns[i + 1] =
                    (byte)(_M9[a0] ^
                        _M14[a1] ^
                        _M11[a2] ^
                        _M13[a3]);

                ns[i + 2] =
                    (byte)(_M13[a0] ^
                        _M9[a1] ^
                        _M14[a2] ^
                        _M11[a3]);

                ns[i + 3] =
                    (byte)(_M11[a0] ^
                        _M13[a1] ^
                        _M9[a2] ^
                        _M14[a3]);
            }

            s = ns;
        }

        byte finalS0  = s[0];
        byte finalS4  = s[4];
        byte finalS8  = s[8];
        byte finalS12 = s[12];

        byte finalS1  = s[1];
        byte finalS5  = s[5];
        byte finalS9  = s[9];
        byte finalS13 = s[13];

        byte finalS2  = s[2];
        byte finalS6  = s[6];
        byte finalS10 = s[10];
        byte finalS14 = s[14];

        byte finalS3  = s[3];
        byte finalS7  = s[7];
        byte finalS11 = s[11];
        byte finalS15 = s[15];

        s = new byte[]
        {
            finalS0,  finalS13, finalS10, finalS7,
            finalS4,  finalS1,  finalS14, finalS11,
            finalS8,  finalS5,  finalS2,  finalS15,
            finalS12, finalS9,  finalS6,  finalS3
        };

        for (int i = 0; i < 16; i++)
        {
            s[i] = invSbox[s[i]];
        }

        byte[] result = new byte[16];

        for (int i = 0; i < 16; i++)
        {
            result[i] = (byte)(s[i] ^ rk[i]);
        }

        return result;
    }


    private static byte GfMultiply(byte a, byte b)
    {
        byte result = 0;

        for (int i = 0; i < 8; i++)
        {
            if ((b & 1) != 0)
                result ^= a;

            bool highBit = (a & 0x80) != 0;
            a <<= 1;

            if (highBit)
                a ^= 0x1B;

            b >>= 1;
        }

        return result;
    }

    private static byte[] CreateMultiplicationTable(byte multiplier)
    {
        byte[] table = new byte[256];

        for (int i = 0; i < 256; i++)
        {
            table[i] = GfMultiply((byte)i, multiplier);
        }

        return table;
    }


    // This is reading through the save file and finding all the events we care about.
    // It uses Regex to find all the fields we need.
    private AllEvents ParseSlotFields(string savePlainText, string saveSlot)
    {
        AllEvents Results = new AllEvents();

        MatchCollection slotMatches = SLOT_RE.Matches(savePlainText);
        foreach(Match slotMatch in slotMatches)
        {
            string slotKey = slotMatch.Groups[1].Value;
            if (slotKey != saveSlot)
            {
                continue;
            }

            AllEvents slotFields = new AllEvents();
            slotFields.bools = new List<BoolEntry>();
            slotFields.ints = new List<IntEntry>();
            MatchCollection fieldMatches = FIELD_RE.Matches(slotMatch.Groups[2].Value);
            foreach(Match fieldMatch in fieldMatches)
            {
                string key = fieldMatch.Groups[1].Value;
                string type = fieldMatch.Groups[2].Value ;
                string value = fieldMatch.Groups[3].Value.Trim();

                if (EVENTS_BOOLS.Contains(key))
                {
                    BoolEntry boolEntry = new BoolEntry();
                    boolEntry.key = key;
                    boolEntry.value = bool.Parse(value);
                    slotFields.bools.Add(boolEntry);
                    //Debug.Log(slotKey + ": bool \"" + key + "\" is " + value);
                }
                else if (EVENTS_INTS.Contains(key))
                {
                    IntEntry intEntry = new IntEntry();
                    intEntry.key = key;
                    intEntry.value = Int32.Parse(value);
                    slotFields.ints.Add(intEntry);
                    //Debug.Log(slotKey + ": int \"" + key + "\" is " + value);
                }
            }

            Results = slotFields;
            break;
        }

        return Results;
    }

    // This is reading through the save file and getting the amount of times each room was drafted.
    // It uses Regex to find all the fields we need.
    private List<RoomEntry> ParseRoomRecords(string savePlainText, string saveSlot)
    {
        List<RoomEntry> Results = new List<RoomEntry>();

        string slotPattern = Regex.Escape($"\"{saveSlot}\"") + @".*?""arrays""\s*:\s*\{(.*?)\},\s*""obj""";

        List<RoomEntry> saveSlotRooms = new List<RoomEntry>();

        Match match = Regex.Match(savePlainText, slotPattern, RegexOptions.Singleline);

        if (match.Success)
        {
            var arrays = new Dictionary<string, List<(string Type, string Value)>>();

            foreach (Match arrayMatch in ARRAY_BLOCK_RE.Matches(match.Groups[1].Value))
            {
                string name = arrayMatch.Groups[1].Value;
                var values = new List<(string Type, string Value)>();

                foreach (Match vm in ARRAY_VALUE_RE.Matches(arrayMatch.Groups[2].Value))
                {
                    string type = vm.Groups[1].Value;
                    string value = vm.Groups[2].Value.Trim().Trim('"');

                    values.Add((type, value));
                    //Debug.Log(name + ", " + type + ", " + value);
                }

                arrays[name] = values;
            }

            arrays.TryGetValue("RoomRecords Keys", out var keysArr);
            arrays.TryGetValue("RoomRecords Values", out var valuesArr);

            keysArr ??= new List<(string Type, string Value)>();
            valuesArr ??= new List<(string Type, string Value)>();

            int count = Math.Min(keysArr.Count, valuesArr.Count);

            for (int i = 0; i < count; i++)
            {
                string roomName = keysArr[i].Value;
                string countStr = valuesArr[i].Value;

                if (ROOM_NAME_TO_ID.ContainsKey(roomName))
                {
                    RoomEntry room = new RoomEntry();
                    room.roomId = ROOM_NAME_TO_ID[roomName];
                    if (Int32.TryParse(countStr, out int roomCount))
                    {
                        room.globalDrafts = roomCount;
                    }
                    //Debug.Log(saveSlot + " room " + roomName + ": Drafted " + room.globalDrafts + " times.");
                    saveSlotRooms.Add(room);
                }
            }
        }
        else
        {
            Debug.Log("No match for " + saveSlot);

            // If no save slot found, create a list of save slots that are empty
            foreach(KeyValuePair<string, int> roomEntry in ROOM_NAME_TO_ID)
            {
                RoomEntry room = new RoomEntry();
                room.roomId = roomEntry.Value;
                saveSlotRooms.Add(room);
            }
        }

        Results = saveSlotRooms;

        return Results;
    }

    // Utility function to print the save file into the Unity project, putting it in a .txt file in the Assets folder.
    private void LogSavefile(string savePlainText)
    {
        string docPath = Application.dataPath;

        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, "BPSave.txt")))
        {
            outputFile.Write(savePlainText);
        }
    }
}
