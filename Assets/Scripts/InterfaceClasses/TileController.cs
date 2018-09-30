using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class TileController : MonoBehaviour
{
    public Sprite spriteTexture
    {
        get
        {
            fetchSpriteRenderer();
            return spriteRenderer.sprite;
        }
        set
        {
            fetchSpriteRenderer();
            spriteRenderer.sprite = value;
            if (spriteRenderer)
                spriteName = spriteRenderer.sprite.name;
        }
    }
    public SpriteRenderer spriteRenderer;

    public string spriteName
    {
        get
        {
            fetchSpriteRenderer();
            _spriteName = spriteRenderer.sprite.name;
            return spriteRenderer.sprite.name;
        }
        set
        {
            _spriteName = value;
            fetchSpriteRenderer();
            spriteRenderer.sprite = StaticResourceProvider.GetSpriteObject(value);
        }
    }
    public string _spriteName;
    public bool walkableByPlayer;
    public bool walkableByEnemy;
    public bool walkableByGlitch;

    // Use this for initialization
    void Start()
    {
        fetchSpriteRenderer();

        /*if (spriteRenderer)
            spriteName = spriteRenderer.sprite.name; */
    }

    void fetchSpriteRenderer()
    {
        if (!spriteRenderer)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
            spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (!spriteRenderer)
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        if (!spriteRenderer)
            Debug.Log("Could not create a spriteRenderer?!");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup()
    {
        spriteName = _spriteName;
    }

    public void GettingOverwritten(TileController src)
    {
        walkableByEnemy = src.walkableByEnemy;
        walkableByGlitch = src.walkableByGlitch;
        walkableByPlayer = src.walkableByPlayer;

        spriteTexture = src.spriteTexture;

    }
}
