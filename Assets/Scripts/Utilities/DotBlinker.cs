using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBlinker : MonoBehaviour
{
    public float stepTime = 0.2f;
    public string fullText = "...";
    public UnityEngine.UI.Text tElem;

    // Use this for initialization
    void Start()
    {
        tElem = GetComponent<UnityEngine.UI.Text>();
        StartCoroutine(TextBlink());
    }
    IEnumerator TextBlink()
    {
        while (true)
        {
            tElem.text = "";
            foreach (char i in fullText)
            {
                tElem.text += i;
                yield return new WaitForSeconds(stepTime);
            }
            yield return new WaitForSeconds(stepTime);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
