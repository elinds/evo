using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class CreaturePatterns
    {
        private int[,,] p2, p3, p4, p5, p6, p7, p8;
        private List<int[,,]> pattern = new List<int[,,]>();
 
        private System.Random rnd;
        public CreaturePatterns()
        {
            rnd = new System.Random();

            /*        columns  ->
             *
             *        -1,1    0,1    1,1              ^
             *        -1,0    0,0    1,0        lines | 
             *        -1,-1   0,-1   1,-1
             */
            
            /*
             *     { col, row }
             */
            
            
            p2 = new int[,,]   //size 2 patterns
            {
                { {0,0}, {0,1} },        //all possible patterns length 2 covered 
                { {0,0}, {1,0} },
                { {0,0}, {1,1} },
                { {0,0}, {0,-1} },
                { {0,0}, {-1,0} },
                { {0,0}, {-1,1} },
                { {0,0}, {1,-1} },
                { {0,0}, {-1,-1} }
            };
            p3 = new int[,,]   //size 3 patterns
            {                                       
                { {0,0},{0,1}, {0,-1} },  //  !
                { {0,0},{-1,0}, {1,0} },  //  -
                { {0,0},{-1,-1},{1,1} },  //  /
                { {0,0},{1,-1}, {-1,1} }, //  \
                { {0,0},{-1,0}, {1,-1} }, //  -,
                { {0,0},{1,0},{1,1} },    //  -'
                { {0,0},{-1,0},{-1,1} },  //  '-
                { {0,0},{-1,0},{-1,-1} }  //  ,-
            };
            p4 = new int[,,] //size 4 patterns  
            {
                {{0, 0}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 0}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 1}, {0, 1}, {0, 1}}
            };
            p5 = new int[,,] //size 5 patterns 
            {
                {{0, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 0}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 1}, {0, 1}, {0, 1}, {0, 1}}

            };
            p6 = new int[,,] //size 6 patterns    
            {
                {{0, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}}

            };
            p7 = new int[,,] //size 7 patterns    
            {
                {{0, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}}

            };
            p8 = new int[,,] //size 8 patterns 
            {
                {{0, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 0}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}},
                {{0, 0}, {1, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}, {0, 1}}

            };
            
            pattern.Add(p2);
            pattern.Add(p3);
            pattern.Add(p4);
            pattern.Add(p5);
            pattern.Add(p6);
            pattern.Add(p7);
            pattern.Add(p8);
            
        }

        public int[,] getRandomPattern(int size)  //pattern size: 2 to 8
        {
            int[,] pat = new int[size,2]; //supposing max length of pattern = 12
            int[,,] p = pattern[size - 2];
            
            int patternIndex = rnd.Next(0, p.GetLength(0));   Debug.Log("   PAT IDX:"+patternIndex);
            
            for (int i = 0; i < size; i++)
            {
                pat[i, 0] = p[patternIndex, i, 0]; //column            
                pat[i, 1] = p[patternIndex, i, 1]; //row
            }
            return pat;            
        }
        public int[,] getByIndexPattern(int size, int patternIndex)  //pattern size: 2 to 8
        {
            int[,] pat = new int[size,2];
            int[,,] p = pattern[size - 2];
            
            for (int i = 0; i < size; i++)
            {
                pat[i, 0] = p[patternIndex, i, 0]; //column
                pat[i, 1] = p[patternIndex, i, 1]; //row
            }
            return pat;
        }
    }
}