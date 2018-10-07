using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LayerRandomLevelGenerator : LayerController
{
    public int minRoomWidth = 3;
    public int minRoomHeight = 4;

    public int maxRoomWidth = 7;
    public int maxRoomHeight = 8;

    public int maxNoOfRooms = 30;
    public int minNoOfRooms = 10;
    public List<TileController> possibleTiles;
    private List<GameObject> tileObjectPrefabs;
    public TileController selectedTile;
    public PlayerScript playerRef;
    public GameManager gm;

    public int NoOfRooms = 0;
    public int MaxNoOfTries = 100;
    public int NoOfTries = 0;

    // Use this for initialization
    void Start()
    {
        gm = GameManager.instance;
        playerRef = gm.playerScript;

        this.layerWidth = gm.mapWidth;
        this.layerHeight = gm.mapHeight;

        tileObjectPrefabs = new List<GameObject>();
        /*foreach (TileController t in possibleTiles)
        {
            GameObject o = GameObject.Instantiate(t, this.gameObject.transform).gameObject;
            //o.SetActive(false);

            tileObjectPrefabs.Add(o);
        }*/
        //this.BuildLayer(null, null);
    }

    public class Room
    {
        public int x;
        public int y;
        public int h;
        public int w;
        public Room(int X, int Y, int W, int H)
        {
            x = X; y = Y; h = H; w = W;
        }
    }

    public override void BuildLayer(LayerController prevLayer, LayerController nextLayer)
    {
        base.BuildLayer(prevLayer, nextLayer);

        if (prevLayer == null)
        {
            List<Room> rooms = new List<Room>();
            gm = GameManager.instance;
            NoOfRooms = 0;
            NoOfTries = 0;
            int NoOfRoomsSelected = Mathf.RoundToInt(Random.Range(minNoOfRooms, maxNoOfRooms));
            while (NoOfRooms < NoOfRoomsSelected && NoOfTries < MaxNoOfTries)
            {
                int roomWidth = Random.Range(minRoomWidth, maxRoomWidth);
                int roomHeight = Random.Range(minRoomHeight, maxRoomHeight);
                int x = Random.Range(0, gm.mapWidth - roomWidth);
                int y = Random.Range(0, gm.mapHeight - roomHeight);
                bool roomIsOK = true;
                for (int a = x; a < roomWidth + x; a++)
                {
                    for (int b = y; b < roomHeight + y; b++)
                    {
                        if (tiles.ContainsKey(Coordinate2D.Is(a, b)))
                        {
                            if (tiles[Coordinate2D.Is(a, b)].spriteName != "" && tiles[Coordinate2D.Is(a, b)].spriteName != "Dungeon_Crawler_Tileset_26")
                            {
                                roomIsOK = false;
                                //Debug.Log("Tile " + a + "/" + b + ": occupied! - not OK. - " + tiles[Coordinate2D.Is(a, b)].spriteName);
                            }
                        }
                        else
                        {
                            roomIsOK = false;
                            Debug.Log("Invalid: " + a + "/" + b);
                        }
                    }
                }
                if (roomIsOK)
                {
                    rooms.Add(new Room(x, y, roomWidth, roomHeight));
                    for (int a = x; a < roomWidth + x; a++)
                    {
                        for (int b = y; b < roomHeight + y; b++)
                        {
                            string tileToUse = "Dungeon_Crawler_Tileset_22";
                            bool enemyWalk = false;
                            bool playerWalk = false;
                            if (b == y)
                            {
                                if (a == x)
                                {
                                    tileToUse = "Dungeon_Crawler_Tileset_11"; // top left
                                }
                                else if (a == x + roomWidth - 1)
                                {
                                    tileToUse = "Dungeon_Crawler_Tileset_10"; // top right
                                }
                                else
                                {
                                    tileToUse = "Dungeon_Crawler_Tileset_9"; // top
                                }
                            }
                            else if (a == x)
                            {
                                if (b == y + roomHeight - 1)
                                    tileToUse = "Dungeon_Crawler_Tileset_13"; // Bottom Left
                                else
                                    tileToUse = "Dungeon_Crawler_Tileset_7"; // Left


                            }
                            else if (a == x + roomWidth - 1)
                            {
                                if (b == y + roomHeight - 1)
                                    tileToUse = "Dungeon_Crawler_Tileset_12"; // Bottom Right
                                else
                                    tileToUse = "Dungeon_Crawler_Tileset_6"; // Right

                            }
                            else if (b == y + roomHeight - 1)
                            {
                                tileToUse = "Dungeon_Crawler_Tileset_8"; // Bottom
                            }
                            else if (b == y + 1)
                            {
                                tileToUse = "Dungeon_Crawler_Tileset_23"; // Special 'wall'
                            }
                            else
                            {
                                enemyWalk = true;
                                playerWalk = true;
                            }

                            tiles[Coordinate2D.Is(a, b)].spriteName = tileToUse;
                            tiles[Coordinate2D.Is(a, b)].walkableByEnemy = enemyWalk;
                            tiles[Coordinate2D.Is(a, b)].walkableByPlayer = playerWalk;
                        }
                    }
                    NoOfRooms++;
                    Debug.Log("Room instanciated!");
                }
                if (!roomIsOK)
                    NoOfTries++;
                Debug.Log("Room generation finished - " + NoOfTries + "/" + MaxNoOfTries + " attempts used before force quitting");
            }
            Debug.Log("Start path generation...");
            // try to create paths between all rooms.
            foreach (Room r in rooms)
            {
                Debug.Log("Room paths for: " + r.x + "/" + r.y + "(" + r.w + "x" + r.h + ")");
                int a = r.x;
                int b = r.y;
                //int dir = Mathf.RoundToInt(Random.Range(0, 3));
                int dir = 0;

                // 0 = up
                // 1 = right
                // 2 = down
                // 3 = left
                if (dir == 0)
                {
                    a = r.x + Mathf.RoundToInt(Random.Range(1, r.w - 1));
                    b = r.y - 1;
                    setTile(a, b + 1, "Dungeon_Crawler_Tileset_25", false, false);
                    setTile(a, b + 2, "Dungeon_Crawler_Tileset_25", false, false);
                    while (b >= 0 && (tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_13" ||
                                               tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_12" ||
                                               tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_8" ||
                                             tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_26"))
                    {
                        Debug.Log("Path: " + a + "/" + b);
                        /*if (tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_13")
                        {
                            b++;
                            if (a - 2 >= 0)
                                setTile(a - 2, b, "Dungeon_Crawler_Tileset_25", false, false);
                            if (a - 1 >= 0)
                                setTile(a - 1, b, "Dungeon_Crawler_Tileset_25", false, false);
                            if (a - 2 >= 0 && b - 1 >= 0)
                                setTile(a - 2, b - 1, "Dungeon_Crawler_Tileset_25", false, false);

                        }
                        if (tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_12")
                        {
                            b++;
                            if (a + 1 < layerWidth)
                                setTile(a + 1, b, "Dungeon_Crawler_Tileset_25", false, false);
                            if (a + 1 < layerWidth && b + 1 < layerHeight)
                                setTile(a + 1, b + 1, "Dungeon_Crawler_Tileset_25", false, false);
                            if (a - 1 >= 0)
                                setTile(a - 1, b, "Dungeon_Crawler_Tileset_25", false, false);

                        }*/
                        setTile(a, b, "Dungeon_Crawler_Tileset_25", false, false);
                        b--;
                    }
                    dir = 1;
                }
                if (dir == 1)
                {
                    a = r.x + r.w;
                    b = r.y + Mathf.RoundToInt(Random.Range(1, r.h - 1));
                    setTile(a - 1, b, "Dungeon_Crawler_Tileset_25", false, false);
                    setTile(a - 2, b, "Dungeon_Crawler_Tileset_25", false, false);
                    while (a < layerWidth && (tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_13" ||
                                               tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_11" ||
                                               tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_7" ||
                                             tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_26"))
                    {
                        Debug.Log("Path: " + a + "/" + b);
                        if (tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_13")
                        {
                            b++;
                            setTile(a - 2, b, "Dungeon_Crawler_Tileset_25", false, false);
                            setTile(a - 1, b, "Dungeon_Crawler_Tileset_25", false, false);
                            setTile(a - 2, b - 1, "Dungeon_Crawler_Tileset_25", false, false);

                        }
                        if (tiles[Coordinate2D.Is(a, b)].spriteName == "Dungeon_Crawler_Tileset_11")
                        {
                            b--;
                            setTile(a + 1, b, "Dungeon_Crawler_Tileset_25", false, false);
                            setTile(a + 1, b + 1, "Dungeon_Crawler_Tileset_25", false, false);
                            setTile(a - 1, b, "Dungeon_Crawler_Tileset_25", false, false);

                        }
                        setTile(a, b, "Dungeon_Crawler_Tileset_25", false, false);
                        a++;
                    }
                }


            }


            foreach (Coordinate2D c in tiles.Keys)
            {
                if (tiles[c].spriteName == "Dungeon_Crawler_Tileset_25")
                {
                    tiles[c].spriteName = checkAroundTile(c);
                    //if (tiles[c].spriteName == "Dungeon_Crawler_Tileset_22")
                    setTile(c.x, c.y, tiles[c].spriteName, true, true);
                }
            }

            // replace all template tiles
        }
        else
        {
            List<Coordinate2D> spawnPoints = new List<Coordinate2D>();
            foreach (Coordinate2D c in prevLayer.tiles.Keys)
            {
                if (prevLayer.tiles[c].spriteName == "Dungeon_Crawler_Tileset_22")
                {
                    spawnPoints.Add(c);
                }
            }
            int selectedPoint = Mathf.RoundToInt(Random.Range(0, spawnPoints.Count - 1));
            tiles[spawnPoints[selectedPoint]].triggerType = TileTriggers.SpawnPoint;
            tiles[spawnPoints[selectedPoint]].isTriggering = true;
            selectedPoint = Mathf.RoundToInt(Random.Range(0, spawnPoints.Count - 1));
            tiles[spawnPoints[selectedPoint]].triggerType = TileTriggers.NextFloorStairs;
            tiles[spawnPoints[selectedPoint]].isTriggering = true;
            tiles[spawnPoints[selectedPoint]].spriteName = "Dungeon_Crawler_Stairs_5";
            tiles[spawnPoints[selectedPoint]].triggerParameters = "static_Level3_GlitchGun";


        }
    }

    string checkAroundTile(Coordinate2D coord)
    {
        if (coord.x > 0)
        {

        }
        return "Dungeon_Crawler_Tileset_22";
    }

    void setTile(int X, int Y, string tileName, bool walkEnemy, bool walkPlayer)
    {
        tiles[Coordinate2D.Is(X, Y)].spriteName = tileName;
        tiles[Coordinate2D.Is(X, Y)].walkableByEnemy = walkEnemy;
        tiles[Coordinate2D.Is(X, Y)].walkableByPlayer = walkPlayer;

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        if (playerRef.inMenu) return;

    }
}
