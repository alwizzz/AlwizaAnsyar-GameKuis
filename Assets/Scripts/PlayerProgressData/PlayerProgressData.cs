using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerProgressData",
    menuName = "QuizGame/PlayerProgressData"
)]
public class PlayerProgressData : ScriptableObject
{
    [System.Serializable] 
    public struct ProgressData
    {
        public int coins;
        public Dictionary<string, int> progresses;
    }

    public ProgressData data = new ProgressData();
    [SerializeField] private string fileName;
    [SerializeField] private bool usingBinaryFormatter;

#if UNITY_EDITOR
    readonly private string DIR = Application.dataPath + "/Temporary/";
#elif (UNITY_ANDROID || UNITY_IOS) // && !UNITY_EDITOR
    readonly private string DIR = Application.persistentDataPath + "/Temporary/";
#endif

    private string path;

    [Header("Config")]
    [SerializeField] private string initialLevelPack;
    [SerializeField] private int initialCoin;

    public void Setup()
    {
        path = DIR + fileName;
        if (!Directory.Exists(DIR))
        {
            Directory.CreateDirectory(DIR);
            Debug.Log($"Directory created : {DIR}");
        }

        if (!File.Exists(path))
        {
            File.Create(path).Dispose();
            Debug.Log($"File created : {path}");
        }
    }



    public bool Load()
    {
        bool isSuccess;
        FileStream fileStream = File.Open(path, FileMode.Open);

        if(usingBinaryFormatter)
        {
            isSuccess = LoadByBinaryFormatter(fileStream);  
        } else
        {
            isSuccess = LoadByBinaryReader(fileStream);   
        }

        Debug.Log("file loadeded " + (isSuccess ? "successfully" : "in failure") + ", path: " + path);
        fileStream.Dispose();
        return isSuccess;
    }

    private bool LoadByBinaryFormatter(FileStream fileStream)
    {
        try
        {
            BinaryFormatter formatter = new BinaryFormatter();

            data = (ProgressData)formatter.Deserialize(fileStream);


            Debug.Log("File loaded successfully");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            return false;
        }
    }
#region BinaryReader
    private bool LoadByBinaryReader(FileStream fileStream)
    {
        BinaryReader reader = new BinaryReader(fileStream);

        if (fileStream.Length == 0) { Debug.Log("file is empty"); return false; }
        data.coins = reader.ReadInt32();
        while (reader.PeekChar() != -1)
        {
            string key = reader.ReadString();
            int value = reader.ReadInt32();

            data.progresses.Add(key, value);
        }

        reader.Dispose();
        return true;
    }
#endregion

    public void Save()
    {
        // dummy data
        if (data.progresses == null)
        {
            data.progresses = new();
        }

        FileStream fileStream = File.Open(path, FileMode.Open);
        fileStream.Flush();

        if (usingBinaryFormatter)
        {
            SaveByBinaryFormatter(fileStream);
        }
        else
        {
            SaveByBinaryWriter(fileStream);
        }


        fileStream.Dispose();
        Debug.Log($"File saved at : {path}");
    }

    private void SaveByBinaryFormatter(FileStream fileStream)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        // pada kasus kodingan saya lebih cocok cek null directory lewat jumlah isinya
        if(data.progresses.Count == 0)
        {
            data.coins = initialCoin;
            // TODO: cek kalo initialLevelPack itu nama yg valid
            data.progresses.Add(initialLevelPack, 1);
        }

        formatter.Serialize(fileStream, data);
    }

#region BinaryWriter
    private void SaveByBinaryWriter(FileStream fileStream)
    {
        BinaryWriter writer = new BinaryWriter(fileStream);

        writer.Write(data.coins);
        foreach(KeyValuePair<string, int> kvp in data.progresses)
        {
            writer.Write(kvp.Key);
            writer.Write(kvp.Value);
        }

        writer.Dispose();
    }
#endregion


}
