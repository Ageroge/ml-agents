using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{

    public bool is_collided;

    // Start is called before the first frame update
    void Start()
    {
        is_collided = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collided)
    {
        if (collided.gameObject.tag == "swordsman")
        {
            is_collided = true;
        }

    }
}
