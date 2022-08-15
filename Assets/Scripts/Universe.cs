
using System;
using System.Collections.Generic;
using UnityEngine;

public class Universe: ScriptableObject
{
    
    private GameObject boardQuadrant;
    private GameObject floorQuadrant;
    private GameObject wall;

    public GameObject cell;

    public GameObject Quadrant
    {
        get => boardQuadrant;
        set => boardQuadrant = value;
    }

    private int universeSizeX, 
                universeSizeY, 
                universeSize,    //total quadrants in universe
                universeFree;    //free quadrants in universe

    private int columnFree, rowFree;

    public int ColumnFree
    {
        get => columnFree;
        set => columnFree = value;
    }

    public int RowFree
    {
        get => rowFree;
        set => rowFree = value;
    }

    public int UniverseSizeX
    {
        get => universeSizeX;
        set => universeSizeX = value;
    }
    public int UniverseSizeY
    {
        get => universeSizeY;
        set => universeSizeY = value;
    }
    public int UniverseSize
    {
        get => universeSize;
        set => universeSize = value;
    }

    public float centerX, centerY;

    public  QuadManager qm;
    private Texturer tx;
    
    public float quadSize = 1.0f;  //1.1f
    
    
    /*
     * QUADRANT TYPE Control
     */
    
    private Quad.QuadTypes [,] quadType;
    public Quad.QuadTypes [,] QuadType
    {
        get => quadType;
        //set => quadType = value;
    }

    public void SetQuadType(int x, int y, Quad.QuadTypes qt)
    {
        quadType[x,y] = qt;
        
        //setting quad as free
        if ((qt == Quad.QuadTypes.Empty ||
             qt == Quad.QuadTypes.None))
        {
            if (!InFreeQuads(x, y))
            {
                freeQuads.Add(new []{x,y});
                universeFree++;
            }
        }
        else  //setting quad as Cell, Target, Rotor, Boom, Portal, QuadTarget
        {
            if (InFreeQuads(x, y))  //necessary because its possible to change quadType from a non empty quad...
            {
                RemoveQuadFromFreeQuadsList(x, y);
            }
        }
        
    }
    /*
     * FREE QUADRANTS LIST
     * each list item is a pair column,row corresponding to a free quad
     */
    
    public List<int[]> freeQuads = new List<int[]>();
    
    /*
     * UNIVERSE QUADS
     */
    
    private GameObject[,] quads;

    public GameObject[,] Quads
    {
        get => quads;
        set => quads = value;
    }

    /*
     *  Scenery objects
     */
    
    private GameObject [] walls = new GameObject[4];
    private GameObject [] internalWalls = new GameObject[4];
    private GameObject [] baseBoards = new GameObject[4];          //rodapés...
    private GameObject [] verticalBaseBoards = new GameObject[4];
    
    private GameObject floor = null;

    private Texture2D[] boardTxW = new Texture2D[8];  //White
    private Texture2D[] boardTxB = new Texture2D[8];  //Black
    
    private GameObject povs;
    
    /*
     * 
     */
    
    public bool selectedTarget;
    public int selectedTargetX;
    public int selectedTargetY;

    /*
     * 
     */
    private int currLevel;
    private System.Random rnd;
    
    public float globalVelocity;

    private Material transparent;
    
    /*
     *
     *  Initialization
     * 
     */
    public void Init(GameObject boardQuadrant, GameObject floorQuadrant, GameObject cell, GameObject wall, Texturer tx, GameObject povs, Material transparent)
    {
        this.tx = tx;
        this.cell = cell;  
        this.boardQuadrant = boardQuadrant;
        this.floorQuadrant = floorQuadrant;
        this.wall = wall;
        this.povs = povs;

        this.transparent = transparent;
        
        globalVelocity = Time.deltaTime * 5;

        //Load checkerboard textures
        boardTxW = Resources.LoadAll<Texture2D>("boardw");
        boardTxB = Resources.LoadAll<Texture2D>("boardb");

    }
    
    /*
     *
     *    Generates new Universe
     *
     *    sizeX and sizeY need to be odd for proper board checkering effect
     * 
     */
    
