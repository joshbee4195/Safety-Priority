using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{

    public float speed;
    public float origSpeed;

    public Animator anim;

    public bool moving;

    public NavMeshAgent agent;

    public Transform destination;

    public int ID; //interact object ID - same for all NPCs

    public bool trainHere;

    public int NPC_ID; //NPC ID - diff for each

    //1 is police man, 2 police woman
    //3 girl, 4 boy A, 5 boy B
    //6 man A, 7 man B, 8 man C, 9 man D
    //10 woman A, 11 woman B, 12 woman C, 13 woman D



    public bool randomPoint;

    public float navmeshRadius = 5f;

    public float traversalThreshold = 0.5f;

    public bool randomMoving;

    public int waitTime;
    public int waitTimeThresh;
    public bool setWaitThresh;

    public bool sitting;

    public bool willChange;


    public float timeStuck;
    public float timeStuckThresh = 5;

    public Vector3 pos;


    // Start is called before the first frame update
    void Start()
    {
        //  if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        anim = GetComponent<Animator>();

        agent.speed = speed;

        if (destination != null)
        {
            agent.destination = destination.transform.position;
        }

        origSpeed = speed;


        //need to change to not police

        if (NPC_ID == 1 || NPC_ID == 2)
        {
            anim.SetInteger("action", 1);
        }
        else
        {
            anim.SetInteger("action", 2);
        }


        if (sitting)
        {
            anim.SetInteger("action", 3);

            agent.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

       // pos = transform.position;


        if (!data.isPaused)
        {
            if (NPC_ID == 1 || NPC_ID == 2)
            {
                anim.speed = 1;
            }

            else
            {


                agent.speed = origSpeed;
                anim.speed = 1;

            }

           // agent.isStopped = false;

            if (Input.GetKeyDown(KeyCode.N))
            {
              //  RandomMovement();
            }

            if (randomMoving)
            {
                if (agent.destination != null)
                {
                    if (Vector3.Distance(transform.position, agent.destination) < traversalThreshold) //close to target point
                    {
                        //choose new point

                        if (!setWaitThresh)
                        {
                           // waitTimeThresh = Random.Range(200, 500);

                            anim.SetInteger("action", 1);
                        }

                        waitTime++;

                        if (waitTime > waitTimeThresh)
                        {

                            randomPoint = false;
                            waitTime = 0;
                            anim.SetInteger("action", 2);
                            RandomMovement();
                        }
                    }
                }
            }           
        }

        if (data.isPaused)
        {
            if (NPC_ID != 1 && NPC_ID != 2)
            {
                agent.speed = 0;

                anim.speed = 0;

                //  agent.isStopped = true;

            }

            else
            {
                //police

                agent.speed = 0;

                anim.speed = 0;
            }
        }


        if (willChange)
        {
            if (trainHere)
            {
                randomMoving = true;
                sitting = false;

                agent.enabled = true;
            }
        }

        if (destination != null)
        {
            if (Vector3.Distance(transform.position, agent.destination) < traversalThreshold) //close to target point
            {
                randomMoving = true;
            }
        }


        if (!sitting)
        {
            if (NPC_ID != 1 && NPC_ID != 2)
            {
                if (pos == transform.position)
                {
                    timeStuck += 1;
                }

                pos = transform.position;

                if (timeStuck > timeStuckThresh)
                {
                    //find new target

                    RandomMovement();
                    timeStuck = 0;
                }
            }
        }
    }


    public void RandomMovement()
    {
        if (!randomPoint)
        {

            //find random point on navmesh


            agent.SetDestination(RandomNavmeshLocation(navmeshRadius));

            randomPoint = true;
        }
    }


    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
