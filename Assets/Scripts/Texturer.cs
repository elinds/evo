
using System.Collections.Generic;

using UnityEngine;

/*
 
 *                                  ### TEXTURER ###
     
*/

public class Texturer : ScriptableObject
{
    //Textures
    private Texture2D mainTexture;
    private int textureSize = 160;

    public int TextureSize
    {
        get => textureSize;
        set => textureSize = value;
    }

    private const int MaxLevels = 3;  //max number of fagocitation levels
   
    //COLORS
    private Palettes _palettes;
    private Color32[] colors;

    public TextAsset imageAsset;
    
    public void CreateTexturer()
    {
        //initialize color control
        _palettes = new Palettes();
        colors = _palettes.getPalette(3);
    }

    public Texture2D CreateCellTexture(Cell c) //int cellIndex
    {
        int[,,] tex = new int[textureSize + 1, textureSize + 1, MaxLevels]; //3 levels MAX !
        for (int k = 0; k < MaxLevels; k++)
        for (int i = 0; i < textureSize + 1; i++)
        for (int j = 0; j < textureSize + 1; j++)
            tex[i, j, k] = -1;


        //Cell c = cells[cellIndex].GetComponent<CellScript>().Cell;

        CreateTexture(c.getAllLevels(), tex, 0);

        TexturesFlattening(c.getAllLevels(), tex);

        /*int cx = 0;
        int cy = 0;

        for (int i = 0; i < 30; i++)
        {
            for (int j = 0; j < 30; j++)
            {
                c.SetPixel(i, j, tex[cx, cy, 0]);
                cy += 5;
            }

            cx += 5;
            cy = 0;
        }*/

        return ApplyTexture(tex); //
    }

