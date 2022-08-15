
using System;
using System.Collections.Generic;
using UnityEngine;



public class QuadManager: ScriptableObject
{

    private Material quadSelect;
    private Material quadUnselect;
    private Material quadTransparent;
    public  Material[] quadTargetTransp = new Material[3];
    public  Material quadTarget;
    private Material creatureSelect;
    private Material transparent;

    public GameObject selectedQuadTarget;
    public bool selectedQuadTargetVisible;    // <---------------------------------------------


    private GameObject quadrant;
    private GameObject sprite;
    
    private List<GameObject> cells;
    
    private Universe uni;
    private Texturer tx;
    private SoundManager sm;
    private PlatformManager pm;

    private float targetQuadZ = -0.005f;
    private float activeQuadZ = -0.01f;
    
   public void Init(Universe uni, Texturer tx, SoundManager sm, PlatformManager pm, List<GameObject> cells,
       GameObject quadrant, GameObject sprite ,List<Material> materialList)
   {
      this.uni  = uni;
      this.tx = tx;
      this.sm = sm;
      this.pm = pm;
      uni.qm = this;

      this.quadrant = quadrant;
      this.sprite = sprite;
      this.cells = cells;
      
      this.quadSelect = materialList[0];
      this.quadUnselect = materialList[1];
      this.quadTransparent = materialList[2];
      this.quadTarget = materialList[3];

      for(int i = 3; i< 6; i++)
      {
          quadTargetTransp[i - 3] =  materialList[i];
      }
      
      
      this.creatureSelect = materialList[6];
      this.transparent = materialList[7];
      
      
      if (pm.CurrPlatform == PlatformManager.Platform.Desktop)
      {
          selectedQuadTarget = CreateTargetQuad();
          selectedQuadTarget.SetActive(true);
          selectedQuadTargetVisible = true;
      }
      
   }

   public void SelectCreature(List<GameObject> creatureCells)
    {
        for (int i = 0; i < creatureCells.Count; i++)
        {
             int x = creatureCells[i].GetComponent<Quad>().GetX();
             int y = creatureCells[i].GetComponent<Quad>().GetY();
                
             uni.Quads[x,y].GetComponent<MeshRenderer>().material = creatureSelect;
        }
    }
    public void SelectQuad(int index)
    {
        //Debug.Log(("sel index:" + index));
        int x = cells[index].GetComponent<CellScript>().X;
        int y = cells[index].GetComponent<CellScript>().Y;
        //Debug.Log(("sel x:" + x + " y:" + y));   
        //uni.Quads[x,y].GetComponent<MeshRenderer>().material = quadSelect;  //Quadselect
        uni.Quads[x, y].GetComponent<Renderer>().material.mainTexture = quadSelect.mainTexture;
    }
    public void UnselectQuad(int index)
    {
        //Debug.Log(("unsel index:" + index));
        int x = cells[index].GetComponent<CellScript>().X;
        int y = cells[index].GetComponent<CellScript>().Y;
        //Debug.Log(("unsel x:" + x + " y:" + y));
        //uni.Quads[x,y].GetComponent<MeshRenderer>().material = quadUnselect;  //QuadUnselect
       // uni.Quads[x, y].GetComponent<MeshRenderer>().material = uni.Quads[x, y].GetComponent<Quad>().texture;
        uni.Quads[x, y].GetComponent<Renderer>().material.mainTexture = uni.Quads[x, y].GetComponent<Quad>().texture;
    }
    public void SelectTargetQuad(int x, int y)
    {
        selectedQuadTarget.GetComponent<Quad>().x = x;
        selectedQuadTarget.GetComponent<Quad>().y = y;
        selectedQuadTarget.transform.position = new Vector3( x * uni.quadSize, y * uni.quadSize, targetQuadZ);
    }   
    public void MoveTargetQuad(int x, int y)
    {
        if (x != 0)
        {
          if(selectedQuadTarget.GetComponent<Quad>().x + x >= 0 && selectedQuadTarget.GetComponent<Quad>().x + x < uni.UniverseSizeX)
              selectedQuadTarget.GetComponent<Quad>().x += x;  
        }

        if (y != 0)
        { 
          if(selectedQuadTarget.GetComponent<Quad>().y + y >= 0 && selectedQuadTarget.GetComponent<Quad>().y + y < uni.UniverseSizeY)
                selectedQuadTarget.GetComponent<Quad>().y += y; 
        }
 
        selectedQuadTarget.transform.position = new Vector3( selectedQuadTarget.GetComponent<Quad>().x * uni.quadSize,
            selectedQuadTarget.GetComponent<Quad>().y * uni.quadSize, targetQuadZ);
    }   
    
    /*
     *
     *  TARGET Quad
     * 
     */
    public GameObject CreateTargetQuad()
    {
        GameObject activeQuad = Instantiate(quadrant);
        activeQuad.GetComponent<Quad>().qm = this;
        activeQuad.GetComponent<Quad>().thisQuadType = Quad.QuadTypes.QuadTarget;

        activeQuad.GetComponent<Quad>().x = 0;
        activeQuad.GetComponent<Quad>().y = 0;
        activeQuad.transform.position = new Vector3( 0,  0, targetQuadZ);
        activeQuad.transform.localScale = new Vector3( 1.1f, 1.1f, 1.0f );

        activeQuad.GetComponent<MeshRenderer>().material = quadTarget;

        return activeQuad;

    }
    
    /*
     *
     *  ACTIVE Quad
     * 
     */
    public GameObject CreateActiveQuad(Quad.QuadTypes activeQuadTypes)
    {
        GameObject activeQuad = Instantiate(quadrant);
        activeQuad.GetComponent<Quad>().qm = this;
        activeQuad.GetComponent<Quad>().thisQuadType = activeQuadTypes;

        activeQuad.GetComponent<Quad>().sm = sm;

        uni.Allocate(activeQuadTypes, null); //its a single cell
        
        activeQuad.GetComponent<Quad>().x = uni.ColumnFree;
        activeQuad.GetComponent<Quad>().y = uni.RowFree;
        activeQuad.transform.position = new Vector3((uni.ColumnFree) * uni.quadSize,
            uni.RowFree * uni.quadSize, activeQuadZ);

        activeQuad.GetComponent<MeshRenderer>().material = quadTransparent;
        
        activeQuad.GetComponent<Renderer>().material.mainTexture = tx.GetActiveQuadTexture(activeQuadTypes);
        
        return activeQuad;
    }
    
    /*
     *
     *  SPRITE Quad
     * 
     */
    public void CreateActiveSprite(String spriteName)
    {
        GameObject spriteQuad = Instantiate(sprite);
        
        spriteQuad.GetComponent<SpriteController>().spriteArray = Resources.LoadAll<Sprite>(spriteName);

        uni.Allocate(Quad.QuadTypes.Sprite, null); //its a single cell
        
        //activeQuad.transform.position = new Vector3((uni.ColumnFree) * uni.quadSize,
        //    uni.RowFree * uni.quadSize, activeQuadZ);
        spriteQuad.transform.position = new Vector3(2, 0, -1);
        
    }
}
