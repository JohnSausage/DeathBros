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

    public bool[] skillAvailable;

    public virtual void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/savedata_" + saveDataName + ".save";

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, this);
        stream.Close();

        Debug.Log(saveDataName + " saved.");
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
            this.saveDataName = loaded.saveDataName;
            this.spawnX = loaded.spawnX;
            this.spawnY = loaded.spawnY;

            this.skillAvailable = new bool[loaded.skillAvailable.Length];

            for (int i = 0; i < loaded.skillAvailable.Length; i++)
            {
                this.skillAvailable[i] = loaded.skillAvailable[i];
            }

            Debug.Log(saveDataName + " loaded.");
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