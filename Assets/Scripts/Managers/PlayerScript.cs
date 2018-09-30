using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    [Header("Setup this stuff:")]

    [Header("Game Objects")]
    public GameObject HPBar;
    public GameObject LevelUIObject;

    [Header("Animations")]
    public string idleAnimation;
    public string attackAnimation;
    public string walkAnimation;
    public string dyingAnimation;

    [Header("Configuration")]
    public float zPos;
    public float animationSpeed;
    public int maxHP;
    public int maxLevel;

    [Header("You can look at this stuff")]
    public GameManager gm;
    public GameObject player;
    public Animator playerAnimator;
    public SpriteRenderer playerSprite;
    public Vector3 targetPosition;

    [Header("States")]
    public bool isWalking;
    public bool isAttacking;
    public bool isDying;

    public int mapX;
    public int mapY;
    public int _currentHP;
    public int _currentLevel;

    public UnityEngine.UI.Image HPBarFilling;
    public UnityEngine.UI.Text HPBarText;
    public UnityEngine.UI.Text LevelUIObjectText;

    public int currentHP
    {
        get
        {
            return _currentHP;
        }
        set
        {
            _currentHP = value;
            if (_currentHP <= 0)
            {
                _currentHP = 0;
                isDying = true;
            }

            HPBarFilling.fillAmount = currentHP / maxHP;
            HPBarText.text = currentHP + " / " + maxHP;
        }
    }
    public int currentLevel
    {
        get
        {
            return _currentLevel;
        }
        set
        {
            _currentLevel = value;
            LevelUIObjectText.text = "Lv " + _currentLevel;
        }
    }


    // Use this for initialization
    void Start()
    {
        gm = GameManager.instance;
        player = GameObject.FindWithTag("PlayerObject");
        playerAnimator = player.GetComponent<Animator>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        gm.mapSpawnDone += gameManager_MapSpawningDone;
        if (HPBar)
        {
            HPBarText = HPBar.GetComponentInChildren<UnityEngine.UI.Text>();
            HPBarFilling = GameObject.FindWithTag("UI_HPBar").GetComponent<UnityEngine.UI.Image>();
        }
        if (LevelUIObject)
            LevelUIObjectText = LevelUIObject.GetComponentInChildren<UnityEngine.UI.Text>();

        if (_currentHP == 0)
            _currentHP = maxHP;
        if (_currentLevel == 0)
            _currentLevel = 1;

        currentHP = _currentHP;
        currentLevel = _currentLevel;

    }

    void gameManager_MapSpawningDone()
    {
        player.transform.localPosition = new Vector3(gm.cellWidth / 2f, gm.cellHeight / 2f, zPos);
        mapX = 0;
        mapY = gm.mapHeight - 1;
    }


    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            player.transform.localPosition = Vector3.MoveTowards(player.transform.localPosition, targetPosition, animationSpeed);
            if (Vector3.Distance(player.transform.localPosition, targetPosition) <= 0f)
            {
                isWalking = false;
                playerAnimator.SetBool("isWalking", false);
                playerAnimator.Play(idleAnimation);
            }
        }
        if (isAttacking)
        {
            if (playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name != attackAnimation)
                isAttacking = false;
        }
    }
    private void FixedUpdate()
    {
        if (!isWalking && !isAttacking && !isDying && !gm.isBusy)
            if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0f)
            {
                targetPosition = new Vector3(player.transform.localPosition.x + gm.cellWidth, player.transform.localPosition.y, zPos);

                playerAnimator.SetBool("isWalking", true);
                playerAnimator.Play(walkAnimation);
                isWalking = true;
                playerSprite.flipX = false;
                mapX++;
            }
            else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0f)
            {
                targetPosition = new Vector3(player.transform.localPosition.x - gm.cellWidth, player.transform.localPosition.y, zPos);

                playerAnimator.SetBool("isWalking", true);
                playerAnimator.Play(walkAnimation);
                isWalking = true;
                playerSprite.flipX = true;
                mapX--;
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0f)
            {

                targetPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y + gm.cellHeight, zPos);
                playerAnimator.SetBool("isWalking", true);
                playerAnimator.Play(walkAnimation);
                isWalking = true;
                mapY--;
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0f)
            {
                targetPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y - gm.cellHeight, zPos);
                playerAnimator.SetBool("isWalking", true);
                playerAnimator.Play(walkAnimation);
                isWalking = true;
                mapY++;
            }
            else if (Input.GetButton("Fire1"))
            {
                //playerAnimator.SetBool("isAttacking", true);
                playerAnimator.Play(attackAnimation);
                isAttacking = true;
            }
    }
}
