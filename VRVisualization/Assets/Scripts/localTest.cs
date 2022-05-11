using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class localTest : MonoBehaviour
{
    public Transform grand;
    public Transform parent;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("world pos:" + this.transform.position);
        Debug.Log("parent world space:" + parent.TransformPoint(this.transform.position));
        Debug.Log("grand world space:" + grand.TransformPoint(this.transform.position));
        Debug.Log("grand + parent world space:" + grand.TransformPoint(parent.TransformPoint(this.transform.position)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
