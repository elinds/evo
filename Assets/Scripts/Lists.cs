
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

public class Lists
{
    
    public List<GameObject> cells = new List<GameObject>();
    public List<GameObject> actives = new List<GameObject>();
    public List<Creature>   creatures = new List<Creature>();

    public List<List<GameObject>> targets = new List<List<GameObject>>();
    public List<List<CellInfo>> targetsInfo = new List<List<CellInfo>>(); 
    
    public int idCellCntr;
    public int idTargetCellCntr;
    

    public Lists()
    
    {
        idCellCntr = 1;
        idTargetCellCntr = 1;
    }

    public int GetCellIndex(int id)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].GetComponent<CellScript>()._cell.Id == id)
                return i;
        }
        return -1;
    }
    public int[] GetTargetCellIndexes(int id)
    {
        int[] pairOfIndexes = new int[2];
        pairOfIndexes[0] = -1;
        pairOfIndexes[1] = -1;
        
        for (int i = 0; i < targets.Count; i++)
        {
            for (int j = 0; j < targets[i].Count; j++)
            {
                if (targets[i][j].GetComponent<CellScript>()._cell.Id == id)
                {
                    pairOfIndexes[0] = i;
                    pairOfIndexes[1] = j;
                    return pairOfIndexes;
                }
            }
        }
        
        Debug.Log("ERROR @ GetTargetCellIndex: Didnt found target cell "+ id +" in targets!");
        
        return pairOfIndexes;
    }
}
