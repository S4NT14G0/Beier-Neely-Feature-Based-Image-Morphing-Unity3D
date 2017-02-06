﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MorphTextures : MonoBehaviour {

    [SerializeField]
    GameObject src2DGameObject, dest2DGameObject;

    [SerializeField]
    List<Line> srcLines, destLines;

    [SerializeField, Range(0, 1)]
    float a = 0.001f;
    [SerializeField, Range(0.5f, 2f)]
    float p = 1.4f;
    [SerializeField, Range(0, 1)]
    float b = 0.5f;

    [SerializeField, Range(0, 1)]
    float alpha = 0.5f;


	// Use this for initialization
	void Start () {
        // Get access to the tect
        Sprite srcSprite = src2DGameObject.GetComponent<SpriteRenderer>().sprite;
        Sprite destSprite = dest2DGameObject.GetComponent<SpriteRenderer>().sprite;

        if (srcLines.Count == 1)
        {
            // Use single line algorithm to warp a texture with one line
            GameObject newSprite = new GameObject("Single Line Transformed Texture");
            newSprite.AddComponent<SpriteRenderer>().sprite = TextureToSprite(SingleLineTransformation(srcSprite));
            newSprite.transform.position = new Vector3(5f, 0f);
            newSprite.transform.localScale = new Vector3(5, 5, 5);
        }
        else
        {
            // User Multiple line algorithm to modify a texture with two different lines
            GameObject sourceSprite = new GameObject("Source");
            sourceSprite.AddComponent<SpriteRenderer>().sprite = TextureToSprite(MultiLineWarp(srcSprite, destSprite, srcLines, destLines));
            sourceSprite.transform.position = new Vector3(5f, 0f);
            sourceSprite.transform.localScale = new Vector3(5, 5, 5);

            // User Multiple line algorithm to modify a texture with two different lines
            GameObject newSprite = new GameObject("Destination");
            newSprite.AddComponent<SpriteRenderer>().sprite = TextureToSprite(MultiLineWarp(destSprite, srcSprite, destLines, srcLines));
            newSprite.transform.position = new Vector3(5f, 5f);
            newSprite.transform.localScale = new Vector3(5, 5, 5);

            GameObject result = new GameObject("Final");
            result.AddComponent<SpriteRenderer>().sprite = TextureToSprite( BlendWarpedImages(sourceSprite.GetComponent<SpriteRenderer>().sprite.texture, newSprite.GetComponent<SpriteRenderer>().sprite.texture, alpha));
            result.transform.position = new Vector3(10f, 2.5f);
            result.transform.localScale = new Vector3(5, 5, 5);
            
        }
    }

    Texture2D SingleLineTransformation(Sprite srcSprite)
    {
        Texture2D warpedTexture = new Texture2D(SpriteToTexture(srcSprite).width, SpriteToTexture(srcSprite).height);
        Line destLine = destLines[0];
        Line srcLine = srcLines[0];

        for (int x = 0; x < warpedTexture.width; x++)
        {
            for (int y = 0; y < warpedTexture.height; y++)
            {
                // Find UV relative to line source line
                Vector2  X = new Vector2(x, y);
                float u = Vector2.Dot((X - destLine.P()), (destLine.Q() - destLine.P())) / Vector2.SqrMagnitude(destLine.Q() - destLine.P());
                float v = Vector2.Dot((X - destLine.P()), (destLine.Q() - destLine.P()).Perpendicular()) / (destLine.Q() - destLine.P()).magnitude;
                Vector2 xPrime = srcLine.P() + u * (srcLine.Q() - srcLine.P()) + (v * (srcLine.Q() - srcLine.P()).Perpendicular() / (srcLine.Q() - srcLine.P()).magnitude);
                warpedTexture.SetPixel(x, y, srcSprite.texture.GetPixel((int)xPrime.x,(int) xPrime.y));
            }
        }
        warpedTexture.Apply();

        return warpedTexture;
    }
    
    UV CalculateUV (Line destLine, Vector2 X)
    {
        float u = Vector2.Dot((X - destLine.P()), (destLine.Q() - destLine.P())) / Vector2.SqrMagnitude(destLine.Q() - destLine.P());
        float v = Vector2.Dot((X - destLine.P()), (destLine.Q() - destLine.P()).Perpendicular()) / (destLine.Q() - destLine.P()).magnitude;

        return new UV(u, v);
    }

    Vector2 CalculateXPrime (UV uv, Line srcLine)
    {
        return srcLine.P() + uv.u * (srcLine.Q() - srcLine.P()) + (uv.v * (srcLine.Q() - srcLine.P()).Perpendicular() / (srcLine.Q() - srcLine.P()).magnitude);
    }

    Texture2D BlendWarpedImages(Texture2D warpedSrcImage, Texture2D warpedDestImage, float alphaVal)
    {
        Texture2D resultImage = new Texture2D(Mathf.Max(warpedSrcImage.width, warpedDestImage.width), Mathf.Max(warpedSrcImage.height, warpedDestImage.height));

        for (int x = 0; x < resultImage.width; x++)
        {
            for (int y = 0; y < resultImage.height; y++)
            {
                Color tempCol = new Color();

                tempCol.r = warpedSrcImage.GetPixel(x, y).r + alphaVal * (warpedDestImage.GetPixel(x, y).r - warpedSrcImage.GetPixel(x, y).r);
                tempCol.g = warpedSrcImage.GetPixel(x, y).g + alphaVal * (warpedDestImage.GetPixel(x, y).g - warpedSrcImage.GetPixel(x, y).g);
                tempCol.b = warpedSrcImage.GetPixel(x, y).b + alphaVal * (warpedDestImage.GetPixel(x, y).b - warpedSrcImage.GetPixel(x, y).b);
                tempCol.a = warpedSrcImage.GetPixel(x, y).a + alphaVal * (warpedDestImage.GetPixel(x, y).a - warpedSrcImage.GetPixel(x, y).a);
                resultImage.SetPixel(x, y, tempCol);
            }

        }

        resultImage.Apply();
        return resultImage;
    }

    Texture2D MultiLineWarp (Sprite srcSprite, Sprite destSprite, List<Line> sourceLines, List<Line> destinationLines)
    {
        Texture2D destinationTexture = new Texture2D(SpriteToTexture(destSprite).width, SpriteToTexture(destSprite).height);

        // Foreach pixel X in the destination

        for (int x = 0; x < destinationTexture.width; x++)
        {
            for (int y = 0; y < destinationTexture.height; y++)
            {
                Vector2 xPixel = new Vector2(x, y);
                Vector2 xPrimePixel = new Vector2();

                // DSUM = (0,0)
                // weightsum = 0
                Vector2 DSUM = new Vector2(0, 0);
                float weightSum = 0.0f;

                int lineIndex = 0;

                // Foreach Line in Pi Qi
                foreach (Line destLine in destinationLines) {
                    // Calculate u,v based on Pi Qi
                    UV uv = CalculateUV(destLine, xPixel);
                    // Calculate Xi' based on u,v and Pi' Qi'
                    xPrimePixel = CalculateXPrime(uv, sourceLines[lineIndex]);
                    // Calculate displacement Di = Xi' - Xi for this line
                    Vector2 Di = xPrimePixel - xPixel;
                    // dist = shortest distance from X to PiQi
                    float dist = 0;

                    if (0 < uv.u && uv.u < 1)
                    {
                        dist = Mathf.Abs(uv.v);
                    }
                    else if (uv.u < 0)
                    {
                        dist = Vector2.Distance(xPixel, destLine.P());
                    }
                    else if (uv.u > 1)
                    {
                        dist = Vector2.Distance(xPixel, destLine.Q());
                    }

                    // weight = (length^p / (a + dist)))^b
                    float weight = Mathf.Pow(Mathf.Pow(destLine.Length(), p) / (a + dist), b);
                    // DSUM += Di * weight
                    DSUM += Di * weight;
                    // weightSum += weight;
                    weightSum += weight;

                    lineIndex++;
                }


                // X' = X + DSUM / weightsum
                xPrimePixel = xPixel + DSUM / weightSum;

                // destinationImage (X) = sourceImage (X')
                destinationTexture.SetPixel(x, y, srcSprite.texture.GetPixel((int)xPrimePixel.x, (int)xPrimePixel.y));

            }
        }
        destinationTexture.Apply();
        return destinationTexture;
    }

    Sprite TextureToSprite(Texture2D tex)
    {
        Rect rec = new Rect(0, 0, tex.width, tex.height);

        Sprite spriteFromTex = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteFromTex;
    }

    static Texture2D SpriteToTexture(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }
}

struct UV {
    public float u;
    public float v;

    public UV(float _u, float _v)
    {
        this.u = _u;
        this.v = _v;
    }

}

[System.Serializable]
public class Line {
    [SerializeField, Header("Start Vector")]
    private Vector2 p;
    [SerializeField, Header("End Vector")]
    private Vector2 q;

    public Line(Vector2 _p, Vector2 _q)
    {
        this.p = _p;
        this.q = _q;
    }

    public Vector2 P()
    {
        return this.p;
    }

    public Vector2 Q()
    {
        return this.q;
    }

    public float Length()
    {
        return Vector2.Distance(this.p, this.q);
    }
}
