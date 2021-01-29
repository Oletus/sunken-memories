using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class FishingLine : MonoBehaviour
{
    [SerializeField] private Rigidbody2D RopeSectionTemplate;

    void Awake ()
    {
        
    }

    void Update ()
    {
        if ( Input.GetButton("FishingLineVertical") )
        {
            Debug.Log("Foobar!");
        }
    }
}
