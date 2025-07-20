using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class RunLevelManager : MonoBehaviour
{
    public GameObject mowerPrefab;  
    public List<GameObject> levels;
    public TextMeshProUGUI timerDisplay;
    public TextMeshProUGUI EndTime;
    public TextMeshProUGUI AreaCovered;
    public TextMeshProUGUI MoneyEarned;
    public GameObject EndScreen;

    private int currentLevelIndex = 0;
    private float elapsedTime = 0f;
    private bool ended = false;

    private GameObject mowerRoot;
    private Rigidbody mowerRigidbody;

    public float initialBreakChance = 0.2f;
    public float timeToReachFullChance = 30f;
    public float checkInterval = 1f;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ShowCurrentLevel();
        elapsedTime = 0f;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        elapsedTime = 0f;
        ended = false;
        FindMowerRoot();
        CancelInvoke(nameof(CheckMowerChildrenAndEnd));
        if (scene.name == "RunMower")
        {
            InvokeRepeating(nameof(CheckMowerChildrenAndEnd), checkInterval, checkInterval);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        CancelInvoke(nameof(CheckMowerChildrenAndEnd));
    }

    void FindMowerRoot()
    {
        mowerRoot = GameObject.Find("Mower");
        if (mowerRoot != null)
        {
            mowerRigidbody = mowerRoot.GetComponent<Rigidbody>();
        }
        else
        {
            mowerRigidbody = null;
        }
    }

    void Update()
    {
        if (!ended)
        {
            elapsedTime += Time.deltaTime;
        }

        if (timerDisplay != null)
        {
            timerDisplay.text = $"Time: {elapsedTime:F1}s";
        }

        UpdateBreakChances();
    }

    private void UpdateBreakChances()
    {
        if (mowerRoot == null) return;

        float t = Mathf.Clamp01(elapsedTime / timeToReachFullChance);
        float newChance = Mathf.Lerp(initialBreakChance, 1f, t);

        BreakPart[] parts = mowerRoot.GetComponentsInChildren<BreakPart>(true);
        foreach (var part in parts)
        {
            part.breakChance = newChance;
        }
    }

    private void CheckMowerChildrenAndEnd()
    {
        if (SceneManager.GetActiveScene().name != "RunMower") return;

        if (mowerRoot == null)
        {
            FindMowerRoot();
        }

        if (mowerRoot == null) return;

        int count = CountAllChildren(mowerRoot.transform);



        if (count <= 8 && !ended)
        {
            ended = true;

            if (EndTime != null)
            {
                EndTime.text = $"Your Time: {elapsedTime:F1}s";
            }
            var areagetter = FindObjectOfType<areacovered>();
            float areaCovered = 100;

            if (areagetter != null) { areaCovered = areagetter.getscore(); }
            Debug.Log(areaCovered);

            if (AreaCovered != null)
            {
                AreaCovered.text = $"Area Covered: ${areaCovered}";
            }
            if (MoneyEarned != null)
            {
                int moneyEarned = (int)elapsedTime / 5 * 2;
                moneyEarned += (int)areaCovered / 100 * 2;
                MoneyEarned.text = $"Funds Increased: ${moneyEarned}";
                var eventManager = FindObjectOfType<EventManager>();
                if (eventManager != null)
                {
                    eventManager.AddMoney(moneyEarned);
                }
                else
                {
                    Debug.LogWarning("EventManager gone");
                }
            }

            if (EndScreen != null)
            {
                EndScreen.SetActive(true);
            }

            if (mowerRoot != null)
            {
                var runner = mowerRoot.GetComponent<lawnmower_runner>();
                if (runner != null)
                {
                    runner.enabled = false;
                }
            }

            RefreshMowerChildrenFromPrefab();



        }
    }

    private int CountAllChildren(Transform parent)
    {
        int total = 0;
        foreach (Transform child in parent)
        {
            total++;
            total += CountAllChildren(child);
        }
        return total;
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = levels.Count - 1;
        }
        ShowCurrentLevel();
    }

    private void ShowCurrentLevel()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            if (levels[i] != null)
                levels[i].SetActive(i == currentLevelIndex);
        }
    }
    private void RefreshMowerChildrenFromPrefab()
    {
    FindMowerRoot();
    if (mowerPrefab == null || mowerRoot == null)
    {
        return;
    }

    while (mowerRoot.transform.childCount > 0)
    {
        DestroyImmediate(mowerRoot.transform.GetChild(0).gameObject);
    }

    GameObject temp = Instantiate(mowerPrefab);
    temp.SetActive(true);

    Transform innerMower = temp.transform.Find("mower");
    if (innerMower == null)
    {
        innerMower = temp.transform.GetChild(0);
    }

    List<Transform> childrenToMove = new List<Transform>();
    foreach (Transform child in innerMower)
    {
        childrenToMove.Add(child);
    }

    foreach (Transform child in childrenToMove)
    {
        child.SetParent(mowerRoot.transform, false);
        child.gameObject.SetActive(true);
    }

    DestroyImmediate(temp);
    }
}
