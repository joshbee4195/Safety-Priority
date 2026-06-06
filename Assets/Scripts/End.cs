using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{

    public GameObject endScreen;

    public GameObject pause;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            data.ended = true;

            Debug.Log("Ended");

            data.isPaused = true;
            endScreen.SetActive(true);

            pause.SetActive(false);
        }
    }
}
