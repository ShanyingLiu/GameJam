using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lawnmower_runner : MonoBehaviour
{
    public Rigidbody body;
    public float acceleration, friction;
    bool swinging;
    public Transform[] plugs;
    public Transform nearest_plug;
    public float startspeed;


    public GameObject playermodel;
    GameObject viewcopy;

    public void begin()
    {
        playermodel = GameObject.Find("Mower(Clone)");

        if (viewcopy != null)
        {
            Destroy(viewcopy);
        }
        //viewcopy = Instantiate(playermodel);
        viewcopy = playermodel;
        viewcopy.transform.SetParent(transform, false);
        viewcopy.transform.localPosition = Vector3.zero;
        viewcopy.transform.localRotation = Quaternion.identity;
        viewcopy.transform.localScale = Vector3.one;

    }


    float dot(Vector3 a, Vector3 b){
        return a.x * b.x + a.z * b.z;
    }

    Vector3 proj(Vector3 a, Vector3 b){
        return b * dot(a, b) / dot(b, b);
    }

    // Start is called before the first frame update
    void Start()
    {
        body.velocity = new Vector3(startspeed, 0, 0);
        nearest_plug = plugs[0];
        begin();
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
        body.AddForce(dir * (m * v * v / r), ForceMode.Acceleration);
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        push();
        if (swinging){
            swing(nearest_plug);
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