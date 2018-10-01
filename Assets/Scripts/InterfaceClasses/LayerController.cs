using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(fileName = "Layer Controller", menuName = "Layer Controller")]
public class LayerController : MonoBehaviour
{
    public GameObject tileControllerPrefab;
    public Dictionary<int, TileController> tiles;
    public int layerWidth;
    public int layerHeight;
    public Transform parentObjectForTiles;

    public virtual void BuildLayer(LayerController prevLayer, LayerController nextLayer)
    {
        tiles = new Dictionary<int, TileController>();
        for (int i = 0; i < layerHeight; i++)
        {
            for (int j = 0; j < layerWidth; j++)
            {
                GameObject tc = GameObject.Instantiate(tileControllerPrefab, parentObjectForTiles);
                tiles.Add(i * layerHeight + j * layerWidth, tc.GetComponent<TileController>());

            }
        }
    }


}
