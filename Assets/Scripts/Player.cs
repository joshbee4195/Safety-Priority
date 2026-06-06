using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movement")]

    public float moveSpeed;
    public float moveSpeedOrig;
    public float runSpeed;


    public float climbSpeed = 0.5f;
    public bool climbing;
    public bool canClimb;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode interactKey = KeyCode.E;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;


    public bool canInteract;

    public float timeClimbing;
    public float timeClimbingThresh;

    public GameObject interactUI;

    public GameObject objInteracted; //which object player has interacted with

    public int interactID;


    public float timeSprinting;
    public float timeSprintingThresh;


    public Gamecontrol gamecontrol;


    public AudioSource jump;
    public AudioSource ow;
    public AudioSource crash;

    public float binForce = 1f;


    public int binTimer;
    public int binTimerThresh;
    public bool binOver;

    public int pushTimer;
    public int pushTimerThresh;
    public bool pushOver;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;


        moveSpeedOrig = moveSpeed;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (!data.isPaused)
        {
            MyInput();
            SpeedControl();
        }

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
        }

        else
        {
            rb.drag = 0;
        }

        if (!data.isPaused)
        {
            if (Input.GetKey(runKey)) //change to get key down for toggling instead of holding - then also add sprintSpeed if true, moveSpeed if false
            {
                moveSpeed = runSpeed;
            }

            else
            {
                moveSpeed = moveSpeedOrig;
            }


            if (Input.GetKeyDown(KeyCode.Q))
            {
                //data.DoneBad = !data.DoneBad;
            }


            if (Input.GetKeyDown(KeyCode.C))
            {
               // climbing = !climbing;
            }


            if (canClimb)
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    climbing = true;
                }
            }


            Climb();

        }


        if (data.isPaused)
        {
            rb.velocity = Vector3.zero;

            rb.useGravity = false;
        }

        if (!data.isPaused)
        {
            // rb.useGravity = true;


            if (canInteract)
            {
                if (Input.GetKeyDown(interactKey))
                {
                    //interact with object - differs depending on what it is


                    //default for now - always bad
                    // data.DoneBad = true;

                    //^change when display based on what it is?


                    if (interactID == 0) //NPC
                    {
                        //give time for slight stumble / sound

                        ow.Play();

                        pushOver = true;
                        //start timer
                    }

                    //run - done run, climb, jump separately with timers


                    else if (interactID == 5) //bin
                    {
                        //give time for bin to fall over, then done bad

                        objInteracted.GetComponent<Rigidbody>().isKinematic = false;

                        //need to get direction of push

                        objInteracted.GetComponent<Rigidbody>().AddForce(orientation.transform.forward * binForce, ForceMode.Impulse);

                        crash.Play();

                        binOver = true;

                    }

                }

            }
            //after x time, done bad

            if (binOver)
            {
                binTimer += 1;
            }

            if (binTimer > binTimerThresh)
            {
                interactID = 5;
                data.DoneBad = true;
            }

            //or after bin over


            if (pushOver)
            {
                pushTimer += 1;
            }

            if (pushTimer > pushTimerThresh)
            {
                interactID = 0;
                data.DoneBad = true;
            }


            SprintCheck();

        }


        if (transform.position.y < -5)
        {
            //restart

            gamecontrol.RestartButton();
        }
    }


    private void FixedUpdate()
    {
        if (!data.isPaused)
        {
            MovePlayer();
        }
    }



    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {

        if (!climbing)
        {

            // calculate movement direction
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;


            // on ground
            if (grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }

            // in air
            else if (!grounded)
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
            }

        }
    }


    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        //need to add doneBad

        jump.Play();
    }
    private void ResetJump()
    {
        readyToJump = true;

        data.DoneBad = true;
        interactID = 2;
    }


    public void SprintCheck()
    {
        //track time sprinting - if too much, activate badDone
        if (Input.GetKey(runKey))
        {

            //change to only be when moving?

            timeSprinting += 1;

            if (timeSprinting > timeSprintingThresh)
            {
                data.DoneBad = true;

                interactID = 1;
            }
        }

        if (Input.GetKeyUp(runKey))
        {
            timeSprinting = 0;
        }
    }

    public void Climb()
    {
        if (climbing)
        {
            //need to stop regular movement

            //if move forward, go up
            //if move backward, go down

            rb.useGravity = false;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                //move up + ignore gravity

                transform.position += new Vector3(0, climbSpeed * Time.deltaTime, 0);
            }


            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                //move down + ignore gravity

                transform.position -= new Vector3(0, climbSpeed * Time.deltaTime, 0);
            }

            //if jump

            //detach from object + fall

            if (Input.GetKeyDown(jumpKey) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                climbing = false;

                rb.useGravity = true;

                timeClimbing = 0;


                //need to also add direction launch? - poss not needed

                //calculate direction
                //provide force to that direction
            }


            //trigger bad action based on time climbing or height climbed? - or instant climb at all = bad?



            //change to only go up when moving - or when above ground
            if (transform.position.y > 0.1f)
            {
                timeClimbing += 1;
            }

            if (transform.position.y <= 0.1f)
            {
                timeClimbing = 0;
            }

            if (timeClimbing > timeClimbingThresh)
            {
                data.DoneBad = true;
                interactID = 3;
            }
        }
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Interactable" || other.gameObject.tag == "NPC")
        {
            if (!data.isPaused)
            {

                canInteract = true;

                if (other.gameObject.name == "Main Bin")
                {
                    //change to get parent object
                    objInteracted = other.gameObject.transform.parent.gameObject;
                }

                else
                {
                    objInteracted = other.gameObject;
                }
                //get from obj

                if (other.gameObject.GetComponent<NPC>() != null)
                {
                    interactID = other.gameObject.GetComponent<NPC>().ID;

                    //ow.Play();
                }

                else if (other.gameObject.GetComponent<InteractOBJ>() != null)
                {
                    interactID = other.gameObject.GetComponent<InteractOBJ>().ID;

                    //crash.Play();
                }
                // interactID = 

                //show interact UI

                if (interactUI != null)
                {
                    interactUI.SetActive(true);
                }
            }
            //same with climbing

        }


        if (other.gameObject.tag == "Climbable")
        {
            if (!data.isPaused)
            {
                //able to start climbing

                //canInteract = true;

                canClimb = true;
                //if move forwards when next to climbable - go up

                
            }

        }

    }

    /*
    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Yellow")
        {
            if (!data.isPaused)
            {
                if (gamecontrol.trainSpeed > 0) //still moving
                {
                    data.DoneBad = true;

                    interactID = 4;
                }

            }
        }

    }
    */



    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Interactable" || other.gameObject.tag == "NPC")
        {
            if (!data.isPaused)
            {

                canInteract = false;

                objInteracted = null;

                //hide interact UI

                if (interactUI != null)
                {
                    interactUI.SetActive(false);
                }

                interactID = 0;
            }

        }

        if (other.gameObject.tag == "climbable")
        {
            if (!data.isPaused)
            {
                //can't start climbing
                canClimb = false;
                //


            }

        }
    }
}




