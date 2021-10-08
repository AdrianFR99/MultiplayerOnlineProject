using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderMOVEMENT : MonoBehaviour
{



    public CharacterController controller;
    public float speed = 6f;
    float turnsmoothTime=0.1f;
    float turnSmoothVel=2;


    void Start()
    {
        
    }

    void Update()
    {


        //GET AXIS MOVEMENT
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertiacal = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertiacal).normalized;

       
       

       // CHARACTER Y ROTATION TO MOUSE
        if (Input.GetAxis("Mouse X") > 0.01f || Input.GetAxis("Mouse Y") > 0.01f)
        {
         
            Vector3 MousePos = Input.mousePosition;
               // Camera.main.farClipPlane * .5f;
            Vector3 MousePositionWorld = Camera.main.ScreenToWorldPoint(new Vector3(MousePos.x,MousePos.y,Camera.main.transform.position.y));

            Debug.Log("MousePos:"+ MousePositionWorld);
            Vector3 positionTolookAt = new Vector3(MousePositionWorld.x,0f, MousePositionWorld.z);


            Vector3 aux = positionTolookAt - transform.position;
            float TargetAngle = Mathf.Atan2(aux.x, aux.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, TargetAngle, ref turnSmoothVel, turnsmoothTime);
            transform.rotation = Quaternion.Euler(0f,angle,0f);

            //OPTION1
            //Quaternion currentRotation = transform.rotation;
            //Quaternion targetRotation = Quaternion.LookRotation(positionTolookAt-transform.position);
            //transform.rotation = Quaternion.Slerp(currentRotation,targetRotation,15f*Time.deltaTime);

            //OPTION2
            //float TargetAngle = Mathf.Atan2(positionTolookAt.x, positionTolookAt.z) *Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0f,TargetAngle,0f);

            //OPTION3 On current use
           // Vector3 aux = positionTolookAt - transform.position;
            //float TargetAngle = Mathf.Atan2(aux.x, aux.z) * Mathf.Rad2Deg;
            //transform.rotation = Quaternion.Euler(0f, TargetAngle, 0f);




        }


        if (direction.magnitude>=0.1f)
        {

           controller.Move(direction * speed * Time.deltaTime);

        }




        
       
        
        
    }
}
