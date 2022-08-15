using UnityEngine;

namespace DefaultNamespace
{
    public class Keyboard: ScriptableObject
    {        
        public enum KeyState
        {
            Idle = 0,
            //directionals
            W = 10,
            A = 11,
            S = 12,
            D = 13,
            //others
            E = 14,
            Q  = 15,
            Z  = 16,
            //digits
            One = 30,
            Two = 31,
            Three = 32,
            //specials
            ESC = 55,
            SPACE = 56,
            ENTER = 57,
            PLUS = 58,
            MINUS = 59,
            TAB = 60,
            CtrlLEFT = 61,
            CtrlRIGHT = 62,
            //directionals
            ArrowLeft = 90,
            ArrowRight = 91,
            ArrowUp = 92,
            ArrowDown = 93
        }
        
        private KeyState currState;

        public KeyState CurrState
        {
            get => currState;
            set => currState = value;
        }

        public delegate void Del(KeyState ms);
        public Del callbackKeyStateChanged;
        
        public void Init()
        {
            currState = KeyState.Idle;
        }
        
        public void Refresh()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                currState = KeyState.SPACE;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                currState = KeyState.ENTER;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currState = KeyState.ESC;
                callbackKeyStateChanged(currState);
            }
            // +
            if (Input.GetKeyDown(KeyCode.Plus))
            {
                currState = KeyState.PLUS;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                currState = KeyState.PLUS;
                callbackKeyStateChanged(currState);
            }  
            // -
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                currState = KeyState.MINUS;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                currState = KeyState.MINUS;
                callbackKeyStateChanged(currState);
            }
            
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                currState = KeyState.TAB;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                currState = KeyState.CtrlLEFT;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                currState = KeyState.CtrlRIGHT;
                callbackKeyStateChanged(currState);
            }            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currState = KeyState.ArrowUp;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                currState = KeyState.ArrowDown;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                currState = KeyState.ArrowRight;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                currState = KeyState.ArrowLeft;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                currState = KeyState.W;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                currState = KeyState.A;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                currState = KeyState.S;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                currState = KeyState.D;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                currState = KeyState.Q;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                currState = KeyState.Z;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                currState = KeyState.E;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                currState = KeyState.One;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                currState = KeyState.One;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                currState = KeyState.Two;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                currState = KeyState.Two;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                currState = KeyState.Three;
                callbackKeyStateChanged(currState);
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                currState = KeyState.Three;
                callbackKeyStateChanged(currState);
            }
        }
    }
}