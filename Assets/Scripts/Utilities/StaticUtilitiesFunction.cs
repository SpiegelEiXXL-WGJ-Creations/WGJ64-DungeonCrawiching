using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StaticUtilitiesFunction
{
    public static List<GameObject> LoadMapFromFile(string fileName, GameObject tileControllerPrefab, bool replaceLayers = false)
    {
        string x = System.IO.File.ReadAllText(fileName);
        string[] layersX = x.Split('\n');
        List<GameObject> spawnedObjects = new List<GameObject>();
        int i = 0;

        if (replaceLayers)
            foreach (LayerController lc in GameManager.instance.layers)
                GameObject.DestroyImmediate(lc.gameObject);

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
            spawnedObjects.Add(o);
        }
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
        outputStr = System.Text.RegularExpressions.Regex.Replace(outputStr, @",?""[^""]*"":\{""instanceID"":[^\}]*\},?", "");
        System.IO.File.WriteAllText(fileFile, outputStr);

    }
}
