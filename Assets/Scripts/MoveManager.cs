/*
 
 *                             O O  O    O         O   MOVE MANAGER   O         O    O  O O
     
*/

using System;
using System.Collections.Generic;
using System.Data;
using DefaultNamespace;
using UnityEngine;


public class MoveManager : ScriptableObject
{

    public bool cellMoving;
    public bool fagocitation;

    public const bool NotFlying = false;
    public const bool Flying = true;
    

    public bool CellMoving
    {
        get => cellMoving;
        set => cellMoving = value;
    }

    public bool Fagocitation
    {
        get => fagocitation;
        set => fagocitation = value;
    }

    /*
     * origin cell selected (by player); contains cell ID; its the INDEX in cells array
     */
    public int originCell;

    public int OriginCell
    {
        get => originCell;
        set => originCell = value;
    }

    /*
     * Managers access
     */
    private QuadManager qm;
    private CameraManager cm;
    private SoundManager sm;
    private LevelManager lm;

    public LevelManager Lm
    {
        get => lm;
        set => lm = value;
    }
    
    private Universe uni;
    private Texturer tx;

    /*
     * Lists access
     */
    private Lists ls;
    private List<GameObject> cells;
    private List<Creature> creatures;

    
    /*
     *   PLATFORM & INPUT specs
     */

    //MOUSE
    private Mouse mouse; //todo mouse should me deactivable by player

    //KEYBOARD
    private Keyboard keyboard;

    //INPUT MODE
    private PlatformManager.InputMode currInputMode;

    //PLATFORM
    private PlatformManager.Platform currPlatform;

    public delegate void Del(Cell c, int indexPlayerCell);
    public Del callbackCompareWithTargets;

    public FSM moveFSM = new FSM("move"); 
    
    /*
     *  Move Manager Initialization
     */
    public void Init(QuadManager qm, CameraManager cm, SoundManager sm, PlatformManager pm, Lists ls, Universe uni,
        Texturer tx, List<Creature> creatures)
    {
        this.qm = qm;
        this.cm = cm;
        this.sm = sm;
        this.ls = ls;
        this.cells = ls.cells;
        this.uni = uni;
        this.tx = tx;
        this.creatures = creatures;

        /*
         * initialize Move and Mode
         */ 
        Reset();

        /*
         * initialize mouse
         */ 
        mouse = (Mouse) ScriptableObject.CreateInstance("Mouse"); //param: name of script
        mouse.Init();
        mouse.callbackMouseStateChanged = MouseStateChanged;

        /*
         * initialize keyboard
         */
        keyboard = (Keyboard) ScriptableObject.CreateInstance("Keyboard");
        keyboard.Init();
        keyboard.callbackKeyStateChanged = KeyStateChanged;

        /*
         * initialize platform and input mode
         */
        currPlatform = pm.CurrPlatform;
        currInputMode = pm.CurrInputMode;

        CreateFSM_move();

    }


    public void Reset()
    {
        cellMoving = false;
        fagocitation = false;

        originCell = -1;
    }


    /*
     *     REFRESH   called by GameManager.Update()
     */
    public void Refresh()
    {
        keyboard.Refresh();
        mouse.Refresh();

    }

