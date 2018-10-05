using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameManagerGameTypeEnum
{
    Player,
    Enemy,
    Glitch,
    PlayerAndEnemy,
    PlayerAndEnemyAndGlitch,
    EnemyAndGlitch,
    EnemyAndPlayer
}

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

    public void initGameManager()
    {
        // <setup ALL THE STUFF>
        playerScript = GetComponent<PlayerScript>();

        Sprite t = TilePrefab.GetComponent<SpriteRenderer>().sprite;
        cellHeight = t.textureRect.height / t.pixelsPerUnit;
        cellWidth = t.textureRect.width / t.pixelsPerUnit;

        GameGrid.GetComponent<RectTransform>().sizeDelta = new Vector3(mapWidth * cellWidth, mapHeight * cellHeight);
        // </setup>
    }

    public void initLayer(int layerIndex, bool build = false)
    {
        layers[layerIndex].layerWidth = mapWidth;
        layers[layerIndex].layerHeight = mapHeight;

        layers[layerIndex].name = "Layer#" + layerIndex;

        layers[layerIndex].transform.Translate(0f, 0f, -1 * layerIndex);
        layers[layerIndex].GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize = new Vector3(cellHeight, cellWidth);

        if (build)
            layers[layerIndex].BuildLayer(layerIndex > 0 ? layers[layerIndex - 1] : null, layerIndex == layers.Count - 1 ? null : layers[layerIndex + 1]);
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
                layers[i].transform.Translate(0f, 0f, -1f * i);
            }
            for (int i = 0; i < layers.Count; i++)
            {
                initLayer(i, false);
            }
            for (int i = 0; i < layers.Count; i++)
                layers[i].BuildLayer(i > 0 ? layers[i - 1] : null, i == layers.Count - 1 ? null : layers[i + 1]);

        }

        // static map load
        if (mapToLoad != "")
            StaticUtilitiesFunction.LoadMapFromResources(mapToLoad, TilePrefab);

        triggerMapLoadingEvent();

        mapIsInitialized = true;

    }

    public void triggerMapLoadingEvent()
    {
        if (mapSpawnDone != null)
            mapSpawnDone();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool checkIfMoveableTile(int x, int y, GameManagerGameTypeEnum checkType)
    {
        if (x >= mapWidth || y >= mapHeight) return false;

        bool totalTileCheck = true;
        foreach (LayerController l in layers)
        {
            TileController t = l.tiles[Coordinate2D.Is(x, y)];
            if (checkType == GameManagerGameTypeEnum.Player)
                totalTileCheck = totalTileCheck && t.walkableByPlayer;
            else if (checkType == GameManagerGameTypeEnum.Enemy)
                totalTileCheck = totalTileCheck && t.walkableByEnemy;
            else if (checkType == GameManagerGameTypeEnum.Glitch)
                totalTileCheck = totalTileCheck && t.walkableByGlitch;
        }
        return totalTileCheck;
    }

    public Coordinate2D MapGetPlayerSpawnTrigger()
    {

        foreach (LayerController l in layers)
        {
            foreach (Coordinate2D c in l.tiles.Keys)
            {
                if (l.tiles[c].triggerType == TileTriggers.SpawnPoint)
                    return c;
            }
        }
        return null;
    }
    public void evaluateTile(int x, int y)
    {
        foreach (LayerController l in layers)
        {

            Coordinate2D currPos = Coordinate2D.Is(x, y);
            TileController t = l.tiles[currPos];
            if (!t) continue; // check Controller

            if (!t.isTriggering) continue; // only active

            if (t.triggerType == TileTriggers.Item)
            {
                ItemScript i = t.GetComponent<ItemScript>();
                if (!i) continue; // check if Item

                if (playerScript.playerInventory.Count < playerScript.maxInvetorySpace)
                {
                    l.tiles[currPos].isTriggering = false;
                    l.tiles[currPos].spriteRenderer.enabled = false;
                    playerScript.playerInventory.Add(i);
                    //TODO: Pickup sound
                    //TODO: Pickup info message
                }
            }

        }
    }
}
