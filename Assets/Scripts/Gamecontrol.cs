using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gamecontrol : MonoBehaviour
{

    public Player player;

    public GameObject badPopup;
    public GameObject StartScreen;

    [Header("Train")]

    public GameObject train;
    public float trainSpeed;
    public float trainSpeedOrig;
    public GameObject targetTrainPos;
    public float trainSlowDist = 5f;
    public float trainSlowRate = 0.9f;



    [Header("Pause")]
    public TextMeshProUGUI pauseText;

    public GameObject pauseParent;
    public GameObject pauseOBJ;


    public Rigidbody playerRB;
    //public bool usedPause;


    [Header("Info")]

    public string[] infoTexts;
    public TextMeshProUGUI infoText;
    public int interactID;


    //ID nums:

    //0 is push past someone
    //1 is run
    //2 is jump
    //3 is climb
    //4 is yellow line
    //5 is bin
    //6 is stairs wrong way


    public GameObject[] NPCs;

    public AudioSource trainSound;

    public int slowCounter;

    public bool audioPlayed;

    public GameObject controls;

    public GameObject endScreen;


    // Start is called before the first frame update
    void Start()
    {
        data.DoneBad = false;


        if (data.Started == false)
        {
            data.isPaused = true;
            StartScreen.SetActive(true);
        }

        else
        {
            data.isPaused = false;
        }

        trainSpeedOrig = trainSpeed;


        Debug.Log("Started");

    }

    // Update is called once per frame
    void Update()
    {

        interactID = player.interactID;

        if (data.DoneBad)
        {
            badPopup.SetActive(true);

            //pause

            data.isPaused = true;

            pauseOBJ.SetActive(false);

           // PauseButton();


            //set info text



            //need to change to correct string based on what interacted with - obj name, layer or tag?
            //poss use if contains on name strings?



           // infoText.SetText(infoTexts[0]);

            infoText.SetText(infoTexts[interactID]);


        }

        else
        {
            badPopup.SetActive(false);

           // PauseButton();
           // data.isPaused = false;


        }

        //no pause if done bad

        if (!data.DoneBad)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PauseButton();
            }
        }


        if (!data.isPaused)
        {

            Train();
        }
    }


    public void Train()
    {

        //spawn in train?
        //train comes  through in x time - keep moving until reach targetPos

        if (train != null)
        {
            train.transform.position += train.transform.right * trainSpeed * Time.deltaTime;


            if (Vector3.Distance(train.transform.position, targetTrainPos.transform.position) < trainSlowDist)
            {
                //slow down

                slowCounter += 1;

                //trainSpeed = trainSpeed * trainSlowRate;

                if (slowCounter % 2 == 0)
                {
                    trainSpeed = trainSpeed * trainSlowRate;
                }
            }

            if (trainSpeed < 0.05f)
            {
                trainSpeed = 0;
            }
        }


        if (trainSpeed < 0)
        {
            trainSpeed = 0;
        }

        if (trainSpeed == 0)
        {
            //train here
            if (!audioPlayed)
            {
                trainSound.Play();

                audioPlayed = true;
            }


            //when train gets close, slow down
            //train stops at station

            //train moves away after y time?



            //when train stopped, NPCs can go to train

            //for loop, NPC set trainHere to true

            for (int i = 0; i < NPCs.Length; i++)
            {
                NPCs[i].GetComponent<NPC>().trainHere = true;
            }
        }

        else
        {
            audioPlayed = false;
        }
    }


    //part of done bad popup
    public void resetButton()
    {
        //reload scene? - or start from checkpoint


        //reload scene option

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void PauseButton()
    {

        if (!data.DoneBad)
        {
            data.isPaused = !data.isPaused;

            //unpause
            if (!data.isPaused)
            {

                pauseText.SetText("||");
                Debug.Log("playing");

                pauseParent.SetActive(false);

                playerRB.useGravity = true;

                controls.SetActive(false);
            }

            //pause
            else if (data.isPaused)
            {

                pauseText.SetText("X");
                Debug.Log("paused");

                pauseParent.SetActive(true);
            }
        }
    }

    public void StartButton()
    {
        StartScreen.SetActive(false);

        data.isPaused = false;
        data.Started = true;

        playerRB.useGravity = true;

        //enable pause button

        pauseOBJ.SetActive(true);
    }


    public void QuitButton()
    {
        //close game

        Application.Quit();
    }

    public void RestartButton()
    {
        resetButton();
    }

    public void ResumeButton()
    {
        //unpause

        PauseButton();
    }


    public void ControlsButton()
    {
        controls.SetActive(true);
    }

    public void CloseControlsButton()
    {
        controls.SetActive(false);
    }
}

