using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Prefabs")]
    public GameObject GameGrid;
    public GameObject LayerPrefab;
    public GameObject TilePrefab;

    [Header("Settings")]
    public int NumberOfLayers = 3;
    public int mapWidth = 15;
    public int mapHeight = 15;

    [Header("Do not change!")]
    public bool isBusy = false; // while enemy is doing attacks
    public float cellWidth;
    public float cellHeight;
    public List<LayerController> layers;
    public delegate void finishedMapSpawning();
    public event finishedMapSpawning mapSpawnDone;
    public PlayerScript playerScript;

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
        playerScript = GetComponent<PlayerScript>();
        Sprite t = TilePrefab.GetComponent<SpriteRenderer>().sprite;
        cellHeight = t.textureRect.height / t.pixelsPerUnit;
        cellWidth = t.textureRect.width / t.pixelsPerUnit;

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
        GameGrid.GetComponent<RectTransform>().sizeDelta = new Vector3(mapHeight * cellHeight, mapWidth * cellWidth);
        //GameGrid.GetComponent<RectTransform>().localRotation = Quaternion.Euler(180f, 0f, 0f);
        for (int i = 0; i < layers.Count; i++)
            layers[i].BuildLayer(i > 0 ? layers[i - 1] : null, i == layers.Count - 1 ? null : layers[i + 1]);

        if (mapSpawnDone != null)
            mapSpawnDone();
        /*        Sprite t = TilePrefab.GetComponent<SpriteRenderer>().sprite;
                Layers = new List<GameObject>();
                LayerTiles = new Dictionary<int, Dictionary<int, GameObject>>();
                for (int i = 0; i < NumberOfLayers; i++)
                    Layers.Add(GameObject.Instantiate(LayerPrefab, GameGrid.transform));

                for (int i = 0; i < Layers.Count; i++)
                {
                    Layers[i].name = "Layer: " + i;
                    Layers[i].GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize = new Vector3(t.textureRect.height / t.pixelsPerUnit, t.textureRect.width / t.pixelsPerUnit);
                    //TilePrefab.GetComponent<Sprite>()
                    //Layers[i].GetComponent<RectTransform>().sizeDelta = new Vector3(mapHeight * 0.19f, mapWidth * 0.19f);
                    Tiles = new Dictionary<int, GameObject>();
                    for (int h = 0; h < mapHeight; h++)
                        for (int w = 0; w < mapWidth; w++)
                        {
                            int idx = h * mapWidth + w + h;
                            Tiles.Add(idx, GameObject.Instantiate(TilePrefab, Layers[i].transform));
                            Tiles[idx].name = "Layer" + i + "Tile: " + idx + " (" + h + " / " + w + ")";
                        }
                    LayerTiles.Add(i, Tiles);
                }

                GameGrid.GetComponent<RectTransform>().sizeDelta = new Vector3(mapHeight * (t.textureRect.height / t.pixelsPerUnit), mapWidth * (t.textureRect.width / t.pixelsPerUnit));

        */


    }

    // Update is called once per frame
    void Update()
    {

    }
}
