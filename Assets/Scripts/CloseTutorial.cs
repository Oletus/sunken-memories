using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649


public class CloseTutorial : MonoBehaviour
{
    public GameObject Tutorial;
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        Tutorial.SetActive(false);
    }
}
