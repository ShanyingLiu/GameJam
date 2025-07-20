using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowOnce : MonoBehaviour
{
    void Start()
    {
        string sceneKey = "Visited_" + SceneManager.GetActiveScene().name;
        
        if (PlayerPrefs.GetInt(sceneKey, 0) == 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            PlayerPrefs.SetInt(sceneKey, 1);
            PlayerPrefs.Save();
            Invoke("HideObject", 8f);
        }
    }

    void HideObject()
    {
        gameObject.SetActive(false);
    }
}