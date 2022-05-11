using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserForDestroySc : MonoBehaviour
{
   public void laserForDestroyMethod()
    {
        Collider2D[] col =  Physics2D.OverlapCircleAll(transform.position, 11);
        foreach(Collider2D c in col)
        {
            if (c.transform.CompareTag("Obstacle"))
            {
                Destroy(c.gameObject);
            }
        }
    }
}
