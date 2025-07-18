using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [Header("Prefabs & Icons")]
    public List<GameObject> prefabs;       // List of prefabs
    public List<Sprite> prefabIcons;       // Icons matching prefabs

    [Header("UI References")]
    public GameObject buttonPrefab;        // UI Button prefab
    public Transform contentParent;        // Scroll View Content
    public Camera sceneCamera;             // Your orthographic/perspective camera

    void Start()
    {
        PopulateMenu();
    }

    void PopulateMenu()
    {
        if (prefabs.Count != prefabIcons.Count)
        {
            Debug.LogError("Prefab list and icon list counts do not match!");
            return;
        }

        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject newButtonObj = Instantiate(buttonPrefab, contentParent);
            Button newButton = newButtonObj.GetComponent<Button>();
            Image iconImage = newButtonObj.GetComponentInChildren<Image>();

            if (iconImage != null)
            {
                iconImage.sprite = prefabIcons[i];
                iconImage.preserveAspect = true;
            }

            // Add drag script dynamically & set references
            DragUI dragScript = newButtonObj.AddComponent<DragUI>();
            dragScript.prefabToSpawn = prefabs[i];
            dragScript.sceneCamera = sceneCamera;

            // Keep your existing click logic if needed
            int index = i;
            newButton.onClick.AddListener(() => OnPrefabSelected(index));
        }
    }

    void OnPrefabSelected(int index)
    {
        Debug.Log("Clicked prefab: " + prefabs[index].name);
        // optional: handle non-drag click
    }
}
