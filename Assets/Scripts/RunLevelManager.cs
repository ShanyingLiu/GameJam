using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RunLevelManager : MonoBehaviour
{
    public List<GameObject> levels; 
    public TextMeshProUGUI timerDisplay; 

    private int currentLevelIndex = 0;
    private float elapsedTime = 0f;

    private GameObject mowerRoot; 

    public float initialBreakChance = 0.2f;
    public float timeToReachFullChance = 30f; 

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        ShowCurrentLevel();
        elapsedTime = 0f;

        FindMowerRoot();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        elapsedTime = 0f;
        FindMowerRoot();
    }

    void FindMowerRoot()
    {
        mowerRoot = GameObject.Find("Mower");
        if (mowerRoot == null)
        {
            Debug.LogWarning("Mower object not found in scene!");
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

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
}