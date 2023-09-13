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
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Age { get; set; }
        public bool Gender { get; set; }
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Os { get; set; }
        public string AppVersion { get; set; }
        public string OsVer { get; set; }
        public int Status { get; set; }
        public int AvatarId { get; set; }
    }

    [Serializable]
    public class GameData
    {
        [PrimaryKey] public string PlayerId { get; set; }
        public int CharacterSelect { get; set; }
        public int LevelId { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        public int Score { get; set; }
        public int Gold { get; set; }
        public int Diamond { get; set; }
        public int Health { get; set; }
    }

    [Serializable]
    public class PlayerSetting
    {
        [PrimaryKey] public string PlayerId { get; set; }
        public float SoundMusic { get; set; }
        public float SoundEffect { get; set; }
    }
    
    [Serializable]
    public class Item
    {
        [PrimaryKey] [AutoIncrement] public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}