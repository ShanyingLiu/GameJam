using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public GameObject prefabToSpawn;
    [HideInInspector] public Camera sceneCamera;
    public int price;

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

        Plane xzPlane = new Plane(Vector3.up, new Vector3(0, 0, 0));
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
                Debug.Log("Not enough money to place object!");
                Destroy(previewInstance);
                return;
            }

            Vector3 finalPos = previewInstance.transform.position;
            Destroy(previewInstance);

            GameObject finalInstance = Instantiate(prefabToSpawn, finalPos, Quaternion.identity);

            if (!IsTouchingMower(finalInstance))
            {
                Debug.Log("Part not touching Mower. Destroying");
                Destroy(finalInstance);
            }
            else
            {
                finalInstance.transform.parent = mowerObject.transform;
                EventManager.instance.AddMoney(-price);
            }
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
