using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Mouse: ScriptableObject
    {
        public enum MouseState
        {
            Idle = 0,
            SingleClickLeft = 11,
            SingleClickRight = 12,
            DoubleClickLeft = 13,
            DoubleClickRight  = 14,
            HoldLeft = 15,
            HoldRight = 16,
            HoldEnd = 17,
            FirstLeftCLickDone = 20,
            FirstRightCLickDone = 21,
            Wheel_In = 25,
            Wheel_Out = 26,
            PreHold = 31,
            ErrorState = 99
        }

        public enum MouseCollision
        {
            None = 0,
            Quad = 1,
            Cube = 2,
            Floor = 3,
            Wall = 4,
            NPC = 5,
            Symbol = 6,
            UX_Rot = 7 //Rotation Bars (UX) |_|
        }

        public class MouseCollisionInfo
        {
            public MouseCollision CurrCollision
            {
                get => currCollision;
                set => currCollision = value;
            }

            public int CurrCollisionX
            {
                get => currCollisionX;
                set => currCollisionX = value;
            }

            public int CurrCollisionY
            {
                get => currCollisionY;
                set => currCollisionY = value;
            }

            public int CurrCellId
            {
                get => currCellId;
                set => currCellId = value;
            }

            public int CurrCreatureId
            {
                get => currCreatureId;
                set => currCreatureId = value;
            }

            private MouseCollision currCollision;
            private int currCollisionX;
            private int currCollisionY;
            private int currCellId; 
            private int currCreatureId;
            
        }
        
        private float doubleClickMinThreshold = 0.35f;
        private float holdMinThreshold = 0.25f; //0.18f;  0.22f
        
                        
        private bool firstClickDone;                   //TODO: firstLeftClickDone +  firstRightClickDone 
        
        private Timer timerHold, timerDoubleClick;     //TODO: separate timers for left and right   

        private MouseState currState;

        public MouseState CurrState
        {
            get => currState;
            set => currState = value;
        }
        
        
        /*
         *
         * Position info
         * 
         */

        private float mousePosX, mousePosY;

        public float MousePosX
        {
            get => mousePosX;
            set => mousePosX = value;
        }

        public float MousePosY
        {
            get => mousePosY;
            set => mousePosY = value;
        }
        
        
        public delegate void Del(MouseState ms);

        public Del callbackMouseStateChanged;
        
        
        /*
         *
         * Collision info
         * 
         */
        
        private Ray ray;
        public MouseCollisionInfo mci = new MouseCollisionInfo();
        
        /*
         *
         * Initializing mouse
         * 
         */
        public void Init()
        {
            timerDoubleClick = new Timer("DoubleClick");
            timerHold = new Timer("Hold");
            
            currState = MouseState.Idle;
            firstClickDone = false;
            
            
        }

        
        public void Refresh()
        { 
            float holdDuration;


            /*
             * MOUSE WHEEL processing
             *
             * mouse scrollwheel or two-finger slide on a Mac trackpad
             * 
             */
            
            float wheel = Input.mouseScrollDelta.y;
            if (wheel != 0)
            {
                if (wheel > 0)
                {
                    //IN
                    currState = MouseState.Wheel_In;
                }
                else
                {
                    //OUT 
                    currState = MouseState.Wheel_Out;
                }
                callbackMouseStateChanged(currState);
                //SetIdle()                              ????????????    <<<<<<<<<<<<<<<<<<<<<<<<
                // apos chamada a todos callbacks, fazer mouse idle?
            }
            
            
            
            /*
             *  UP BUTTON detection
             */

            
            //it may occur that AFTER button down, mouse state turns idle, so subsequent mouse Up must be ignored
            //thats the case when (while trying moving cell) player selects same origin & dest positions, instead
            //different ones
            
            if (currState != MouseState.Idle)   
            {
                if (Input.GetMouseButtonUp(0)) // LEFT  up
                {
                    timerHold.Stop();
                    if (currState == MouseState.HoldLeft)
                    {
                        if (timerHold.Duration <= holdMinThreshold)
                        {
                            if (!firstClickDone)
                            {
                                firstClickDone = true;
                                currState = MouseState.FirstLeftCLickDone;
                                callbackMouseStateChanged(currState);
                            }
                            else
                            {
                                timerDoubleClick.Stop();
                                currState = MouseState.DoubleClickLeft;
                                callbackMouseStateChanged(currState);
                            }
                        }
                        else
                        {
                            currState = MouseState.HoldEnd;
                            timerDoubleClick.Stop();
                            callbackMouseStateChanged(currState);
                        }
                    }
                    else
                    {
                        Debug.Log("(2) Wrong mouse state: Up but previously " + currState);
                    }
                }

                if (Input.GetMouseButtonUp(1)) // RIGHT  up
                {
                    timerHold.Stop();
                    if (currState == MouseState.HoldRight)
                    {
                        if (timerHold.Duration <= holdMinThreshold)
                        {
                            if (!firstClickDone)
                            {
                                firstClickDone = true;
                                currState = MouseState.FirstRightCLickDone;
                                callbackMouseStateChanged(currState);
                            }
                            else
                            {
                                timerDoubleClick.Stop();
                                currState = MouseState.DoubleClickRight;
                                callbackMouseStateChanged(currState);
                            }
                        }
                        else
                        {
                            currState = MouseState.HoldEnd;
                            timerDoubleClick.Stop();
                            callbackMouseStateChanged(currState);
                        }
                    }
                    else
                    {
                        Debug.Log("(3) Wrong mouse state: Up but previously " + currState);
                    }
                }
            }
      
            /*
             *  TIME PROGRESSION DETECTION
             */
            
            if (currState == MouseState.FirstLeftCLickDone)  
            {
                if (timerDoubleClick.Count() > doubleClickMinThreshold)
                {
                    timerDoubleClick.Stop();
                    firstClickDone = false;
                    currState = MouseState.SingleClickLeft;
                    callbackMouseStateChanged(currState);
                }
                else
                {
                    currState =  MouseState.FirstLeftCLickDone;  //stays in that state...
                }
            }
            if (currState == MouseState.FirstRightCLickDone)  
            {
                if (timerDoubleClick.Count() > doubleClickMinThreshold)
                {
                    timerDoubleClick.Stop();
                    firstClickDone = false;
                    currState = MouseState.SingleClickRight;
                    callbackMouseStateChanged(currState);
                }
                else
                {
                    currState =  MouseState.FirstRightCLickDone;  //stays in that state...
                }
            }           
            /*
             *  DOWN BUTTON DETECTION
             */       
            
            if(Input.GetMouseButtonDown(0))  // LEFT  Down               
            {
                if (currState == MouseState.Idle)
                {
                    timerDoubleClick.Trigger();
                    timerHold.Trigger();
                    firstClickDone = false;
                    currState = MouseState.HoldLeft;
                    callbackMouseStateChanged(currState);
                }
                else
                {
                    if (currState == MouseState.FirstLeftCLickDone)
                    {
                        timerHold.Trigger();
                        firstClickDone = true;
                        currState = MouseState.HoldLeft;
                        callbackMouseStateChanged(currState);
                    }
                    else
                    {
                        Debug.Log("(1) Wrong mouse state: Down but previously " + currState);
                    }  
                }
            }
            
            if(Input.GetMouseButtonDown(1)) // RIGHT  Down
            {
                if (currState == MouseState.Idle)
                {
                    timerDoubleClick.Trigger();
                    timerHold.Trigger();
                    firstClickDone = false;
                    currState = MouseState.HoldRight;
                    callbackMouseStateChanged(currState);
                }
                else
                {
                    if (currState == MouseState.FirstRightCLickDone)
                    {
                        timerHold.Trigger();
                        firstClickDone = true;
                        currState = MouseState.HoldRight;
                        callbackMouseStateChanged(currState);
                    }
                    else
                    {
                        Debug.Log("(0) Wrong mouse state: Down but previously " + currState);
                    }  
                }
               
            }
        }
        
        
        /*
         * Old Mouse FSM: with Pre-Hold State
         */
        
        
        /*
        public void Refresh()
        { 
            float holdDuration;
            
            /*
             *  UP BUTTON detection
             #1#

            if (Input.GetMouseButtonUp(0)) // LEFT  up
            {
                if (currState == MouseState.HoldLeft)
                {
                    currState = MouseState.HoldEnd;
                    timerDoubleClick.Stop();
                    callbackMouseStateChanged(currState);
                }
                else
                {
                    if (currState == MouseState.PreHold)
                    {
                        timerHold.Stop();
                        if (!firstClickDone)
                        {
                            firstClickDone = true;
                            currState = MouseState.FirstCLickDone;
                            callbackMouseStateChanged(currState);
                        }
                        else
                        {
                            timerDoubleClick.Stop();
                            currState = MouseState.DoubleClickLeft;
                            callbackMouseStateChanged(currState);                           
                        }
                    }
                    else
                    {
                        Debug.Log("(2) Wrong mouse state: Up but previously " + currState);
                    }
                }
            }

            
            /*
             *  TIME PROGRESSION DETECTION
             #1#
            
            if (currState == MouseState.FirstCLickDone)  
            {
                if (timerDoubleClick.Count() > doubleClickMinThreshold)
                {
                    timerDoubleClick.Stop();
                    firstClickDone = false;
                    currState = MouseState.SingleClickLeft;
                    callbackMouseStateChanged(currState);
                }
                else
                {
                    currState =  MouseState.FirstCLickDone;  //stays in that state...
                }
            }
            
            if (currState == MouseState.PreHold)  
            { 
                if (timerHold.Count() > holdMinThreshold)
                {
                   timerHold.Stop();
                   currState = MouseState.HoldLeft;
                   callbackMouseStateChanged(currState);
                }
            }
            

            /*
             *  DOWN BUTTON DETECTION
             #1#       
            
            if(Input.GetMouseButtonDown(0))  // LEFT                 
            {
                if (currState == MouseState.Idle)
                {
                    timerDoubleClick.Trigger();
                    timerHold.Trigger();
                    firstClickDone = false;
                    currState = MouseState.PreHold;
                }
                else
                {
                    if (currState == MouseState.FirstCLickDone)
                    {
                        timerHold.Trigger();
                        firstClickDone = true;
                        currState = MouseState.PreHold;
                    }
                    else
                    {
                        Debug.Log("(1) Wrong mouse state: Down but previously " + currState);
                    }  
                }
            }
            
            if(Input.GetMouseButtonDown(1)) // RIGHT
            {
               
            }
        }
        */

        public void SetIdle()
        {
            currState = MouseState.Idle;
            timerHold.Reset();
            timerDoubleClick.Reset();
            ClearCollisionState();
        }

        public void GetMousePosition()
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    mousePosX = hit.collider.gameObject.transform.position.x;
                    mousePosY = hit.collider.gameObject.transform.position.y;

                    
                    Debug.Log("colliding with:" + hit.collider.name);
                    /*if (currPosX != initialPosX || currPosY != initialPosY)
                    {
                        newPos = true;
                    }*/
                }
            } 
        }
        public void GetCollisionState()
        {
            ClearCollisionState();
            
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);  //POSSIBLE NULL REF
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    Debug.Log("################# colliding with: " + hit.collider.name);
                    switch (hit.collider.name)
                    {
                        case "Quad(Clone)":
                            mci.CurrCollision = MouseCollision.Quad;
                            mci.CurrCollisionX = hit.collider.gameObject.GetComponent<Quad>().x;
                            mci.CurrCollisionY = hit.collider.gameObject.GetComponent<Quad>().y;
                            //currCellId = -2; //to inform ChangeCellMode/Select that a QUAD was selected...                           
                            break;
                        case "Cube(Clone)":
                            mci.CurrCollision = MouseCollision.Cube;
                            mci.CurrCollisionX = hit.collider.gameObject.GetComponent<CellScript>().X;
                            mci.CurrCollisionY = hit.collider.gameObject.GetComponent<CellScript>().Y;
                            mci.CurrCellId = hit.collider.gameObject.GetComponent<CellScript>()._cell.Id;
                            mci.CurrCreatureId = hit.collider.gameObject.GetComponent<CellScript>()._cell.CreatureId;
                            break;
                        case "QuadFloor(Clone)":
                            mci.CurrCollision = MouseCollision.Floor;
                            break;
                        case "Wall(Clone)":
                            mci.CurrCollision = MouseCollision.Wall;
                            break;
                        default:
                            mci.CurrCollision = MouseCollision.None;
                            Debug.Log("\n\n_-_-_-_-_-_-_-_-_-_-  this type of mouse collision not yet supported -_-_-_-_-_-_-_\n\n");
                            Debug.Log("colliding with:" + hit.collider.name);
                            break;
                    }
                }
                else  //NOT COLLIDING!!!
                {
                    Debug.Log("GetCollisionState: not colliding...");
                }
            }
        }

        private void ClearCollisionState()
        {
            mci.CurrCollision = MouseCollision.None;
            mci.CurrCollisionX = -1;
            mci.CurrCollisionY = -1;
            mci.CurrCellId = -1;
            mci.CurrCreatureId = -1;
        }
    }
    
}