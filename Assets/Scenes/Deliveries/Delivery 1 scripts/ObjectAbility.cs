using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectAbility : MonoBehaviour
{
    public GameObject BallPrefab;
    List<GameObject> BallPool;
    System.DateTime myTime;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (BallPool.Count < 5)
                UseAbility();

            else
                Debug.Log("ball limit reached");
        }

        if (BallPool != null)
        {
            for (var i = 0; i < BallPool.Count; i++)
            {
                if (BallPool[i].GetComponent<ObjectBehavior>().preparedToDie)
                {
                    BallPool.RemoveAt(i);
                    Debug.Log("deleted ball at pos:" + i);
                }
            }
        }
    }

    void UseAbility()
    {
        //spawn ball, add it into ballPool, and start cooldown coroutine 
        BallPool.Add(Instantiate(BallPrefab, new Vector3(0, 0, 0), Quaternion.identity));
        Debug.Log("new ball added. total:" + BallPool.Count);

    }


}
