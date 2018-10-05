using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Usable,
    Equipable,
    Collectable,
    EventItems
}

public enum ItemEffect
{
    Healing,
    Damaging,
    RevealMap,
    KillEnemy,
    Die
}

public class ItemScript : MonoBehaviour
{
    public string itemDisplayName;
    public ItemType itemType;
    public SpriteRenderer itemSpriteRenderer;
    public Sprite itemSprite;
    public bool inInventory;
    public ItemEffect itemEffect;
    public int itemEffectValue;

    // Use this for initialization
    void Start()
    {
        if (!itemSpriteRenderer)
            itemSpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void itemUse(PlayerScript user)
    {
        //TODO: Graphical Effects
        //TODO: Sound
        // play some particles
        if (itemEffect == ItemEffect.Healing)
            user.currentHP = user.currentHP + itemEffectValue;
    }
}
