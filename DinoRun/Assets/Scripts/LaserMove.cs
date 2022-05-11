using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMove : MonoBehaviour
{
    Container container;
    public float laserSpeed;
    void Start()
    {
        container = GameObject.Find("Container").GetComponent<Container>();
        InvokeRepeating("destroyObject", 3, 1);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * Time.deltaTime * container.speed*laserSpeed);
    }
    void destroyObject()
    {
        if (transform.position.x <= -75)
        {
            Destroy(gameObject);
        }
    }
}
