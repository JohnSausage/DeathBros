using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class SaveData
{
    public string saveDataName;

    public bool gameStarted;

    public float spawnX;
    public float spawnY;


    public virtual void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedata_" + saveDataName + ".save";

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, this);
        stream.Close();
    }

    public virtual void Load(string saveDataName)
    {
        string path = Application.persistentDataPath + "/savedata_" + saveDataName + ".save";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData loaded = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            //Writing the loaded data
            saveDataName = loaded.saveDataName;
            spawnX = loaded.spawnX;
            spawnY = loaded.spawnY;
        }
        else
        {
            Debug.LogError(saveDataName + " not found");
        }
    }
}

[System.Serializable]
public class SaveData2 : SaveData
{

}