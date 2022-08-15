using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;


public class SoundManager: ScriptableObject
{
    public AudioClip mergeSound;
    public AudioClip fagocitaSound;
    //public AudioClip quadSound;
    public AudioClip phaseSound;
    //public AudioClip moveSound;
    public AudioClip explodeSound;

    public AudioSource _audios;
    public AudioSource _audios2;
    
    //AUDIO 
    public float bpmBase,bpm2;

    public float BpmBase
    {
        get => bpmBase;
        set => bpmBase = value;
    }

    public float Bpm2
    {
        get => bpm2;
        set => bpm2 = value;
    }

    public void Init(List<AudioClip> ac, List<AudioSource> s)
    {
        this.mergeSound = ac[0];
        this.fagocitaSound = ac[1];
        this.phaseSound = ac[2];
        this.explodeSound = ac[3];
        
        this._audios = s[0];
        this._audios2 = s[1];
        
        
        _audios.clip = phaseSound;
        _audios.Play();
    }

    public void SetBase(int bpm)
    {
        bpmBase = 114f;
        bpm2 = bpmBase / 4;
        
    }
    
    //stopcoroutines("nome");
}