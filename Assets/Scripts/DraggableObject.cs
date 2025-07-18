using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera sceneCamera;
    private Plane dragPlane;
    private Vector3 offset;
    private bool isDragging = false;
    private float fixedY;

    private GameObject mowerObject;

    void Start()
    {
        sceneCamera = Camera.main;

        mowerObject = GameObject.Find("Mower");

        if (mowerObject == null)
        {
            Debug.LogError("Could not find mower");
        }
    }

    void OnMouseDown()
    {
        isDragging = true;

        fixedY = transform.position.y;

        // Create horizontal X-Z plane at object's current Y
        dragPlane = new Plane(Vector3.up, new Vector3(0, fixedY, 0));

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            offset = transform.position - hitPoint;
        }
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 targetPos = hitPoint + offset;

            targetPos.y = fixedY; // lock Y
            transform.position = targetPos;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;

        if (IsTouchingMower())
        {
            Debug.Log("Placement valid: object is touching Mower or its children.");
            gameObject.transform.parent = mowerObject.transform;
        }
        else
        {
            Debug.Log("part not touching Mower. Destroying");
            // CALL REFUND
            Destroy(gameObject);

        }
    }

    bool IsTouchingMower()
    {
        if (mowerObject == null) return false;

        Collider thisCollider = GetComponent<Collider>();
        if (thisCollider == null)
        {
            return false;
        }
        Collider[] mowerColliders = mowerObject.GetComponentsInChildren<Collider>();

        foreach (var mowerCol in mowerColliders)
        {
            if (mowerCol.transform.IsChildOf(this.transform)) continue;

            if (thisCollider.bounds.Intersects(mowerCol.bounds))
            {
                return true;
            }
        }


        return false;
    }

}
