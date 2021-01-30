using System.Collections.Generic;
using System.Collections;
using UnityEngine;

// Unity generates a lot of spurious "never assigned" warnings for serialized variables, so disable those.
#pragma warning disable 649


public class LerpFollow : MonoBehaviour
{
    public GameObject target;
    public float speed;

    static float t = 0.0f;

    void Update ()
    {
        t = Time.deltaTime / speed;
        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, target.transform.position, t);
    }
}
