using UnityEngine;
using System.Collections;

public class TextGlitcher : MonoBehaviour
{
    public float yTargetLow;
    public float yTargetHigh;
    public float range = 3f;
    public float cooldown = 0f;
    public float cooldownMin = 2f;
    public float cooldownMax = 4f;
    private bool firstTime = true;
    private RectTransform rectT;

    // Use this for initialization
    void Start()
    {
        rectT = this.GetComponent<RectTransform>();
        Invoke("firstTimeInit", .5f);
    }

    void firstTimeInit()
    {
        yTargetLow = rectT.anchoredPosition.y + Random.Range(0, range);
        yTargetHigh = rectT.anchoredPosition.y - Random.Range(0, range);
        rectT.anchoredPosition = new Vector3(rectT.anchoredPosition.x, Random.Range(yTargetLow, yTargetHigh));
        firstTime = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (firstTime)
        {
            return;
        }
        if (cooldown <= 0f)
        {
            rectT.anchoredPosition = new Vector3(rectT.anchoredPosition.x, Random.Range(yTargetLow, yTargetHigh));
            cooldown = Random.Range(cooldownMin, cooldownMax);
        }
        else
            cooldown = cooldown - Time.deltaTime;

    }
}