    /*
     *     MOUSE STATE CHANGED:  CALLBACK from Mouse/Refresh()
     */
    private void MouseStateChanged(Mouse.MouseState ms)
    {
        currInputMode = PlatformManager.InputMode.Mouse;

        Debug.Log("MSC -----------------  ");
        Debug.Log("mouse state:" + ms);

        mouse.GetCollisionState();
 

        switch (ms)
        {
            /*
             *  IDLE
             */
            case Mouse.MouseState.Idle:
                break;
            /*
             *  SINGLE CLICK
             */
            case Mouse.MouseState.SingleClickLeft:
            case Mouse.MouseState.SingleClickRight:
                switch (mouse.mci.CurrCollision)
                {
                    case Mouse.MouseCollision.Cube:
                        moveFSM.Trigger("SCcube");
                        break;
                    case Mouse.MouseCollision.Quad:
                        moveFSM.Trigger("SCquad");
                        break;
                }
                mouse.SetIdle(); //to (re)enabling mouse; only after final mouse state,
                                 //like SingleClickLeft/DoubleClickLEft/etc 
                break;
            /* 
             *  DOUBLE CLICK
             */
            case Mouse.MouseState.DoubleClickLeft:
            case Mouse.MouseState.DoubleClickRight:    
                break;
            /*
             *  HOLD
             */
            case Mouse.MouseState.HoldLeft:
            case Mouse.MouseState.HoldRight: 
                switch (mouse.mci.CurrCollision)
                {
                    case Mouse.MouseCollision.Cube:
                        //moveFSM.Trigger("2Dress");  //or undress
                        break;
                }                        
                break;
            case Mouse.MouseState.HoldEnd:
                mouse.SetIdle();
                break;
            /*
             *  WHEEL
             */
            case Mouse.MouseState.Wheel_In:  //ZOOM IN
                break;
            case Mouse.MouseState.Wheel_Out: //ZOOM OUT
                break;
            default:
                break;
        }       


    }

    /*
    *     Keyboard STATE CHANGED:  CALLBACK from Keyboard/Refresh()
    */
    private void KeyStateChanged(Keyboard.KeyState ks)
    {
        currInputMode = PlatformManager.InputMode.Keyboard;

        /*
         *    ZOOM control
         */
        if (ks == Keyboard.KeyState.PLUS || ks == Keyboard.KeyState.MINUS)
        {
            // float x = cm.GetPosX();
            // float y = cm.GetPosY();
             float z = cm.GetPosZ();
            //if(z>)
            z += ((ks == Keyboard.KeyState.PLUS) ? 1.0f : -1.0f);
            //camera.transform.position = new Vector3(x,y,z);
            cm.Zoom(cm.GetPosX(), cm.GetPosY(), z);
        }

        /*
         * ROTATION control
         */
        switch (ks)
        {
            case Keyboard.KeyState.One:
                cm.Positionate(uni.UniverseSizeX, CameraManager.Angle.West, false);
                break;
            case Keyboard.KeyState.Two:
                cm.Positionate(uni.UniverseSizeX, CameraManager.Angle.Front, false);
                break;
            case Keyboard.KeyState.Three:
                cm.Positionate(uni.UniverseSizeX, CameraManager.Angle.East, false);
                break;
        }


        if (ks == Keyboard.KeyState.Q)
        {
            Application.Quit();
        }

        int x, y;

        switch (ks)
        {
            case Keyboard.KeyState.ArrowUp:
            case Keyboard.KeyState.W:
                qm.MoveTargetQuad(0, 1);
                break;
            case Keyboard.KeyState.ArrowDown:
            case Keyboard.KeyState.S:
                qm.MoveTargetQuad(0, -1);
                break;
            case Keyboard.KeyState.ArrowLeft:
            case Keyboard.KeyState.A:
                qm.MoveTargetQuad(-1, 0);
                break;
            case Keyboard.KeyState.ArrowRight:
            case Keyboard.KeyState.D:
                qm.MoveTargetQuad(1, 0);
                break;
            case Keyboard.KeyState.SPACE:
                x = qm.selectedQuadTarget.GetComponent<Quad>().x;
                y = qm.selectedQuadTarget.GetComponent<Quad>().y;
                if (uni.QuadType[x, y] == Quad.QuadTypes.Cell)
                {
                    moveFSM.Trigger("SCcube");
                }
                else
                {
                    moveFSM.Trigger("SCquad");
                }
                

                break;
            case Keyboard.KeyState.E:

                fagocitation = true;
                
                x = qm.selectedQuadTarget.GetComponent<Quad>().x;
                y = qm.selectedQuadTarget.GetComponent<Quad>().y;
                if (uni.QuadType[x, y] == Quad.QuadTypes.Cell)
                {
                    moveFSM.Trigger("SCcube");
                }
                else
                {
                    moveFSM.Trigger("SCquad");
                }

                break;
        }
    }


    /*
     *  MOVECELL:  called by state: State_Moving
     */

