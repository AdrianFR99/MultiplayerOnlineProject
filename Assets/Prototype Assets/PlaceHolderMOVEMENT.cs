using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderMOVEMENT : MonoBehaviour
{



    public CharacterController controller;
    public float speed = 6f;


    void Start()
    {
        
    }

    void Update()
    {


        //GET AXIS MOVEMENT
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertiacal = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertiacal).normalized;

        //GET AXIS MOUSE MOVEMENT 2AXIS BASED 
        float MousedeltaX = Input.GetAxis("Mouse X");
        float MousedeltaY = Input.GetAxis("Mouse Y");


       // CHARACTER Y ROTATION TO MOUSE
        if (MousedeltaX>0.1f || MousedeltaY>0.1f)
        {

            Vector3 MousePositionWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 DirectionToMouse = new Vector3(MousePositionWorld.x-transform.position.x,MousePositionWorld.y-transform.position.y,MousePositionWorld.z-transform.position.z).normalized;

            float targetangle = Mathf.Atan2(DirectionToMouse.x, DirectionToMouse.z) *Mathf.Rad2Deg;


            transform.rotation = Quaternion.Euler(0f,targetangle,0f);
        }


        if (direction.magnitude>=0.1f)
        {

           controller.Move(direction * speed * Time.deltaTime);

        }




        
       
        
        
    }
}
