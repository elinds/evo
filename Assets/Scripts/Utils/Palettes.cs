
using UnityEngine;

public class Palettes
{
    private Color32[,] colors;

    public Palettes()
    {
        colors = new [,]
    {
        {
            (Color32)Color.black, (Color32)Color.blue, (Color32)Color.grey, (Color32)Color.cyan, (Color32)Color.gray, (Color32)Color.green, (Color32)Color.magenta,
            (Color32)Color.red, (Color32)Color.white, (Color32)Color.yellow, (Color32)Color.clear
        },
        {
            new Color32(12,15,10,1), new Color32(134,24,60,1), new Color32(255,32,110,1), new Color32(253,144,64,1), new Color32(251,255,18,1),
            new Color32(205,250,67, 1), new Color32(158,245,115,1), new Color32(65,234,212,1 ), new Color32(160,245,234,1 ), new Color32(255,255,255,1),  (Color32)Color.clear
        },
        {
            new Color32(23,126,137,1), new Color32(16,101,117,1), new Color32(8,76,97,1), new Color32(114,67,75,1), new Color32(167,63,64,1),
            new Color32(219,58,52,1), new Color32(237,129,70,1), new Color32(255,200,87,1), new Color32(153,124,68,1), new Color32(50,48,49,1), (Color32)Color.clear
        },
        {
            new Color32(238,96,85,1), new Color32(203,125,101,1), new Color32(167,154,117,1), new Color32(96,211,148,1), new Color32(133,229,140,1),
            new Color32(170,246,131,1), new Color32(213,232,128,1), new Color32(255,217,125,1), new Color32(255,186,129,1), new Color32(255,155,133,1), (Color32)Color.clear
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        },
        {
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1),
            new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,1), new Color32(0,0,0,0)
        }
        
      };
    }

    public Color32[] getPalette(int index)
    {
        Color32[] arrColor32 = new Color32[11];
        for (int i = 0; i < 11; i++)
            arrColor32[i] = colors[index, i];
        return arrColor32;
    }
}
