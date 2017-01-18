using UnityEngine;
using System.Collections;

public class MorphTextures : MonoBehaviour {

    public GameObject src2DGameObject, dest2DGameObject;

    private SpriteRenderer srcSpriteRenderer, destSpriteRenderer;

	// Use this for initialization
	void Start () {
        srcSpriteRenderer = src2DGameObject.GetComponent<SpriteRenderer>();
        destSpriteRenderer = dest2DGameObject.GetComponent<SpriteRenderer>();

        Texture2D texToModify = textureFromSprite(srcSpriteRenderer.sprite);

        srcSpriteRenderer.sprite = spriteFromTexture(modifyTextures(texToModify));

    }

    // Update is called once per frame
    void Update () {
    }

    Texture2D modifyTextures(Texture2D texture)
    {
        // colors used to tint the first 3 mip levels
        Color[] colors = new Color[3];
        colors[0] = Color.red;
        colors[1] = Color.green;
        colors[2] = Color.blue;
        int mipCount = Mathf.Min(3, texture.mipmapCount);

        // tint each mip level
        for (int mip = 0; mip < mipCount; ++mip)
        {
            Color[] cols = texture.GetPixels(mip);
            for (int i = 0; i < cols.Length; ++i)
            {
                if (cols[i].a != 0f)
                    cols[i] = Color.Lerp(cols[i], colors[mip], 0.5f);
            }
            texture.SetPixels(cols, mip);

        }
        // actually apply all SetPixels, don't recalculate mip levels
        texture.Apply(false);

        return texture;
    }

    Sprite spriteFromTexture(Texture2D tex)
    {
        Rect rec = new Rect(0, 0, tex.width, tex.height);

        Sprite spriteFromTex = Sprite.Create(tex, rec, new Vector2(0.5f, 0.5f), 100);

        return spriteFromTex;
    }

    public static Texture2D textureFromSprite(Sprite sprite)
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
