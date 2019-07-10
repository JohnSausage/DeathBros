using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneToLoad)
    {
        GameManager.Instance.LoadScene(sceneToLoad);
    }


    public void ExitToMainMenu()
    {
        GameManager.Instance.ExitToMainMenu();
    }

    public void SaveGame()
    {
        GameManager.Instance.saveData.Save();
    }

    public void LoadGame(string saveDataName)
    {
        GameManager.Instance.saveData.Load(saveDataName);
    }
}
