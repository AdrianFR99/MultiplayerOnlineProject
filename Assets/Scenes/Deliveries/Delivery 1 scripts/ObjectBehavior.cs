using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object-controlling script. Purpose is to get it destroyed after a given time using a coroutine to check if that time has passed.
public class ObjectBehavior : MonoBehaviour
{
    public float lifespan;
    public bool preparedToDie;

    System.DateTime myTime;

    private bool lifetimeOver; //only accessed from the coroutine

    // Start is called before the first frame update
    void Start()
    {
        preparedToDie = false;

        StartCoroutine(ManageLifetime(lifespan));
    }

    // Update is called once per frame
    void Update()
    {
        if(lifetimeOver)
        {
            preparedToDie = true;
        }
    }
    IEnumerator ManageLifetime(float duration)
    {
        lifetimeOver = false;
        //Activate myTime and calculate the transurred time since activation subtracting it to the 
        //the current time. If the time passed is longer than object's lifespan, then destroy it.
        myTime = System.DateTime.UtcNow;
        while ((System.DateTime.UtcNow - myTime).Seconds < duration)
        {
            yield return null;
        }
        lifetimeOver = true;
        Debug.Log("Lifetime Over");
    }
}
