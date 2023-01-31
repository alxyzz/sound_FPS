using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Saviour : MonoBehaviour
{

    public GameObject endpoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //CharacterController c = other.GetComponent<CharacterController>();
            //c.enabled = false;
            other.transform.position = endpoint.transform.position;
            //c.enabled = true;
        }
    }


}
