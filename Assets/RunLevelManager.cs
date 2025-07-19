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

    void Start()
    {
        ShowCurrentLevel();
        elapsedTime = 0f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (timerDisplay != null)
        {
            timerDisplay.text = $"Time: {elapsedTime:F1}s";
        }
    }

    public void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = levels.Count - 1; // stay at last level
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
