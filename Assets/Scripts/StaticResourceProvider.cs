using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class StaticResourceProvider
{
    private static Dictionary<string, Sprite> sprites;
    static StaticResourceProvider
    ()
    {
        Sprite[] spr = Resources.LoadAll<Sprite>("Graphics/");
        sprites = new Dictionary<string, Sprite>();
        foreach (Sprite s in spr)
        {
            sprites.Add(s.name, s);
        }

    }
    public static Sprite GetSpriteObject(string SpriteName)
    {
        if (sprites.ContainsKey(SpriteName))
            return sprites[SpriteName];
        else
            return null;

    }
    public static string GetMapJSONObject(string mapName)
    {
        TextAsset lvl = Resources.Load<TextAsset>("FixedLevels/" + mapName);
        return lvl.text;
    }
}
