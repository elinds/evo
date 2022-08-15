
using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    
    public delegate void Del(params  object[] parmList);

    
    /*
     * TRANSITION
     */
    
    public class Trans
    {

        public String DestinyStateName
        {
            get => destinyStateName;
            set => destinyStateName = value;
        }
        
        public string TransitionName
        {
            get => transitionName;
            set => transitionName = value;
        }

        public bool Inconditional
        {
            get => inconditional;
            set => inconditional = value;
        }

        private String destinyStateName;
        private String transitionName;
        private bool inconditional;
        
        public Trans(String destinyStateName, String transitionName, bool inconditional = false)
        {
            this.destinyStateName = destinyStateName;
            this.transitionName = transitionName;
            this.inconditional = inconditional;
        }
    }

    /*
     * STATE
     */
    
    public class State
    {
        private String name;
        private Trans[] transitions;
        private bool isCurrent;
        private Del callbackFromState;


        public String Name
        {
            get => name;
            set => name = value;
        }

        public Trans[] Transitions
        {
            get => transitions;
            set => transitions = value;
        }

        public bool IsCurrent
        {
            get => isCurrent;
            set => isCurrent = value;
        }

        public Del CallbackFromState
        {
            get => callbackFromState;
            set => callbackFromState = value;
        }
        public State(String name, Trans[] transitions, Del callbackFromState, bool isCurrent = false)
        {
            this.name = name;
            this.transitions = transitions;
            this.isCurrent = isCurrent;
            this.callbackFromState = callbackFromState;
        }
    }
    
    
    // List of States
    private List<State> states = new List<State>();
    
    // Current State
    private State currState = null;


    public Boolean debugMode = false;

    private String FSMname;
    
    /*
     * Public methods
     */
    
    public void AddState(String name, Trans[] transList, Del callback )
    {
        states.Add(new State(name, transList, callback));
    }

    public State GetState(String name)
    {
        foreach (var s in states)
        {
            if (s.Name == name)
            {
                return s;
            }
        }

        return null;
    }

    public State SetCurrent(String name)
    {
        State curr = null;
         foreach (var s in states)
         {
             if (s.Name == name)
             {
                 s.IsCurrent = true;
                 curr = s;
             }
             else
             {
                 s.IsCurrent = false;
             }
         }

         return curr;
    }

    public String GetCurrentStateName()
    {
        return currState.Name;
    }

    public void Trigger(String transitionName, params  object[] parmList)
    {
        foreach (var t in currState.Transitions)
        {
            if (t.TransitionName == transitionName)
            {
                GotoNextState(t, parmList);
                CheckInconditional();
                return;
            }
        }
        Debug.Log("FSM: Move-Doing Nothing: no transition(" + transitionName + " in state " + currState.Name + "! +++ +++ +++");
    }

    // DO NOT SUPPORT MULTIPLE (in sequence) INCONDITIONAL transitions!
    private void CheckInconditional()
    {
        if (currState.Transitions.Length == 1)
        {
            if ((currState.Transitions)[0].Inconditional)  //true
            {
                GotoNextState(currState.Transitions[0], null);
            }
        }
    }

    private void GotoNextState(Trans t, params  object[] parmList)
    {
        currState.IsCurrent = false;
        currState = GetState(t.DestinyStateName);
        currState.IsCurrent = true;
        
        if(debugMode)
            Debug.Log("FSM " + FSMname + " @ state " + currState.Name + " <<<<<<");
        
        if(currState.CallbackFromState !=  null)
            currState.CallbackFromState(parmList);        
    }
    
    public void SetStart(String name)
    {
        currState = SetCurrent(name);
    }
    
     /*
      *
      * FSM
      * 
      */

     public FSM(String FSMname)
     {
         this.FSMname = FSMname;

     } 
}
