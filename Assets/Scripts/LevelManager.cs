
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using DefaultNamespace;
using UnityEngine;
using Random = System.Random;

public class LevelManager : ScriptableObject
{
    class LevelDescriptor
    {
        private int level;
        
        private int uniSizeX;
        private int uniSizeY;

        private int qtyTargets;
        private int targetsCellsSize;

        private int soundBase;

        
        public LevelDescriptor(int level, int uniSizeX, int uniSizeY, int qtyTargets, int targetsCellsSize, int soundBase){
            this.level = level;
            this.uniSizeX = uniSizeX;
            this.uniSizeY = uniSizeY;
            this.qtyTargets = qtyTargets;
            this.targetsCellsSize = targetsCellsSize;
            this.soundBase = soundBase;
        }
        
        public int SoundBase
        {
            get => soundBase;
            set => soundBase = value;
        }
        
        public int CurrLevel
        {
            get => level;
            set => level = value;
        }

        public int UniSizeX
        {
            get => uniSizeX;
            set => uniSizeX = value;
        }

        public int UniSizeY
        {
            get => uniSizeY;
            set => uniSizeY = value;
        }

        public int QtyTargets
        {
            get => qtyTargets;
            set => qtyTargets = value;
        }
        
        public int TargetsCellsSize
        {
            get => targetsCellsSize;
            set => targetsCellsSize = value;
        }
    }

    private List<LevelDescriptor> levs = new List<LevelDescriptor>();

    private GameManager.Scheduler scheduler;
    
    //MANAGERS
    private MoveManager mm;
    private QuadManager qm;
    private CameraManager cm;
    private SoundManager sm;
    private Universe uni;
    private Texturer tx;    
    
    //LISTS
    private Lists ls;
    private List<GameObject> cells;
    private List<Creature> creatures;
    
    //COLORS
    private Palettes _palettes;
    private Color32[] colors;   
    
    //PATTERNS & CREATURES
    private CreaturePatterns cPatterns;

    private Creature creature;
    
    //PARTICLE SYSTEMS
    private ParticleSystem targetBorningPS,targetDestroyPS,playerDestroyPS;
    private ParticleSystem[] ps;

    private int currLevel;
    private int totalLevels;
    
    private bool noPlayersCells;
    private bool noTargets;    
    
    private Random rnd;

    private const int MaxLevels = 3; //max number of fagocitation levels

    /*
     * LEVEL MANAGER INITIALIZATION
     */
    public void Init(GameManager.Scheduler scheduler, MoveManager mm, QuadManager qm, CameraManager cm, SoundManager sm, PlatformManager pm, Lists ls,
        Universe uni, Texturer tx, List<Creature> creatures, ParticleSystem[] ps)
    {
        this.scheduler = scheduler;
        this.mm = mm;
        this.qm = qm;
        this.cm = cm;
        this.sm = sm;
        this.ls = ls;
        mm.Lm = this;
        
        this.cells = ls.cells;
        this.uni = uni;
        this.tx = tx;
        this.creatures = creatures;

        this.ps = ps;

        playerDestroyPS = ps[0];
        targetDestroyPS = ps[0];
        targetBorningPS = ps[1];
        
        
        mm.callbackCompareWithTargets = CompareWithTargets;
        
        rnd = new Random();
        
        /*
         * initialize color control
         */
        _palettes = new Palettes();
        colors = _palettes.getPalette(3);
        
        /*
         *  generate creatures patterns
         */
        cPatterns = new CreaturePatterns();

        /*
         * control of which level player is
         */
        currLevel = 0;
        totalLevels = levs.Count;
        
        PopulateLevs();       
        
        /*
         * instantiating particle systems
         */
        targetDestroyPS = Instantiate(ps[0]);
        playerDestroyPS = Instantiate(ps[0]);
        targetBorningPS = Instantiate(ps[1]);
    }

