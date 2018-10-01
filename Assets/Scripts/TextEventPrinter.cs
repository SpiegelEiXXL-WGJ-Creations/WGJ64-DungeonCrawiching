using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextEventPrinter : MonoBehaviour
{

    [Header("Assign this item(s)")]
    public UnityEngine.UI.Text textObj;
    public float TypePause = 0.01f;

    [Header("For debugging purposes:")]
    public string textPageBreakObject = "=PB=";
    public List<string> fullTexts;
    public int fullTextIdx;
    public bool isDone = false;
    public bool isCompletlyDone = false;
    public delegate void isDoneEvent();
    public event isDoneEvent currentTextBlockDoneEvent;
    public event isDoneEvent allTextBlocksDoneEvent;

    // Use this for initialization
    void Start()
    {
        if (!textObj)
            textObj = GetComponent<UnityEngine.UI.Text>();
        prepareText();
    }

    public void prepareText()
    {
        fullTexts = new List<string>();
        fullTexts.AddRange(textObj.text.Split(new string[] { textPageBreakObject }, System.StringSplitOptions.RemoveEmptyEntries));
        textObj.text = "";
        fullTextIdx = 0;
        isDone = true;
    }

    public void StartPrintingText()
    {
        if (isDone)
            StartCoroutine(PrintText());
    }

    IEnumerator PrintText()
    {
        textObj.text = "";
        isDone = false;

        foreach (char c in fullTexts[fullTextIdx])
        {
            textObj.text += c;
            yield return new WaitForSeconds(TypePause);
        }
        isDone = true;
        if (currentTextBlockDoneEvent != null)
            currentTextBlockDoneEvent();

        if (fullTextIdx < fullTexts.Count - 1)
            fullTextIdx++;
        else
        {
            isCompletlyDone = true;
            if (allTextBlocksDoneEvent != null)
                allTextBlocksDoneEvent();
        }

        yield return null;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
