using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lawnmower_runner : MonoBehaviour
{
    public Rigidbody body;
    public float jerk, friction;
    bool accelerating;
    public Transform defender;

    float dot(Vector3 a, Vector3 b){
        return a.x * b.x + a.z * b.z;
    }

    Vector3 lin(Vector3 a, Vector3 b, float v){
        return a * v + b * (1 - v);
    }

    // Start is called before the first frame update
    void Start()
    {
        body.velocity = new Vector3(1, 0, 0);
    }

    void push(){
        float change = accelerating ? jerk : -friction;
        body.AddForce(body.velocity.normalized * change, ForceMode.Acceleration);
    }

    void swing(Transform plug){
        float m = body.mass;
        float v = body.velocity.magnitude;
        float r = (plug.position - transform.position).magnitude;
        Vector3 dir = plug.position - transform.position;
        body.AddForce(dir * (m * v * v / r), ForceMode.Acceleration);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        push();
    }

    void Update(){
        accelerating = Input.GetKey(KeyCode.A);
        if (Input.GetKey(KeyCode.S)){
            swing(defender);
        }
    }
}