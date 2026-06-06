using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractOBJ : MonoBehaviour
{

    public int ID;

    public Player player;

    public Gamecontrol gamecontrol;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerStay(Collider other) //for yellow line trigger
    {
        if (ID == 4)
        {
            if (other.gameObject.tag == "Player")
            {
                if (!data.isPaused)
                {
                    Debug.Log("Detecting");

                   // if (gamecontrol.trainSpeed > 0) //still moving
                    {
                        data.DoneBad = true;

                        player.interactID = 4;

                        Debug.Log("Over yellow");
                    }

                }
            }
        }

    }

}
