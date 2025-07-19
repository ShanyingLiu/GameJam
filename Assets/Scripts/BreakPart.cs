using UnityEngine;
using UnityEngine.SceneManagement;

public class BreakPart : MonoBehaviour
{
    public float breakChance = 0.4f; 
    private float forceStrength = 5f;
    private bool hasBroken = false;

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
    }

    void OnTriggerEnter(Collider other)
    {
        if (SceneManager.GetActiveScene().name != "RunMower") return;

        Debug.Log("trigger entered");
        if (hasBroken) return;

        if (other.gameObject.name == "Mower") return;

        if (Random.value < breakChance)
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

        Destroy(gameObject, 2f);
    }
}