    public void CreateUniverse(int sizeX, int sizeY, int currLevel, bool IsCA)
    {

        this.currLevel = currLevel;
        
        selectedTarget = false;
        columnFree = -1;
        rowFree = -1;
        

        
        Debug.Log("gamemanager global deltaTime:" + globalVelocity);
        
        rnd = new System.Random();

        /*
         * destroys old board
         *
         * must be called before updating universe geometry
         */
        
        if (currLevel > 0)
        {
            DestroyQUs();
        } 
        
        /*
         * updates universe geometry
         */
        
        this.UniverseSizeX = sizeX;
        this.UniverseSizeY = sizeY;
        this.UniverseSize = sizeX * sizeY; //TOTAL QUANTITY OF QUADRANTS in universe
        this.universeFree = this.UniverseSize;

        DrawsScenery();

        /*
         *  BOARD creation
         */

        quads = new GameObject[sizeX, sizeY];

        quadType = new Quad.QuadTypes[sizeX, sizeY];

        for (int i = 0; i < universeSizeX; i++)
        for (int j = 0; j < universeSizeY; j++)
            quadType[i, j] = Quad.QuadTypes.Empty;

        freeQuads.Clear();
        
        int txIndex, txIndexW, txIndexB;
        txIndex = txIndexW = txIndexB = 0;
        
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
                if (!(boardQuadrant is null))
                {
                    freeQuads.Add(new[] {i, j});

                    //qu[i,j] = Instantiate(quadrant);
                    //quads[i, j] = qu[i,j];
                    quads[i, j] = Instantiate(boardQuadrant);
                    quads[i,j].GetComponent<Quad>().x = i;
                    quads[i,j].GetComponent<Quad>().y = j;
                    quads[i,j].GetComponent<Quad>().thisQuadType = Quad.QuadTypes.None;
                    quads[i, j].GetComponent<Quad>().quadVelocity = globalVelocity / 8f; // /10
                    quads[i, j].GetComponent<Quad>().qm = qm;
                    
                    quads[i,j].transform.position = new Vector3(i * quadSize,j * quadSize, 0);

                    //generating checkered board
                    
                    if (txIndex % 2 == 0)
                    {
                        
                        quads[i, j].GetComponent<Quad>().texture = boardTxW[(txIndexW < 8)?txIndexW++:txIndexW = 0];
                    }
                    else
                    {
                        quads[i, j].GetComponent<Quad>().texture = boardTxB[(txIndexB < 8)?txIndexB++:txIndexB = 0];
                    }

                    quads[i, j].GetComponent<Renderer>().material.mainTexture = quads[i, j].GetComponent<Quad>().texture;
                    txIndex++;
                }
        }
    }

    private void DrawsScenery()
    {
        float edgeSize = 3.0f;

        centerX = (universeSizeX / 2.0f) - 0.5f;
        centerY = (universeSizeY / 2.0f) - 0.5f;

        float width = UniverseSizeX + 2 * edgeSize;
        float height = UniverseSizeY + 2 * edgeSize;

        if (currLevel == 0)
        {
            floor = Instantiate(floorQuadrant);
            floor.GetComponent<Renderer>().material.mainTexture = tx.GetTexture("floor1");
            for (int i = 0; i < 4; i++)
            {
                walls[i] = Instantiate(wall);
                walls[i].GetComponent<Renderer>().material.mainTexture = tx.GetTexture("wood2");
                
                internalWalls[i] = Instantiate(wall);
                internalWalls[i].GetComponent<Renderer>().material.mainTexture = tx.GetTexture("wood2");

                baseBoards[i] = Instantiate(wall);
                baseBoards[i].GetComponent<Renderer>().material.mainTexture = tx.GetTexture("wood2");
                
                verticalBaseBoards[i] = Instantiate(wall);
                verticalBaseBoards[i].GetComponent<Renderer>().material.mainTexture = tx.GetTexture("wood2");
            }
        }
        
        //Floor
        floor.transform.position = new Vector3( centerX, centerY, 0.01f);
        floor.transform.localScale = new Vector3( width, height, 1.0f );

        //West face
        walls[0].transform.position = new Vector3( centerX - width / 2 - 0.5f , centerY, -2.0f);
        walls[0].transform.localScale = new Vector3( 0.5f, height+1, 4.0f );
        
        internalWalls[0].transform.position = new Vector3( centerX - width / 2 , centerY, -2.0f);
        internalWalls[0].transform.localScale = new Vector3( 0.5f, height+1, 3.2f );       

        baseBoards[0].transform.position = new Vector3( centerX - width / 2 + 0.3f , centerY, 0.0f);
        baseBoards[0].transform.localScale = new Vector3( 0.5f, height+1, 0.6f );

        //East face
        walls[1].transform.position = new Vector3( centerX + width / 2 + 0.5f, centerY, -2.0f); //9.3
        walls[1].transform.localScale = new Vector3( 0.5f, height+1, 4.0f );

        internalWalls[1].transform.position = new Vector3( centerX + width / 2 , centerY, -2.0f);
        internalWalls[1].transform.localScale = new Vector3( 0.5f, height+1, 3.2f );       

        baseBoards[1].transform.position = new Vector3( centerX + width / 2 - 0.3f , centerY, 0.0f);
        baseBoards[1].transform.localScale = new Vector3( 0.5f, height+1, 0.6f );

        //South face
        walls[2].transform.position = new Vector3( centerX, centerY - height / 2 - 0.25f, -2.0f);
        walls[2].transform.localScale = new Vector3( width+1, 0.5f, 4.0f );

        internalWalls[2].transform.position = new Vector3( centerX , centerY - height / 2 + 0.25f, -2.0f);
        internalWalls[2].transform.localScale = new Vector3( width+1, 0.5f, 3.2f );       

        baseBoards[2].transform.position = new Vector3( centerX  , centerY - height / 2 + 0.55f, 0.0f);
        baseBoards[2].transform.localScale = new Vector3( width+1, 0.5f, 0.5f ); 
        
        //North face
        walls[3].transform.position = new Vector3( centerX, centerY + height / 2 + 0.25f, -2.0f); //9.3
        walls[3].transform.localScale = new Vector3( width+1, 0.5f, 4.0f );
        
        internalWalls[3].transform.position = new Vector3( centerX , centerY + height / 2 - 0.25f , -2.0f);
        internalWalls[3].transform.localScale = new Vector3( width+1, 0.5f, 3.2f );       

        baseBoards[3].transform.position = new Vector3( centerX , centerY + height / 2 - 0.55f, 0.0f);
        baseBoards[3].transform.localScale = new Vector3( width+1, 0.5f, 0.5f ); 

        //vertical baseboards
        verticalBaseBoards[0].transform.position = new Vector3( centerX - width / 2 + 0.3f , centerY + height / 2 - 0.55f, -2.0f);
        verticalBaseBoards[0].transform.localScale = new Vector3( 0.6f, 0.5f, 2.6f );
        
        verticalBaseBoards[1].transform.position = new Vector3( centerX + width / 2 - 0.3f , centerY + height / 2 - 0.55f, -2.0f);
        verticalBaseBoards[1].transform.localScale = new Vector3( 0.6f, 0.5f, 2.6f );  
        
        verticalBaseBoards[2].transform.position = new Vector3( centerX + width / 2 - 0.3f , centerY - height / 2 + 0.55f, -2.0f);
        verticalBaseBoards[2].transform.localScale = new Vector3( 0.6f, 0.5f, 2.6f );  

        verticalBaseBoards[3].transform.position = new Vector3( centerX - width / 2 + 0.3f , centerY - height / 2 + 0.55f, -2.0f);
        verticalBaseBoards[3].transform.localScale = new Vector3( 0.6f, 0.5f, 2.6f );
        
        /*
         * updates POVS click zone
         */
        float povCenter = -1;
        float povFactor = -1;
        
        switch (UniverseSizeX)
        {
            case 5:
                povCenter = 2f;
                povFactor = 2.33f;
                break;
            case 7:
                povCenter = 3f;
                povFactor = 2.76f;
                break;
            case 9:
                povCenter = 4f;
                povFactor = 3.26f;
                break;
            case 11:
                povCenter = 5f;
                povFactor = 4.0f;
                break;
            case 13:
                povCenter = 6f;
                povFactor = 4.76f;    //checar fatores a partir daqui
                break;
            case 15:
                povCenter = 7f;
                povFactor = 5.66f;
                break;
            case 17:
                povCenter = 8f;
                povFactor = 6.8f;
                break;
            case 19:
                povCenter = 9f;
                povFactor = 8f;
                break;
        }

        //povs.transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = transparent.mainTexture; <-dont work
        povs.transform.GetChild(0).GetComponent<MeshRenderer>().material = transparent;
        povs.transform.GetChild(1).GetComponent<MeshRenderer>().material = transparent;
        povs.transform.GetChild(2).GetComponent<MeshRenderer>().material = transparent;
        
        povs.transform.position = new Vector3( povCenter, -1.6f, -0.06f);
        povs.transform.localScale = new Vector3( povFactor, 1.2f, 1.0f );   
        
    }
    
    
    /*
     *
     *  ALLOCATION section
     * 
     */
    public bool Allocate(Quad.QuadTypes objType, int[,] pat)
    {
        bool freePosition = false;

        CountFreeQuads();

        int cellsQuantity = (pat != null) ? pat.GetLength(0) : 1; //change to null-coalescing expression


        if (cellsQuantity == 1 && universeFree < 1)
        {
            Debug.Log("(A)>>> Cant allocate object in universe (its full)!");
            return false;
        }

        if (cellsQuantity > 1 && universeFree < (cellsQuantity + 2)) //2 is just an operational margin...
        {
            Debug.Log("(A)>>> Cant allocate pattern in universe (not enough room)!");
            return false;
        }

        /*
         *  checks if its a LITTLE ROOM  (too few free quads avaliable...)
         */

        if (((double) universeFree / (double) universeSize) < 0.3)
        {
            Debug.Log(
                "(A)Its a little room:" + ((double) universeFree / (double) universeSize) * 100.0 +
                "% free quads");
        }

        /*
         *  TRY TO ALLOCATE - fingers crossed :)
         */

        bool allocated = false;
        
        if (cellsQuantity > 1) //trying to allocate a pattern (Target multicell or Creature)
        {
            int dontLockControl = 0;

            while (!allocated)
            {
                int[] coordinates = ChoiceRandomFreePosition();  //also removes choiced (only FIRST) quad from free list
                if (coordinates != null) //found a free quad
                {
                    if (ChecksIfPatternFitsInPosition(coordinates, pat))
                    {
                        this.columnFree = coordinates[0];
                        this.rowFree = coordinates[1];
                        for (int i = 0; i < pat.GetLength(0); i++) //fills ALL pattern quads with obj
                        {
                            quadType[coordinates[0] + pat[i, 0], coordinates[1] + pat[i, 1]] = objType;
                            
                            if (!RemoveQuadFromFreeQuadsList(coordinates[0] + pat[i, 0], coordinates[1] + pat[i, 1]))
                            {
                                Debug.Log("(A1)trying to mark cell as allocated but cell already free! FUCKING ERROR!");
                            }
                        }

                        allocated = true;
                    }
                    else
                    {
                        //pattern didnt fit in that particular position
                        
                        //INSERTS BACK (removed by ChoiceRandomFreePosition) quad in free list !!!

                        freeQuads.Add(new []{coordinates[0],coordinates[1]});
                        
                        Debug.Log(">>> (A5)Pattern didnt fit in place:");
                        Debug.Log(">>> (A5)Tryied @:" + coordinates[0] + "," + coordinates[1]);
                        Debug.Log(">>> (A5)Lock cntrl:" + dontLockControl);
                    }
                }
                else
                {
                    Debug.Log(">>> (A3)Allocation error: no free quads avaliable!");
                    return false;
                }

                dontLockControl++;
                if (dontLockControl > 50) // MAX 50 attempts!
                {
                    Debug.Log(">>> (A4)Allocation error: anti lock cntl");
                    return false;
                }
            }
        }
        else //trying to allocate  SINGLE cell
        {
            int[] coordinates = ChoiceRandomFreePosition();  //also removes choiced quad from free list
            if (coordinates != null) //found a free quad
            {
                this.columnFree = coordinates[0];
                this.rowFree = coordinates[1];
                quadType[coordinates[0], coordinates[1]] = objType;
                
                //ChoicedRandomFreePosition already removed quad from free list
                //if (!RemoveQuadFromFreeQuadsList(coordinates[0], coordinates[1])) //also does universeFree--
                //{
                //      Debug.Log("(A6)trying to mark cell as allocated but cell already free! FUCKING ERROR!");
                //}
        
                allocated = true;
            }
            else
            {
                Debug.Log(">>> (A7)Allocation error: no free quads avaliable!");
                return false;
            }
        }
        return allocated;
    }

    private bool ChecksIfPatternFitsInPosition(int []coordinates, int[,] pat)
    {
        int co = coordinates[0];
        int ro = coordinates[1];

        for (int i = 0; i < pat.GetLength(0); i++)
        {
            // checks if pattern inside universe boundaries
            if (co + pat[i, 0] > (universeSizeX - 1) || co + pat[i, 0] < 0)
                return false;
            if (ro + pat[i, 1] > (universeSizeY - 1) || ro + pat[i, 1] < 0)
                return false;
            
            //if so, checks if pattern over free quad
            if ((quadType[co + pat[i, 0], ro + pat[i, 1]] != Quad.QuadTypes.Empty &&
                 quadType[co + pat[i, 0], ro + pat[i, 1]] != Quad.QuadTypes.None))
            {
                return false;
            }
        }

        return true;
    }

    private int[] ChoiceRandomFreePosition()
    {
        if (freeQuads.Count > 0)
        {
         int index = rnd.Next(0, freeQuads.Count); 
         int[] rndCoordinates = new int[2];
         rndCoordinates[0] = freeQuads[index][0];  //column
         rndCoordinates[1] = freeQuads[index][1];  //row
         freeQuads.RemoveAt(index);
         universeFree--;
         return rndCoordinates;           
        }
        else
        {
            return null;
        }
    }

    public bool RemoveQuadFromFreeQuadsList(int x, int y)
    {
        for (int i = 0; i < freeQuads.Count; i++)
        {
            if (freeQuads[i][0] == x && freeQuads[i][1] == y)
            {
                freeQuads.RemoveAt(i);
                universeFree--;
                return true;
            }
        }
        return false;
    }

    private bool InFreeQuads(int x, int y)
    {
        for (int i = 0; i < freeQuads.Count; i++)
        {
            if (freeQuads[i][0] == x && freeQuads[i][1] == y)
            {
                return true;  //found!!! cell already in free quads list
            }
        }
        return false; //didnt found this cell coordinates in free quads list
    }
    
    public int CountFreeQuads()
    {
        universeFree = this.UniverseSize;
        for (int i = 0; i < universeSizeX; i++)
        {
            for (int j = 0; j < universeSizeY; j++)
                if (quadType[i, j] != Quad.QuadTypes.Empty)
                    universeFree--;
        }

        if (universeFree != freeQuads.Count)
        {
            Debug.Log("++++ ATTENTION: universeFree:"+universeFree+" freeQuads.Count:"+freeQuads.Count);
        }

        return universeFree;
    }

    private void DestroyQUs()
    {
        for (int i = 0; i < universeSizeX; i++)
        {
            for (int j = 0; j < universeSizeY; j++)
            {
                Destroy(quads[i, j].GetComponent<CellScript>());
                Destroy(quads[i, j]);
                //quadType[i, j] = null;

            }
        }
    }

    private String showQuads()
    {
        String deb = "";
        for (int j = UniverseSizeX - 1; j >= 0; j--)
        {
            for (int i = 0; i < UniverseSizeY; i++)
            {
                if (QuadType[i, j] == Quad.QuadTypes.Empty) deb += ".";
                if (QuadType[i, j] == Quad.QuadTypes.Cell) deb += "c";
                if (QuadType[i, j] == Quad.QuadTypes.Rotor) deb += "r";
                if (QuadType[i, j] == Quad.QuadTypes.Target) deb += "t";
                if (QuadType[i, j] == Quad.QuadTypes.Boom) deb += "b";
                if (QuadType[i, j] == Quad.QuadTypes.Sprite) deb += "s";
                if (QuadType[i, j] == Quad.QuadTypes.None) deb += "-";
            }
            deb += "|";
        }
        return deb;
    }
}
