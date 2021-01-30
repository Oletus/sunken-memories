using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class TresureTrigger : MonoBehaviour
{
    [SerializeField] private List<GameObject> WinScreen;

    [SerializeField] private Magnet Magnet;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Magnetic magnetic = collision.GetComponent<Magnetic>();
        if (magnetic == null)
        {
            return;
        }
        int treasureID = magnetic.TreasureID;
        if (treasureID < 0 || treasureID >= WinScreen.Count)
        {
            return;
        }
        WinScreen[treasureID].SetActive(true);

        Magnet.RemoveMagnetic(magnetic);
    }
}