using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class lawnmower_runner : MonoBehaviour
{
    public Rigidbody body;
    public float acceleration = 3;
    public float friction = 1;
    bool swinging;
    public Transform[] plugs;
    public Transform nearest_plug;
    public float startspeed = 3;
    public float maxspeed = 8;
    public float maxdist = 3;
    public float spinspeed = 2;
    public LineRenderer randy;

    float dot(Vector3 a, Vector3 b){
        return a.x * b.x + a.z * b.z;
    }

    Vector3 proj(Vector3 a, Vector3 b){
        return b * dot(a, b) / dot(b, b);
    }

    void Start()
    {
        FindPlugs();
        body.velocity = new Vector3(-startspeed, 0, 0);
        if (plugs.Length > 0)
            nearest_plug = plugs[0];
        else
            nearest_plug = null;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindPlugs();
        if (plugs.Length > 0)
            nearest_plug = plugs[0];
        else
            nearest_plug = null;
    }

    void FindPlugs()
    {
        GameObject plugsParent = GameObject.Find("Plugs");
        if (plugsParent != null)
        {
            List<Transform> children = new List<Transform>();
            foreach (Transform child in plugsParent.transform)
            {
                children.Add(child);
            }
            plugs = children.ToArray();
        }
        else
        {
            plugs = new Transform[0];
        }
    }

    void push(){
        float change = swinging ? acceleration : -friction;
        body.AddForce(body.velocity.normalized * change, ForceMode.Acceleration);
    }

    void swing(Transform plug){
        if (plug == null) return;
        Vector3 dir = plug.position - transform.position;
        if (dot(dir, body.velocity) / dot(body.velocity, body.velocity) > 0){
            return;
        }
        Vector3 rot = new Vector3(-dir.z, 0, dir.x);
        float m = body.mass;
        float v = proj(body.velocity, rot).magnitude;
        float r = (plug.position - transform.position).magnitude;
        if (r > maxdist){
            swinging = false;
            return;
        }
        body.AddForce(dir * (m * v * v / r), ForceMode.Acceleration);
        //Debug.Log("log");
    }

    float abs(float a){
        return a < 0 ? -a : a;
    }

    void turn(){
        if (abs(dot(transform.forward, body.velocity) / dot(body.velocity, body.velocity)) < 0.05){
            body.AddTorque(-body.angularVelocity);
            return;
        }
        if (dot(transform.forward, body.velocity) / dot(body.velocity, body.velocity) < 0){
            body.AddTorque(transform.up * spinspeed);
        }
        else{
            body.AddTorque(-transform.up * spinspeed);
        }
    }

    void FixedUpdate()
    {
        push();
        if (swinging && nearest_plug != null)
        {
            swing(nearest_plug);
        }

        if (body.velocity.magnitude > maxspeed)
        {
            body.velocity = body.velocity.normalized * maxspeed;
        }
        if (body.velocity.magnitude > 0.5){
            //turn();
        }
        if (swinging && nearest_plug != null){
            randy.SetPosition(0, transform.position);
            randy.SetPosition(1, nearest_plug.position);
        }
        else{
            randy.SetPosition(0, transform.position);
            randy.SetPosition(1, transform.position);
        }
    }

    void Update(){
        if (Input.GetKey(KeyCode.Space) != swinging){
            swinging = Input.GetKey(KeyCode.Space);
            if (plugs.Length > 0)
            {
                nearest_plug = plugs[0];
                for (int i = 1; i < plugs.Length; ++i){
                    if (Vector3.Distance(transform.position, nearest_plug.position) > Vector3.Distance(transform.position, plugs[i].position)){
                        nearest_plug = plugs[i];
                    }
                }
            }
            else
            {
                nearest_plug = null;
            }
        }
        
    }
}
