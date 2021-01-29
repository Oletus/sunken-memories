using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class SceneLoader: MonoBehaviour
{
    public void LoadScene(int sceneId) 
    {
        SceneManager.LoadScene(sceneId);
    }
}
