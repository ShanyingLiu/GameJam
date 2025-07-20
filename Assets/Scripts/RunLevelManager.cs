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
    public GameObject CreationButton;
    public GameObject TimerBackground;


    private int currentLevelIndex = 0;
    private float elapsedTime = 0f;
    private bool ended = true;

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
            CreationButton.SetActive(false);
            TimerBackground.SetActive(true);
            InvokeRepeating(nameof(CheckMowerChildrenAndEnd), checkInterval, checkInterval);
        }
    }

    public void StartTimer()
    {
        ended = false;
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

            if (mowerRoot != null)
            {
                mowerRoot.transform.position = new Vector3(1000, 0, 1000);

            }

            ended = true;

            if (EndTime != null)
            {
                EndTime.text = $"Your Time: {elapsedTime:F1}s";
            }
            var areagetter = FindObjectOfType<areacovered>();
            int areaCovered = 100;

            if (areagetter != null) { areaCovered = (int)areagetter.getscore(); }
            Debug.Log(areaCovered);

            if (AreaCovered != null)
            {
                AreaCovered.text = $"Area Covered: {areaCovered}";
            }
            if (MoneyEarned != null)
            {
                int moneyEarned = (int)elapsedTime / 5;
                moneyEarned += (int)areaCovered / 50 * 2;
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
                CreationButton.SetActive(true);
                TimerBackground.SetActive(false);
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
    Debug.Log("=== REFRESH CHILDREN CALLED ===");
    FindMowerRoot();
    if (mowerPrefab == null || mowerRoot == null)
    {
        Debug.LogWarning("Cannot refresh mower: prefab or mowerRoot missing.");
        return;
    }

    // Clear existing children
    while (mowerRoot.transform.childCount > 0)
    {
        DestroyImmediate(mowerRoot.transform.GetChild(0).gameObject);
    }

    // Instantiate the prefab
    GameObject temp = Instantiate(mowerPrefab);
    temp.SetActive(true);

    // Find the inner "mower" object in the instantiated prefab
    Transform innerMower = temp.transform.Find("mower"); // Adjust name if different
    if (innerMower == null)
    {
        // If it's the first child, get it directly
        innerMower = temp.transform.GetChild(0);
        Debug.Log($"Using first child as inner mower: {innerMower.name}");
    }
    else
    {
        Debug.Log($"Found inner mower: {innerMower.name}");
    }

    // Move children from the INNER mower to our mowerRoot
    List<Transform> childrenToMove = new List<Transform>();
    foreach (Transform child in innerMower)
    {
        Debug.Log($"Found child to move: {child.name}");
        childrenToMove.Add(child);
    }

    foreach (Transform child in childrenToMove)
    {
        Debug.Log($"Moving child: {child.name} to mowerRoot");
        child.SetParent(mowerRoot.transform, false); // Keep local transforms
        child.gameObject.SetActive(true);
    }

    // Clean up
    DestroyImmediate(temp);
    
    Debug.Log($"Final mowerRoot children count: {mowerRoot.transform.childCount}");
    Debug.Log("=== REFRESH COMPLETE ===");
}

}