using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649

public class Magnetic : MonoBehaviour
{
    public int TreasureID = -1;

    [ColorUsage(true, true)]
    public Color color;
    [ColorUsage(true, true)]
    public Color color2;
    public Renderer objectRenderer;

    private GameObject Magnet;

    public bool IsMagnetic { get; set; }

    private void Awake()
    {
        IsMagnetic = true;
        Magnet = GameObject.Find("Magnet");
        
        if (objectRenderer == null)
        {
            return;
        }
        //objectRenderer.sharedMaterial.SetColor("_Color", color);
        objectRenderer.material.SetColor("_EmissionColor", color);
        objectRenderer.material.SetColor("_BaseColor", color);

    }

    private void Update()
    {

        float distance = Vector3.Distance(gameObject.transform.position, Magnet.transform.position);

        if (objectRenderer == null)
        {
            return;
        }
        if (distance < 10)
        {
            float t = Mathf.InverseLerp(10, 2, distance);
            Color color3 = Color.Lerp(color, color2, t);

            objectRenderer.material.SetColor("_EmissionColor", color3);
        }

    }
}
