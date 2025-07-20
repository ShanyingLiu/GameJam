using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraManager : MonoBehaviour
{
    public Transform mower;
    public float smoothSpeed = 5f;

    private float initialY;
    private float initialZ;
    public RunLevelManager manager;

    bool picked = false;

    public GameObject map1, map2, map3, map4, everythingElse;

    public void Start(){
        transform.position = new Vector3(-23f, 43.1f, -29.3f);
        if (mower == null)
        {
            GameObject mowerObj = GameObject.Find("Mower");
            if (mowerObj != null)
                mower = mowerObj.transform;
        }
        mower.gameObject.SetActive(false);
    }

    public void level_picked(){
        manager.StartTimer();
        transform.position = new Vector3(0f, 5.5f, -1f);
        Destroy(map1);
        Destroy(map2);
        Destroy(map3);
        Destroy(map4);
        everythingElse.SetActive(true);
        
        initialY = transform.position.y;
        initialZ = transform.position.z;
        mower.gameObject.SetActive(true);
        picked = true;
    }

    void LateUpdate()
    {
        if (mower == null || !picked) return;

        Vector3 targetPos = new Vector3(mower.position.x, initialY, mower.position.z-5.0f);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
        //transform.LookAt(mower.position);
    }
}
