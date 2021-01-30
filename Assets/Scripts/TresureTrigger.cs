using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class TresureTrigger : MonoBehaviour
{
    public List<GameObject> WinScreen;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Magnetic>() != null) 
        {
            int treasureID = collision.GetComponent<Magnetic>().TreasureID;
            WinScreen[treasureID].SetActive(true);
        }
        

    }
}
