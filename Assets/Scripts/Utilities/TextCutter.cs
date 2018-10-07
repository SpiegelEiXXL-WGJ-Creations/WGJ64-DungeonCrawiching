using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextCutter : MonoBehaviour
{
    public GameObject cutterSpace;
    public HorizontalLayoutGroup gl;
    public Text baseText;
    public float CutterSpaceSpacingCorrection = -70f;
    public TextAnchor CutterSpaceLetterTextAlignment = TextAnchor.UpperRight;
    public MonoBehaviour AdditionalScriptToAttachToLetter;


    // Use this for initialization
    void Start()
    {
        if (!baseText)
            baseText = GetComponent<Text>();

        cutterSpace = new GameObject("CutterSpace");
        cutterSpace.transform.SetParent(this.transform);
        cutterSpace.transform.localPosition = new Vector3(0f, 0f, 0f);
        cutterSpace.AddComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
        baseText.enabled = false;
        gl = cutterSpace.AddComponent<HorizontalLayoutGroup>();
        gl.spacing = CutterSpaceSpacingCorrection;
        string templateData = System.Text.RegularExpressions.Regex.Replace(UnityEngine.JsonUtility.ToJson(AdditionalScriptToAttachToLetter), @",?""[^""]*"":\{""instanceID"":[^\}]*\},?", "");
        foreach (char c in baseText.text)
        {
            GameObject g = new GameObject("Cut: " + c);
            Text t = g.AddComponent<Text>();
            t.font = baseText.font;
            t.fontSize = baseText.fontSize;
            t.text = c.ToString();
            t.alignment = CutterSpaceLetterTextAlignment;
            g.transform.SetParent(cutterSpace.transform);
            if (AdditionalScriptToAttachToLetter)
            {
                Object o = g.AddComponent(AdditionalScriptToAttachToLetter.GetType());
                Newtonsoft.Json.JsonConvert.PopulateObject(templateData, o);

            }
            //GridLayoutGroup glg = g.AddComponent<>();
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
