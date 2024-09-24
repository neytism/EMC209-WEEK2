using System.Collections;
using System.Collections.Generic;
using Fusion;
using GNW.PlayerController;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            col.GetComponent<Player>().ReachGoal();
            GetComponent<Collider>().enabled = false;
        }
    }
}
