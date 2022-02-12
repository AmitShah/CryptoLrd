using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    Camera camera;
    Rigidbody2D rigidbody2D;

    private void Awake()
    {
        camera = Camera.main;
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        updateCursor();
    }


    void updateCursor() {
        rigidbody2D.transform.position =  camera.ScreenToWorldPoint(Input.mousePosition);
    }
}
