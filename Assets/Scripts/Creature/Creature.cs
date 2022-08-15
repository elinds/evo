
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature
{
    private List<Cell> body;

    private int cellIndex;
    private int _id;

    private bool _selected;
    private bool _addingCellsStarted;

    public Universe u;

    public bool Selected
    {
        get => _selected;
        set => _selected = value;
    }

    public int Id
    {
        get => _id;
        set => _id = value;
    }

    public Creature(Universe universe)
    {
        u = universe;
        body = new List<Cell>();
        _selected = false;
        _addingCellsStarted = false;
    }

    public void AddCell()
    {
        
    }
    public void RemoveCell()
    {
        
    }
    public bool AllNeighborsAdded()
    {
        return false;
    }

    public List<GameObject> AddCells(int posX, int posY)   //its a non recursive FloodFill 
    {
        List<GameObject> b = new List<GameObject>();
        bool walk = true;
        Stack bodyCells = new Stack();
        
        if (u.QuadType[posX, posY] == Quad.QuadTypes.Cell)
        {
            bodyCells.Push(u.Quads[posX,posY]);  //FOUND
        }
        else
        {
            walk = false;
        }

        int pX = posX;
        int pY = posY;
        
        while (walk)
        {
            
            if (bodyCells.Count == 0)
            {
                walk = false;
            }
            else
            {   
                
                b.Add((GameObject)bodyCells.Pop());
                pX = b[b.Count - 1].GetComponent<Quad>().GetX();
                pY = b[b.Count - 1].GetComponent<Quad>().GetY();
                
                
                if (pX < u.UniverseSizeX - 1)
                {
                   if (u.QuadType[pX + 1, pY] == Quad.QuadTypes.Cell)
                   {
                       if (u.Quads[pX + 1,pY].GetComponent<Quad>().Cell.CreatureId < 0)
                       {
                           u.Quads[pX + 1, pY].GetComponent<Quad>().Cell.CreatureId = _id;
                           bodyCells.Push(u.Quads[pX + 1,pY]);  //FOUND
                       }
                       
                   } 
                }
                if (pX > 0)
                {
                    if (u.QuadType[pX - 1, pY] == Quad.QuadTypes.Cell)
                    {
                        if (u.Quads[pX - 1,pY].GetComponent<Quad>().Cell.CreatureId < 0)
                        {
                            u.Quads[pX - 1, pY].GetComponent<Quad>().Cell.CreatureId = _id;
                            bodyCells.Push(u.Quads[pX - 1,pY]);  //FOUND
                        }
                       
                    }
                }
                if (pY < u.UniverseSizeY - 1)
                {
                    if (u.QuadType[pX, pY + 1] == Quad.QuadTypes.Cell)
                    {
                        if (u.Quads[pX ,pY + 1].GetComponent<Quad>().Cell.CreatureId < 0)
                        {
                            u.Quads[pX , pY + 1].GetComponent<Quad>().Cell.CreatureId = _id;
                            bodyCells.Push(u.Quads[pX ,pY + 1]);  //FOUND
                        }                       
                    }
                }
                if (pY > 0)
                {
                   if (u.QuadType[pX, pY - 1] == Quad.QuadTypes.Cell)
                   {
                       if (u.Quads[pX ,pY - 1].GetComponent<Quad>().Cell.CreatureId < 0)
                       {
                           u.Quads[pX , pY - 1].GetComponent<Quad>().Cell.CreatureId = _id;
                           bodyCells.Push(u.Quads[pX ,pY - 1]);  //FOUND
                       }   
                   } 
                }
            }
        }

        /*//marks every cell as owned by this creature
        for (int i = 0; i < b.Count; i++)
        {
            b[i].GetComponent<Quad>().Cell.CreatureId = _id; 
            
        }*/
        
        return b;
    }
}
