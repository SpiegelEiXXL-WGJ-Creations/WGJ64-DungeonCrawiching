using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(GradientMaker))]
public class GradientMakerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GradientMaker g = (GradientMaker)target;
        if (GUILayout.Button("Generate"))
        {
            Texture2D newTex;
            float imgW = g.imgReference.gameObject.GetComponent<RectTransform>().rect.width;
            float imgH = g.imgReference.gameObject.GetComponent<RectTransform>().rect.height;
            int iImgW = Mathf.RoundToInt(imgW);
            int iImgH = Mathf.RoundToInt(imgH);
            newTex = new Texture2D(Mathf.RoundToInt(imgW), Mathf.RoundToInt(imgH));

            Color[] evalColors;
            if (g.gradDirection == GradientMaker.Direction.LeftToRight)
            {
                for (int i = 0; i < iImgW; i++)
                {
                    evalColors = new Color[iImgH];
                    for (int j = 0; j < iImgH; j++)
                    {
                        evalColors[j] = g.grad.Evaluate((i * 1.0f) / imgW);
                        //newTex.SetPixel(i, j, g.grad.Evaluate(i / iImgW));
                    }
                    newTex.SetPixels(i, 0, 1, iImgH, evalColors);
                }

            }
            else if (g.gradDirection == GradientMaker.Direction.DownToUp)

                for (int i = 0; i < iImgH; i++)
                {
                    evalColors = new Color[iImgW];
                    for (int j = 0; j < iImgW; j++)
                    {
                        evalColors[j] = g.grad.Evaluate((i * 1.0f) / imgH);
                        //newTex.SetPixel(i, j, g.grad.Evaluate(i / iImgW));
                    }
                    newTex.SetPixels(0, i, iImgW, 1, evalColors);
                }



            newTex.Apply();

            g.imgReference.sprite = Sprite.Create(newTex, new Rect(0f, 0f, imgW, imgH), new Vector2(0, 0));

        }
    }
}
