using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextJumper : MonoBehaviour
{
    public float jumpiness = 5f;
    public float jumpTime = 1f;
    public bool jumping = false;
    public bool falling = false;
    public float cooldown = 0f;
    public float cooldownRecharge = 2f;
    public float yTarget = 0f;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!jumping && !falling && cooldown <= 0f)
        {
            yTarget = this.transform.localPosition.y + jumpiness;
            jumping = true;
        }
        else if (jumping || falling)
        {
            this.transform.localPosition = new Vector3(this.transform.localPosition.x, Mathf.Lerp(this.transform.localPosition.y, yTarget, jumpTime / 2f), this.transform.localPosition.z);
            if (jumping && this.transform.localPosition.y >= yTarget - 0.001f)
            {
                yTarget = yTarget - jumpiness;
                jumping = false;
                falling = true;
            }
            if (falling && this.transform.localPosition.y <= yTarget + 0.001f)
            {
                falling = false;
                cooldown = cooldownRecharge;
            }

        }
        else
            cooldown = cooldown - Time.deltaTime;

    }
}
