using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Transform mower;
    public float smoothSpeed = 5f;

    private float initialY;
    private float initialZ;

    void Start()
    {
        if (mower == null)
        {
            GameObject mowerObj = GameObject.Find("Mower");
            if (mowerObj != null)
                mower = mowerObj.transform;
        }
        initialY = transform.position.y;
        initialZ = transform.position.z;
    }

    void LateUpdate()
    {
        if (mower == null) return;

        Vector3 targetPos = new Vector3(mower.position.x, initialY, mower.position.z-5.0f);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}