    private void MoveCell(int index, int destX, int destY)
    {
        //if(mouse.CurrState != Mouse.MouseState.HoldLeft && mouse.CurrState != Mouse.MouseState.HoldRight)
        //    _audios2.PlayOneShot(moveSound, 0.25f);

        int x = cells[index].GetComponent<CellScript>().X;
        int y = cells[index].GetComponent<CellScript>().Y;

        
        /*
         *  verifies if its a VALID Move
         *
         *  thats already verified @ CCM/CellMode.OnMove   [and reverified here...]
         * 
         */
        
        if ((x != destX || y != destY) && uni.QuadType[destX,destY] != Quad.QuadTypes.Target) //if so, really start moving
        {
            //Debug.Log("MoveCell > SET TARGET  x:" + destX + " y:" + destY);

               
            uni.SetQuadType(x, y, Quad.QuadTypes.Empty);   //quadrant is emptied!
            uni.Quads[x, y].GetComponent<Quad>().Cell = null;

            // if (moveMode == MoveMode.Drag)
            // {
            //     cells[index].GetComponent<CellScript>().velocity = uni.globalVelocity * 2; //    TURBO MODE !!!
            // }
            
            cells[index].GetComponent<CellScript>().SetTarget(destX, destY, NotFlying); 
            Debug.Log("MoveCell!");
        }
        else
        {
            //Debug.Log("MoveCell: origin and destiny are same, not moving!!!");
        }
        
        //WHAT TO DO IF OTHER  CELLS IN PATH?  collision will occure and consequently a merge
    }
    
    
    /*
     *  EXPLODE CELL
     */

    private void explodeCell(int index, int x, int y)
    {
        Cell c = ls.cells[index].GetComponent<CellScript>()._cell;

        //TraverseDebugger(c.getAllLevels(), null, 0);

        dismantleCell(c.getAllLevels(), x, y); // x,y si starting point for flying routes
        
                            
        // todo detach cell from creature
        // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        //                   
                    
        ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.CreatureId = -1;
        // find creature in creatures list and remove this cell
        //Creature.RemoveCell()

        
        // do that stuff in move ended ???
        // <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        
    }

