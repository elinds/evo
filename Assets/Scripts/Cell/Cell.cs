using System.Collections.Generic;

public class Cell
{
    public enum State
    {
        Idle = 0,      
        Flying = 1,
        Moving = 2,
        Ploinking = 3,
        Matching = 4,
               
    }
    
    private List<CellLevel> levels;

    private int cellIndex;
    private int _id;
    private int _creatureId; //creature that owns that cell (if no owner, than -1)

    private bool isTargetPart;

    public bool IsTargetPart
    {
        get => isTargetPart;
        set => isTargetPart = value;
    }

    public CellDNA dna = new CellDNA();
    
    public int CreatureId
    {
        get => _creatureId;
        set => _creatureId = value;
    }

    public State state;
    
    //public bool flying, moving, ploinking;
    public float flyingCntr;
    public float matchCntr;
    public float rotOffset;
    public int Id
    {
        get => _id;
        set => _id = value;
    }
    
    //private int[,] miniTex = new int[32,32];

    public Cell()
    {
        levels = new List<CellLevel>();

        _creatureId = -1;
        
        //flying = moving = ploinking = false;

        state = State.Idle;
        
        flyingCntr = 0;
        matchCntr = 0;
        rotOffset = 0f;
    }
    public void AddLevel(CellLevel cl)
    {
        levels.Add(cl);
    }

    public CellLevel getLevel(int index)
    {
        return levels[index];
    }
    
    public CellLevel getLastLevel()
    {
        return levels[levels.Count - 1];
    }
    public int getLevelsQuantity()
    {
        return levels.Count;
    }

    public List<CellLevel> getAllLevels()
    {
        return levels;
    }

    public void SetLevels(List<CellLevel> levs)
    {
        levels = levs;
    }

    /*public void SetPixel(int x, int y, int c)
    {
        miniTex[x, y] = c;
    }
    public int GetPixel(int x, int y)
    {
        return miniTex[x, y];
    }*/   
    
}