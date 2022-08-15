
using System.Collections.Generic;
using UnityEngine;

public class CameraManager: ScriptableObject
{

    public enum Angle
    {
        Front = 0,
        East = 1,
        West = 2,
        Up = 3
    }
    

    //private Vector3[,] povEast, povWest, povFront, povUp;
    private Dictionary<int,int> povIndex = new Dictionary<int,int>();
    
    Vector3 povEast = new Vector3(-33.8f, -43.7f, 59);
    Vector3 povWest = new Vector3(-33.8f, 43.7f, -59);
    Vector3 povFront = new Vector3(-45, 0, 0);
    private float[] camHolderZ = new[] {1.2f, 1.65f, 2.0f, 2.44f, 3.0f, 3.5f, 3.93f, 4.4f, 4.9f};
    private float[] camRotX = new[] {1.1f, 1.68f, 1.55f, 1.94f, 2.4f, 2.7f, 2.7f, 2.84f, 2.84f};
    
    private GameObject cam;

    private GameObject camHolder;
    
    private Vector3 positionCamera;

    public Vector3 PositionCamera
    {
        get => positionCamera;
        set => positionCamera = value;
    }

    private Universe uni;
    
    
    
    /*
     *  CAMERA Initialization
     */
    public void Init(GameObject cam, GameObject camHolder, Universe uni)
    {
        this.cam = cam;
        this.uni = uni;
        this.camHolder = camHolder;

        createPOVs();
    }

    /*
     * Camera positioning methods
     */
    public void Pos(float x, float y, float z)
    {
        cam.transform.position = new Vector3(x,y,z);
    }
    public void Pos(float z, float rotX, Vector3 rot, bool firstPositioning)
    {

        if (firstPositioning)
        {           
            //camHolder.GetComponent<CamHolderController>().centerX = uni.centerX;
            //camHolder.GetComponent<CamHolderController>().centerY = uni.centerY;
            
            camHolder.GetComponent<CamHolderController>().transform.position = new Vector3(uni.centerX, uni.centerY, -0.01f);
            camHolder.GetComponent<CamHolderController>().transform.localScale = new Vector3( 1f, 1f, z ); //1.65
            camHolder.GetComponent<CamHolderController>().transform.rotation = Quaternion.Euler(rot);

            cam.GetComponent<CameraController>().transform.Rotate( new Vector3( rotX, 0f, 0f ));

        } else
        {
           Debug.Log("rot da cam atual:" + cam.transform.eulerAngles.y);
           Debug.Log("rot alvo:" + rot.y);

           camHolder.GetComponent<CamHolderController>().targetRotation = rot;
           camHolder.GetComponent<CamHolderController>().rotating = true;
        }
        
        //cam.transform.LookAt(new Vector3(uni.centerX + 20, uni.centerY, -0.1f));
    }

    public void Positionate(int sizeX, Angle angle, bool firstPositioning)
    {
        switch (angle)
        {
            case Angle.Front:
                Pos(camHolderZ[povIndex[sizeX]], camRotX[povIndex[sizeX]], povFront, firstPositioning);
                break;
            case Angle.East:
                Pos(camHolderZ[povIndex[sizeX]], camRotX[povIndex[sizeX]], povEast, firstPositioning);
                break;
            case Angle.West:
                Pos(camHolderZ[povIndex[sizeX]], camRotX[povIndex[sizeX]], povWest, firstPositioning);
                break;
           // case Angle.Up:
             //   Pos(camHolderZ[povIndex[sizeX]], camRotX[povIndex[sizeX]], povFront,firstPositioning);
             //   break;
        }
    }
    
    /*
     * ZOOM
     */
    public void Zoom(float x, float y, float z)
    {
        cam.transform.position = new Vector3(x,y,z);
    }

    /*
     * Getters for camera position
     */
    public float GetPosX()
    {
        return positionCamera.x;
    }
    public float GetPosY()
    {
        return positionCamera.y;
    }
    public float GetPosZ()
    {
        return positionCamera.z;
    }
    
    private void createPOVs()
    {
        float m = 25;
        float angle = 45 * 0.0174533f; //angle in radians  ,i.e., 45 degrees

        povIndex.Add(5,0);
        povIndex.Add(7,1);
        povIndex.Add(9,2);
        povIndex.Add(11,3);
        povIndex.Add(13,4);
        povIndex.Add(15,5);
        povIndex.Add(17,6);
        povIndex.Add(19,7);
        povIndex.Add(21,8);
    }
}
