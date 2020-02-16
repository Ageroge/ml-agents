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
        Debug.Log("is_collided set back to false");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collided)
    {
        if (collided.gameObject.tag == "swordsman")
        {
            Debug.Log("arrow-swordsman");
            is_collided = true;
        }

    }
}
