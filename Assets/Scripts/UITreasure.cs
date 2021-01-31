using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class UITreasure : MonoBehaviour
{
    public static bool TreasureUIOn;

    public IEnumerator CloseUI()
    {
        // Wait for one frame so that magnet sound is disabled.
        yield return null;
        if (TresureTrigger.AllTreasuresFound)
        {
            SceneManager.LoadScene("EndScene");
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        TreasureUIOn = true;
    }

    private void OnDisable()
    {
        TreasureUIOn = false;
    }

    private void Update()
    {
        if ( Input.GetButtonDown("Submit") == true ) 
        {
            StartCoroutine(CloseUI());
        }      
    }
}
