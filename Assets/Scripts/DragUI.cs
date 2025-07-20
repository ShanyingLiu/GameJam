using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public GameObject prefabToSpawn;
    [HideInInspector] public Camera sceneCamera;
    public int price;
    public GameObject errorBackground;

    private GameObject mowerObject;
    private GameObject previewInstance;

    void Awake()
    {
        mowerObject = GameObject.Find("Mower");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        previewInstance = Instantiate(prefabToSpawn);
        MakePreviewTransparent(previewInstance);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (previewInstance == null) return;

        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, 0.07f, 0));

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
            if (EventManager.instance.money - price < 0)
            {
                ShowError("Not enough funds for this purchase");
                Destroy(previewInstance);
                return;
            }

            Vector3 finalPos = previewInstance.transform.position;
            Destroy(previewInstance);

            GameObject finalInstance = Instantiate(prefabToSpawn, finalPos, previewInstance.transform.rotation);


            if (!IsTouchingMower(finalInstance))
            {
                Debug.Log("Part not touching Mower. Destroying");
                Destroy(finalInstance);
            }
            else
            {
                TrySnapOntoMower(finalInstance);
                finalInstance.transform.parent = mowerObject.transform;
                EventManager.instance.AddMoney(-price);
            }
        }
    }

    private void ShowError(string message)
    {
        if (errorBackground != null)
        {
            TextMeshProUGUI errorText = errorBackground.transform.Find("ErrorDisplay")?.GetComponent<TextMeshProUGUI>();
            if (errorText != null)
            {
                errorText.text = message;
            }
            errorBackground.SetActive(true);
        }
        else
        {
            Debug.LogWarning("ErrorBackground not assigned.");
        }
    }

    private bool IsTouchingMower(GameObject obj)
    {
        if (mowerObject == null) return false;
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider == null) return false;

        Collider[] mowerColliders = mowerObject.GetComponentsInChildren<Collider>();
        foreach (var mowerCol in mowerColliders)
        {
            if (mowerCol.transform.IsChildOf(obj.transform)) continue;
            if (objCollider.bounds.Intersects(mowerCol.bounds))
                return true;
        }
        return false;
    }

    private void TrySnapOntoMower(GameObject obj)
    {
        Collider objCollider = obj.GetComponent<Collider>();
        if (objCollider == null) return;

        Collider[] mowerColliders = mowerObject.GetComponentsInChildren<Collider>();
        foreach (var mowerCol in mowerColliders)
        {
            if (mowerCol.transform.IsChildOf(obj.transform)) continue;
            if (objCollider.bounds.Intersects(mowerCol.bounds))
            {
                Vector3 targetPos = mowerCol.bounds.max;
                Vector3 offset = objCollider.bounds.extents;
                Vector3 newPos = new Vector3(
                    obj.transform.position.x,
                    targetPos.y + offset.y * 0.95f,
                    obj.transform.position.z
                );
                obj.transform.position = newPos;
                break;
            }
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
                c.a = 0.5f;
                mat.color = c;
                if (mat.shader.name.Contains("Opaque"))
                {
                    mat.shader = Shader.Find("Standard");
                    mat.SetFloat("_Mode", 3);
                }
            }
        }
    }
}
