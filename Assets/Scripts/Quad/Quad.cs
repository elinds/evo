using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Quad : MonoBehaviour
{
    public enum QuadTypes
    {
        None = 10,         //quad is a new born board quad 
        Empty = 11,
        Cell = 12,         //quad has a cell
        Target = 13,       //quad has a target cell
        Rotor  = 14,       //quad IS an active quad rotor
        Boom = 15,         //quad IS an active quad boom
        Portal = 16,       //quad IS an active quad portal
        Sprite = 50,
        QuadTarget = 99    //quad IS an quad target
    }

    public enum QuadState
    {
        None = 0,
        Borning = 1,
        Active = 2
    }
    
    public QuadTypes thisQuadType;

    public QuadState state;
    
    
    /*
     *
     *  ACTIVE QUADS info
     * 
     */

    class QuadNone
    {
        public float scaleCntr;
        public bool started;

        public QuadNone()
        {
            scaleCntr = 0;
            started = false;
        }
        public void reset()
        {
            scaleCntr = 0f;
            started = false;
        }
    }
    class QuadBoom
    {
        public float scaleCntr;
        public bool active;

        public QuadBoom()
        {
            scaleCntr = 0;
            active = true;
        }
        public void reset()
        {
            scaleCntr = 0f;
            this.active = true;
        }
    }

    class QuadRotor
    {
        
    }

    class QuadTarget
    {       
        public bool targetSelected;
        public int transparency;
        private Color32 targetColor;

        public QuadTarget()
        {
            targetSelected = false;
            transparency = 160;
        }
    }

    private QuadNone qn = null;
    private QuadBoom qb = null;
    private QuadRotor qr = null;
    private QuadTarget qt = null;

    public Texture2D texture;
    
    private System.Random rnd;
    
    /*
     *  bridge to Cell info
     */
    
    private Cell cell;

    public Cell Cell
    {
        get => cell;
        set => cell = value;
    }

    /*
     *  Bridges to Managers 
     */
    
    public SoundManager sm;
    public QuadManager qm;
   
    /*
     * Quad position
     */
    public int x, y;

    public float quadVelocity;
    /*
     * 
     * Quad initialization
     * 
     */
    void Start()
    {
        switch (thisQuadType)
        {
            case QuadTypes.Boom:
                qb = new QuadBoom();
                state = QuadState.Active;
                StartCoroutine("SynchonizeBase");
                break;
            case QuadTypes.Rotor:
                qr = new QuadRotor();
                state = QuadState.Active;
                break;
            case QuadTypes.QuadTarget:
                qt = new QuadTarget();
                state = QuadState.Active;
                StartCoroutine("Pulsate");
                break;
            case QuadTypes.None:
                qn = new QuadNone();
                state = QuadState.Borning;
                StartCoroutine("WaitToStart");
                transform.localScale = new Vector3( 0.0f, 0.0f, 1.0f );
                break;
        }

    }
    void Update()
    {

        switch (state)
        {
            case QuadState.Borning:
                if (qn.started)
                {
                    qn.scaleCntr += quadVelocity;
                    transform.localScale = new Vector3( qn.scaleCntr, qn.scaleCntr, 1.0f );

                    if (qn.scaleCntr >= 1.0)
                    {
                        qn.scaleCntr = 0f;
                        state = QuadState.Active;
                        //transform.localScale = new Vector3( 0f, 0f, 0f );
                    }                    
                }

                break;
            case QuadState.Active:
                break;
        }
        
    }
    void FixedUpdate()
    {
        switch (thisQuadType)
        {
            case QuadTypes.Rotor:
                transform.Rotate( new Vector3(0,0,-90) * Time.deltaTime);
                break;
            case QuadTypes.Boom:
                if(qb.active) //(scaleCntr == 0 && reseted)
                {
                    if (qb.scaleCntr == 0)
                    {
                        transform.localScale = new Vector3( 0.1f, 0.1f, 1.0f );
                        qb.scaleCntr += 0.05f;  
                    }
                    else
                    {
                        qb.scaleCntr+= 0.1f; //0.05f
                        transform.localScale = new Vector3( 0.1f + qb.scaleCntr, 0.1f + qb.scaleCntr, 1.0f );
                    }
                    if (qb.scaleCntr >= 0.85)
                    {
                        qb.scaleCntr = 0f;
                        qb.active = false;
                        transform.localScale = new Vector3( 0f, 0f, 0f );
                    }
                }
                break;
            case QuadTypes.Target:
                //this.GetComponent<MeshRenderer>().material.color;
                break;
        }
    }
    
    public void SetX(int x)
    {
        this.x = x;
    }
    public void SetY(int y)
    {
        this.y = y;
    }

    public int GetX()
    {
        return x;
    }

    public int GetY()
    {
        return y;
    }


    public void reset2()
    {
        //scaleCntr = 0f;
        //this.active = true;
    }
    
    IEnumerator SynchonizeBase()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f / sm.bpmBase);
            qb.reset();
        }
    }

    IEnumerator Synchonize2()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f / sm.bpm2);
            //ls.actives[1].GetComponent<Quad>().reset2();
        }
    }
    
    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(0.02f + (Random.value) * 0.6f);
        qn.started = true;
    }
    IEnumerator Pulsate()
    {
        byte transpLevel = 0;


        while (true)
        {
            this.GetComponent<MeshRenderer>().material = qm.quadTargetTransp[transpLevel++];

            if (transpLevel == 3)
                transpLevel = 0;

            /*
             *
             * Dont deactive because it will stop coroutine!
             * qm.selectedQuadTarget.SetActive(qm.selectedQuadTargetVisible);
             */
            
            yield return new WaitForSeconds(0.3f); //5 beats per second
        }
    }
    
    //stopcoroutines("nome");
    
}
