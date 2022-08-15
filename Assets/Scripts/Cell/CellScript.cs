using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellScript : MonoBehaviour
{
    /*
     *  Movimentation info
     */
    public int X, Y;
    private int targetX, targetY;
    private float finalX, finalY, currX, currY;
    private int displacementInX, displacementInY;

    private float moveHorizontal, moveVertical;
    public float velocity;

    private bool foundColumn, foundRow;

    private float quadSize;

    /*
     * Size & scale info and cell Z
     */
    private float CELLSIZE = 0.7f;
    private float cellZ; //-0.55f;   

    /*
     * Identity dependent controls  (Target & single (player) cells)
     */
    public bool isTarget;
    public bool borning;
    private int borningPhase;
    private float scaleCntr, posCntr;

    /*
     * Ploinking auxiliary vars
     */
    private CellLevel.Side ploinkSide;

    /*
     * universe info
     */
    public int universeSize;

    public int UniverseSize
    {
        get => universeSize;
        set => universeSize = value;
    }

    /*
     * owner CELL of this script
     */
    public Cell _cell = new Cell();

    public FSM moveFSM;
    
    /*
     *  callbacks
     */
    public delegate void Del(int idOrig, int idDest);

    public delegate void Del2(int creatureId, int x, int y);

    public delegate void Del3(int cellId, bool isTarget);

    public delegate void Del4(int cellId, bool isTarget);

    public delegate void Del5(float X, float Y);

    public Del callbackMergeCells;
    public Del2 callbackMoveEnded;
    public Del3 callbackDestroyCell;
    public Del4 callbackToNewLevel;
    public Del5 callbackCreateNewTarget;

    /*
     * OSCILLATORs
     */
    private Oscillator oscMatch;
    private Oscillator oscPloink;
    private Oscillator oscFagocitation;
    private Oscillator oscPluft;


    /*
     * START
     */
    void Start()
    {
        Debug.Log("delta time:" + Time.deltaTime);
        Debug.Log("cell velocity:" + velocity);

        cellZ = -(CELLSIZE / 2f) - 0.05f;

        //borning = true;
        borningPhase = -1; //used by player cells

        //Target counters
        scaleCntr = 0;

        if (!isTarget)
        {
            if(borning)
                posCntr = -18f;
            else 
                posCntr = cellZ;
            this.transform.position = new Vector3(currX * quadSize, currY * quadSize, posCntr);
            StartCoroutine("WaitToStart");
        }
        else
        {
            //posCntr = 0;
            posCntr = -cellZ;
            transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
            this.transform.position = new Vector3(currX * quadSize, currY * quadSize, posCntr);
        }


        oscMatch = new Oscillator(75, 180, 1.25f, velocity * 12);
        oscPloink = new Oscillator(step: velocity * 40f);
        oscPluft = new Oscillator(inf: 0, sup: 180, amplitude: 1f, velocity * 20f);
    }

    private void Update()
    {
        int currX_int, currY_int, tgX, tgY;

        if (borning)
        {
            if (!isTarget)
            {
                switch (borningPhase)
                {
                    case 0:
                        posCntr += velocity; // /2f; is slower
                        this.transform.position = new Vector3((currX * quadSize), (currY * quadSize), posCntr);

                        if (posCntr >= cellZ)
                        {
                            posCntr = -18f;
                            borningPhase = 1;
                            transform.localScale = new Vector3(CELLSIZE, CELLSIZE, CELLSIZE);
                        }

                        break;
                    case 1:
                        float scale = oscPluft.Next() * CELLSIZE;
                        float scaleZ;
                        float scaleX = CELLSIZE;
                        float scaleY = CELLSIZE;

                        if (oscPluft.GetValue() < 179)
                        {
                            //crashes to ground
                            scaleZ = CELLSIZE - scale;
                        }
                        else
                        {
                            scaleX = scaleY = scaleZ = CELLSIZE;
                            borningPhase = 2;
                        }

                        this.transform.position =
                            new Vector3((currX * quadSize), (currY * quadSize), cellZ + (scale / 2));
                        transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                        break;
                    case 2:
                        borningPhase = 0;
                        borning = false;
                        break;
                }
            }
            else //its a Target cell
            {
                if (scaleCntr < CELLSIZE)
                {
                    scaleCntr += (velocity / 30);
                    transform.localScale = new Vector3(scaleCntr, scaleCntr, scaleCntr);
                }
                else
                {
                    transform.localScale = new Vector3(CELLSIZE, CELLSIZE, CELLSIZE);
                }

                if (posCntr >= cellZ)
                {
                    posCntr -= (velocity / 30);
                    this.transform.position = new Vector3((currX * quadSize), (currY * quadSize), posCntr);
                }
                else
                {
                    borning = false;
                    callbackCreateNewTarget(currX * quadSize, currY * quadSize);
                }
            }
        }
        else
        {
            switch (_cell.state)
            {
                case Cell.State.Flying:

                    currX += moveHorizontal;
                    currY += moveVertical;

                    _cell.flyingCntr++;

                    this.transform.position = new Vector3(currX * quadSize, currY * quadSize,
                        cellZ + (-Mathf.Sin((_cell.flyingCntr) * 1.5f * 0.017453292f)) * 5f);

                    transform.Rotate(new Vector3(0f, 0f, 3f)); //z: 1.5f

                    if (Mathf.Round(currX * universeSize) == targetX * universeSize &&
                        Mathf.Round(currY * universeSize) == targetY * universeSize)
                    {
                        //complete rotation movement if needed
                        for (int z = 0; z < (120 - _cell.flyingCntr); z++)
                        {
                            transform.Rotate(new Vector3(0f, 0f, 3f)); //z: 1.5f
                        }

                        //derotate cell (necessary to mantain texture orientation)
                        transform.Rotate(new Vector3(0f, 0f, -360f)); //z: -180f

                        _cell.state = Cell.State.Idle;

                        _cell.flyingCntr = 0;

                        this.X = (int) Mathf.Round(currX);
                        this.Y = (int) Mathf.Round(currY);

                        this.transform.position =
                            new Vector3(this.X * quadSize, this.Y * quadSize, cellZ);
                    }

                    break;

                case Cell.State.Moving:

                    if (!foundColumn)
                    {
                        currX += moveHorizontal;

                        if (moveHorizontal < 0)
                        {
                            transform.Rotate(new Vector3(0f, +90f, 0f) * velocity);
                        }
                        else
                        {
                            if (moveHorizontal > 0)
                            {
                                transform.Rotate(new Vector3(0f, -90f, 0f) * velocity);
                            }
                        }
                    }
                    else
                    {
                        if (!foundRow)
                        {
                            currY += moveVertical;

                            if (moveVertical < 0)
                            {
                                transform.Rotate(new Vector3(-90f, 0f, 0f) * velocity);
                            }
                            else
                            {
                                if (moveVertical > 0)
                                {
                                    transform.Rotate(new Vector3(+90f, 0f, 0f) * velocity);
                                }
                            }
                        }
                    }

                    //TRANSLATION

                    this.transform.position = new Vector3((currX * quadSize), (currY * quadSize), cellZ);

                    currX_int = (int) (Mathf.Round(currX * 100));
                    currY_int = (int) (Mathf.Round(currY * 100));

                    //if (!foundColumn && (Mathf.Round(currX*100000) == targetX * 100000)) foundColumn = true;

                    if (!foundColumn && (currX_int == targetX * 100))
                    {
                        foundColumn = true;

                        /*
                         * undo previous horizontal rotations
                         */
                        transform.Rotate(new Vector3(0f, 90f * (displacementInX % 4), 0f));
                        Debug.Log("derotate X:" + (displacementInX % 4));
                    }

                    if (foundColumn && (currY_int == targetY * 100))
                    {
                        foundRow = true;

                        /*
                         * undo previous vertical rotations
                         */
                        transform.Rotate(new Vector3(-90f * (displacementInY % 4), 0f, 0f));
                        Debug.Log("derotate Y:" + (displacementInY % 4));
                    }

                    if (foundRow && foundColumn)
                    {
                        _cell.state = Cell.State.Idle;
                        foundColumn = false;
                        foundRow = false;

                        this.X = (int) Mathf.Round(currX);
                        this.Y = (int) Mathf.Round(currY);
                        
                        //callbackMoveEnded(_cell.Id, X, Y); //calls MOVEENDED
                        moveFSM.Trigger("2MoveEnded",_cell.Id, X, Y);
                    }

                    break;

                case Cell.State.Ploinking:

                    float scale = oscPloink.Next() * (CELLSIZE - 0.1f);
                    switch (ploinkSide)
                    {
                        case CellLevel.Side.All:
                            transform.localScale = new Vector3(CELLSIZE - scale, CELLSIZE - scale, CELLSIZE);

                            break;
                        case CellLevel.Side.Down:
                            transform.localScale = new Vector3(CELLSIZE, CELLSIZE + scale, CELLSIZE);
                            this.transform.position = new Vector3((currX * quadSize),
                                (currY * quadSize) + scale, cellZ);

                            break;
                        case CellLevel.Side.Up:
                            transform.localScale = new Vector3(CELLSIZE, CELLSIZE + scale, CELLSIZE);
                            this.transform.position = new Vector3((currX * quadSize),
                                (currY * quadSize) - scale, cellZ);

                            break;
                        case CellLevel.Side.Right:
                            transform.localScale = new Vector3(CELLSIZE + scale, CELLSIZE, CELLSIZE);
                            this.transform.position =
                                new Vector3((currX * quadSize) + scale,
                                    currY * quadSize, cellZ);

                            break;
                        case CellLevel.Side.Left:
                            transform.localScale = new Vector3(CELLSIZE + scale, CELLSIZE, CELLSIZE);
                            this.transform.position =
                                new Vector3((currX * quadSize) - scale,
                                    currY * quadSize, cellZ);

                            break;
                    }

                    if (oscPloink.GetCicle() > oscPloink.HalfCicle())
                    {
                        _cell.state = Cell.State.Idle;
                        oscPloink.Reset();
                    }

                    break;

                case Cell.State.Matching:

                    float size = oscMatch.Next();

                    transform.localScale = new Vector3(size, size, size);

                    if (oscMatch.GetCicle() > oscMatch.HalfCicle())
                    {
                        _cell.state = Cell.State.Idle;
                        _cell.matchCntr = 0;

                        oscMatch.Reset();
                        StartCoroutine("DelayToTransition");

                        /*
                         * calls back animation -> particle systems in origin and dest.
                         */
                        callbackDestroyCell(_cell.Id, (_cell.IsTargetPart ? true : false));
                    }

                    break;
            }
        }
    }

    /*
     * Waits for animation to conclude
     */
    IEnumerator DelayToTransition()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            callbackToNewLevel(_cell.Id, (_cell.IsTargetPart ? true : false));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
         * merges from origin to destiny:
         * both cells will trigger, but only origin must call merge!!!
         * otherwise merge will be called twice...
         */

        int idO = _cell.Id;

        Debug.Log("trigger id:" + idO);

        if (_cell.state == Cell.State.Moving) //Cell.moving
        {
            //derotate cell 
            transform.Rotate(new Vector3(0f, 0f, 0f)); //z -180f 

            if (other == null)
                Debug.Log("other is null! better to test that in cellscript");
            
            if (other.name == "Cube(Clone)")
            {
                int idD = other.gameObject.GetComponent<CellScript>()._cell.Id;

                _cell.state = Cell.State.Idle;
                
                //callbackMergeCells(idO, idD); //calls MERGECELLS  
                moveFSM.Trigger("2Collision",idO, idD);
            }
            else  //COLLIDING WITH SOMETHING ELSE...
            {
                 Debug.Log("na CellScript/OnTriggerEnter: other.name == " + other.name);
                 
                 if (other.gameObject == null)
                 { 
                     Debug.Log("na CellScript/OnTriggerEnter: other.gameObject  its null");
                 }
                 else
                 {
                     if (other.gameObject.GetComponent<CellScript>() == null)
                     {
                         Debug.Log("na CellScript/OnTriggerEnter: other.gameObject.GetComponent<CellScript>()  its null");  
                     }
                 }               
            }
        }
    }

    public void SetTarget(int x, int y, bool flying)
    {
        this.targetX = x;
        this.targetY = y;

        displacementInX = x - this.X;
        displacementInY = y - this.Y;

        Debug.Log("DISPLACE x:" + displacementInX + " y:" + displacementInY);

        Debug.Log("X:" + X + "  Y:" + Y);

        print("pre curr  X:" + this.currX + " Y:" + this.currY);

        if (!flying) //than MOVING !!!
        {
            _cell.state = Cell.State.Moving;
            foundColumn = false;
            foundRow = false;

            Debug.Log("settarget:" + this.targetX + " " + this.targetY);
            Debug.Log("veloc:" + velocity);

            if (targetX > X)
            {
                moveHorizontal = velocity;
            }
            else
            {
                if (targetX < X)
                {
                    moveHorizontal = -velocity;
                }
                else
                {
                    moveHorizontal = 0;
                    foundColumn = true;
                }
            }

            if (targetY > Y)
            {
                moveVertical = velocity;
            }
            else
            {
                if (targetY < Y)
                {
                    moveVertical = -velocity;
                }
                else
                {
                    moveVertical = 0;
                    foundRow = true;
                }
            }

            print("moves hor:" + moveHorizontal + "   " + moveVertical);
            print("pos curr  X:" + this.currX + " Y:" + this.currY);
        }
        else //than FLYING
        {
            Debug.Log("will fly!");

            _cell.state = Cell.State.Flying;
            float diffX = (targetX - X);
            float diffY = (targetY - Y);
            float dist = Mathf.Sqrt(diffX * diffX + diffY * diffY);

            moveHorizontal = diffX / 120f;
            moveVertical = diffY / 120f;
            _cell.rotOffset = 0.75f;
        }
    }

    public void SetPosition(int x, int y, float quadSize)
    {
        this.X = x;
        this.Y = y;
        this.quadSize = quadSize;

        currX = x;
        currY = y;

        //this.transform.position = new Vector3(currX * quadSize, currY * quadSize, cellZ);
    }

    public void Ploink(CellLevel.Side side)
    {
        _cell.state = Cell.State.Ploinking;
        ploinkSide = side;
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(1f + Random.value); //WaitForSeconds(0.05f + Random.value);
        borningPhase = 0;
    }
}