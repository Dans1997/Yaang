using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem 
{
    const string pathEnd = "/player.yaang";

    public static void SavePlayer ()
    {
        Debug.Log("Saving game...");
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + pathEnd;
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData playerData = new PlayerData();

        formatter.Serialize(stream, playerData);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + pathEnd;
        if(File.Exists(path))
        {
            Debug.Log("Save file detected!");
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData playerData = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return playerData;
        }
        else
        {
            Debug.LogWarning("Save File not found in " + path);
            return null;
        }
    }

    public static void DeletePlayer()
    {
        string path = Application.persistentDataPath + pathEnd;
        try
        {
            File.Delete(path);
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