    /*
     * traverses cell recursively
     */
    private void dismantleCell(List<CellLevel> level, int x, int y)
    {
        int cBack, cFore;
        CellQuadrant.Shape shape;
        int tt;


        for (int lev = 0; lev < level.Count; lev++)
        {
            List<CellQuadrant> qList = level[lev].getQuadrants();

            for (int q = 0; q < qList.Count; q++)
            {
                shape = qList[q].getShape();
                
                if (shape != CellQuadrant.Shape.None)
                {
                    cBack = qList[q].getBackColor();
                    cFore = qList[q].getForeColor();

                    tt = qList[q].TriangleType;

                    uni.Allocate(Quad.QuadTypes.Cell, null); //its a single cell

                    ls.cells.Add(Instantiate(uni.cell));
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().velocity = uni.globalVelocity;
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.Id = ls.idCellCntr++;
                    
                    
                    /*
                     * this new free cell dont takes part in any creature yet
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.CreatureId = -1;

                    /*
                     * sets cell as not target
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().isTarget = false;

                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().borning = false;

                    /*
                     *  plant callbacks
                     */
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().moveFSM = moveFSM;
                    
                    //ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackMergeCells = MergeCells;
                    //ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackMoveEnded = MoveEnded;
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackDestroyCell = lm.AnimateCellDestruction;
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackToNewLevel = lm.TransitionToNewLevel;
                    
                    /*
                     *  injects universe info in cell
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().universeSize = uni.UniverseSize;

                    /*
                     *  positionates and creates quadrant
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().SetPosition(x, y, uni.quadSize);

                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.AddLevel(new CellLevel(tx.TextureSize));

                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.getLevel(0).AddHorizontalQuadrant(
                        new CellQuadrant(cBack, cFore, (CellQuadrant.Shape) shape, tt), CellLevel.Side.Left);

                    /*
                     *  create texture of free cell
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<Renderer>().material.mainTexture =
                        tx.CreateCellTexture(ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell);

                    /*
                     *  Move cells in Flying mode...
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()
                        .SetTarget(uni.ColumnFree, uni.RowFree, Flying);

                    /*
                     *  updates universe info
                     */
                    uni.Quads[uni.ColumnFree, uni.RowFree].GetComponent<Quad>().Cell =
                        ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell;

                    /*
                     *  inserts new cell info in dest cell DNA
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.dna
                        .insert(lm.CalculateDNACellType(cBack, cFore, (int) shape, tt));
                }
                
                if (qList[q].refLevels != null)
                {
                    //Debug.Log((" going rec in dismantleCell!  @quad=" + q + " lev=" + lev));
                    dismantleCell(qList[q].refLevels, x, y);
                }
            }
        }
    }

    public void Fagocitates(Cell cDest, Cell cOrig)
    {
        cDest.getLastLevel().AddHorizontalQuadrant(new CellQuadrant(7, 7,
            (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), CellLevel.Side.Left);       
             
        cDest.getLastLevel().getQuadrants()[0].refLevels = cOrig.getAllLevels();

        CellQuadrant quadrantGeometry = cDest.getLevel(0).getQuadrants()[0]; 
             
        //traverses origin (modifying last level coordinates...)
        int currLevel = (cDest.getLevelsQuantity() <= 1) ? 1 : cDest.getLevelsQuantity() - 1;
        TraverseAllLevels(cDest.getLastLevel().getQuadrants()[0].refLevels, quadrantGeometry, CellLevel.Side.All,
            currLevel);       
    }
    
    private void CopyOriginToDestiny(Cell orig, Cell dest, CellLevel.Side side)
    {
       
        /*
         *  PLOINK insertion always occurs at level 0, creating a new root level
         *  and with only 2 new quadrants underneath
         */

        //creates new root level
        List<CellLevel> oldDestLevelsList = dest.getAllLevels();
        dest.SetLevels(new List<CellLevel>());
        dest.getAllLevels().Add(new CellLevel(tx.TextureSize));
        
        //add 2 quadrants
        if (side == CellLevel.Side.Right || side == CellLevel.Side.Left)
        {
            dest.getLevel(0).AddHorizontalQuadrant(new CellQuadrant(7, 7,
                (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
            dest.getLevel(0).AddHorizontalQuadrant(new CellQuadrant(7, 7,
                (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);            
        }
        else
        {
            dest.getLevel(0).AddVerticalQuadrant(new CellQuadrant(7, 7,
                (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
            dest.getLevel(0).AddVerticalQuadrant(new CellQuadrant(7, 7,
                (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
        }

        //connects (links) these 2 quadrants with dest & orig quadrant hierarchies... 

        CellQuadrant quadrantGeometry_1 = dest.getLevel(0).getQuadrants()[1];
        CellQuadrant quadrantGeometry_0 = dest.getLevel(0).getQuadrants()[0];
        
        dest.getLevel(0).getQuadrants()[0].refLevels = oldDestLevelsList;
        dest.getLevel(0).getQuadrants()[1].refLevels = orig.getAllLevels(); 
            
        TraverseAllLevels(dest.getLevel(0).getQuadrants()[1].refLevels, quadrantGeometry_1, side, 0);
        TraverseAllLevels(dest.getLevel(0).getQuadrants()[0].refLevels, quadrantGeometry_0, side, 0);
    }

    private void TraverseDebugger(List<CellLevel> level, CellQuadrant cqGeometry, int currLevel)
    {
        Debug.Log("|| traverse debug || lev count " + level.Count + " --------------------------------");
        
        for (int lev = 0; lev < level.Count; lev++) //foreach level
        {
            
            List<CellQuadrant> qList = level[lev].getQuadrants();
            
            Debug.Log("@lev:" + lev + "  quad count:" + qList.Count);
            
            for (int q = 0; q < qList.Count; q++)
            {
                Debug.Log("@quad:" + q + "stx:" + qList[q].StartX + " sty:" + qList[q].StartY + " six:" +
                          qList[q].SizeX +
                          " siy:" + qList[q].SizeY + " sh:" + qList[q].getShape() + " r:" + qList[q].refLevels);
                if (qList[q].refLevels != null)
                {
                    Debug.Log((" going rec!  @quad=" + q + " lev=" + lev));
                    TraverseDebugger(qList[q].refLevels, cqGeometry, lev);
                }
                else 
                {
                    Debug.Log(("its a leaf @quad:" + q + " lev:" + lev));
                }
            }
        }
        Debug.Log("------ traverse end -----");
    }

    public void TraverseAllLevels(List<CellLevel> level, CellQuadrant cqGeometry, CellLevel.Side side, int currLevel)
    {
        //offs maps area coordinates for 3 levels (i.e., levels 0, 1 and 2); so MaxLevels is 3
        int[] offs = new[]
        {
            /* lev 0*/0,
            /* lev 1*/tx.TextureSize / 4, 
            /* lev 2*/(tx.TextureSize / 4 + (tx.TextureSize / 8)),
            /* lev 3*/(tx.TextureSize / 4 + (tx.TextureSize / 8) + (tx.TextureSize / 16))  //not used, by now...
        };

        int startX, startY, sizeX, sizeY;

        startX = cqGeometry.StartX;
        startY = cqGeometry.StartY;
        
        for (int lev = 0; lev < level.Count; lev++) //foreach level
        {
            List<CellQuadrant> qList = level[lev].getQuadrants();
            for (int q = 0; q < qList.Count; q++)
            {
                if (qList[q].refLevels != null)
                {
                    TraverseAllLevels(qList[q].refLevels, cqGeometry, side, lev);
                }
                else //adjusts (translates) quadrants coordinates
                {
                    switch (side)
                    {
                        
                        case CellLevel.Side.Right: case CellLevel.Side.Left:
                            
                             qList[q].StartX = startX + (qList[q].StartX / 2); 
                             qList[q].SizeX /= (2);                                

                             break;
                        case CellLevel.Side.Up: case CellLevel.Side.Down:
                            
                            qList[q].StartY = startY + (qList[q].StartY / 2);
                            qList[q].SizeY /= (2 ); 
                            break;
                        
                        case CellLevel.Side.All:  //occurs only w/ FAGOCITATION !
                            
                            //fagocitation needs quadrant geometry???

                            if (currLevel < 1)
                                currLevel = 1;
                            
                            qList[q].StartX = qList[q].StartX / 2 + offs[currLevel] ;
                            qList[q].SizeX /= (currLevel * 2);
                           
                            qList[q].StartY = qList[q].StartY / 2 + offs[currLevel] ;
                            qList[q].SizeY /= (currLevel * 2);                            
                            break;
                    }
                }
            }
        }       
    }
    
    
    public String showCells()
    {
        
        String deb = "";
        deb += ls.cells.Count;
        deb += " > ";
        for (int i = 0; i < ls.cells.Count; i++)
        {
            deb += i;
            deb += ":";
            deb += ls.cells[i].GetComponent<CellScript>()._cell.Id;
            deb += "  ";
        }

        deb += "|\n\t";
        for (int j = uni.UniverseSizeX - 1; j >= 0; j--)
        {
            for (int i = 0; i < uni.UniverseSizeY; i++)
            {
                if (uni.QuadType[i, j] == Quad.QuadTypes.Empty) deb += "_ ";
                if (uni.QuadType[i, j] == Quad.QuadTypes.Cell) deb += "C ";
                if (uni.QuadType[i, j] == Quad.QuadTypes.Rotor) deb += "R ";
                if (uni.QuadType[i, j] == Quad.QuadTypes.Target) deb += "T ";
                if (uni.QuadType[i, j] == Quad.QuadTypes.Boom) deb += "B ";
                if (uni.QuadType[i, j] == Quad.QuadTypes.None) deb += "n ";
                
                //deb += " ";
            }

            deb += "\n\t";
        }
        //Debug.Log(deb);
        return deb;
    }


    /***************************************************************************************************
     *
     * 
     *  FSM move
     *
     * 
     ***************************************************************************************************/
    
    
    
    private void CreateFSM_move()
    {
        moveFSM.AddState("Idle", new FSM.Trans[]
        {
            new FSM.Trans("SelStart", "SCcube"),
            new FSM.Trans("DressTexture", "2Dress"),
            new FSM.Trans("UndressTexture", "2Undress"),
        }, State_Idle);
        moveFSM.AddState("SelStart", new FSM.Trans[]
        {
            new FSM.Trans("Moving", "SCcube"),
            new FSM.Trans("Moving", "SCquad")
        }, State_SelStart);
        moveFSM.AddState("Moving", new FSM.Trans[]
        {
            new FSM.Trans("MoveEnded", "2MoveEnded"),
            new FSM.Trans("Collision", "2Collision"),
            new FSM.Trans("SelWhileMoving", "SCcube"),
            new FSM.Trans("SelWhileMoving", "SCquad"),
            
        }, State_Moving);
        moveFSM.AddState("SelWhileMoving", new FSM.Trans[] {new FSM.Trans("Moving", "", true)}, State_SelWhileMoving);
        moveFSM.AddState("Collision", new FSM.Trans[] {new FSM.Trans("Idle", "", true)}, State_Collision);
        moveFSM.AddState("MoveEnded", new FSM.Trans[] {new FSM.Trans("Idle", "", true)}, State_MoveEnded);
        moveFSM.AddState("DressTexture", new FSM.Trans[] {new FSM.Trans("Idle", "", true)}, State_Dress);
        moveFSM.AddState("UndressTexture", new FSM.Trans[] {new FSM.Trans("Idle", "", true)}, State_Undress);
        
        moveFSM.SetStart("Idle");
        moveFSM.debugMode = true;
        Debug.Log("fsm criada");
    }
    public void State_Idle(params  object[] parmList)
    {
        fagocitation = false;
        cellMoving = false;
                
        mouse.SetIdle();


        if (originCell >= 0)
        {
            Debug.Log("Idle but originCell:" + originCell);

            /*
             * originCell may be already deleted, if MergeCells occured...
             */

            //qm.UnselectQuad(originCell);
        }

        originCell = -1;
    }

    public void State_SelStart(params  object[] parmList)
    {
        Debug.Log("entrou selstart");
        switch (currInputMode)
        { case PlatformManager.InputMode.Mouse:

                originCell = ls.GetCellIndex(mouse.mci.CurrCellId);
                if (originCell >= 0)
                    qm.SelectQuad(originCell);

                mouse.SetIdle();
                
                break;
            case PlatformManager.InputMode.Keyboard:

                //Debug.Log("- CS - by keyboard selected");

                int x = qm.selectedQuadTarget.GetComponent<Quad>().x;
                int y = qm.selectedQuadTarget.GetComponent<Quad>().y;

                if (uni.QuadType[x, y] == Quad.QuadTypes.Cell)
                {
                    originCell = ls.GetCellIndex(uni.Quads[x, y].GetComponent<Quad>().Cell.Id);
                    qm.SelectQuad(originCell);
                }
                else
                {
                    //Debug.Log(">>> SELECTING EMPTY QUAD WITH KEYBOARD...");
                }

                break;
        }
    }

    public void State_Moving(params  object[] parmList)
    {
        switch (currInputMode)
        {
            case PlatformManager.InputMode.Mouse:
                //Debug.Log("- CS - on move (by mouse)");
                if (originCell < 0)
                {
                    //Debug.Log("- CS - Error: cell on move but no origin selected...");
                }
                else
                {
                    cellMoving = true;
                    Debug.Log("========== - CS - origin:" + originCell + " dst >  x:" + mouse.mci.CurrCollisionX +
                              " y:" + mouse.mci.CurrCollisionY);

                    int x = cells[originCell].GetComponent<CellScript>().X;
                    int y = cells[originCell].GetComponent<CellScript>().Y;

                    int destX = mouse.mci.CurrCollisionX;
                    int destY = mouse.mci.CurrCollisionY;
                    if ((x != destX || y != destY) && uni.QuadType[destX,destY] != Quad.QuadTypes.Target)
                    {
                        qm.UnselectQuad(originCell);
                        MoveCell(originCell, mouse.mci.CurrCollisionX, mouse.mci.CurrCollisionY);
                    }
                    else
                    {
                        cellMoving = false;
                    }
                }

                break;
            case PlatformManager.InputMode.Keyboard:
                //Debug.Log("- CS - on move (by keyboard)"); 
                cellMoving = true;

                int xOrig = cells[originCell].GetComponent<CellScript>().X;
                int yOrig = cells[originCell].GetComponent<CellScript>().Y;
                int xDst = qm.selectedQuadTarget.GetComponent<Quad>().x;
                int yDst = qm.selectedQuadTarget.GetComponent<Quad>().y;

                if ((xOrig != xDst || yOrig != yDst) && uni.QuadType[xDst,yDst] != Quad.QuadTypes.Target)
                {
                    qm.UnselectQuad(originCell);
                    MoveCell(originCell, xDst, yDst);
                }
                else
                {
                    cellMoving = false;
                }

                break;
        }        
    }

    public void State_SelWhileMoving(params  object[] parmList)
    {
        
    }

    public void State_MoveEnded(params  object[] parmList)
    {

        int cellId = Convert.ToInt32(parmList[0]);
        int x= Convert.ToInt32(parmList[1]);
        int y= Convert.ToInt32(parmList[2]);
        
        if (currInputMode == PlatformManager.InputMode.Mouse)
        {
            if (mouse.CurrState != Mouse.MouseState.HoldLeft && mouse.CurrState != Mouse.MouseState.HoldRight)
                sm._audios2.Stop();
        }
        else
        {
            //_audios2.Stop();      //todo if keyboard
        }

        int index = ls.GetCellIndex(cellId);
        
        if (uni.QuadType[x, y] == Quad.QuadTypes.Boom)
        {
            //int index = ls.GetCellIndex(cellId);
            cells[index].SetActive(false);

            sm._audios2.PlayOneShot(sm.explodeSound, 1.0f);
            explodeCell(index, x, y);

            Destroy(cells[index].GetComponent<CellScript>());
            Destroy(cells[index]);
            cells.RemoveAt(index);
            
            /*
             * its not necessary to clean cell x y because it is not in any moment marked as used in universe
             *
             * uni.SetQuadType(x, y, Quad.QuadTypes.Empty);
             * uni.Quads[x, y].GetComponent<Quad>().Cell = null;
             * 
             */

            Debug.Log("EXPLODIU! \n" + showCells());
        }
        else
        {
            //old version:  uni.QuadType[x, y] = Quad.QuadTypes.Cell;
            uni.SetQuadType(x, y, Quad.QuadTypes.Cell);
            uni.Quads[x, y].GetComponent<Quad>().Cell = cells[ls.GetCellIndex(cellId)].GetComponent<CellScript>()._cell;
        }
        
        //not necessary: falls automatically to Idle !!! 
        //moveFSM.Trigger("Idle");
        
        Debug.Log("moveended: "+showCells());        
    }

    public void State_Collision(params  object[] parmList)
    {
        int origId = Convert.ToInt32(parmList[0]);
        int destId = Convert.ToInt32(parmList[1]);
        
        
        Debug.Log("merging... origin=" + originCell + "  origId=" + origId);

        //Debug.Log(" MERGE IN moveMode:" + _moveMode + " moveMode:" + _cellMode);
        
        sm._audios2.Stop();
        int indexOrig = ls.GetCellIndex(origId);
        int indexDest = ls.GetCellIndex(destId);

        int originX = ls.cells[indexOrig].GetComponent<CellScript>().X;
        int originY = ls.cells[indexOrig].GetComponent<CellScript>().Y;
        int destX = ls.cells[indexDest].GetComponent<CellScript>().X;
        int destY = ls.cells[indexDest].GetComponent<CellScript>().Y;
        
        Cell cOrig = ls.cells[indexOrig].GetComponent<CellScript>()._cell;
        Cell cDest = ls.cells[indexDest].GetComponent<CellScript>()._cell;

        CellLevel.Side ploinkSide = CellLevel.Side.None;
        
        if (fagocitation)
        {
             
            cDest.AddLevel(new CellLevel(tx.TextureSize));

            Fagocitates(cDest, cOrig);
            
            sm._audios.PlayOneShot(sm.fagocitaSound,1.0F);
        }
        else   // its a PLOINK
        {
            sm._audios.PlayOneShot(sm.mergeSound,1.0F);
              
              if (destY == originY && originX > destX) // approaching by Right
              {
                  ploinkSide = CellLevel.Side.Right;  
                  CopyOriginToDestiny(cOrig, cDest, CellLevel.Side.Left);
              }
              else if (destY == originY && originX < destX) // approaching by Left
              {
                  ploinkSide = CellLevel.Side.Left;
                  CopyOriginToDestiny(cOrig, cDest, CellLevel.Side.Right);
              }
              else if (destY > originY) // approaching by Downside 
              {
                  ploinkSide = CellLevel.Side.Up;
                  CopyOriginToDestiny(cOrig, cDest, CellLevel.Side.Down);
              }
              else //  approaching by Upside
              {
                  ploinkSide = CellLevel.Side.Down;
                  CopyOriginToDestiny(cOrig, cDest, CellLevel.Side.Up);
              }

              //TraverseDebugger(cDest.getAllLevels(), null, 0);
        }
        
        /*
         *  apply texture to destiny cell 
         */
        ls.cells[indexDest].GetComponent<Renderer>().material.mainTexture = tx.CreateCellTexture(cDest); 

        
        /*
         * updates DNA at destination cell
         */

        CellInfo.DirectionMode dir;
        
        if (fagocitation)
        {
            dir = CellInfo.DirectionMode.All; 
        }
        else
        {
            dir = (CellInfo.DirectionMode) ploinkSide;
        }

        CellDNA origDNA = ls.cells[indexOrig].GetComponent<CellScript>()._cell.dna;
        
        Debug.Log("@MERGE: dir is " + dir);
        Debug.Log("orig dna:" + origDNA.show());

        //Debug.Log("dest dna PRE:" + ls.cells[indexDest].GetComponent<CellScript>()._cell.dna.show());
        
        ls.cells[indexDest].GetComponent<CellScript>()._cell.dna.insert(dir,origDNA.getTypes(),origDNA.getOps());
        
        //Debug.Log("dest dna POS:" + ls.cells[indexDest].GetComponent<CellScript>()._cell.dna.show());
        
        /*
         *  DELETES origin cell
         */
        
        ls.cells[indexOrig].SetActive(false);


        qm.UnselectQuad(indexOrig);    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
        
        
        Destroy(ls.cells[indexOrig].GetComponent<CellScript>());
        Destroy(ls.cells[indexOrig]);

        /*
         *  does the PLOINK  -  MERGE ANIMATION
         */
        
        if (fagocitation)
        {
            ls.cells[indexDest].GetComponent<CellScript>().Ploink(CellLevel.Side.All);
            fagocitation = false;
        }
        else
        {
            ls.cells[indexDest].GetComponent<CellScript>().Ploink(ploinkSide); 
        }
        Debug.Log("dest dna callback(1):" + ls.cells[indexDest].GetComponent<CellScript>()._cell.dna.show());      
        
        

        /*
         * changing cellMode to MoveEnded here prevents the call of MoveEnded method,
         * because ChangeCellMode(CellMode.MoveEnded) will do cellMoving = false;
         */
        
        //ChangeCellMode(MoveManager.CellMode.MoveEnded); 
        
        
        /*
         * its a MATCH ?
         *
         * verifies if resulting merge is equal/equivalent to some target cell...
         * 
         */
        
       callbackCompareWithTargets(ls.cells[indexDest].GetComponent<CellScript>()._cell, indexDest);
        
        /*
         *   Removes (ONLY AT THIS POINT!!!) origin cell from cells array  => to prevent affecting indexDest
         *   (deleting indexOrigin modifies cells sequence in cells array, and indexDest may be pointing
         *   to wrong cell...
         */
        
        ls.cells.RemoveAt(indexOrig);
        
        //Debug.Log(" MERGE OUT moveMode:" + _moveMode + " moveMode:" + _cellMode);
        
        //print(showCells());       
        
        
    }
    public void State_Dress(params  object[] parmList)
    {
        
    }
    public void State_Undress(params  object[] parmList)
    {
        
    }
}