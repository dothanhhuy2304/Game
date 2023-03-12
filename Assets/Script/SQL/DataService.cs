using System;
using System.IO;
using SQLite4Unity3d;
using UnityEngine;

public class DataService
{
    //Exits database name. it in streaming assets
    private const string DatabaseName = "F2Ps.db";

    private static DataService _instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitOnLoad()
    {
        _instance = null;
    }

    private static DataService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataService();
            }

            return _instance;
        }
    }

    private SQLiteConnection connection;

    private DataService()
    {
        InitDb();
    }

    public static void CleanConnection()
    {
        _instance.connection.Close();
        _instance = null;
    }

    public static SQLiteConnection GetConnection()
    {
        return Instance.connection;
    }

    private void InitDb()
    {
        Debug.LogWarning("InitDb");
        var filepath = Path.Combine(Application.persistentDataPath, DatabaseName);
        if (!File.Exists(filepath))
        {
            Debug.LogError($"F2Ps copy db: {filepath}");
        }

        connection = new SQLiteConnection(filepath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex);
        connection.CreateTable<PlayerProfileData>();
        connection.CreateTable<GameData>();
        connection.CreateTable<PlayerSetting>();
        connection.CreateTable<Item>();
    }


    [Serializable]
    public class PlayerProfileData
    {

        [PrimaryKey] public string Id { get; set; }
        public string userName { get; set; }
        public DateTime createdDate { get; set; }
        public int age { get; set; }
        public bool gender { get; set; }
        public string deviceId { get; set; }
        public string deviceName { get; set; }
        public string os { get; set; }
        public string appVersion { get; set; }
        public string osVer { get; set; }
        public int status { get; set; }
        public int avatarId { get; set; }
    }

    [Serializable]
    public class GameData
    {
        [PrimaryKey] public string PlayerId { get; set; }
        public int characterSelect { get; set; }
        public int levelId { get; set; }
        public float positionX { get; set; }
        public float positionY { get; set; }
        public float positionZ { get; set; }
        public int score { get; set; }
        public int gold { get; set; }
        public int diamond { get; set; }
        public int health { get; set; }
    }

    [Serializable]
    public class PlayerSetting
    {
        [PrimaryKey] public string PlayerId { get; set; }
        public float soundMusic { get; set; }
        public float soundEffect { get; set; }
    }
    
    [Serializable]
    public class Item
    {
        [PrimaryKey] [AutoIncrement] public int Id { get; set; }
        public string name { get; set; }
        public int value { get; set; }
    }
}