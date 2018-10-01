using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // static elemens for reference purposes
    public static GameManager instance;

    [Header("Prefabs")]
    public GameObject GameGrid;
    public GameObject LayerPrefab;
    public GameObject TilePrefab;

    [Header("Settings>Fixed stage")]
    public string mapToLoad;


    [Header("Settings>Random stage")]
    public int NumberOfLayers = 3;
    public int mapWidth = 15;
    public int mapHeight = 15;

    [Header("Debug view (no touchy!)")]
    public bool isBusy = false; // while enemy is doing attacks
    public bool mapIsInitialized;
    public float cellWidth;
    public float cellHeight;
    public List<LayerController> layers;
    public PlayerScript playerScript;

    // Events
    public delegate void finishedMapSpawning();
    public event finishedMapSpawning mapSpawnDone;

    // Use this for initialization
    public GameManager()
    {
        if (!GameManager.instance)
            GameManager.instance = this;
        else
            Debug.LogError("! Game Manager is already instanciated (statically), what are you doing?");

    }
    void Start()
    {
        initGameManager();
        initMap();
    }

    void initGameManager()
    {
        // <setup ALL THE STUFF>
        playerScript = GetComponent<PlayerScript>();

        Sprite t = TilePrefab.GetComponent<SpriteRenderer>().sprite;
        cellHeight = t.textureRect.height / t.pixelsPerUnit;
        cellWidth = t.textureRect.width / t.pixelsPerUnit;

        GameGrid.GetComponent<RectTransform>().sizeDelta = new Vector3(mapWidth * cellWidth, mapHeight * cellHeight);
        // </setup>
    }

    void initMap()
    {
        mapIsInitialized = false;

        // non-static map load
        if (mapToLoad == "")
        {
            layers = new List<LayerController>();
            for (int i = 0; i < NumberOfLayers; i++)
            {
                layers.Add(GameObject.Instantiate(LayerPrefab, GameGrid.transform).GetComponent<LayerController>());
            }
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].layerWidth = mapWidth;
                layers[i].layerHeight = mapHeight;

                layers[i].name = "Layer#" + i;
                layers[i].GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize = new Vector3(cellHeight, cellWidth);
            }

            for (int i = 0; i < layers.Count; i++)
                layers[i].BuildLayer(i > 0 ? layers[i - 1] : null, i == layers.Count - 1 ? null : layers[i + 1]);

        }

        // static map load
        if (mapToLoad != "")
            StaticUtilitiesFunction.LoadMapFromFile(mapToLoad, TilePrefab);
        if (mapSpawnDone != null)
            mapSpawnDone();

        mapIsInitialized = true;

    }

    // Update is called once per frame
    void Update()
    {

    }
}
