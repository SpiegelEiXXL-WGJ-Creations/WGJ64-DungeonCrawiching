using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coordinate2D
{
    public int x;
    public int y;
    private static Dictionary<int, Dictionary<int, Coordinate2D>> _tmp;

    static Coordinate2D()
    {
        _tmp = new Dictionary<int, Dictionary<int, Coordinate2D>>();
    }
    private Coordinate2D(int X, int Y)
    {
        x = X;
        y = Y;

    }
    public static Coordinate2D Is(int X, int Y)
    {
        if (!_tmp.ContainsKey(X))
        {

            _tmp.Add(X, new Dictionary<int, Coordinate2D>());
        }
        if (!_tmp[X].ContainsKey(Y))
        {

            _tmp[X].Add(Y, new Coordinate2D(X, Y));
        }
        return _tmp[X][Y];

    }
}
//[CreateAssetMenu(fileName = "Layer Controller", menuName = "Layer Controller")]
public class LayerController : MonoBehaviour
{
    public GameObject tileControllerPrefab;
    public Dictionary<Coordinate2D, TileController> tiles;
    public int layerWidth;
    public int layerHeight;
    public Transform parentObjectForTiles;

    public virtual void BuildLayer(LayerController prevLayer, LayerController nextLayer)
    {
        tiles = new Dictionary<Coordinate2D, TileController>();
        for (int i = 0; i < layerHeight; i++)
        {
            for (int j = 0; j < layerWidth; j++)
            {
                GameObject tc = GameObject.Instantiate(tileControllerPrefab, parentObjectForTiles);
                tc.name = "Tile " + j + "/" + i;
                tiles.Add(Coordinate2D.Is(j, i), tc.GetComponent<TileController>());

            }
        }
    }


}
