using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
 
    
    public GameObject flameSpot;
    public GameObject flameDirectional;
    
    private Light flame;
    private bool tremble;
    
    private System.Random rnd;

    //public bool moveCamera;
    //public Vector3 cameraTarget, centerPoint;

    //public float speed = 100;

    //public float centerX, centerY;
    //public float rotX, rotY, rotZ;
    
    
    void Start()
    {
        //rotX = rotY = rotZ = 0;
        rnd = new System.Random();

        //moveCamera = false;
        
        //initialize flame trembling
        tremble = true;
        flame = flameDirectional.GetComponent<Light>();
        //StartCoroutine("FlameTrembling");
    }


    void Update()
    {

        /*if (moveCamera)
        {
            float step =  speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, cameraTarget, step);
            transform.Rotate( new Vector3(rotX,rotY,rotZ) * step/100);
            //transform.LookAt(new Vector3(centerX , centerY, -0.1f));
            //transform.RotateAround(centerPoint, Vector3.up, step);
        }
        if (Vector3.Distance(transform.position, cameraTarget) < 0.001f)
        {
            moveCamera = false;
        }*/
    }
    
    
    IEnumerator FlameTrembling()
    {
        while (true)
        {
            //tremble = true;
            //flame.intensity = (float)(0.8f + (0.6f * rnd.NextDouble()));
            double lightInc = rnd.NextDouble() - 0.5f;
            flame.intensity += (lightInc >= 0.0) ? 0.1f : -0.1f;
            yield return new WaitForSeconds(0.1f);
            //flame.intensity = 1.0f;
            //tremble = false;
            //Debug.Log("trembling...");
            float timeToNextTrembling = 0.5f + (float) rnd.NextDouble() * 0.5f;
            yield return new WaitForSeconds(timeToNextTrembling);
            //yield return new WaitForSeconds(2f + (float)(0.3 + 2.0f * rnd.NextDouble()));  //wait next trembling
        }
    }
}
