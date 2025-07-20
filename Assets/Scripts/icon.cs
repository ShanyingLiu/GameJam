using UnityEngine;

public class icon : MonoBehaviour
{
    public Transform target;

    public float bounceSpeed = 0.5f;     
    public float bounceHeight = 0.5f;  

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target);
        }

        float newY = Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.localPosition = new Vector3(initialPosition.x, initialPosition.y + newY, initialPosition.z);
    }
}
