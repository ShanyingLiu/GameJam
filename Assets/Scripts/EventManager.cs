using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*if (scene.name == "RunMower")
        {
            GameObject playermodel = GameObject.Find("Mower");
            GameObject lawnmowerrunner = GameObject.Find("MowerRunner");
            if(!playermodel){ Debug.Log("playermodel null"); }
            if(!lawnmowerrunner){ Debug.Log("mowerrunner null"); }


            Instantiate(playermodel, lawnmowerrunner.transform);
        }*/

        FindAndBindButton("StartButton", LoadNextScene);
        FindAndBindButton("StartRun", LoadRunMowerScene);
        FindAndBindButton("Creation", LoadCreationScene);
    }

    void FindAndBindButton(string buttonName, UnityEngine.Events.UnityAction action)
    {
        GameObject buttonObj = GameObject.Find(buttonName);
        if (buttonObj != null)
        {
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
                Debug.Log($"Bound button '{buttonName}'");
            }
            else
            {
                Debug.LogWarning($"Button component not found on '{buttonName}'");
            }
        }
        else
        {
            Debug.LogWarning($"Button '{buttonName}' not found in scene '{SceneManager.GetActiveScene().name}'");
        }
    }

    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning("No next scene");
        }
    }

    public void LoadCreationScene()
    {
        SceneManager.LoadScene("Creation");
    }

    public void LoadRunMowerScene()
    {
        SceneManager.LoadScene("RunMower");
        
    }
}
