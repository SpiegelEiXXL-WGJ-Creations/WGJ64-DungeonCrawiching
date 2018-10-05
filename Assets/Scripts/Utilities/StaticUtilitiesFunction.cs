using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StaticUtilitiesFunction
{
    public static List<GameObject> LoadMapFromFile(string fileName, GameObject tileControllerPrefab, bool replaceLayers = false, string content = "")
    {

        string x;
        if (content != "")
            x = content;
        else
            x = System.IO.File.ReadAllText(fileName);

        x = x.Replace("\r\n", "\n");
        string[] gameManagerX = x.Split('\t');
        Newtonsoft.Json.JsonConvert.PopulateObject(gameManagerX[0], GameManager.instance);

        string[] layersX = gameManagerX[1].Split('\n');

        List<GameObject> spawnedObjects = new List<GameObject>();
        int i = 0;

        if (replaceLayers)
            while (GameManager.instance.layers.Count > 0)
            {
                LayerController lc = GameManager.instance.layers[0];
                GameManager.instance.layers.Remove(lc);
                GameObject.DestroyImmediate(lc.gameObject);

            }

        foreach (string layerX in layersX)
        {
            i++;
            if (layerX.Trim() == "")
            {
                Debug.Log("Dangling new-line!");
                continue;
            }

            GameObject o = GameObject.Instantiate(GameManager.instance.LayerPrefab, GameManager.instance.GameGrid.transform);
            LayerController l = o.GetComponent<LayerController>();
            l.tileControllerPrefab = tileControllerPrefab;
            l.parentObjectForTiles = o.transform;
            l.name = "Layer#" + i + " (loaded from JSON)";

            l.transform.Translate(0f, 0f, -1f * i);
            l.GetComponent<UnityEngine.UI.GridLayoutGroup>().cellSize = new Vector3(GameManager.instance.cellHeight, GameManager.instance.cellWidth);
            string[] layerXData = layerX.Split('\r');
            Newtonsoft.Json.JsonConvert.PopulateObject(layerXData[0], l);
            l.BuildLayer(null, null);
            if (layerXData.Length < 2)
            {
                Debug.Log("Dangling... line? >" + layerX);
                continue;
            }

            string[] tilesX = layerXData[1].Split(new string[] { "#!#" }, System.StringSplitOptions.RemoveEmptyEntries);
            int xPos = 0;
            int yPos = 0;
            foreach (string tileX in tilesX)
            {
                Coordinate2D tmp = Coordinate2D.Is(xPos, yPos);
                Newtonsoft.Json.JsonConvert.PopulateObject(tileX, l.tiles[tmp]);
                l.tiles[tmp].Setup();
                xPos++;
                if (xPos >= GameManager.instance.mapWidth)
                {
                    xPos = 0;
                    yPos++;
                }

            }
            GameManager.instance.layers.Add(l);
            spawnedObjects.Add(o);
        }
        GameManager.instance.initGameManager();
        GameManager.instance.triggerMapLoadingEvent();
        return spawnedObjects;
    }

    public static void SaveMapToFile(string fileFile)
    {
        string outputStr = "";
        foreach (LayerController l in GameManager.instance.layers)
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
        outputStr = UnityEngine.JsonUtility.ToJson(GameManager.instance) + "\t" + outputStr;

        outputStr = System.Text.RegularExpressions.Regex.Replace(outputStr, @"(""[^""]*"":)?(\{""instanceID"":[^\},]*\},?)", "");
        System.IO.File.WriteAllText(fileFile, outputStr);

    }
    public static List<GameObject> LoadMapFromResources(string mapName, GameObject tileControllerPrefab, bool replaceLayers = false)
    {
        return LoadMapFromFile("", tileControllerPrefab, replaceLayers, StaticResourceProvider.GetMapJSONObject(mapName));
    }
}
