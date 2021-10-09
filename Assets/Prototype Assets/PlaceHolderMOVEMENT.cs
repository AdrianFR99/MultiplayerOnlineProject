using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceHolderMOVEMENT : MonoBehaviour
{



    public CharacterController controller;

    public GameObject FollowGO;

    public float speed = 6f;

    [SerializeField]
   private Vector3 AimingPoint;
    [SerializeField]
    private float aimingPercentageToPoint =25;
  
    void Start()
    {

        aimingPercentageToPoint /= 100;
        Debug.Log(aimingPercentageToPoint);

    }

    void Update()
    {


        //GET AXIS MOVEMENT
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertiacal = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertiacal).normalized;


        Plane playerPlane = new Plane(Vector3.up,transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDist =0f;

        if(playerPlane.Raycast(ray, out hitDist))
        {

            Vector3 targetPoint = ray.GetPoint(hitDist);
            AimingPoint = targetPoint;
            Quaternion targetRotation = Quaternion.LookRotation(targetPoint-transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,7f*Time.deltaTime);


        }


        /*
        if (Input.GetButtonDown("Aim"))
        {
            Vector3 directionAimingVector = AimingPoint - transform.position;
            Vector3 finalAimingPoint = directionAimingVector.normalized*(directionAimingVector.magnitude*aimingPercentageToPoint);

            FollowGO.transform.position = finalAimingPoint;

        }
        if (Input.GetButtonUp("Aim"))
        {
           

            FollowGO.transform.position = transform.position;

        }
        */
        if (direction.magnitude>=0.1f)
        {

           controller.Move(direction.normalized * speed * Time.deltaTime);

        }




        
       
        
        
    }



   


}







