using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MorphTextures : MonoBehaviour {

    [SerializeField]
    GameObject gameObjectA, gameObjectB;

    [SerializeField]
    List<Line> aLines, bLines;

    [SerializeField]
    float a = 0.001f;
    [SerializeField, Range(0.0f, 2f)]
    float p = 1.4f;
    [SerializeField, Range(0, 2)]
    float b = 0.5f;

    [SerializeField, Range(0, 1)]
    float alpha = 0.5f;


	// Use this for initialization
	void Start () {
        // Get access to the tect
        Sprite spriteA = gameObjectA.GetComponent<SpriteRenderer>().sprite;
        Sprite spriteB = gameObjectB.GetComponent<SpriteRenderer>().sprite;

        CreateTargetImage("A Lines", DebugLines(aLines, spriteA.texture), new Vector3(-15f, 0), new Vector3(5, 5, 5));
        CreateTargetImage("B Lines", DebugLines(bLines, spriteA.texture), new Vector3(-15f, -15f), new Vector3(5, 5, 5));

        Texture2D texA;
        Texture2D texB;
        Texture2D blend;

        //for (int i = 1; i <= 1; i++)
        //{
        //    List<Line> frameLinesA = new List<Line>();
        //    List<Line> frameLinesB = new List<Line>();

        //    for (int j = 0; j < aLines.Count; j++)
        //    {
        //        Vector2 deltaPa = (aLines[i].P() - bLines[i].P()) / i;
        //        Vector2 deltaQa = (aLines[i].P() - bLines[i].P()) / i;

        //        Line intermediateALine = new Line((aLines[i].P() + deltaPa).ToUnityCoordSys(spriteA.texture.height), (aLines[i].Q() + deltaQa).ToUnityCoordSys(spriteA.texture.height));

        //        frameLinesA.Add(intermediateALine);

        //        Vector2 deltaPb = (bLines[i].P() - aLines[i].P()) / i;
        //        Vector2 deltaQb = (bLines[i].P() - aLines[i].P()) / i;

        //        Line intermediateBLine = new Line((bLines[i].P() + deltaPb).ToUnityCoordSys(spriteB.texture.height), (bLines[i].Q() + deltaQb).ToUnityCoordSys(spriteB.texture.height));

        //        frameLinesB.Add(intermediateBLine);
        //    }


        //    // Warp image A 
        //    texA = MultiLineWarp(spriteA, spriteB, frameLinesA, frameLinesB, spriteA.texture.width, spriteA.texture.width);

        //    spriteA = TextureToSprite(texA);
        //    // Warp image B
        //    texB = MultiLineWarp(spriteB, spriteA, frameLinesB, frameLinesA, spriteA.texture.width, spriteA.texture.width);
        //    texB.Apply();

        //    spriteB = TextureToSprite(texB);
        //    // Blend them
        //    blend = BlendWarpedImages(texA, texB, alpha);

        //    CreateTargetImage("Warp A " + i, texA, new Vector3(i * 15f, 0, 0), new Vector3(5, 5, 5));
        //    CreateTargetImage("Warp B " + i, texB, new Vector3(i * 15f, 15f, 0), new Vector3(5, 5, 5));
        //    CreateTargetImage("Blend " + i, blend, new Vector3(i * 15f, 30f, 0), new Vector3(5, 5, 5));
        //}

        List<Line> flippedA = new List<Line>(), flippedB = new List<Line>();
        for(int i = 0; i < aLines.Count; i++)
        {
            Line lineA = new Line(aLines[i].P().ToUnityCoordSys(spriteA.texture.height), aLines[i].Q().ToUnityCoordSys(spriteA.texture.height));
            Line lineB = new Line(bLines[i].P().ToUnityCoordSys(spriteA.texture.height), bLines[i].Q().ToUnityCoordSys(spriteA.texture.height));

            flippedA.Add(lineA);
            flippedB.Add(lineB);
        }
        texA = MultiLineWarp(spriteA, spriteB, flippedA, flippedB, spriteA.texture.width, spriteA.texture.height);
        texB = MultiLineWarp(spriteB, spriteA, flippedB, flippedA, spriteA.texture.width, spriteA.texture.height);
        blend = BlendWarpedImages(texA, texB, alpha);
        CreateTargetImage("Warp A", texA, new Vector3(15f, 0, 0), new Vector3(5, 5, 5));
        CreateTargetImage("Warp B", texB, new Vector3(15f, 15f, 0), new Vector3(5, 5, 5));
        CreateTargetImage("Blend", blend, new Vector3(15f, 30f, 0), new Vector3(5, 5, 5));
    }

    Texture2D DebugLines (List<Line> lines, Texture2D tex)
    {
        Texture2D debug = new Texture2D(tex.width, tex.height);
        Color[] colors = new Color[tex.width * tex.height];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = Color.clear;
        debug.SetPixels(colors);
        foreach (Line line in lines)
        {
            Line flippedLine = new Line(line.P().ToUnityCoordSys(tex.height), line.Q().ToUnityCoordSys(tex.height));
            debug.SetPixel((int)flippedLine.P().x, (int)flippedLine.P().y, Color.red);
            debug.SetPixel((int)flippedLine.Q().x, (int)flippedLine.Q().y, Color.red);
        }
        debug.Apply();
        return debug;
    }

    void CreateTargetImage (string name, Texture2D tex, Vector3 position, Vector3 scale)
    {
        GameObject go = new GameObject(name);
        go.AddComponent<SpriteRenderer>().sprite = TextureToSprite(tex);
        go.transform.position = position;
        go.transform.localScale = scale;

    }

    Texture2D SingleLineTransformation(Sprite srcSprite)
    {
        Texture2D warpedTexture = new Texture2D(srcSprite.texture.width,srcSprite.texture.height);
        Line destLine = bLines[0];
        Line srcLine = aLines[0];

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
        float u = Vector2.Dot((X - destLine.P()), (destLine.Q() - destLine.P())) / (destLine.Q() - destLine.P()).sqrMagnitude;
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

    Texture2D MultiLineWarp (Sprite srcSprite, Sprite destSprite, List<Line> sourceLines, List<Line> destinationLines, int width, int height)
    {
        Texture2D destinationTexture = new Texture2D(width, height);

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
                for (int i = 0; i < destinationLines.Count; i++)
                {
                    // Calculate u,v based on Pi Qi
                    UV uv = CalculateUV(destinationLines[i], xPixel);
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
                        dist = Vector2.Distance(xPixel, destinationLines[i].P());
                    }
                    else if (uv.u > 1)
                    {
                        dist = Vector2.Distance(xPixel, destinationLines[i].Q());
                    }

                    // weight = (length^p / (a + dist)))^b
                    float weight = Mathf.Pow((Mathf.Pow(destinationLines[i].Length(), p) / (a + dist)), b);
                    // DSUM += Di * weight
                    DSUM += Di * weight;
                    // weightSum += weight;
                    weightSum += weight;

                    lineIndex++;
                }


                // X' = X + DSUM / weightsum
                xPrimePixel = xPixel + DSUM / weightSum;

                if (xPrimePixel.x >= 0 && xPrimePixel.x < destinationTexture.width && xPrimePixel.y >= 0 && xPrimePixel.y < destinationTexture.height)
                {
                    // destinationImage (X) = sourceImage (X')
                    destinationTexture.SetPixel(x, y, srcSprite.texture.GetPixel((int)xPrimePixel.x, (int)xPrimePixel.y));
                }

                if (xPrimePixel.x > destinationTexture.width || xPrimePixel.y > destinationTexture.height)
                {

                }
            }
        }
        destinationTexture.Apply();
        return destinationTexture;
    }

    Sprite TextureToSprite(Texture2D tex)
    {
        Rect rec = new Rect(0, 0, tex.width, tex.height);

        Sprite spriteFromTex = Sprite.Create(tex, rec, new Vector2(0, 0), 100);

        return spriteFromTex;
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
        //float length = Mathf.Sqrt(Mathf.Pow(q.x - p.x, 2) + Mathf.Pow(q.y - p.y, 2));
        return Vector2.Distance(this.q, this.p);
    }
}