    /*
     * 
     * Generates NEW LEVEL
     * 
     */
    public void generateLevel()
    {
        
        Debug.Log("GENERATE CELLS currLevel:"+currLevel);
        
        noPlayersCells = false;
        noTargets = false;
        
        mm.Reset();
        
        
        uni.CreateUniverse(levs[currLevel].UniSizeX, levs[currLevel].UniSizeY, currLevel,false);

        /*
         * updates level audio 
         */
        
        sm.SetBase(levs[currLevel].SoundBase);
        
        /*
         * updates active quads
         */
        
        for (int i = 0; i < ls.actives.Count; i++)
        {
            //Destroy(ls.actives[i].GetComponent<Quad>());
            Destroy(ls.actives[i]);           
        }

        
        ls.actives.Add(qm.CreateActiveQuad(Quad.QuadTypes.Rotor));
        ls.actives.Add(qm.CreateActiveQuad(Quad.QuadTypes.Boom));
        //ls.actives.Add(qm.CreateActiveSprite("test"));
        qm.CreateActiveSprite("garden");
        
        /*
         * update cameras setup for new level
         */

        cm.Positionate(levs[currLevel].UniSizeX, CameraManager.Angle.East, true);

        scheduler.Schedule(new GameManager.Scheduling("generateTargets", 2));
        
        //scheduler.Schedule(new GameManager.Scheduling("generateLevel", 5));

        currLevel++;
        
        
        
            //generate target(s) (creatures) 

            //creature = new Creature(uni);

    }
    
    /*
     * generates creatures & free (player) cells for new level
     */
    public void generateTargets()
    {
        for (int i = 0; i < levs[currLevel].QtyTargets; i++)
        {
            CreateRandomNewTarget(levs[currLevel].QtyTargets, levs[currLevel].TargetsCellsSize);
        }
    }

    public void CreateRandomNewTarget(int cellsQuantity, int maxTargetSize)
    {
        ls.targets.Add(new List<GameObject>()); //adds new target    

        List<CellInfo>[] infoCellsArray = new List<CellInfo>[cellsQuantity];  //max size = 8;

        int[] targetSize = new int[cellsQuantity];

        /*
         *  ALLOCATION PRE-ANALYSIS
         *
         *  are there enough free cells to accomodate new target (cells and free cells)?
         */

        bool okToProceed = false;
        while (!okToProceed)
        {
            int totalTargetSize = 0;
            for (int c = 0; c < cellsQuantity; c++)
            {
                targetSize[c] = rnd.Next(2, maxTargetSize);
                totalTargetSize += targetSize[c];
            }

            if (totalTargetSize < uni.CountFreeQuads())
            {
                okToProceed = true;
            }
            else
            {
                Debug.Log("Creating RandomTarget (too big) -> free cells:" + uni.CountFreeQuads());
                Debug.Log(("totalTargetSiz:" + totalTargetSize));
            }
        }


        for (int c = 0; c < cellsQuantity; c++)
        {
            List<CellInfo> q = new List<CellInfo>();

            //randomic
            int qq = targetSize[c];
            int fagoCntr = 0;

            for (int i = 0; i < qq; i++) //for each QUADRANT
            {
                q.Add(new CellInfo());
                CellInfo ci = q[q.Count - 1];

                //background and foreground colors must be different
                bool sameColor = true;
                ci.CBack = rnd.Next(0, colors.Length - 1);
                while (sameColor)
                {
                    ci.CFore = rnd.Next(0, colors.Length - 1);
                    if (ci.CBack != ci.CFore)
                    {
                        sameColor = false;
                    }
                }

                ci.Shape = rnd.Next(1, 4); //0:None 1:ellipse 2:triangle 3:rectangle Note:next upperbound is exclusive
                ci.Tt = 0; //type of triangle

                if (ci.Shape == 2) //if triangle
                {
                    ci.Tt = rnd.Next(1, 4); //which triangle:  1:<  2:>  3:/\  4:\/ 
                }

                //direction will be 1, 2, 3, 4  or 5 (to fagocitation)
                ci.Direction = (CellInfo.DirectionMode) (rnd.Next(1, 100) % 4) + 1; //to increase chances of >1 results;

                /*
                 * Its fagocitation cycle?
                 * 30% fago chance each cycle; also prevents fago from occurring too soon -> i > 1
                 * must prevent also too deep fagocitation sequence (>2)
                 */
                bool isFagocitation = (rnd.Next(0, 10) < 4 && i > 1) ? true : false; // <3
                if (isFagocitation)
                {
                    if (fagoCntr < 2)
                    {
                        ci.Direction = CellInfo.DirectionMode.All;
                        fagoCntr++;
                    }
                    else //too deep fago sequence
                    {
                        fagoCntr = 0;
                    }
                }
                else
                {
                    fagoCntr = 0;
                }

                //incluir wait direction
            }

            infoCellsArray[c] = q;
            ShowCellInfo(q);
        }

        CreateTarget(ls.targets.Count - 1, infoCellsArray);
    }

