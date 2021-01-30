using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class Magnetic : MonoBehaviour
{
    public bool IsMagnetic;

    private void Awake()
    {
        IsMagnetic = true;
    }
}
