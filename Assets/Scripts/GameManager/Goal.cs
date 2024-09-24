using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW.PlayerController;
using UnityEngine;

public class Goal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Player>().ReachGoal();
            GetComponent<Collider>().enabled = false;
        }
    }
}
