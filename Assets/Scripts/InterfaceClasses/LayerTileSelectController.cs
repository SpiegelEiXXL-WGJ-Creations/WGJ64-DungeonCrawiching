using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LayerTileSelectController : LayerController
{

    public List<TileController> possibleTiles;
    private List<GameObject> tileObjectPrefabs;
    public TileController selectedTile;
    public UnityEngine.UI.Image tilePreview;

    public int selectedTileNumber
    {
        get
        {
            return _selectedTileNumber;
        }
        set
        {

            if (value < possibleTiles.Count && value >= 0)
                _selectedTileNumber = value;
            selectedTile = possibleTiles[_selectedTileNumber];


            tilePreview.sprite = tileObjectPrefabs[_selectedTileNumber].GetComponent<TileController>().spriteTexture;

        }
    }
    private int _selectedTileNumber = 0;
    public PlayerScript playerRef;
    public GameManager gm;

    // Use this for initialization
    void Start()
    {
        gm = GameManager.instance;
        playerRef = gm.playerScript;

        tileObjectPrefabs = new List<GameObject>();
        foreach (TileController t in possibleTiles)
        {
            GameObject o = GameObject.Instantiate(t, this.gameObject.transform).gameObject;
            //o.SetActive(false);

            tileObjectPrefabs.Add(o);
        }
        selectedTileNumber = 0;


    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            string saveFile = UnityEditor.EditorUtility.SaveFilePanel("Save current map", "", "map", "json");
            if (saveFile == "")
                return;

            string outputStr = "";

            foreach (LayerController l in gm.layers)
            {
                string expStr = "";
                expStr = UnityEngine.JsonUtility.ToJson(l);
                List<string> ls = new List<string>();
                foreach (TileController t in l.tiles.Values)
                {
                    ls.Add(UnityEngine.JsonUtility.ToJson(t));
                }

                expStr = expStr + "\r" + string.Join("#!#", ls.ToArray());
                if (outputStr == "")
                    outputStr = expStr;
                else
                    outputStr = outputStr + "\n" + expStr;
            }
            outputStr = System.Text.RegularExpressions.Regex.Replace(outputStr, @",?""[^""]*"":\{""instanceID"":[^\}]*\},?", "");
            System.IO.File.WriteAllText(saveFile, outputStr);
            Debug.Log("File successfully saved to: " + saveFile);


            //Debug.Log(UnityEngine.JsonUtility.ToJson(.tiles[0].GetComponent<TileController>()));
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            string loadFile = UnityEditor.EditorUtility.OpenFilePanel("Load map", "", "json");
            if (loadFile == "")
                return;

            string x = System.IO.File.ReadAllText(loadFile);
            string[] layersX = x.Split('\n');
            int i = 0;
            foreach (string layerX in layersX)
            {
                i++;

                GameObject o = GameObject.Instantiate(GameManager.instance.LayerPrefab, GameManager.instance.GameGrid.transform);
                LayerController l = o.GetComponent<LayerController>();
                l.tileControllerPrefab = tileControllerPrefab;
                l.parentObjectForTiles = o.transform;
                l.name = "Layer#" + i + " (loaded from JSON)";
                l.GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize = new Vector3(GameManager.instance.cellHeight, GameManager.instance.cellWidth);
                string[] layerXData = layerX.Split('\r');
                Newtonsoft.Json.JsonConvert.PopulateObject(layerXData[0], l);
                l.BuildLayer(null, null);

                string[] tilesX = layerXData[1].Split(new string[] { "#!#" }, System.StringSplitOptions.RemoveEmptyEntries);
                int idx = -1;
                foreach (string tileX in tilesX)
                {
                    idx++;
                    /*Dictionary<string, object> y = UnityEngine.JsonUtility.FromJson<Dictionary<string, object>>(tileX);
                    Debug.Log(y.Keys.ToString() + " : " + y.Keys.Count);*/

                    Newtonsoft.Json.JsonConvert.PopulateObject(tileX, l.tiles[idx]);
                    l.tiles[idx].Setup();

                }
            }
        }
        if (Input.GetKey(KeyCode.Plus))
            Camera.main.orthographicSize += 1;
        if (Input.GetKey(KeyCode.Minus))
            Camera.main.orthographicSize -= 1;
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetAxis("Mouse ScrollWheel") >= 0.99f)
            selectedTileNumber++;
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetAxis("Mouse ScrollWheel") <= -0.99f)
            selectedTileNumber--;

        if (Input.GetButtonDown("Fire1"))
        {
            if (!playerRef)
                playerRef = gm.playerScript;
            if (!playerRef)
                return;
            GameManager.instance.layers[0].tiles[playerRef.mapY * gm.mapWidth + playerRef.mapX].GettingOverwritten(tileObjectPrefabs[selectedTileNumber].GetComponent<TileController>());
        }

    }
}
