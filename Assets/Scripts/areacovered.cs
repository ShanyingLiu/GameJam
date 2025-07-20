using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class areacovered : MonoBehaviour
{
    public int size, spacing;
    public float area;
    public Vector3 origin = new Vector3(-5, -0.28f, 2);
    bool[] visited;
    public LayerMask player;

    public GameObject red, white;

    public float getscore(){
        return area;
    }
    void clear(){
        area = 0;
        for (int i = 0; i < size; ++i){
            for (int j = 0; j < size; ++j){
                visited[i*size+j] = false;
            }
        }
    }
    void Start(){
        visited = new bool[(size + 1)*(size + 1)];
        clear();
        int max = ((size - 1) - size / 2) * spacing;
        int min = ((0) - size / 2) * spacing;
        if (red != null){
            Instantiate(red, new Vector3(max, 0, max), Quaternion.identity);
            Instantiate(red, new Vector3(max, 0, min), Quaternion.identity);
            Instantiate(red, new Vector3(min, 0, max), Quaternion.identity);
            Instantiate(red, new Vector3(min, 0, min), Quaternion.identity);
        }
    }

    float getscore(float a){
        if (a > 40){
            return 1;
        }
        return 0.6f + 0.4f * a / 40;
    }

    void Update(){
        area = 0;
        for (int i = 0; i < size; ++i){
            for (int j = 0; j < size; ++j){
                Vector3 tile = origin + transform.right * (i - size / 2) * spacing + transform.forward * (j - size / 2) * spacing;
                visited[i*size+j] |= Physics.CheckSphere(tile, spacing / 2, player);
                area += (visited[i*size+j] ? 1 : 0) * (origin - tile).magnitude;
            }
        }
    }
}
