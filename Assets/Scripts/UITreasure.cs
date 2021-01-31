using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class UITreasure : MonoBehaviour
{
    public void CloseUI()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit") == true) 
        {
            CloseUI();
        }      
    }
}