    public void CreateTexture(List<CellLevel> level, int[,,] tex, int currLevel)
    {
        CellQuadrant cq;
        List<CellQuadrant> quads;

        
        for (int lev = 0; lev < level.Count; lev++)
        {
            quads = level[lev].getQuadrants();
            
            for (int q = 0; q < quads.Count; q++)
            {
                cq = quads[q];

                if (cq.refLevels != null)
                {
                    CreateTexture(cq.refLevels, tex, lev);  //currLevel + lev
                }
                else
                {
                    //Draws background
                    for (int x = cq.StartX; x < cq.StartX + cq.SizeX; x++)
                    {
                        for (int y = cq.StartY; y < cq.StartY + cq.SizeY; y++)
                        {
                            tex[x, y, currLevel + lev] = cq.getBackColor();
                        }
                    }

                    //Draws foreground
                    switch (cq.getShape())
                    {
                        case CellQuadrant.Shape.Triangle:
                            float tx = cq.SizeX / 10F;
                            float ty = cq.SizeY / 10F;
                            float factor = ((float) cq.SizeX / (float) cq.SizeY);
                            float cntr = 0;

                            // triangle <
                            if (cq.TriangleType == 1 || cq.TriangleType == 2)
                            {
                                // triangle <
                                for (int y = cq.StartY + (int) (ty); y < cq.StartY + (int) (5 * ty); y++)
                                {
                                    cntr++;
                                    for (int x = (cq.StartX + (int) (2 * tx));
                                        x <= (cq.StartX + (int) (2 * tx)) + (cntr * factor) * 1.5f;
                                        x++)
                                    {
                                        tex[x, y, currLevel + lev] = cq.getForeColor();
                                        //tex[x, cq.StartY + (int) ((10 * ty) - y - 1), lev] = cq.getForeColor(); 
                                    }
                                }

                                for (int y = cq.StartY + (int) (5 * ty); y < cq.StartY + (int) (9 * ty); y++)
                                {
                                    cntr--;
                                    for (int x = (cq.StartX + (int) (2 * tx));
                                        x <= (cq.StartX + (int) (2 * tx)) + (cntr * factor) * 1.5f;
                                        x++)
                                    {
                                        tex[x, y, currLevel + lev] = cq.getForeColor();
                                    }
                                }

                                // triangle >    (mirroring triangle <)
                                if (cq.TriangleType == 2)
                                {
                                    int auxtex;
                                    int width = (int) (cntr * factor);
                                    for (int y = cq.StartY + (int) (ty); y < cq.StartY + (int) (10 * ty); y++) //5
                                    {
                                        int aux = 0;
                                        for (int x = cq.StartX + (int) (10 * tx); x > cq.StartX + (int) (5 * tx); x--)
                                        {
                                            auxtex = tex[x, y, currLevel + lev];
                                            tex[x, y, currLevel + lev] = tex[cq.StartX + aux, y, currLevel + lev];
                                            tex[cq.StartX + aux, y, currLevel + lev] = auxtex;
                                            aux++;
                                        }
                                    }
                                }
                            }
                            // triangle ^
                            else if (cq.TriangleType == 3)
                                for (int y = cq.StartY + (int) (2 * ty); y < cq.StartY + (int) (8 * ty); y++)
                                {
                                    if (y % 2 == 0) cntr++;
                                    for (int x = cq.StartX + cq.SizeX / 2 - (int) (cntr * factor);
                                        x < cq.StartX + cq.SizeX / 2 + (int) (cntr * factor);
                                        x++)
                                    {
                                        tex[x, y, currLevel + lev] = cq.getForeColor();
                                    }
                                }
                            // triangle V
                            else
                                for (int y = cq.StartY + (int) (8 * ty); y > cq.StartY + (int) (2 * ty); y--)
                                {
                                    if (y % 2 == 0) cntr++;
                                    for (int x = cq.StartX + cq.SizeX / 2 - (int) (cntr * factor);
                                        x < cq.StartX + cq.SizeX / 2 + (int) (cntr * factor);
                                        x++)
                                    {
                                        tex[x, y, currLevel + lev] = cq.getForeColor();
                                    }
                                }

                            break;
                        case CellQuadrant.Shape.Ellipse:
                            int borda = 4;
                            if (cq.SizeX < textureSize || cq.SizeY < textureSize)
                                borda = 1;
                            float a = (cq.SizeX / 2) - 3 * borda;
                            float b = (cq.SizeY / 2) - 3 * borda;
                            float cx = cq.StartX + cq.SizeX / 2;
                            float cy = cq.StartY + cq.SizeY / 2;

                            for (int x = cq.StartX + borda; x < cq.StartX + (cq.SizeX); x++)
                            {
                                for (int y = cq.StartY + borda; y < cq.StartY + (cq.SizeY); y++)
                                {
                                    float e1 = ((x - cx) * (x - cx)) / (a * a);
                                    float e2 = ((y - cy) * (y - cy)) / (b * b);
                                    if ((e1 + e2) <= 1)
                                        tex[x, y, currLevel + lev] = cq.getForeColor();
                                }
                            }

                            break;
                        case CellQuadrant.Shape.Rectangle:
                            int sizex = cq.SizeX / 2;
                            int sizey = cq.SizeY / 2;
                            for (int x = cq.StartX + sizex / 4; x < cq.StartX + sizex / 2 + sizex + (sizex / 4); x++)
                            {
                                for (int y = cq.StartY + sizey / 4;
                                    y < cq.StartY + sizey / 2 + sizey + (sizey / 4);
                                    y++)
                                {
                                    tex[x, y, currLevel + lev] = cq.getForeColor();
                                }
                            }

                            break;
                    } //switch    
                } //else
            }
        }
    }

