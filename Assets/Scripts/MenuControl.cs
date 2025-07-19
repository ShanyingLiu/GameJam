using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuControl : MonoBehaviour
{
    [Header("Prefabs & Icons")]
    public List<GameObject> prefabs;      
    public List<Sprite> prefabIcons;  
    public List<int> prefabPrices;

    [Header("UI References")]
    public GameObject buttonPrefab; 
    public GameObject priceLabelPrefab; // TMP prefab
    public Transform contentParent;
    public Camera sceneCamera;
    public GameObject errorBackground; // Drag your ErrorBackground UI object here
    public Button clearAllButton;     // Drag your ClearAll UI button here

    void Start()
    {
        if (errorBackground != null)
            errorBackground.SetActive(false);

        if (clearAllButton != null)
            clearAllButton.onClick.AddListener(ClearAllChildren);

        PopulateMenu();
    }

    void PopulateMenu()
    {
        if (prefabs.Count != prefabIcons.Count || prefabs.Count != prefabPrices.Count)
        {
            Debug.LogError("Prefab list, icon list, and price list counts do not match!");
            return;
        }

        for (int i = 0; i < prefabs.Count; i++)
        {
            GameObject container = new GameObject($"Item_{i}", typeof(RectTransform));
            container.transform.SetParent(contentParent, false);

            VerticalLayoutGroup layout = container.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.spacing = 10;

            ContentSizeFitter fitter = container.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            GameObject newButtonObj = Instantiate(buttonPrefab, container.transform);
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
            dragScript.price = prefabPrices[i];
            dragScript.errorBackground = errorBackground; // pass the reference

            int index = i;
            newButton.onClick.AddListener(() => OnPrefabSelected(index));

            GameObject priceLabelObj = Instantiate(priceLabelPrefab, container.transform);
            TextMeshProUGUI priceText = priceLabelObj.GetComponent<TextMeshProUGUI>();
            if (priceText != null)
            {
                priceText.text = $"${prefabPrices[i]}";
            }
        }
    }

    void ClearAllChildren()
    {
        GameObject mower = GameObject.Find("Mower");
        if (mower != null)
        {
            foreach (Transform child in mower.transform)
            {
                if (child.name != "Mower")
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    void OnPrefabSelected(int index)
    {
        Debug.Log("Clicked prefab: " + prefabs[index].name);
    }
}
