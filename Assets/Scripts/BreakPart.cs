using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakPart : MonoBehaviour
{
    public float breakChance = 0.4f;
    public float partStrength = 0.0f;
    private float forceStrength = 5f;
    private bool hasBroken = false;
    private float originalThickness = 1f;

    void Awake()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            Destroy(rb);
        }

        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }

        if (gameObject.name.Contains("Bigger"))
        {
            Debug.Log("Bigger created!!");

            GameObject mowerRoot = GameObject.Find("Mower");
            if (mowerRoot != null)
            {
                TrailRenderer lr = mowerRoot.GetComponent<TrailRenderer>();
                originalThickness = lr.startWidth;
                lr.startWidth += 1.0f;
                lr.endWidth += 1.0f;

            }

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (SceneManager.GetActiveScene().name != "RunMower") return;

        Debug.Log("trigger entered");
        if (hasBroken) return;

        if (other.gameObject.name == "Mower") return;
        if (other.gameObject.name == "GroundPlane") return;

        if (Random.value < (breakChance - partStrength)) // if part is stronger it will be less likely to break
        {
            Debug.Log("breaking off");
            hasBroken = true;
            BreakOff();
        }
    }

    void BreakOff()
    {
        transform.parent = null;
        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;

        Vector3 randomDir = Random.onUnitSphere;
        rb.AddForce(randomDir * forceStrength, ForceMode.Impulse);

        if (gameObject.name.Contains("Bigger"))
        {
            GameObject mowerRoot = GameObject.Find("Mower");
            if (mowerRoot != null)
            {
                TrailRenderer lr = mowerRoot.GetComponent<TrailRenderer>();
                lr.startWidth = originalThickness;
                lr.endWidth = originalThickness;

            }

            var eventManager = FindObjectOfType<EventManager>();
            if (eventManager != null)
            {
                eventManager.largerMowerUsed = true;
            }

        }

        Destroy(gameObject, 2f);
    }
}