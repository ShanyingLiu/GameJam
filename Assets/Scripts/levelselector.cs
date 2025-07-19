using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelselector : MonoBehaviour
{
    public CameraManager bigman;
    public GameObject client;
    public Transform postemplate;
    public GameObject[] plugs;

    void OnMouseDown(){
        GameObject chosen = Instantiate(client, postemplate.position, postemplate.rotation);
        for (int i = 0; i < chosen.GetComponent<sequence>().values.Length; ++i){
            plugs[i].transform.position = chosen.GetComponent<sequence>().values[i].position;
        }
        bigman.level_picked();
        
    }
}
