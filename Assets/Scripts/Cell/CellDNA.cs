
using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;

public class CellDNA
{
    public enum Dir
    {
        None = 0, //wait some cicles before doing fagocitation
        Vertical = 1,
        Horizontal = 2,
        Z = 3,
    }

    private List<int> type = new List<int>();
    private List<int> op = new List<int>();

    public CellDNA()
    {
    }

    /*
     * insertion for first quadrant of new cell - used in LevelManager/CreateTarget
     */
    public void insert(int cellType)
    {
        type.Add(cellType);
    }

    /*
     * insertion for subsequent quadrants of cell - used in LevelManager/CreateTarget
     *
     * inserts one single quadrant
     */
    public void insert(CellInfo.DirectionMode dirWay, int cellType)
    {
        Dir dir;
        
        if (dirWay == CellInfo.DirectionMode.Up || dirWay == CellInfo.DirectionMode.Down)
        {
            dir = Dir.Vertical;
        }
        else
        {
            if (dirWay == CellInfo.DirectionMode.Left || dirWay == CellInfo.DirectionMode.Right)
            {
                dir = Dir.Horizontal;
            }
            else
            {
                dir = Dir.Z;
            }
        }

        switch (dirWay)
        {
            case CellInfo.DirectionMode.Left:
                type.Insert(0, cellType); //adds at beginning   
                break;
            case CellInfo.DirectionMode.Right:
                type.Add(cellType); //adds at end
                break;
            case CellInfo.DirectionMode.Down:
                type.Insert(0, cellType); //adds at beginning   
                break;
            case CellInfo.DirectionMode.Up:
                type.Add(cellType); //adds at end
                break;
            case CellInfo.DirectionMode.All: 
                type.Add(cellType); //adds at end
                break;           
        }
        
        op.Add((int) dir);
        
        /*switch (dir)
        {
            case Dir.Vertical:
                
                op.Insert(0, (int) dir);

                /*if(type[0] > type[1])
                    swaps(type,0);#1#
                
                break;
            
            case Dir.Horizontal:
                
                op.Add((int) dir);

                /*if(type[type.Count - 2] > type[type.Count - 1])
                    swaps(type,type.Count - 2);#1#               
                
                break;

            case Dir.Z:


                op.Add((int) dir);
                break;
        }*/

        ;

        // insert code of insertion direction
        //op.Insert(0, (int) dir);

        /*
         * unifies both ways of same direction:
         * right & left will be left
         * down & up will be up
         * 
         */

        /*
        if (dir == CellInfo.DirectionMode.Right)
        {
            dir = CellInfo.DirectionMode.Left;
        }

        if (dir == CellInfo.DirectionMode.Down)
        {
            dir = CellInfo.DirectionMode.Up;
        }*/
    }

    /*
     *   insertion used when 2 existing cells merge - used in MoveManager/MergeCells
     *
     *   inserts all origin quadrants in dest
     */
    public void insert(CellInfo.DirectionMode dirWay, List<int> types, List<int> ops)
    {
        Dir dir;
        
        if (dirWay == CellInfo.DirectionMode.Up || dirWay == CellInfo.DirectionMode.Down)
        {
            dir = Dir.Vertical;
        }
        else
        {
            if (dirWay == CellInfo.DirectionMode.Left || dirWay == CellInfo.DirectionMode.Right)
            {
                dir = Dir.Horizontal;
            }
            else
            {
                dir = Dir.Z;
            }
        }
        
        switch (dirWay)
        {
            case CellInfo.DirectionMode.Right:
                for (int i = types.Count - 1; i >= 0; i--)
                {
                    type.Insert(0, types[i]); //adds at beginning
                }
                /*foreach (int t in types)
                {
                    type.Insert(0, t); //adds at beginning
                }*/
                break;
            case CellInfo.DirectionMode.Left:
                foreach (int t in types)
                {
                    type.Add(t); //adds at end
                }
                break;
            case CellInfo.DirectionMode.Up:
                for (int i = types.Count - 1; i >= 0; i--)
                {
                    type.Insert(0, types[i]); //adds at beginning
                }
                /*foreach (int t in types)
                {
                    type.Insert(0, t); //adds at beginning
                } */  
                break;
            case CellInfo.DirectionMode.Down:
                foreach (int t in types)
                {
                    type.Add(t); //adds at end
                }
                break;
            case CellInfo.DirectionMode.All: 
                foreach (int t in types)
                {
                    type.Add(t); //adds at end
                }
                break;           
        } 
        
        foreach (int o in ops)
        {
            op.Add(o); //adds at end
        }    
        
        op.Add((int) dir);       
        
        /*switch (dir)
        {

            case Dir.Vertical:

                foreach (int o in ops)
                {
                    op.Insert(0, o); //adds at beginning
                }

                /*if(type[0] > type[1])
                    swaps(type,0);#1#
                
                break;               
                
            case Dir.Horizontal:
                foreach (int t in types)
                {
                    type.Add(t); //adds at end
                }
                foreach (int o in ops)
                {
                    op.Add(o); //adds at end
                }

                /*if(type[type.Count - 2] > type[type.Count - 1])
                    swaps(type,type.Count - 2); #1#
                
                break;

            /*
             * its a FAGOCITATION: its assimetric, i.e., origin always goes at end of dest
             * so its always possible to know whos inside who
             #1#
            case Dir.Z:
                foreach (int t in types)
                {
                    type.Add(t); //adds at end
                }
                foreach (int o in ops)
                {
                    op.Add(o); //adds at end
                }

                break;
        }*/
        // insert code of insertion direction
        //op.Insert(0, (int) dir);
       // op.Add((int) dir);
    }

    public String show()
    {
        String ret = "";
        foreach (int t in type)
        {
            ret += t + "|";
        }

        ret += " :: ";

        foreach (int o in op)
        {
            ret += o + "|";
        }

        return ret;
    }

    public List<int> getTypes()
    {
        return type;
    }

    public List<int> getOps()
    {
        return op;
    }

    /*private void swaps(List<int> typ, int i)
    {
        int aux = typ[i];
        typ[i] = typ[i + 1];
        typ[i + 1] = aux;
    }*/
}
