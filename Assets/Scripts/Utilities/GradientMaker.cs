using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientMaker : MonoBehaviour
{
    [System.Serializable]
    public enum Direction
    {
        UpToDown,
        DownToUp,
        LeftToRight,
        RightToLeft,
        CenterToOutside
    }
    public Gradient grad;
    public Direction gradDirection;
    public UnityEngine.UI.Image imgReference;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
