using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
 /*
  * One SCHEDULING
  */
 public class Scheduling
 {
  public string method;
  public float secondsToWait;

  public Scheduling(string method, float secondsToWait)
  {
   this.method = method;
   this.secondsToWait = secondsToWait;
  }
 }

 /*
  * SCHEDULER
  */
 public class Scheduler
 {
  public List<Scheduling> schedList = new List<Scheduling>();
  public MonoBehaviour mb;

  public Scheduler(MonoBehaviour mb)
  {
   this.mb = mb;
  }

  public void Refresh()
  {
   if (!IsEmpty())
   {
    int i = 0;
    //calls everybody  -->  UP TO 4 METHODS can be simultaneously scheduled!!!
    foreach (var scheduling in GetSchedules())
    {
     //Invoke(scheduling.method, scheduling.secondsToWait);  <- dont work...
     if (i < 4)
      mb.StartCoroutine("CallMethod" + i, (Scheduling) scheduling);
     i++;
    }

    Clear();
   }
  }

  public void Schedule(Scheduling scheduling)
  {
   schedList.Add(scheduling);
  }

  public Scheduling GetFirst()
  {
   Scheduling rs = null;
   if (schedList.Count > 0)
   {
    rs = schedList[0];
    schedList.RemoveAt(0);
   }

   return rs;
  }

  public List<Scheduling> GetSchedules()
  {
   return schedList;
  }

  public void Clear()
  {
   schedList.Clear();
  }

  public bool IsEmpty()
  {
   return (schedList.Count == 0);
  }
 }

 public Scheduler scheduler;

 /*
  *  GAMEOBJECTS
  */

 public GameObject quadrant;
 public GameObject cell;
 public GameObject wall;
 public GameObject sprite;
 public GameObject floor;
 
 // camera stuff
 public GameObject camera;
 public GameObject camHolder;

 public GameObject povs;  //POVS click zone
 
 public GameObject teste;
 
 /*
  * MATERIALS 
  */

 public Material quadSelect;
 public Material quadUnselect;
 public Material quadTransparent;
 public Material quadTarget;
 public Material quadTarget2;
 public Material quadTarget3;
 public Material creatureSelect;
 public Material transparent;

 private List<Material> materialList = new List<Material>();

 /*
  * SOUND stuff
  */

 public AudioClip mergeSound;
 public AudioClip fagocitaSound;
 public AudioClip phaseSound;

 public AudioClip explodeSound;
 //public AudioClip quadSound;
 //public AudioClip moveSound;

 public AudioSource _audios;
 public AudioSource _audios2;

 private List<AudioClip> audioClipList = new List<AudioClip>();
 private List<AudioSource> audioSourceList = new List<AudioSource>();

 /*
  *  PARTICLE SYSTEMS
  */

 public ParticleSystem ps1;
 public ParticleSystem tps;
 public ParticleSystem[] ps;

 /*
  *  MANAGERS
  */

 private Texturer tx;
 public Universe uni;

 public MoveManager mm;
 public QuadManager qm;
 public SoundManager sm;
 public CameraManager cm;
 public LevelManager lm;
 public PlatformManager pm;

 /*
  * Lists of entities (GameObjects, Creatures, CellInfo)
  */

 private Lists ls = new Lists();


 public Text infoText;


 /*
  * 
  *     START
  * 
  */
 void Start()
 {
  
  
  
  scheduler = new Scheduler(this);

  /*
   *  TEXTURER
   */

  tx = (Texturer) ScriptableObject.CreateInstance("Texturer");
  tx.CreateTexturer();


  /*
   *  SOUND MANAGER
   */

  audioClipList.Add(mergeSound);
  audioClipList.Add(fagocitaSound);
  audioClipList.Add(phaseSound);
  audioClipList.Add(explodeSound);

  audioSourceList.Add(_audios);
  audioSourceList.Add(_audios2);

  sm = (SoundManager) ScriptableObject.CreateInstance("SoundManager");
  sm.Init(audioClipList, audioSourceList);

  /*
   *  PLATFORM MANAGER
   */

  pm = (PlatformManager) ScriptableObject.CreateInstance("PlatformManager");
  pm.Init();

  /*
   *  Generate Universe
   */

  uni = (Universe) ScriptableObject.CreateInstance("Universe");
  uni.Init(quadrant, floor, cell, wall, tx, povs, transparent);

  /*
   *  CAMERA MANAGER
   */

  cm = (CameraManager) ScriptableObject.CreateInstance("CameraManager");
  cm.Init(camera, camHolder, uni);

  /*
   *  QUAD MANAGER
   */

  materialList.Add(quadSelect);
  materialList.Add(quadUnselect);
  materialList.Add(quadTransparent);
  materialList.Add(quadTarget);
  materialList.Add(quadTarget2);
  materialList.Add(quadTarget3);
  materialList.Add(creatureSelect);
  materialList.Add(transparent);

  qm = (QuadManager) ScriptableObject.CreateInstance("QuadManager");
  qm.Init(uni, tx, sm, pm, ls.cells, quadrant, sprite, materialList);


  /*
   *  MOVE MANAGER
   */

  mm = (MoveManager) ScriptableObject.CreateInstance("MoveManager");
  mm.Init(qm, cm, sm, pm, ls, uni, tx, ls.creatures);

  /*
   * LEVEL MANAGER
   */

  ps = new[] {ps1, tps};
  
  lm = (LevelManager) ScriptableObject.CreateInstance("LevelManager");
  lm.Init(scheduler, mm, qm, cm, sm, pm, ls, uni, tx, ls.creatures, ps);


  lm.generateLevel(); //move to lm.Init ?

 // GameObject t = Instantiate(teste);
 
  //t.GetComponent<Quad>().texture = tx.GetTexture("flower1");
  //t.GetComponent<Quad>().texture.alphaIsTransparency = true;
 // t.GetComponent<Renderer>().material.mainTexture = teste.GetComponent<Quad>().texture;
  
  //t.transform.position = new Vector3( 2, 3, -1.5f);
  //t.transform.localScale = new Vector3( 2, 2, 1.0f );
  //teste.GetComponent<MeshRenderer>().material = tx.GetTexture("flower1");
  
  
  infoText.text = "versão: 0.11 - 30.04.21 - deltaTime: " + Time.deltaTime.ToString();
 }

 /*
  *     UPDATE
  */
 void Update()
 {
  scheduler.Refresh();

  mm.Refresh();
 }

/*
 * COROUTINES from scheduler
 */
 IEnumerator CallMethod0(Scheduling scheduling)
 {
  yield return new WaitForSeconds(scheduling.secondsToWait);
  DoTheCall(scheduling.method);
 }

 IEnumerator CallMethod1(Scheduling scheduling)
 {
  yield return new WaitForSeconds(scheduling.secondsToWait);
  DoTheCall(scheduling.method);
 }

 IEnumerator CallMethod2(Scheduling scheduling)
 {
  yield return new WaitForSeconds(scheduling.secondsToWait);
  DoTheCall(scheduling.method);
 }

 IEnumerator CallMethod3(Scheduling scheduling)
 {
  yield return new WaitForSeconds(scheduling.secondsToWait);
  DoTheCall(scheduling.method);
 }

 public void DoTheCall(string method)
 {
  switch (method)
  {
   case "generateTargets":
    lm.generateTargets();
    break;
  }
 }
}
