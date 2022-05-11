using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{
    public bool start;
    public float speed;
    private void Start()
    {
        InvokeRepeating("destroyObject", 3, 1);
    }
    void Update()
    {
        if (!start) { return; }
        transform.Translate(Vector2.left * Time.deltaTime * speed);
       
    }
    void destroyObject()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        for(int i = 1; i < children.Length; i++)
        {
            if (children[i].transform.position.x <= -75)
            {
                Destroy(children[i].gameObject);
            }
        }
    }
}
