using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Timer
    {
        private float startTime, endTime, duration;
        private bool running;
        private String name;
        
        public bool Running
        {
            get => running;
            set => running = value;
        }

        public float Duration
        {
            get => duration;
            set => duration = value;
        }

        public Timer(String name)
        {
            this.name = name;
            running = false;
            Debug.Log("Timer " + name + " constructed...");
        }
        
        public int Trigger()
        {
            if (!running)
            {
              startTime = Time.time;  
              running = true;
              return 0;
            }
            Debug.Log("Timer Error: trying Trigger but timer " + name + " still running...");
            return -1;
        }

        public float Stop()
        {
            if (running)
            {
               endTime = Time.time;
               duration = endTime - startTime;
               running = false; 
               return duration;
            }
            else
            {
                Debug.Log("Timer Error: trying to Stop but timer " + name + " not running...");
                return -1f;
            }
        }

        public float Count()
        {
            if(running)
             return (Time.time - startTime);
            else
            {
                Debug.Log("Timer Error: trying Count but timer " + name + " not running...");
                return -1f;
            }
        }

        public void Reset()
        {
            running = false;
            startTime = endTime = duration = -1;
        }
    }
}