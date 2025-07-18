using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public GameObject prefabToSpawn;
    [HideInInspector] public Camera sceneCamera;

    private GameObject previewInstance;
    private Image iconImage;

    void Awake()
    {
        iconImage = GetComponentInChildren<Image>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previewInstance = Instantiate(prefabToSpawn);
        MakePreviewTransparent(previewInstance);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewInstance == null) return;

        // Define X–Z plane at fixed Y=0
        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));

        // Ray from camera through mouse
        Ray ray = sceneCamera.ScreenPointToRay(eventData.position);

        if (xzPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            previewInstance.transform.position = hitPoint;
        }
    }



    public void OnEndDrag(PointerEventData eventData)
    {
        if (previewInstance != null)
        {
            Vector3 finalPos = previewInstance.transform.position;

            Destroy(previewInstance);

            // Spawn final prefab at release position
            Instantiate(prefabToSpawn, finalPos, Quaternion.identity);
        }
    }

    private void MakePreviewTransparent(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            foreach (var mat in r.materials)
            {
                Color c = mat.color;
                c.a = 0.5f; // semi-transparent

                mat.color = c;

                // Ensure transparent shader
                if (mat.shader.name.Contains("Opaque"))
                {
                    mat.shader = Shader.Find("Standard");
                    mat.SetFloat("_Mode", 3); // transparent mode
                }
            }
        }
    }
}
