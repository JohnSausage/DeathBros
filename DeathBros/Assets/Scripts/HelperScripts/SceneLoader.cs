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
}
