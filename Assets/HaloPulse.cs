using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaloPulse : MonoBehaviour
{
    public float currHaloSize = 0f;
    public float currMaxHaloSize = 0f;

    public float minHaloSize = .25f;
    public float maxHaloSize = .75f;
    public float haloTime = 1f;
    public Light haloObject;

    // Use this for initialization
    void Start()
    {
        haloObject.intensity = minHaloSize;
        currHaloSize = minHaloSize;
        currMaxHaloSize = maxHaloSize;
    }

    // Update is called once per frame
    void Update()
    {

        currHaloSize = Mathf.Lerp(currHaloSize, currMaxHaloSize, haloTime);
        if (Mathf.Abs(currMaxHaloSize - currHaloSize) <= 0.001f)
        {
            if (currMaxHaloSize == maxHaloSize) currMaxHaloSize = minHaloSize;
            else currMaxHaloSize = maxHaloSize;
        }
        haloObject.intensity = currHaloSize;
    }
}