    /*
     * FLATTENING of all texture levels
     * 
     * MERGE all levels at level 0  ->  MAX of 3 levels beyond level 0
     */
    public void TexturesFlattening(List<CellLevel> level, int[,,] tex)
    {
        //offs maps area coordinates for 3 levels (i.e., levels 0, 1 and 2); so MaxLevels is 3
        int[] offs = new[]
        {
            /* lev 0*/0,
            /* lev 1*/textureSize / 4,
            /* lev 2*/(textureSize / 4 + (textureSize / 8)),
            /* lev 3*/(textureSize / 4 + (textureSize / 8) + (textureSize / 16))  //not used, by now...
        };
        int offsetX, offsetY;

        /*for (int le = 1; le < MaxLevels; le++) //3 levels MAX !
        {
            offsetX = offs[le - 1]; //offsetX = textureSize / (4 * le);
            for (int x = 0; x < textureSize; x++)
            {
                if (x % (2 * le) == 0) offsetX++;
                offsetY = offs[le - 1]; //offsetY = textureSize / (4 * le);
                for (int y = 0; y < textureSize; y++)
                {
                    if (y % (2 * le) == 0) offsetY++;

                    if (x % (2 * le) == 0 && y % (2 * le) == 0)
                    {
                        if (tex[x, y, le] > -1)
                            tex[offsetX, offsetY, 0] = tex[x, y, le];
                    }
                }
            }*/
        /*
        for (int le = 1; le < MaxLevels; le++)
        {
            for (int x = offs[le]; x < textureSize - offs[le]; x++)
            {
                for (int y = offs[le]; y < textureSize - offs[le]; y++)
                {
                    if (tex[x, y, le] > -1)
                        tex[x, y, 0] = tex[x, y, le];                   
                }
            }
        }
        */
        for (int le = 1; le < MaxLevels; le++)
        {
            for (int x = 0; x < textureSize; x++)
            {
                for (int y = 0; y < textureSize; y++)
                {
                    if (tex[x, y, le] > -1)
                        tex[x, y, 0] = tex[x, y, le];                   
                }
            }
        }
        //prevents -1 values in flattened level 0   just for DEBUG purposes
        int ccc = 0;
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                if (tex[i, j, 0] < 0)
                {
                    ccc++;
                    tex[i, j, 0] = 0;
                }
            }
        }
        Debug.Log("ccc:"+ccc);
    }
    
    public Texture2D ApplyTexture(int [,,] tex) //, int cellIndex
    {
        
        //mainTexture = new Texture2D(textureSize, textureSize,TextureFormat.RGBA32,false);
        //mainTexture = new Texture2D(textureSize, textureSize);
        Texture2D texture = new Texture2D(textureSize, textureSize);
        
        //mainTexture.wrapModeU = TextureWrapMode.Mirror;
        //Level 0
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                //Color32 co = colors[tex[i, j, 0]];

                texture.SetPixel(i, j, colors[tex[i, j, 0]] );//mainTexture.SetPixel(i, j, colors[tex[i, j, 0]] );
                //print(i + " " + j + ":" + colors[tex[i, j, 0]]);
            }
        }

        //texture.mipMapBias = 3.7f;
        texture.Apply(); //mainTexture.Apply();

        //cells[cellIndex].GetComponent<Renderer>().material.mainTextureScale = new Vector2(1f, 1f);
        
        //mainTexture.wrapMode = TextureWrapMode.Clamp;  //.Repeat
        
        //mainTexture.filterMode = FilterMode.Point;
        texture.filterMode = FilterMode.Bilinear;
        return texture;
        //cells[cellIndex].GetComponent<Renderer>().material.mainTexture = mainTexture;

    }
    
    public Texture2D GetTexture(string textureName)
    {
        //int[,] tex = null;

        /*switch (activeQuadTypes)
        {
            case Quad.QuadTypes.Rotor:
                tex = CreateTexture_Rotor();
                break;
            case Quad.QuadTypes.Boom:
                tex = CreateTexture_Boom();
                break;
        }*/

        //Load all textures into an array
        //Object[] textures = Resources.LoadAll("Assets/Resources", typeof(Texture2D));
        
        
        Texture2D im = Resources.Load(textureName) as Texture2D;
        
        Debug.Log("image: "+ im);

        if (im == null)
        {
            Debug.Log("ERROR: Cant load texture...");
        }
        
        /*byte[] FileData;

        if (File.Exists("Assets/Resources/coisa.png"))
        {
            Debug.Log("achou");
            FileData = File.ReadAllBytes("Assets/Resources/coisa.png");
        }
        else
        {
            Debug.Log("================   nao achou");
        }*/
        //Texture2D locTexture = new Texture2D(19, 21);
        //locTexture.LoadImage(imageAsset.bytes);
        /*
        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                Color co = (tex[i, j] == 10) ? Color.clear : Color.grey;
                locTexture.SetPixel(i, j, co);
            }
        }
*/
       /*locTexture.Apply();
        locTexture.wrapMode = TextureWrapMode.Clamp;
        locTexture.filterMode = FilterMode.Point;*/
       //locTexture.filterMode = FilterMode.Point;
       //locTexture.wrapMode = TextureWrapMode.Clamp;
       /*int[,] tex = null;
       tex = CreateTexture_Boom();
       Texture2D locTexture = new Texture2D(textureSize, textureSize);

       for (int i = 0; i < textureSize; i++)
       {
           for (int j = 0; j < textureSize; j++)
           {
               //im.GetPixel(i, j);
               //Color co = (tex[i, j] == 10) ? Color.clear : Color.grey;
               //Color pixel = im.GetPixel(i, j);
              // locTexture.SetPixel(i, j, pixel);
               //locTexture.SetPixel(i, j, co);
           }
       }

       locTexture.Apply();
       locTexture.wrapMode = TextureWrapMode.Clamp;
       locTexture.filterMode = FilterMode.Point;*/
       //im.wrapMode = TextureWrapMode.Repeat;
       im.filterMode = FilterMode.Bilinear;
        return im;
    }   
    
    /*
     *  ACTIVE QUADS TEXTURES
     */

    public Texture2D GetActiveQuadTexture(Quad.QuadTypes activeQuadTypes)
    {
        int[,] tex = null;

        switch (activeQuadTypes)
        {
            case Quad.QuadTypes.Rotor:
                tex = CreateTexture_Rotor();
                break;
            case Quad.QuadTypes.Boom:
                tex = CreateTexture_Boom();
                break;
        }

        Texture2D locTexture = new Texture2D(textureSize, textureSize);

        for (int i = 0; i < textureSize; i++)
        {
            for (int j = 0; j < textureSize; j++)
            {
                Color co = (tex[i, j] == 10) ? Color.clear : Color.grey;
                locTexture.SetPixel(i, j, co);
            }
        }

        locTexture.Apply();
        locTexture.wrapMode = TextureWrapMode.Clamp;
        locTexture.filterMode = FilterMode.Point;
        
        return locTexture;
    }
    public int [,] CreateTexture_Rotor()
    {
        int [,] tex = new int[textureSize + 1, textureSize + 1];
        
        for (int x = 0; x < textureSize; x++)
          for (int y = 0; y < textureSize; y++)
              tex[x, y] = 10;
        
        for (int x = 0; x < textureSize; x++)
        {
           tex[x, textureSize/2] = 2;
           tex[x, textureSize/2 + 1] = 2;
           tex[x, textureSize/2 - 1] = 2;
           tex[x, textureSize/2 + 2] = 2;
           tex[x, textureSize/2 - 2] = 2;           
        }
        for (int y = 0; y < textureSize; y++)
        {
            tex[textureSize/2, y] = 2;
            tex[textureSize/2 + 1, y] = 2;
            tex[textureSize/2 - 1, y] = 2;
            tex[textureSize/2 + 2, y] = 2;
            tex[textureSize/2 - 2, y] = 2;
        }

        int width = 3;
        for (int y = textureSize / 2 - 20; y < textureSize / 2; y++)
        {
            for (int x = textureSize / 2 - width; x < textureSize / 2 + width; x++)
            {
                tex[x, y] = 2;
            }

            width++;
        }
        
        for (int y = textureSize / 2; y < textureSize / 2 + 20; y++)
        {
            for (int x = textureSize / 2 - width; x < textureSize / 2 + width; x++)
            {
                tex[x, y] = 2;
            }

            width--;
        }       
        width = 3;
        for (int y = textureSize / 2 - 10; y < textureSize / 2; y++)
        {
            for (int x = textureSize / 2 - width; x < textureSize / 2 + width; x++)
            {
                tex[x, y] = 10;
            }

            width++;
        }
        
        for (int y = textureSize / 2; y < textureSize / 2 + 11; y++)
        {
            for (int x = textureSize / 2 - width; x < textureSize / 2 + width; x++)
            {
                tex[x, y] = 10;
            }

            width--;
        } 
        width = 3;
        for (int y = textureSize / 2 - 5; y < textureSize / 2; y++)
        {
            for (int x = textureSize / 2 - width; x < textureSize / 2 + width; x++)
            {
                tex[x, y] = 2;
            }

            width++;
        }
        
        for (int y = textureSize / 2; y < textureSize / 2 + 6; y++)
        {
            for (int x = textureSize / 2 - width; x < textureSize / 2 + width; x++)
            {
                tex[x, y] = 2;
            }

            width--;
        } 
        return tex;
    }
    
    public int [,]  CreateTexture_Boom()
    {
        int [,] tex = new int[textureSize + 1, textureSize + 1];
        
        for (int x = 0; x < textureSize; x++)
        for (int y = 0; y < textureSize; y++)
            tex[x, y] = 10;
        
        float center = textureSize / 2;
        float a = (textureSize / 2) ;
        float b = (textureSize / 2) ;
        for (int x =0; x < textureSize - 1; x++)
        {
            for (int y = 0; y < textureSize - 1; y++)
            {
                float e1 = ((x - center) * (x - center)) / (a * a);
                float e2 = ((y - center) * (y - center)) / (b * b);
                if((e1 + e2) < 0.8)
                    tex[x, y] = 2;
            }
        }
        for (int x =0; x < textureSize - 1; x++)
        {
            for (int y = 0; y < textureSize - 1; y++)
            {
                float e1 = ((x - center) * (x - center)) / (a * a);
                float e2 = ((y - center) * (y - center)) / (b * b);
                if ((e1 + e2) < 0.6)
                    tex[x, y] = 10; //Color.clear;
            }
        }

        return tex;   
    }
}