    private void ShowCellInfo(List<CellInfo> qList)
    {
        String commands = "";
        commands += qList.Count + ":";
        foreach (var i in qList)
        {
            switch (i.Shape)
            {
                case 0:
                    commands += "N ";
                    break;
                case 1:
                    commands += "e ";
                    break;
                case 2:
                    commands += "T ";
                    break;
                case 3:
                    commands += "r ";
                    break;
            }

            switch (i.Tt)
            {
                case 0:
                    commands += "- ";
                    break;
                case 1:
                    commands += "< ";
                    break;
                case 2:
                    commands += "> ";
                    break;
                case 3:
                    commands += "^ ";
                    break;
                case 4:
                    commands += "v ";
                    break;
            }

            commands += i.Direction + " ";
            commands += "|";
        }

        Debug.Log(">>> " + commands);
    }

    private void CreateTarget(int targetIndex, List<CellInfo>[] cellInfoArray)
    {
        List<CellInfo> qList;
        int[,] pat = null;
        int targetCo = -1, targetRo = -1;

        /*
         * choose pattern
         */
        if (cellInfoArray.Length > 1)
        {
            pat = cPatterns.getRandomPattern(cellInfoArray.Length);
            
            /*
             *  or pat = cPatterns.getByIndexPattern(cellInfoArray.Length , 7);
             */
        }

        else
        {
            //offsets for target single cells  (other single cells pass null as 2nd arg to Allocate)
            pat = new int[,] {{0, 0}};
        }

        /*
         *  ALLOCATION ANALYSIS
         *
         *  are there enough free cells to accomodate new target (cells and free cells)?
         * 
         */

        int totalCellsToAllocate = 0;

        for (int i = 0; i < cellInfoArray.Length; i++)
        {
            totalCellsToAllocate++; //counts target cell
            totalCellsToAllocate += cellInfoArray[i].Count; //counts free cells for that target cell
        }

        Debug.Log("free cells:" + uni.CountFreeQuads());
        Debug.Log(("totalCellsToAllocate:" + totalCellsToAllocate));

        if (uni.CountFreeQuads() < totalCellsToAllocate)
        {
            Debug.Log("Cant allocate Target: not enough free cells in universe!!!");
            Debug.Log("free cells:" + uni.CountFreeQuads());
            Debug.Log(("totalCellsToAllocate:" + totalCellsToAllocate));
            return;
        }


        /*
         *
         * ALLOCATING !!!
         * 
         */
        if (!uni.Allocate(Quad.QuadTypes.Target, pat)) //uni.Allocate tries to accomodate target pattern
        {
            // SHOULD TRY A DIFFERENT PATTERN...

            Debug.Log("Cant allocate Target...");
            return;
        }
        else
        {
            /*
             * 
             *  Success!  all target cells allocated!!!
             *
             *  go creating TARGET
             * 
             */

            targetCo = uni.ColumnFree;
            targetRo = uni.RowFree;


            for (int c = 0; c < cellInfoArray.Length; c++)
            {
                ls.targets[targetIndex].Add(Instantiate(uni.cell)); //adds new Cell to this target

                int lastTargetCell = ls.targets[targetIndex].Count - 1;
                
                /*
                 *  velocity its no set for targets
                 */
                
                //ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().velocity = uni.globalVelocity;
 
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()
                    .SetPosition(targetCo + pat[c, 0], targetRo + pat[c, 1], uni.quadSize);

                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell.Id = ls.idTargetCellCntr++; 
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell.IsTargetPart = true;
               
                /*
                 * plant callbacks
                 */
                
                //ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().callbackMergeCells = mm.MergeCells;
                //ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().callbackMoveEnded = mm.MoveEnded;
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().callbackDestroyCell = AnimateCellDestruction;
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().callbackToNewLevel = TransitionToNewLevel;
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().callbackCreateNewTarget = AnimateTargetCreation;

                
                /*
                 * injects universe info in cell
                 */
                
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().universeSize = uni.UniverseSize;

                /*
                 * updates universe info
                 */
                
                uni.Quads[uni.ColumnFree, uni.RowFree].GetComponent<Quad>().Cell =
                    ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell;


                /*
                 * sets cell as target
                 */
                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().isTarget = true;

                ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>().borning = true;
                
                /*
                 * create all quadrants for this target´s cell...
                 */
                
                int cBack = -1, cFore = -1, shape, tt;
                CellInfo.DirectionMode direction;

                qList = cellInfoArray[c];

                //for each QUADRANT
                for (int i = 0; i < qList.Count; i++)
                {
                    cBack = qList[i].CBack;
                    cFore = qList[i].CFore;
                    shape = qList[i].Shape;
                    tt = qList[i].Tt;
                    direction = qList[i].Direction;

                    

                    /*
                     *  SPAWNS Free Cells
                     */

                    uni.Allocate(Quad.QuadTypes.Cell, null); //its a single cell

                    ls.cells.Add(Instantiate(uni.cell));
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().velocity = uni.globalVelocity;
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.Id = ls.idCellCntr++;
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.IsTargetPart = false;
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().borning = true;
                    
                    /*
                     * plant callbacks
                     */

                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().moveFSM = mm.moveFSM;
                    
                    //ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackMergeCells = mm.MergeCells;
                    //ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackMoveEnded = mm.MoveEnded;
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackDestroyCell = AnimateCellDestruction;
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().callbackToNewLevel = TransitionToNewLevel;
                    
                    /*
                     * injects universe info in cell
                     */
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().universeSize = uni.UniverseSize;
                    
                    /*
                     * positionates and creates quadrant
                     */
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()
                        .SetPosition(uni.ColumnFree, uni.RowFree, uni.quadSize);
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.AddLevel(new CellLevel(tx.TextureSize));
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.getLevel(0).AddHorizontalQuadrant(
                        new CellQuadrant(cBack, cFore, (CellQuadrant.Shape) shape, tt), CellLevel.Side.Left);
                    
                    /*
                     * inserts cell info in cell DNA
                     */
                    
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell.dna.insert(
                        CalculateDNACellType(cBack, cFore, shape, tt));
                    
                    /*
                     * updates universe info
                     */
                    
                    uni.Quads[uni.ColumnFree, uni.RowFree].GetComponent<Quad>().Cell =
                        ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell;

                    /*
                     * create texture of free cell
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<Renderer>().material.mainTexture =
                        tx.CreateCellTexture(ls.cells[ls.cells.Count - 1].GetComponent<CellScript>()._cell);

                    /*
                     * sets cell as not target
                     */
                    ls.cells[ls.cells.Count - 1].GetComponent<CellScript>().isTarget = false;
                    
                    
                    
                    
                    
                    /*
                     *  completing target assembly:
                     *  
                     *  ADD quadrant to Target
                     * 
                     */

                    //creates new auxiliary cell to store new quadrant (just created)
                    
                    Cell copyOfNewCell = new Cell();
                    copyOfNewCell.AddLevel(new CellLevel(tx.TextureSize));
                    copyOfNewCell.getLevel(0).AddHorizontalQuadrant(
                        new CellQuadrant(cBack, cFore, (CellQuadrant.Shape) shape, tt), CellLevel.Side.Left);

                    Cell dest = ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell;
                    
                    /*
                     * inserts new cell info in dest cell DNA
                     */

                    if (i > 0)
                    {
                      dest.dna.insert(direction, CalculateDNACellType(cBack, cFore, shape, tt));  
                    }
                    else
                    {
                        dest.dna.insert(CalculateDNACellType(cBack, cFore, shape, tt)); 
                    }
                    
                    
                    if (i == 0)
                    {
                        
                        dest.AddLevel(new CellLevel(tx.TextureSize));
                        dest.getLevel(0).AddHorizontalQuadrant(
                            new CellQuadrant(cBack, cFore, (CellQuadrant.Shape) shape, tt), CellLevel.Side.Left);
                    }
                    else
                    {
                        if (direction == CellInfo.DirectionMode.Wait) //DEFERRED
                        {
                            //wait some cycles before doing fagocitation  -> FREE QUADRANTS WITH + than just 1 LEVEL !!!
                        }

                        if (direction == CellInfo.DirectionMode.All) //FAGOCITATION
                        {
                            dest.AddLevel(new CellLevel(tx.TextureSize));
                            mm.Fagocitates(dest, copyOfNewCell);
                        }
                        else //PLOINK
                        {
                            //creates new root level
                            List<CellLevel> oldDestLevelsList = dest.getAllLevels();
                            dest.SetLevels(new List<CellLevel>());
                            dest.getAllLevels().Add(new CellLevel(tx.TextureSize));

                            CellLevel.Side side = CellLevel.Side.None;
                            if (direction == CellInfo.DirectionMode.Left) side = CellLevel.Side.Left;
                            if (direction == CellInfo.DirectionMode.Right) side = CellLevel.Side.Right;
                            if (direction == CellInfo.DirectionMode.Up) side = CellLevel.Side.Up;
                            if (direction == CellInfo.DirectionMode.Down) side = CellLevel.Side.Down;

                            switch (direction)
                            {
                                case CellInfo.DirectionMode.Left:
                                case CellInfo.DirectionMode.Right:
                                    dest.getLevel(0).AddHorizontalQuadrant(new CellQuadrant(7, 7,
                                        (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
                                    dest.getLevel(0).AddHorizontalQuadrant(new CellQuadrant(7, 7,
                                        (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
                                    break;
                                case CellInfo.DirectionMode.Up:
                                case CellInfo.DirectionMode.Down:
                                    dest.getLevel(0).AddVerticalQuadrant(new CellQuadrant(7, 7,
                                        (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
                                    dest.getLevel(0).AddVerticalQuadrant(new CellQuadrant(7, 7,
                                        (CellQuadrant.Shape) CellQuadrant.Shape.None, 0), side);
                                    break;
                            }
                            
                            /*
                             * inserts quadrant in cell quadrant (tree) structure
                             */
                            
                            CellQuadrant quadrantGeometry_1 = dest.getLevel(0).getQuadrants()[1];
                            CellQuadrant quadrantGeometry_0 = dest.getLevel(0).getQuadrants()[0];

                            dest.getLevel(0).getQuadrants()[0].refLevels = oldDestLevelsList;
                            dest.getLevel(0).getQuadrants()[1].refLevels = copyOfNewCell.getAllLevels();

                            mm.TraverseAllLevels(dest.getLevel(0).getQuadrants()[1].refLevels, quadrantGeometry_1, side,
                                0);
                            mm.TraverseAllLevels(dest.getLevel(0).getQuadrants()[0].refLevels, quadrantGeometry_0, side,
                                0);
                        }
                    } //Fagocitation or Ploink done
                    

                    Debug.Log("target "+dest.Id+ " "+ dest.dna.show());
                    
                } //all quadrants created

                /*
                 *  updates target´s texture
                 */
                ls.targets[targetIndex][lastTargetCell].GetComponent<Renderer>().material.mainTexture =
                    tx.CreateCellTexture(ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell);

                Debug.Log("target final"+ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell.Id+ " "+
                          ls.targets[targetIndex][lastTargetCell].GetComponent<CellScript>()._cell.dna.show());
            }
        }
    }

    public void AnimateTargetCreation(float x, float y)
    {
        //starts particle system to target creation
        //targetBorningPS.transform.position = new Vector3(x,y,0);
        //targetBorningPS.Play();
    }
    
    /*
     * Called back from MoveManager/MergeCells
     */
    public void CompareWithTargets(Cell c, int indexPlayerCell)
    {
        Debug.Log("player:" + c.dna.show());

        for (int t = 0; t < ls.targets.Count; t++)
        {
            for (int k = 0; k < ls.targets[t].Count; k++)
            {
                Debug.Log("compare:" + c.Id + " t:" + t + " k:" + k);
                Debug.Log("target:" + ls.targets[t][k].GetComponent<CellScript>()._cell.dna.show());

                if (CompareToTargetDNA(c, ls.targets[t][k].GetComponent<CellScript>()._cell))
                {
                    Debug.Log("match @ " + t + " " + k);
                    Match(indexPlayerCell, t, k);
                    Debug.Log("******************************  IGUAIS   ******************************");

                    return;
                }
            }
        }

        Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
    }

    public bool CompareToTargetDNA(Cell playerCell, Cell targetCell)
    {
        List<int> pt = playerCell.dna.getTypes();
        List<int> tt = targetCell.dna.getTypes();
        List<int> po = playerCell.dna.getOps();
        List<int> to = targetCell.dna.getOps();

        if (pt.Count != tt.Count || po.Count != to.Count)
            return false;

        for (int t = 0; t < tt.Count; t++)
        {
            if (tt[t] != pt[t]) return false;
        }

        for (int o = 0; o < to.Count; o++)
        {
            if (to[o] != po[o]) return false;
        }

        return true;
    }


    /*
     * starts Matching animation (shrinking and exploding)
     */
    private void Match(int indexPlayerCell, int targetIndex, int targetCellIndex)
    {
        ls.cells[indexPlayerCell].GetComponent<CellScript>()._cell.state = Cell.State.Matching;
        ls.targets[targetIndex][targetCellIndex].GetComponent<CellScript>()._cell.state = Cell.State.Matching;

        /*
         * prevents further movements w/ cells in matching process...
         */
         if (mm.moveFSM.GetCurrentStateName() != "Idle")
         {
             mm.moveFSM.Trigger("Idle");
         }
         
        // if (mm.moveMode == MoveManager.MoveMode.Drag && mm.cellMode == MoveManager.CellMode.MoveEnded)
        // {
        //     mm.moveMode = MoveManager.MoveMode.None;

        
            //mm.ChangeCellMode(MoveManager.CellMode.Idle);
        //}
        
    }

    /*
     * called back from CellScript/Update
     */
    public void AnimateCellDestruction(int id, bool isTarget)
    {
        if (isTarget)
        {
            int[] indexes = ls.GetTargetCellIndexes(id);
            int targetIndex = indexes[0];
            int targetCellIndex = indexes[1];


            //starts particle system 

            targetDestroyPS.transform.position = new Vector3(
                ls.targets[targetIndex][targetCellIndex].GetComponent<CellScript>().X * uni.quadSize,
                ls.targets[targetIndex][targetCellIndex].GetComponent<CellScript>().Y * uni.quadSize,
                -1.2f);

            targetDestroyPS.Play();
        }
        else
        {
            int indexPlayerCell = ls.GetCellIndex(id);

            //starts particle system

            playerDestroyPS.transform.position = new Vector3(
                ls.cells[indexPlayerCell].GetComponent<CellScript>().X * uni.quadSize,
                ls.cells[indexPlayerCell].GetComponent<CellScript>().Y * uni.quadSize, -1.2f);
            playerDestroyPS.Play();
        }
    }

    /*
     * called back from CellScript/DelayToTransition
     */

    public void TransitionToNewLevel(int id, bool isTarget)
    {
        if (isTarget)
        {
            int[] indexes = ls.GetTargetCellIndexes(id);
            int targetIndex = indexes[0];
            int targetCellIndex = indexes[1];

            /*
             *  DESTROYS TARGET CELL
             */

            ls.targets[targetIndex][targetCellIndex].SetActive(false);
            
            /*
             * updates universe info
             */
            int x = ls.targets[targetIndex][targetCellIndex].GetComponent<CellScript>().X;
            int y = ls.targets[targetIndex][targetCellIndex].GetComponent<CellScript>().Y;
            
            uni.SetQuadType(x, y, Quad.QuadTypes.Empty);   //quadrant is emptied!
            uni.Quads[x, y].GetComponent<Quad>().Cell = null;
            
            //do some harm
            
            Destroy(ls.targets[targetIndex][targetCellIndex].GetComponent<CellScript>());
            Destroy(ls.targets[targetIndex][targetCellIndex]);

            ls.targets[targetIndex].RemoveAt(targetCellIndex);

            if (ls.targets[targetIndex].Count == 0)
            {
                ls.targets.RemoveAt(targetIndex);

                if (ls.targets.Count == 0)
                {
                    noTargets = true;
                }
            }
        }
        else
        {
            int indexPlayerCell = ls.GetCellIndex(id);
            
            /*
             *  DESTROYS PLAYER CELL
             */

            ls.cells[indexPlayerCell].SetActive(false);
            
            /*
             * updates universe info
             */
            int x = ls.cells[indexPlayerCell].GetComponent<CellScript>().X;
            int y = ls.cells[indexPlayerCell].GetComponent<CellScript>().Y;
            
            uni.SetQuadType(x, y, Quad.QuadTypes.Empty);   //quadrant is emptied!
            uni.Quads[x, y].GetComponent<Quad>().Cell = null;
            
            //do some harm
            
            Destroy(ls.cells[indexPlayerCell].GetComponent<CellScript>());
            Destroy(ls.cells[indexPlayerCell]);

            ls.cells.RemoveAt(indexPlayerCell);
            
            if (ls.cells.Count == 0)
            {
                noPlayersCells = true;
            }
        }

        if (noTargets && noPlayersCells)
        {
            if (currLevel == totalLevels)
            {
                Application.Quit();
            }
            else
            {
                /*
                 *  generate new level must be called only ONCE!
                 * when last target disappears...
                 */
                generateLevel();
                //TransitionToNewLevel();
            }
        }
    }


    public int CalculateDNACellType(int cBack, int cFore, int shape, int tt)
    {
        //Shape       0:None, 1:ellipse  2:triangle  3:rectangle
        //Type of Triangle ->  1:<  2:>  3:/\  4:\/ 
        //Direction   1:left   2:right   3:up   4:down

        int sh = shape;
        if (shape == 2)
        {
            switch (tt)
            {
                case 1:
                    sh = 4; //   <
                    break;
                case 2:
                    sh = 5; //   >
                    break;
                case 3:
                    sh = 6; //   /\
                    break;
                case 4:
                    sh = 7; //   \/
                    break;
            }
        }

        int cellType = cBack + cFore * 100 + sh * 10000;

        return cellType;
    }

    private void PopulateLevs()
    {
        levs.Add(new LevelDescriptor(1, 5, 5, 1, 2, 114)); //targetCellsSize min == 2
        levs.Add(new LevelDescriptor(2, 7, 7, 2, 3, 114));
        levs.Add(new LevelDescriptor(3, 9, 9, 2, 4, 114));
        levs.Add(new LevelDescriptor(4, 11, 11, 2, 5, 114));

    }

    /*
     public bool CompareToTarget(Cell playerCell, Cell targetCell )
        {
            //bool equal = true;
            
            for (int i = 1; i < 32; i ++)
            for (int j = 1; j < 32; j ++)
            {
                if (playerCell.GetPixel(i, j) != targetCell.GetPixel(i, j))
                {
                    return false;
                }
            }

            return true;
        }
    */
}
