using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    public static EventManager instance;

    public int money = 10;

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
        FindAndBindButton("StartButton", LoadNextScene);
        FindAndBindButton("StartRun", LoadRunMowerScene);
        FindAndBindButton("Creation", LoadCreationScene);
        UpdateMoneyDisplay();
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

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateMoneyDisplay();
    }

    void UpdateMoneyDisplay()
    {
        GameObject moneyTextObj = GameObject.Find("Money");
        if (moneyTextObj != null)
        {
            TMP_Text tmp = moneyTextObj.GetComponent<TMP_Text>();
            if (tmp != null)
            {
                tmp.text = "$ ";
                tmp.text += money.ToString();
            }
        }
        else
        {
            Debug.LogWarning("Money text object not found");
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
