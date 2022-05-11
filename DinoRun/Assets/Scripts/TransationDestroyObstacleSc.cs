using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransationDestroyObstacleSc : MonoBehaviour
{

    Collider2D[] col;
    void Update()
    {
        col = Physics2D.OverlapCircleAll(transform.position, 36);
        foreach(Collider2D c in col)
        {
            if (c.transform.CompareTag("Obstacle"))
            {
                Destroy(c.gameObject);
            }
            if (c.transform.CompareTag("Laser"))
            {
                Destroy(c.gameObject);
            }
        }
        
    }
}
