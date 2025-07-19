using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuControl : MonoBehaviour
{
    [Header("Prefabs & Icons")]
    public List<GameObject> prefabs;      
    public List<Sprite> prefabIcons;  

    [Header("UI References")]
    public GameObject buttonPrefab; 
    public Transform contentParent;
    public Camera sceneCamera;
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
            DragUI dragScript = newButtonObj.GetComponent<DragUI>();
            if (dragScript == null)
                dragScript = newButtonObj.AddComponent<DragUI>();

            dragScript.prefabToSpawn = prefabs[i];
            dragScript.sceneCamera = sceneCamera;

            int index = i;
            newButton.onClick.AddListener(() => OnPrefabSelected(index));
        }
    }

    void OnPrefabSelected(int index)
    {
        Debug.Log("Clicked prefab: " + prefabs[index].name);
    }
}
