using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    [Header("Setup this stuff:")]

    [Header("Game Objects")]
    public GameObject HPBar;
    public GameObject LevelUIObject;
    public GameObject LowHPAlert;

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
    public bool cheatMode;
    public int maxInvetorySpace = 10;

    [Header("You can look at this stuff")]
    public int mapX;
    public int mapY;
    public int _currentHP;
    public int _currentLevel;
    public GameManager gm;
    public GameObject player;
    public RectTransform playerRect;
    public Animator playerAnimator;
    public SpriteRenderer playerSprite;
    public Vector3 targetPosition;
    public UnityEngine.UI.Image HPBarFilling;
    public UnityEngine.UI.Text HPBarText;
    public UnityEngine.UI.Text LevelUIObjectText;
    public List<ItemScript> playerInventory;
    public GameMenuManager gameMenuManager;
    public GameObject playerInventoryMenu;
    public GameObject playerInventoryMenuItemUseMenu;
    public GameMenuManager activeMenu;
    public ItemScript selectedInventoryItem;

    [Header("States")]
    public bool isWalking;
    public bool isAttacking;
    public bool isDying;
    public bool inMenu;


    // Getters & Setters
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
            if (_currentHP <= maxHP / 10)
                LowHPAlert.SetActive(true);
            else
                LowHPAlert.SetActive(false);

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
        player.transform.localPosition = new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, zPos);
        playerAnimator = player.GetComponent<Animator>();
        playerSprite = player.GetComponent<SpriteRenderer>();
        playerRect = player.GetComponent<RectTransform>();
        gm.mapSpawnDone += gameManager_MapSpawningDone;
        if (gm.mapIsInitialized)
            gameManager_MapSpawningDone();


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

        playerInventory = new List<ItemScript>();
        if (!gameMenuManager)
            gameMenuManager = this.GetComponentInChildren<GameMenuManager>();

        gameMenuManager.MenuHasClosed += PlayerMenuClosed;
        gameMenuManager.MenuItemWasSelected += PlayerMenuItemSelected;
        gameMenuManager.menuItems = new List<object>();
        gameMenuManager.menuItems.Add("Items");
        gameMenuManager.menuItems.Add("Status");
        gameMenuManager.menuItems.Add("Save");
        gameMenuManager.InitMenu();
    }

    void PlayerMenuItemSelected(string menuItemText, GameObject menuItem, object menuObj)
    {
        Debug.Log("Menu Item was selected! " + menuItemText);
        if (menuItemText == "Items")
        {
            if (playerInventoryMenu != null)
                GameObject.DestroyImmediate(playerInventoryMenu);

            playerInventoryMenu = new GameObject("inventoryMenu");
            GameMenuManager m = playerInventoryMenu.AddComponent<GameMenuManager>();
            m.takeOverValuesFromTemplate(gameMenuManager);
            m.menuItems = new List<object>();
            foreach (ItemScript i in playerInventory)
            {
                m.menuItems.Add(i);

            }
            if (playerInventory.Count == 0)
                m.menuItems.Add("<no items>");
            m.positionOfMenu.Set(m.positionOfMenu.x + 50f, m.positionOfMenu.y - 20f, m.positionOfMenu.z - 1f);
            m.InitMenu();
            m.menuObject.SetActive(true);
            m.MenuHasClosed += InventoryItemListMenuHasClosed;
            m.MenuItemWasSelected += InventoryItemListMenuItemWasSelected;
            activeMenu = m;
        }
    }

    void InventoryItemListMenuItemWasSelected(string menuItemText, GameObject menuItem, object menuObj)
    {
        if (menuItemText == "<no items>")
        {
            InventoryItemListMenuHasClosed();
            return;
        }
        selectedInventoryItem = (ItemScript)menuObj;

        if (playerInventoryMenuItemUseMenu != null)
            GameObject.DestroyImmediate(playerInventoryMenuItemUseMenu);

        playerInventoryMenuItemUseMenu = new GameObject("itemUseMenu");
        GameMenuManager m = playerInventoryMenuItemUseMenu.AddComponent<GameMenuManager>();
        m.takeOverValuesFromTemplate(playerInventoryMenu.GetComponent<GameMenuManager>());
        m.menuItems = new List<object>();

        if (selectedInventoryItem.itemType == ItemType.Usable)
        {
            m.menuItems.Add("Use");
        }
        if (selectedInventoryItem.itemType == ItemType.Equipable)
            m.menuItems.Add("Equip");

        if (selectedInventoryItem.itemType == ItemType.EventItems)
            m.menuItems.Add("???");

        m.menuItems.Add("Drop");
        m.positionOfMenu.Set(m.positionOfMenu.x + 50f, m.positionOfMenu.y - 20f, m.positionOfMenu.z - 1f);
        m.InitMenu();
        m.menuObject.SetActive(true);
        m.MenuHasClosed += InventoryItemListMenuItemSelectionClosed;
        m.MenuItemWasSelected += InventoryItemListMenuItemSelectionActionChosen;
        activeMenu = m;
    }

    void InventoryItemListMenuItemSelectionActionChosen(string menuItemText, GameObject menuItem, object menuItemObject)
    {
        if (menuItemText == "Drop")
        {
            Debug.Log("Not implemented. :-(");
        }
        if (menuItemText == "Equip")
        {
            Debug.Log("Not implemented. :-(");
        }
        if (menuItemText == "Use")
        {
            if (selectedInventoryItem.itemEffect == ItemEffect.Healing)
                currentHP = currentHP + selectedInventoryItem.itemEffectValue;
            else
                Debug.Log("Not implemented. :-(");

            playerInventory.Remove(selectedInventoryItem);
            GameObject.Destroy(selectedInventoryItem);

            /*playerInventoryMenu.SetActive(false);
            playerInventoryMenuItemUseMenu.SetActive(false);
            gameMenuManager.gameObject.SetActive(false);
            activeMenu = null;
            inMenu = false;*/
            InventoryItemListMenuItemSelectionClosed();
            InventoryItemListMenuHasClosed();
            PlayerMenuClosed();
        }
        if (menuItemText == "???")
        {
            Debug.Log("Not implemented. :-(");
        }

    }


    void InventoryItemListMenuItemSelectionClosed()
    {
        //playerInventoryMenuItemUseMenu.SetActive(false);
        playerInventoryMenuItemUseMenu.GetComponent<GameMenuManager>().menuObject.SetActive(false);
        activeMenu = playerInventoryMenu.GetComponent<GameMenuManager>();
    }


    void InventoryItemListMenuHasClosed()
    {
        activeMenu = gameMenuManager;
        playerInventoryMenu.GetComponent<GameMenuManager>().menuObject.SetActive(false);
    }


    void PlayerMenuClosed()
    {
        inMenu = false;
        gameMenuManager.GetComponent<GameMenuManager>().menuObject.SetActive(false);
    }


    void gameManager_MapSpawningDone()
    {
        Coordinate2D o = gm.MapGetPlayerSpawnTrigger();
        if (o == null)
        {
            mapX = 0;
            mapY = gm.mapHeight - 1;

        }
        else
        {
            mapX = o.x;
            mapY = o.y;

        }

        playerRect.anchoredPosition = new Vector3(gm.cellWidth * mapX + gm.cellWidth / 2f, -1 * mapY * gm.cellHeight + -1f * gm.cellHeight / 2f, zPos);

    }


    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            playerRect.localPosition = Vector3.MoveTowards(playerRect.localPosition, targetPosition, animationSpeed);
            if (Vector3.Distance(playerRect.localPosition, targetPosition) <= 0f)
            {
                isWalking = false;
                playerAnimator.SetBool("isWalking", false);
                playerAnimator.Play(idleAnimation);
                gm.evaluateTile(mapX, mapY);
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
        if (!isWalking && !isAttacking && !isDying && !gm.isBusy && !inMenu)
        {
            if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") > 0f)
            {
                if (gm.checkIfMoveableTile(mapX + 1, mapY, GameManagerGameTypeEnum.Player) || cheatMode)
                {

                    targetPosition = new Vector3(playerRect.localPosition.x + gm.cellWidth, playerRect.localPosition.y, zPos);

                    playerAnimator.SetBool("isWalking", true);
                    playerAnimator.Play(walkAnimation);
                    isWalking = true;
                    playerSprite.flipX = false;
                    mapX++;
                }
            }
            else if (Input.GetButton("Horizontal") && Input.GetAxisRaw("Horizontal") < 0f)
            {
                if (gm.checkIfMoveableTile(mapX - 1, mapY, GameManagerGameTypeEnum.Player) || cheatMode)
                {
                    targetPosition = new Vector3(playerRect.localPosition.x - gm.cellWidth, playerRect.localPosition.y, zPos);

                    playerAnimator.SetBool("isWalking", true);
                    playerAnimator.Play(walkAnimation);
                    isWalking = true;
                    playerSprite.flipX = true;
                    mapX--;
                }
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0f)
            {
                if (gm.checkIfMoveableTile(mapX, mapY - 1, GameManagerGameTypeEnum.Player) || cheatMode)
                {
                    targetPosition = new Vector3(playerRect.localPosition.x, playerRect.localPosition.y + gm.cellHeight, zPos);
                    playerAnimator.SetBool("isWalking", true);
                    playerAnimator.Play(walkAnimation);
                    isWalking = true;
                    mapY--;
                }
            }
            else if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0f)
            {
                if (gm.checkIfMoveableTile(mapX, mapY + 1, GameManagerGameTypeEnum.Player) || cheatMode)
                {
                    targetPosition = new Vector3(playerRect.localPosition.x, playerRect.localPosition.y - gm.cellHeight, zPos);
                    playerAnimator.SetBool("isWalking", true);
                    playerAnimator.Play(walkAnimation);
                    isWalking = true;
                    mapY++;
                }
            }
            else if (Input.GetButton("Fire1"))
            {
                //playerAnimator.SetBool("isAttacking", true);
                playerAnimator.Play(attackAnimation);
                playerAnimator.GetComponentInChildren<AudioSource>().Play();
                isAttacking = true;
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                inMenu = true;
                gameMenuManager.menuObject.SetActive(true);
                activeMenu = gameMenuManager;
            }
        }
        else if (inMenu)
        {
            activeMenu.processInput();
        }

    }
}

