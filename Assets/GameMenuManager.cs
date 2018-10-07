using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuManager : MonoBehaviour
{
    [Header("Assignables")]
    public GameObject textPrefab;
    public GameObject menuContainerPrefab;
    public GameObject selectPrefab;
    public GameObject menuListPrefab;
    public GameObject parentingObject;
    public List<object> menuItems;
    public Vector3 positionOfMenu;
    public float baseYPos = -10f;
    public float yPosPerItem = -15f;
    public float transitTime = 0.05f;
    public bool autoInit = false;

    [Header("State/Instance objects")]
    public Dictionary<int, GameObject> menuItemsInstanced;
    public GameObject menuCursor;
    public GameObject menuObject;
    public GameObject menuList;
    public int selectedItem = 0;
    public float newTargetYPos;
    public bool isTransitioning;


    //public GameMenuManager nestedManager;

    public delegate void MenuClosed();
    public delegate void MenuItemSelected(string menuItemText, GameObject menuItem, object menuItemObject);
    public event MenuClosed MenuHasClosed;
    public event MenuItemSelected MenuItemWasSelected;


    // Use this for initialization
    void Start()
    {
        if (autoInit)
            InitMenu();
    }

    public void takeOverValuesFromTemplate(GameMenuManager src)
    {
        textPrefab = src.textPrefab;
        menuContainerPrefab = src.menuContainerPrefab;
        selectPrefab = src.selectPrefab;
        menuListPrefab = src.menuListPrefab;
        parentingObject = src.parentingObject;
        positionOfMenu = src.positionOfMenu;
        baseYPos = src.baseYPos;
        yPosPerItem = src.yPosPerItem;
        transitTime = src.transitTime;
    }
    public void InitMenu()
    {
        menuObject = GameObject.Instantiate(menuContainerPrefab, parentingObject.transform);
        menuCursor = GameObject.Instantiate(selectPrefab, menuObject.transform);
        menuList = GameObject.Instantiate(menuListPrefab, menuObject.transform);

        menuObject.GetComponent<RectTransform>().anchoredPosition = positionOfMenu;
        menuItemsInstanced = new Dictionary<int, GameObject>();
        int idx = 0;
        foreach (object so in menuItems)
        {
            GameObject o = GameObject.Instantiate(textPrefab);
            o.GetComponent<Text>().text = so.ToString();
            o.transform.SetParent(menuList.transform);
            menuItemsInstanced.Add(idx, o);
            idx++;
        }
        menuObject.SetActive(false);

        //init position
        menuCursor.transform.localPosition = new Vector3(menuCursor.transform.localPosition.x, selectedItem * yPosPerItem + baseYPos, menuCursor.transform.localPosition.z);

    }

    // Update is called once per frame
    void Update()
    {
        if (isTransitioning)
        {
            menuCursor.transform.localPosition = new Vector3(menuCursor.transform.localPosition.x, Mathf.SmoothStep(menuCursor.transform.localPosition.y, newTargetYPos, transitTime), menuCursor.transform.localPosition.z);
            if (Mathf.Abs(menuCursor.transform.localPosition.y - newTargetYPos) < 0.01f)
            {
                // sufficient close
                isTransitioning = false;
            }
        }
    }

    public void processInput()
    {
        if (isTransitioning)
            return;
        if (Input.GetButtonDown("Fire1"))
        {
            if (MenuItemWasSelected != null)
                MenuItemWasSelected(menuItemsInstanced[selectedItem].GetComponent<Text>().text, menuItemsInstanced[selectedItem], menuItems[selectedItem]);
            // select
        }
        if (Input.GetButtonDown("Fire2"))
        {
            menuObject.SetActive(false);
            if (MenuHasClosed != null)
                MenuHasClosed();
        }
        if ((Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") < 0f))
        {
            if (selectedItem < menuItems.Count - 1)
            {
                selectedItem++;
                newTargetYPos = selectedItem * yPosPerItem + baseYPos;
                isTransitioning = true;
            }
        }
        if (Input.GetButton("Vertical") && Input.GetAxisRaw("Vertical") > 0f)
        {
            if (selectedItem > 0)
            {
                selectedItem--;
                newTargetYPos = selectedItem * yPosPerItem + baseYPos;
                isTransitioning = true;
            }
        }
    }

    private void FixedUpdate()
    {

    }
    private void OnDestroy()
    {
        if (menuList)
            GameObject.Destroy(menuList);
        if (menuCursor)
            GameObject.Destroy(menuCursor);
        if (menuObject)
            GameObject.Destroy(menuObject);
    }


}

