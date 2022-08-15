using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{

    public int spritesQty;
    private SpriteRenderer sr;
    public Sprite[] spriteArray;
    private int index;
    public bool fadeOut;

    public float speed = 0.01f;
    private float step, alpha;
    void Start()
    {
        sr = this.GetComponent<SpriteRenderer>();
        spritesQty = spriteArray.Length;
        Debug.Log("+++++++++++++++++++++++++++++++++++++++++ arrya size:" + spritesQty);
        index = 0;

        alpha = 1.0f;
        fadeOut = false;
        
        step = speed * (Time.deltaTime / 25f);
        
        //sr.sprite = spriteArray[0];
        sr.color = new Color(1f, 1f, 1f, 1f);
        //Debug.Log("Texture : " + sr.sprite.texture);
        StartCoroutine("UpdateFrame");
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeOut)
        {
            alpha = alpha - step;
            sr.color = new Color(1f, 1f, 1f, alpha);
            if (alpha <= 0f)
            {
                fadeOut = false;
            }
        }

    }

    void FadeOut()
    {
        fadeOut = true;
    }
    void Reset()
    {
        alpha = 1.0f;
        fadeOut = false;
        sr.color = new Color(1f, 1f, 1f, alpha);
        //restart coroutine?
    }
    IEnumerator UpdateFrame()
    {
        while (true)
        {
            sr.sprite = spriteArray[index];
            yield return new WaitForSeconds(2);
            
            if (index < spritesQty - 1)
                index++;
            else
            {
                index = 0;
            }
        }
    }
}
