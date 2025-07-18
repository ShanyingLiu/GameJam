using UnityEngine;

public class DraggableObject : MonoBehaviour
{
    private Camera sceneCamera;
    private Plane dragPlane;
    private Vector3 offset;
    private bool isDragging = false;
    private float fixedY;

    void Start()
    {
        sceneCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;

        sceneCamera = Camera.main; // make sure sceneCamera is set

        fixedY = transform.position.y;

        // Create horizontal X-Z plane at the object's current Y
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

            // Lock Y coordinate to fixedY
            targetPos.y = fixedY;

            transform.position = targetPos;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}
