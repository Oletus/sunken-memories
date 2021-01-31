using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class ExitGame : MonoBehaviour
{
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }
}
