
using System;
using System.Collections.Generic;
using UnityEngine;

public class CellQuadrant
{

    public List<CellLevel> refLevels;
        
    [Flags]
    public enum Shape
    {
        None = 0,
        Ellipse  = 1,
        Triangle = 2,
        Rectangle = 3
    }

    private Shape _shape;
    private int _triangleType;
    public int TriangleType
    {
        get => _triangleType;
        set => _triangleType = value;
    }

    private int _backColor, _foreColor;
    
    //Geometry
    
    private int _startX, _sizeX, _startY, _sizeY;

    public int StartX
    {
        get => _startX;
        set => _startX = value;
    }

    public int SizeX
    {
        get => _sizeX;
        set => _sizeX = value;
    }

    public int StartY
    {
        get => _startY;
        set => _startY = value;
    }

    public int SizeY
    {
        get => _sizeY;
        set => _sizeY = value;
    }


    //Colors accessors
    
    public void setColorBack(int index)
    {
        _backColor = index;
    }
    public void setColorFore(int index)
    {
        _foreColor = index;
    }

    public int getBackColor()
    {
        return _backColor;
    }
    public int getForeColor()
    {
        return _foreColor;
    }
    
    //Shape accessors
    
    public void setShape(Shape shape)
    {
        _shape = shape;
    }
    public Shape getShape()
    {
        return _shape;
    }

    public CellQuadrant( int indexBack, int indexFore, Shape shape)
    {
        _shape = shape;
        _backColor = indexBack;
        _foreColor = indexFore;
        refLevels = null;
    }
    public CellQuadrant( int indexBack, int indexFore, Shape shape, List<CellLevel> cl)
    {
        _shape = shape;
        _backColor = indexBack;
        _foreColor = indexFore;
        refLevels = cl;
    }
    public CellQuadrant( int indexBack, int indexFore, Shape shape, int triagleType)
    {
        _shape = shape;
        _backColor = indexBack;
        _foreColor = indexFore;
        _triangleType = triagleType;
        refLevels = null;
    }
    public CellQuadrant( int indexBack, int indexFore, Shape shape, int triagleType, List<CellLevel> cl)
    {
        _shape = shape;
        _backColor = indexBack;
        _foreColor = indexFore;
        _triangleType = triagleType;
        refLevels = null;
        refLevels = cl;
    }   
}
