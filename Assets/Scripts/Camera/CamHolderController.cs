using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *
 *  CamHolder is a camera boom arm, centered @ middle of board
 * 
 */
public class CamHolderController : MonoBehaviour
{
    public float centerX, centerY;
    
    public Vector3 targetRotation;

    public bool rotating;
    public bool zooming;
    
    void Start()
    {
        rotating = false;
        zooming = false;

    }


    void Update()
    {
        if (rotating)
        {
          StartCoroutine(LerpFunction(Quaternion.Euler(targetRotation), 1));
          rotating = false;
        } 
    }
    
    /*
     * Lerp: Linear interpolation
     */
    IEnumerator LerpFunction(Quaternion endValue, float duration)
    {
        float time = 0;
        Quaternion startValue = transform.rotation;

        while (time < duration)
        {
            transform.rotation = Quaternion.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endValue;
    }
}
