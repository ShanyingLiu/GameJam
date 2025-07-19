using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class lawnmower_runner : MonoBehaviour
{
    public Rigidbody body;
    public float acceleration, friction;
    bool swinging;
    public Transform[] plugs;
    public Transform nearest_plug;
    public float startspeed;
    public float maxspeed;
    public float maxdist;

    float dot(Vector3 a, Vector3 b){
        return a.x * b.x + a.z * b.z;
    }

    Vector3 proj(Vector3 a, Vector3 b){
        return b * dot(a, b) / dot(b, b);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InitRun();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "RunMower")
            InitRun();
    }

    void InitRun()
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
        body.velocity = new Vector3(startspeed, 0, 0);
        if (plugs.Length > 0)
            nearest_plug = plugs[0];
    }

    void push(){
        float change = swinging ? acceleration : -friction;
        body.AddForce(body.velocity.normalized * change, ForceMode.Acceleration);
    }

    void swing(Transform plug){
        Vector3 dir = plug.position - transform.position;
        if (dot(dir, body.velocity) / dot(body.velocity, body.velocity) > 0){
            return;
        }
        Vector3 rot = new Vector3(-dir.z, 0, dir.x);
        float m = body.mass;
        float v = proj(body.velocity, rot).magnitude;
        float r = (plug.position - transform.position).magnitude;
        if (r > maxdist){
            return;
        }
        body.AddForce(dir * (m * v * v / r), ForceMode.Acceleration);
    }
    
    void FixedUpdate()
    {
        push();
        if (swinging){
            swing(nearest_plug);
        }
        if (body.velocity.magnitude > maxspeed){
            body.velocity = body.velocity.normalized * maxspeed;
        }
    }

    void Update(){
        if (Input.GetKey(KeyCode.Space) != swinging){
            swinging = Input.GetKey(KeyCode.Space);
            for (int i = 0; i < plugs.Length; ++i){
                if (Vector3.Distance(transform.position, nearest_plug.position) > Vector3.Distance(transform.position, plugs[i].position)){
                    nearest_plug = plugs[i];
                }
            }
        }
    }
}
