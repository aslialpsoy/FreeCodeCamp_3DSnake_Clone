using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivatePickUp : MonoBehaviour
{ //attach this script to the pickup prefabs

    void Start(){
        Invoke("Deactivate", Random.Range(3f, 6f));
        
    }

    void Deactivate(){
        gameObject.SetActive(false);
    }
}
