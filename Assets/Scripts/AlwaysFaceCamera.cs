using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 public class AlwaysFaceCamera : MonoBehaviour {
 
 
     private Transform camera;
 
 
     // Use this for initialization
     void Start () {
 
         camera = Camera.main.gameObject.transform;
 
     }
 
     // Update is called once per frame
     void Update()
     {
        //this.transform.LookAt(camera);
        transform.LookAt(transform.position + Camera.main.transform.rotation *- Vector3.back,
        Camera.main.transform.rotation *- Vector3.down);
            
     }
 
 }