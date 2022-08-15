
using System;
using System.Collections.Generic;
using UnityEngine;

public class CellLevel
{
   [Flags]
   public enum Side
   {
      //using this configuration, to be compatible w/ CellInfo.DirectionMode
      
      None = 0,      
      Left = 1,
      Right = 2,
      Up = 3,
      Down = 4,
      All = 5 
      
      /*Right = 0,
      Left  = 1,
      Up = 2,
      Down = 3,
      All = 4,
      None = 5*/
   }
   public List<CellQuadrant> QuadrantsList;
   
   private int _textureSize;

   public CellLevel()
   {

   }
   public CellLevel(int textureSize)
   {
      QuadrantsList = new List<CellQuadrant>();
      _textureSize = textureSize;
   }

   public void SetQuadrant(int index, CellQuadrant cq)
   {
      QuadrantsList[index] = cq;
   }

   public void AddHorizontalQuadrant(CellQuadrant cq, Side side)
   {
      QuadrantsList.Add(cq);
      
      int distance;
      
      //FIRST INSERTION
      if (QuadrantsList.Count == 1)
      {
         cq.StartX = 0;
         cq.SizeX = _textureSize;
         cq.StartY = 0;
         cq.SizeY = _textureSize;
      }
      else
      {
          if (side == Side.Right)
          {            
             for (int i = 0; i < QuadrantsList.Count - 1; i++)
             {
                distance = (QuadrantsList[i].StartX) / 2;
                QuadrantsList[i].StartX -= distance;
                QuadrantsList[i].SizeX /= 2;
                if (QuadrantsList[i].SizeX == 0) QuadrantsList[i].SizeX = 1;
             }
             QuadrantsList[QuadrantsList.Count - 1].SizeX = _textureSize / 2;
             QuadrantsList[QuadrantsList.Count - 1].StartX = _textureSize / 2;
             QuadrantsList[QuadrantsList.Count - 1].SizeY = _textureSize;
             QuadrantsList[QuadrantsList.Count - 1].StartY = 0;            
          }
          else  //Side.Left
          {
             for (int i = 0; i < QuadrantsList.Count - 1; i++)
             {
                distance = (_textureSize - QuadrantsList[i].StartX) / 2;
                QuadrantsList[i].StartX += distance;
                QuadrantsList[i].SizeX /= 2;
                if (QuadrantsList[i].SizeX == 0) QuadrantsList[i].SizeX = 1;
             }
             QuadrantsList[QuadrantsList.Count - 1].SizeX = _textureSize / 2;
             QuadrantsList[QuadrantsList.Count - 1].StartX = 0;
             QuadrantsList[QuadrantsList.Count - 1].SizeY = _textureSize;
             QuadrantsList[QuadrantsList.Count - 1].StartY = 0;    
          }     
      }

   }
   public void AddVerticalQuadrant(CellQuadrant cq, Side side)
   {
      QuadrantsList.Add(cq);
      int distance;
      
      //FIRST INSERTION
      if (QuadrantsList.Count == 1)
      {
         cq.StartX = 0;
         cq.SizeX = _textureSize;
         cq.StartY = 0;
         cq.SizeY = _textureSize;
      }
      else
      {
         if (side == Side.Up)
         {
            //Debug.Log("addver up");
            for (int i = 0; i < QuadrantsList.Count - 1; i++)
            {
               distance = (_textureSize - QuadrantsList[i].StartY) / 2;
               QuadrantsList[i].StartY += distance;
               QuadrantsList[i].SizeY /= 2;
               if (QuadrantsList[i].SizeY == 0) QuadrantsList[i].SizeY = 1;
            }

            QuadrantsList[QuadrantsList.Count - 1].SizeX = _textureSize;
            QuadrantsList[QuadrantsList.Count - 1].StartX = 0;
            QuadrantsList[QuadrantsList.Count - 1].SizeY = _textureSize/2;
            QuadrantsList[QuadrantsList.Count - 1].StartY = 0;            
         }
         else  //Side.Down
         {
            //Debug.Log("addver down");
            for (int i = 0; i < QuadrantsList.Count - 1; i++)
            {
               distance = (QuadrantsList[i].StartY) / 2;
               QuadrantsList[i].StartY -= distance;
               QuadrantsList[i].SizeY /= 2;
               if (QuadrantsList[i].SizeY == 0) QuadrantsList[i].SizeY = 1;
            }

            QuadrantsList[QuadrantsList.Count - 1].SizeY = _textureSize / 2;
            QuadrantsList[QuadrantsList.Count - 1].StartY = _textureSize / 2;
            QuadrantsList[QuadrantsList.Count - 1].SizeX = _textureSize;
            QuadrantsList[QuadrantsList.Count - 1].StartX = 0;   
         }     
      }
   }

   public List<CellQuadrant> getQuadrants()
   {
      return QuadrantsList;
   }

   public CellQuadrant getLastQuadrant()
   {
      return QuadrantsList[QuadrantsList.Count - 1];
   }
}
