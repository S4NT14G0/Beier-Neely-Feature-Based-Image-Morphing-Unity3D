using UnityEngine;
using System.Collections;


public class FindPixelCoords : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}

    void Update()
    {
        if (Input.GetMouseButton(0)) {

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit)
            {
                Debug.Log("Target Position: " + hit.collider.gameObject.transform.position);
            }


        //if (hit.collider != null)
        //    {
        //        Debug.Log(hit.transform.localPosition);
        //        float distance = Mathf.Abs(hit.point.y - transform.position.y);
        //        //float heightError = floatHeight - distance;
        //        //float force = liftForce * heightError - rb2D.velocity.y * damping;
        //        //rb2D.AddForce(Vector3.up * force);
        //        SpriteRenderer renderer = hit.transform.parent.GetComponent<SpriteRenderer>();

        //        Texture2D tex = (Texture2D)renderer.sprite.texture;
        //        Vector2 pixelUV = hit.transform.localPosition;
        //        Debug.Log((int)(pixelUV.x * renderer.material.mainTexture.width) + "--" + (int)(pixelUV.y * renderer.material.mainTexture.height));
        //    }
        //    else
        //    {
        //        Debug.Log("NULL");
        //    }
        }


    }

}
