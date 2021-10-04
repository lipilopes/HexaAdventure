using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

   [SerializeField] RespawMob respawmob;

    WaitForSeconds waitUpdate = new WaitForSeconds(0.5f);

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        if (respawmob == null)
            respawmob = RespawMob.Instance;
    }

    public void StartPollEffects()
    {
        return;


        //if (respawmob==null)
        //respawmob = RespawMob.Instance;

        //#region Effect
        //if (turnEffect==null)
        //{
        //    turnEffect = Instantiate(turnEffectPrefab);
        //    turnEffect.transform.SetParent(this.transform);
        //    respawmob.allRespaws.Add(turnEffect);
        //}
        //#endregion

        //#region Fire Pool
        //fireList = new GameObject[fireSize];

        //for (int i = 0; i < fireList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(firePrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Fire " + i;
        //    fireList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion    

        //#region Poison Pool
        //poisonList = new GameObject[poisonSize];

        //for (int i = 0; i < poisonList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(poisonPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Poison " + i;
        //    poisonList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion    

        //#region Stun Pool
        //stunList = new GameObject[stunSize];

        //for (int i = 0; i < stunList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(stunPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Stun " + i;
        //    stunList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion    

        //#region Hit Pool
        //hitList = new GameObject[hitSize];
        //hitAtived = new bool[hitSize];

        //for (int i = 0; i < hitList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(hitPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Hit " + i;
        //    hitList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion    

        //#region HitBlock Pool
        //hitBlockList = new GameObject[hitBlockSize];
        //hitBlockAtived = new bool[hitBlockSize];

        //for (int i = 0; i < hitBlockList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(hitBlockPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "HitBlock " + i;
        //    hitBlockList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion

        //#region Critical Hit Pool
        //crithitList = new GameObject[crithitSize];
        //crithitAtived = new bool[crithitSize];

        //for (int i = 0; i < crithitList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(crithitPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Critical Hit " + i;
        //    crithitList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion    

        //#region PopUpDamage Pool
        //popUpDamageList = new GameObject[popUpDamageSize];
        ////popUpDamageAtived = new float[popUpDamageSize];

        //for (int i = 0; i < popUpDamageList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(popUpDamagePrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name          = "PopUp Damage " + i;
        //    popUpDamageList[i]   = prefab;
        //    //popUpDamageAtived[i] = -1;
        //    prefab.transform.SetParent(this.transform,false);
        //    prefab.SetActive(false);
        //}
        //#endregion

        //#region Effect
        //targetList = new GameObject[targetSize];

        //for (int i = 0; i < targetList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(targetPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Target " + i;
        //    targetList[i] = prefab;
        //    prefab.transform.SetParent(this.transform, false);
        //    prefab.SetActive(false);
        //}
        //#endregion

        //#region ShowEnemy Pool
        //showEnemyList = new GameObject[showEnemySize];
        ////showEnemyAtived = new float[showEnemySize];

        //for (int i = 0; i < showEnemyList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(showEnemyPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Show Enemy " + i;
        //    showEnemyList[i] = prefab;
        //    //showEnemyAtived[i] = -1;
        //    prefab.transform.SetParent(this.transform, false);
        //    prefab.SetActive(false);
        //}
        //#endregion

        //#region Dead Pool
        //deadList   = new GameObject[deadSize];
        //deadAtived = new bool[deadSize];

        //for (int i = 0; i < deadList.Length; i++)
        //{
        //    GameObject prefab = Instantiate(deadPrefab);
        //    respawmob.allRespaws.Add(prefab);
        //    prefab.name = "Dead " + i;
        //    deadList[i] = prefab;
        //    prefab.transform.SetParent(this.transform);
        //    prefab.SetActive(false);
        //}
        //#endregion   

        //BullingDefense(1);
        //BullingTeleport();
        //PoolPetrify(3);
    }

    //void Update()
    //{
    //    if (recHpAtive)
    //        RecHpReset();

    //    if (hitAtive)
    //        HitReset();

    //    if (hitBlockAtive)
    //        HitBlockReset();

    //    if (crithitAtive)
    //        CriticalHitReset();

    //    if (deadAtive)
    //        DeadReset();

    //    if (popUpAtive)
    //        PopUpDamageReset();

    //    ShowEnemyReset();
    //}

    #region Effect Target
    [Space(7), Header("Target")]
    [SerializeField] GameObject targetPrefab; // Prefab do obj
    //[SerializeField] int targetSize = 10;
    [SerializeField] GameObject[] targetList; // list de prefab do obj

    public void TargetEffect(GameObject who)
    {
        if (who == null || targetList == null)
            return;

        for (int i = 0; i < targetList.Length; i++)
        {
            if (targetList[i].GetComponent<Effects>().target == who && targetList[i].activeSelf)
                return;
        }

        foreach (var t in targetList)
        {
            if (!t.activeSelf)
            {
                t.transform.rotation             = who.transform.rotation;
                t.GetComponent<Effects>().target = who;
                t.SetActive(true);
                return;
            }
        }
    }

    public void TargetTargeteado(GameObject who)
    {
        if (who == null)
            return;

        foreach (var t in targetList)
        {
            if (t.GetComponent<Effects>().target == who)
            {
                t.GetComponent<Effects>().gira            = t.GetComponent<Effects>().target == who;
                t.GetComponent<Effects>().FXAnimatorStart = t.GetComponent<Effects>().target == who;
            }
        }
    }

    public void TargetReset(GameObject who)
    {
        if (who!=null)
            return;

        foreach (var t in targetList)
        {
            if (t.GetComponent<Effects>().target == who)
            {
                t.GetComponent<Effects>().target = null;
                t.GetComponent<Effects>().gira   = false;
            }
        }
    }

    public void TargetReset()
    {
        for (int i = 0; i < targetList.Length; i++)
        {
            if (targetList[i] != null)
                if (targetList[i].activeSelf)
                {
                    targetList[i].GetComponent<Effects>().target = null;
                    targetList[i].GetComponent<Effects>().gira = false;
                }
        }
    }
    #endregion

    #region Turn Effect
    [Space(7), Header("Turn Effect")]
    [SerializeField] GameObject turnEffectPrefab; // Prefab do obj
    [SerializeField] GameObject turnEffect;   // Game Obj do obj
    [SerializeField] Color isGood;
    [SerializeField] Color isBad;

    public void TurnEffect(GameObject who)
    {
        turnEffect.GetComponentInChildren<Effects>().target = who;

        MobManager mob = who.GetComponent<MobManager>();

        if (mob == null || RespawMob.Instance.Player == null)
            return;

        if (RespawMob.Instance.Player.GetComponent<MobManager>().MesmoTime(mob.TimeMob))
            turnEffect.GetComponentInChildren<Light>().color = isGood;
        else
            turnEffect.GetComponentInChildren<Light>().color = isBad;
    }

    #endregion

    #region Dbuff
    #region Effect Fire
    [Space(7), Header("Fire")]
    [SerializeField] GameObject firePrefab; // Prefab do obj
    //[SerializeField] int fireSize = 2;
    [SerializeField] GameObject[] fireList; // list de prefab do obj

    public void FireEffect(GameObject who)
    {
        for (int i = 0; i < fireList.Length; i++)
        {
            if (fireList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < fireList.Length; i++)
        {
            if (!fireList[i].activeSelf)
            {
                fireList[i].SetActive(true);
                fireList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void FireReset(GameObject who)
    {
        if (fireList != null)
        for (int i = 0; i < fireList.Length; i++)
        {
                if (fireList[i].GetComponent<Effects>().target == who)
                {
                    fireList[i].GetComponent<Effects>().target = null;
                    fireList[i].SetActive(false);
                }
        }
    }
    #endregion

    #region Effect Poison
    [Space(7), Header("Poison")]
    [SerializeField] GameObject poisonPrefab; // Prefab do obj
    //[SerializeField] int poisonSize = 2;
    [SerializeField] GameObject[] poisonList; // list de prefab do obj    

    public void PoisonEffect(GameObject who)
    {
        for (int i = 0; i < poisonList.Length; i++)
        {
            if (poisonList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < poisonList.Length; i++)
        {
            if (!poisonList[i].activeSelf)
            {
                poisonList[i].SetActive(true);
                poisonList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void PoisonReset(GameObject who)
    {
        if (poisonList != null) 
            for (int i = 0; i < poisonList.Length; i++)
        {
                if (poisonList[i].GetComponent<Effects>().target == who)
                {
                    poisonList[i].GetComponent<Effects>().target = null;
                    poisonList[i].SetActive(false);
                }
        }
    }
    #endregion

    #region Effect Stun
    [Space(7), Header("Stun")]
    [SerializeField] GameObject stunPrefab; // Prefab do obj
    //[SerializeField] int stunSize = 5;
    [SerializeField] GameObject[] stunList; // list de prefab do obj

    public void StunEffect(GameObject who)
    {
        for (int i = 0; i < stunList.Length; i++)
        {
            if (stunList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < stunList.Length; i++)
        {
            if (!stunList[i].activeSelf)
            {
                stunList[i].SetActive(true);
                stunList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void StunReset(GameObject who)
    {
        if (stunList != null)
            for (int i = 0; i < stunList.Length; i++)
        {
                if (stunList[i].GetComponent<Effects>().target == who)
                {
                    stunList[i].GetComponent<Effects>().target = null;
                    stunList[i].SetActive(false);
                }           
        }
    }
    #endregion

    #region Effect Petrify
    [Space(7), Header("Petrify")]
    [SerializeField] GameObject   petrifyPrefab; // Prefab do obj
    public           Material petrifyMaterial;
    [SerializeField] int          petrifySize = 5;
    [SerializeField] GameObject[] petrifyList; // list de prefab do obj

    public void PoolPetrify(int pool)
    {
        petrifySize = pool;

        petrifyList = new GameObject[petrifySize];

        for (int i = 0; i < petrifyList.Length; i++)
        {
            GameObject prefab = Instantiate(petrifyPrefab);
            respawmob.allRespaws.Add(prefab);
            prefab.name = "Petrify " + i;
            petrifyList[i] = prefab;
            prefab.transform.SetParent(this.transform);
            prefab.SetActive(false);
        }
    }

    public void PetrifyEffect(GameObject who)
    {
        for (int i = 0; i < petrifyList.Length; i++)
        {
            if (petrifyList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < petrifyList.Length; i++)
        {
            if (!petrifyList[i].activeSelf)
            {
                who.GetComponent<MeshRenderer>().material = petrifyMaterial;

                petrifyList[i].transform.position = who.transform.position;

                petrifyList[i].GetComponent<Effects>().target = who;

                petrifyList[i].SetActive(true);
                return;
            }
        }
    }

    public void PetrifyReset(GameObject who)
    {
        if(petrifyList!=null)
            for (int i = 0; i < petrifyList.Length; i++)
        {
                if (petrifyList[i].GetComponent<Effects>().target = who)
                {
                    petrifyList[i].GetComponent<Effects>().target = null;
                    petrifyList[i].SetActive(false);
                }
            }      
    }
    #endregion

    #region Effect SilenceSkill
    [Space(7), Header("SilenceSkill")]
    [SerializeField]
    GameObject silenceSkillPrefab; // Prefab do obj
    //[SerializeField] int silenceSkillSize = 2;
    [SerializeField]
    List<GameObject> silenceSkillList = new List<GameObject>(); // list de prefab do obj

    public void SilenceSkillEffect(GameObject who)
    {
        for (int i = 0; i < silenceSkillList.Count; i++)
        {
            if (silenceSkillList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < silenceSkillList.Count; i++)
        {
            if (!silenceSkillList[i].activeSelf)
            {
                silenceSkillList[i].SetActive(true);
                silenceSkillList[i].GetComponent<Effects>().target = who;
                return;
            }
        }

        //Cria Caso Não tenha
        GameObject _newFx = Instantiate(silenceSkillPrefab,transform);
        _newFx.SetActive(true);
        _newFx.GetComponent<Effects>().target = who;
        silenceSkillList.Add(_newFx);
    }

    public void SilenceSkillReset(GameObject who)
    {
        if (silenceSkillList != null)
            for (int i = 0; i < silenceSkillList.Count; i++)
            {
                if (silenceSkillList[i].GetComponent<Effects>().target == who)
                {
                    silenceSkillList[i].GetComponent<Effects>().target = null;
                    silenceSkillList[i].SetActive(false);
                }
            }
    }
    #endregion

    #region Effect BuffDamage
    [Space(7), Header("BuffDamage")]
    [SerializeField]
    GameObject buffDamagePrefab; // Prefab do obj
    //[SerializeField] int buffDamageSize = 2;
    [SerializeField]
    List<GameObject> buffDamageList = new List<GameObject>(); // list de prefab do obj

    public void BuffDamageEffect(GameObject who)
    {
        for (int i = 0; i < buffDamageList.Count; i++)
        {
            if (buffDamageList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < buffDamageList.Count; i++)
        {
            if (!buffDamageList[i].activeSelf)
            {
                buffDamageList[i].SetActive(true);
                buffDamageList[i].GetComponent<Effects>().target = who;
                return;
            }
        }

        //Cria Caso Não tenha
        GameObject _newFx = Instantiate(buffDamagePrefab, transform);
        _newFx.SetActive(true);
        _newFx.GetComponent<Effects>().target = who;
        buffDamageList.Add(_newFx);
    }

    public void BuffDamageReset(GameObject who)
    {
        if (buffDamageList != null)
            for (int i = 0; i < buffDamageList.Count; i++)
            {
                if (buffDamageList[i].GetComponent<Effects>().target == who)
                {
                    buffDamageList[i].GetComponent<Effects>().target = null;
                    buffDamageList[i].SetActive(false);
                }
            }
    }
    #endregion

    #region Effect DbuffDamage
    [Space(7), Header("DbuffDamage")]
    [SerializeField]
    GameObject dbuffDamagePrefab; // Prefab do obj
    //[SerializeField] int dbuffDamageSize = 2;
    [SerializeField]
    List<GameObject> dbuffDamageList = new List<GameObject>(); // list de prefab do obj

    public void DbuffDamageEffect(GameObject who)
    {
        for (int i = 0; i < dbuffDamageList.Count; i++)
        {
            if (dbuffDamageList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < dbuffDamageList.Count; i++)
        {
            if (!dbuffDamageList[i].activeSelf)
            {
                dbuffDamageList[i].SetActive(true);
                dbuffDamageList[i].GetComponent<Effects>().target = who;
                return;
            }
        }

        //Cria Caso Não tenha
        GameObject _newFx = Instantiate(dbuffDamagePrefab, transform);
        _newFx.SetActive(true);
        _newFx.GetComponent<Effects>().target = who;
        dbuffDamageList.Add(_newFx);
    }

    public void DbuffDamageReset(GameObject who)
    {
        if (dbuffDamageList != null)
            for (int i = 0; i < dbuffDamageList.Count; i++)
            {
                if (dbuffDamageList[i].GetComponent<Effects>().target == who)
                {
                    dbuffDamageList[i].GetComponent<Effects>().target = null;
                    dbuffDamageList[i].SetActive(false);
                }
            }
    }
    #endregion

    #region Effect SilencePassive
    [Space(7), Header("SilencePassive")]
    [SerializeField]
    GameObject silencePassivePrefab; // Prefab do obj
    //[SerializeField] int silencePassiveSize = 2;
    [SerializeField]
    List<GameObject> silencePassiveList = new List<GameObject>(); // list de prefab do obj

    public void SilencePassiveEffect(GameObject who)
    {
        for (int i = 0; i < silencePassiveList.Count; i++)
        {
            if (silencePassiveList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < silencePassiveList.Count; i++)
        {
            if (!silencePassiveList[i].activeSelf)
            {
                silencePassiveList[i].SetActive(true);
                silencePassiveList[i].GetComponent<Effects>().target = who;
                return;
            }
        }

        //Cria Caso Não tenha
        GameObject _newFx = Instantiate(silencePassivePrefab, transform);
        _newFx.SetActive(true);
        _newFx.GetComponent<Effects>().target = who;
        silencePassiveList.Add(_newFx);
    }

    public void SilencePassiveReset(GameObject who)
    {
        if (silencePassiveList != null)
            for (int i = 0; i < silencePassiveList.Count; i++)
            {
                if (silencePassiveList[i].GetComponent<Effects>().target == who)
                {
                    silencePassiveList[i].GetComponent<Effects>().target = null;
                    silencePassiveList[i].SetActive(false);
                }
            }
    }
    #endregion
    #endregion

    #region Effect Hp
    [Space(7), Header("Rec Hp")]
    [SerializeField] GameObject recHpPrefab; // Prefab do obj
    [SerializeField] int recHpSize = 5;
    [SerializeField] bool recHpAtive;        // bool pra ve si tem alguem ativo
    [SerializeField] GameObject[] recHpList; // list de prefab do obj    
    [SerializeField] bool[] recHpAtived;     // bool pra ve quem esta ativo


    public void PullingRecHp(int sizeList)
    {
        if (respawmob == null)
            respawmob = RespawMob.Instance;

            recHpSize = sizeList;

        #region Rec hp Bullet
        recHpList   = new GameObject[recHpSize];
        recHpAtived = new bool[recHpSize];

        for (int i = 0; i < recHpList.Length; i++)
        {
            GameObject prefab = Instantiate(recHpPrefab);
            prefab.name  = "Effect Rec Hp " + i;
            recHpList[i] = prefab;
            respawmob.allRespaws.Add(recHpList[i]);
            prefab.transform.SetParent(this.transform);
            prefab.SetActive(false);
        }
        #endregion
    }

    public void RecHpEffect(GameObject who)
    {
        if(recHpAtive)
        StartCoroutine(RecHpResetCoroutine());

        for (int i = 0; i < recHpList.Length; i++)
        {
            if (!recHpList[i].activeSelf)
            {
                recHpList[i].SetActive(true);
                recHpList[i].GetComponent<Effects>().target = who;
                recHpAtived[i] = true;
                recHpAtive = true;
                break;
            }
        }        
    }

    IEnumerator RecHpResetCoroutine()
    {
        while (recHpAtive)
        {
            yield return waitUpdate;

            for (int i = 0; i < recHpAtived.Length; i++)
            {
                if (recHpAtived[i])
                {
                    if (recHpList[i].GetComponent<ParticleSystem>().isPlaying == false)
                    {
                        recHpAtived[i] = false;
                        recHpList[i].GetComponent<Effects>().target = null;
                        recHpList[i].SetActive(false);
                    }
                }
            }

            for (int i = 0; i < recHpList.Length; i++)
            {
                if (recHpList[i].activeSelf)
                {
                    recHpAtive = true;
                    break;
                }

                if (i == recHpList.Length - 1)
                {
                    recHpAtive = false;
                }
            }
        }
    }
    #endregion

    #region Effect Teleport
    [Space(7), Header("EndTeleport")]
    [SerializeField] GameObject teleportPrefab ; // Prefab do obj
    //[SerializeField] int teleportSize = 1;
    [SerializeField] GameObject[] teleportList = new GameObject[1];  // list de prefab do obj
    [SerializeField] bool[] teleportAtived     = new bool[1]; // bool pra ve quem esta ativo

    public void BullingTeleport()
    {

        #region Teleport Bullet
        for (int i = 0; i < teleportList.Length; i++)
        {
            if (teleportList[i] == null)
            {
                GameObject prefab = Instantiate(teleportPrefab);
                respawmob.allRespaws.Add(prefab);
                prefab.name = "Effect EndTeleport " + i;
                teleportList[i] = (prefab);
                teleportAtived[i] = false;
                prefab.transform.SetParent(this.transform);
                prefab.SetActive(false);
            }
        }
        #endregion
    }

    public GameObject TeleportEffect(GameObject who)
    {
        for (int i = 0; i < teleportList.Length; i++)
        {
            if (teleportList[i].GetComponent<TeleportEffect>().target == who)
                return teleportList[i];
        }

        for (int i = 0; i < teleportList.Length; i++)
        {
            if (!teleportAtived[i])
            {
                teleportList[i].SetActive(true);
                teleportList[i].GetComponent<TeleportEffect>().transform.position = who.transform.position;
                teleportList[i].GetComponent<TeleportEffect>().target = who;
                teleportList[i].GetComponent<TeleportEffect>().PlayAudioSumindo();
                teleportAtived[i] = true;
                return teleportList[i];
                //if (teleportList[i].GetComponent<TeleportEffect>().ActiveTeleport(who))
                //{
                //    teleportAtived[i] = true;
                //    return teleportList[i];
                //}            
            }
        }

        return null;
    }

    public void TeleportEffectTeste(GameObject who)
    {
        for (int i = 0; i < teleportList.Length; i++)
        {
            if (teleportList[i].GetComponent<TeleportEffect>().target == who)
                return;
        }

        for (int i = 0; i < teleportList.Length; i++)
        {
            if (!teleportAtived[i])
            {
                teleportList[i].SetActive(true);
                teleportList[i].GetComponent<TeleportEffect>().target = who;
                teleportList[i].GetComponent<TeleportEffect>().PlayAudioSumindo();
                teleportAtived[i] = true;
                break;
                //if (teleportList[i].GetComponent<TeleportEffect>().ActiveTeleport(who))
                //{
                //    teleportAtived[i] = true;
                //    return teleportList[i];
                //}            
            }
        }
    }

    public void TeleportReset(GameObject who)
    {
        for (int i = 0; i < teleportAtived.Length; i++)
        {
            if (teleportList[i].GetComponent<TeleportEffect>().target == who)
            {
                teleportList[i].GetComponent<TeleportEffect>().PlayAudioReaparecendo();
                teleportAtived[i] = false;
                teleportList[i].GetComponent<TeleportEffect>().target = null;
                teleportList[i].SetActive(false);
                //if (teleportList[i].GetComponent<TeleportEffect>().DesactiveTeleport(who))
                //{
                //    teleportAtived[i] = false;
                //    break;
                //}
            }
        }
    }
    #endregion

    #region Defense
    [Space(7), Header("Defense")]
    [SerializeField] GameObject defensePrefab; // Prefab do obj
    [SerializeField] int defenseSize = 5;
    [SerializeField] GameObject[] defenseList;   // list de prefab do obj
    [SerializeField] bool[]       defenseAtived; // bool pra ve quem esta ativo

    public void BullingDefense(int listSize)
    {
        defenseSize = listSize;

        #region Defense Bullet
        defenseList = new GameObject[defenseSize];
        defenseAtived = new bool[defenseSize];

        for (int i = 0; i < defenseList.Length; i++)
        {
            GameObject prefab = Instantiate(defensePrefab);
            respawmob.allRespaws.Add(prefab);
            prefab.name = "Effect Defense " + i;
            defenseList[i] = prefab;
            prefab.transform.SetParent(this.transform);
            prefab.SetActive(false);
        }
        #endregion
    }

    public void DefeseEffect(GameObject who)
    {
        for (int i = 0; i < defenseList.Length; i++)
        {
            if (defenseList[i].GetComponent<Effects>().target == who)
                return;
        }


        for (int i = 0; i < defenseList.Length; i++)
        {
            if (!defenseList[i].activeSelf)
            {
                defenseList[i].SetActive(true);
                defenseList[i].GetComponent<Effects>().target = who;
                defenseAtived[i] = true;
                return;
            }
        }
    }

    public void DefeseReset(GameObject who)
    {
        for (int i = 0; i < defenseAtived.Length; i++)
        {
            if (defenseAtived[i])
            {
                if (defenseList[i].GetComponentInChildren<Effects>().target == who)
                {
                    defenseAtived[i] = false;
                    defenseList[i].GetComponent<Effects>().target = null;
                    defenseList[i].SetActive(false);
                }
            }
        }
    }
    #endregion

    #region Effect Hit
    [Space(7), Header("Hit")]
    [SerializeField] GameObject hitPrefab; // Prefab do obj
    //[SerializeField] int hitSize = 7;
    [SerializeField] bool hitAtive;        // bool pra ve si tem alguem ativo
    [SerializeField] GameObject[] hitList; // list de prefab do obj   
    [SerializeField] bool[] hitAtived;     // bool pra ve quem esta ativo

    public void HitEffect(GameObject who)
    {
        if(!hitAtive)
            StartCoroutine(HitResetCoroutine());

        for (int i = 0; i < hitList.Length; i++)
        {
            if (!hitList[i].activeSelf)
            {               
                hitList[i].SetActive(true);
                hitList[i].transform.position = who.transform.position;
                hitList[i].transform.rotation = who.transform.rotation;
                hitList[i].GetComponent<Effects>().target = who;
                hitAtived[i] = true;
                hitAtive = true;
                break;
            }
        }       
    }

    IEnumerator HitResetCoroutine()
    {
        while (hitAtive)
        {
            yield return waitUpdate;
            for (int i = 0; i < hitAtived.Length; i++)
            {
                if (hitAtived[i])
                {
                    if (!hitList[i].GetComponentInChildren<ParticleSystem>().isPlaying)
                    {
                        hitAtived[i] = false;
                        hitList[i].GetComponent<Effects>().target = null;
                        hitList[i].SetActive(false);
                    }
                }
            }

            for (int i = 0; i < hitList.Length; i++)
            {
                if (hitList[i].activeSelf)
                {
                    hitAtive = true;
                    break;
                }

                if (i == hitList.Length - 1)
                {
                    hitAtive = false;
                }
            }
        }
    }
    #endregion

    #region Effect HitBlock
    [Space(7), Header("HitBlock")]
    [SerializeField] GameObject hitBlockPrefab; // Prefab do obj
    //[SerializeField] int hitBlockSize = 7;
    [SerializeField] bool hitBlockAtive;        // bool pra ve si tem alguem ativo
    [SerializeField] GameObject[] hitBlockList; // list de prefab do obj   
    [SerializeField] bool[] hitBlockAtived;     // bool pra ve quem esta ativo

    public void HitBlockEffect(GameObject who)
    {
        if (!hitBlockAtive)
            StartCoroutine(HitBlockResetCoroutine());

        for (int i = 0; i < hitBlockList.Length; i++)
        {
            if (!hitBlockList[i].activeSelf)
            {
                hitBlockList[i].SetActive(true);
                hitBlockList[i].transform.rotation = who.transform.rotation;
                hitBlockList[i].GetComponent<Effects>().target = who;
                hitBlockAtived[i] = true;
                hitBlockAtive = true;
                return;
            }
        }
    }

    IEnumerator HitBlockResetCoroutine()
    {
        while (hitBlockAtive)
        {
            yield return waitUpdate;
            for (int i = 0; i < hitBlockAtived.Length; i++)
            {
                if (hitBlockAtived[i])
                {
                    if (hitBlockList[i].GetComponentInChildren<ParticleSystem>().isPlaying == false)
                    {
                        hitBlockAtived[i] = false;
                        hitBlockList[i].GetComponent<Effects>().target = null;
                        hitBlockList[i].SetActive(false);
                    }
                }
            }

            for (int i = 0; i < hitBlockList.Length; i++)
            {
                if (hitBlockList[i].activeSelf)
                {
                    hitBlockAtive = true;
                    break;
                }

                if (i == hitBlockList.Length - 1)
                {
                    hitBlockAtive = false;
                }
            }
        }
    }
    #endregion

    #region Effect Critical Hit
    [Space(7), Header("Critical Hit")]
    [SerializeField] GameObject crithitPrefab; // Prefab do obj
    //[SerializeField] int crithitSize = 7;
    [SerializeField] bool crithitAtive;        // bool pra ve si tem alguem ativo
    [SerializeField] GameObject[] crithitList; // list de prefab do obj    
    [SerializeField] bool[] crithitAtived;     // bool pra ve quem esta ativo

    public void CriticalHitEffect(GameObject who)
    {
        if (!crithitAtive)
            StartCoroutine(CriticalHitResetCoroutine());

        for (int i = 0; i < crithitList.Length; i++)
        {
            if (!crithitList[i].activeSelf)
            {
                crithitList[i].transform.position = who.transform.position;
                crithitList[i].SetActive(true);
                crithitList[i].transform.rotation = who.transform.rotation;
                crithitList[i].GetComponent<Effects>().target = who;
                crithitAtived[i] = true;
                crithitAtive = true;
                return;
            }
        }
    }

    IEnumerator CriticalHitResetCoroutine()
    {
        while (crithitAtive)
        {
            yield return waitUpdate;

            for (int i = 0; i < crithitAtived.Length; i++)
            {
                if (crithitAtived[i])
                {
                    if (!crithitList[i].GetComponentInChildren<ParticleSystem>().isPlaying)
                    {
                        crithitAtived[i] = false;
                        crithitList[i].GetComponent<Effects>().target = null;
                        crithitList[i].SetActive(false);
                    }
                }
            }

            for (int i = 0; i < crithitList.Length; i++)
            {
                if (crithitList[i].activeSelf)
                {
                    crithitAtive = true;
                    break;
                }

                if (i == crithitList.Length - 1)
                {
                    crithitAtive = false;
                }
            }
        }
    }
    #endregion

    #region Effect PopUp Damage
    [Space(7), Header("Pop Up Damage")]
    [SerializeField] GameObject popUpDamagePrefab;                // Prefab do obj
    [SerializeField] float popUpTime;
    //[SerializeField] int popUpDamageSize = 7;
    //[SerializeField] bool popUpAtive = false;
    [SerializeField] GameObject[] popUpDamageList;               // list de prefab do obj  
    //[SerializeField] float[] popUpDamageAtived;                  // bool pra ve quem esta ativo   
    [SerializeField] float scaleTime;

    public void PopUpDamageEffect(string _text, GameObject who,float time = -1,int positionY=0,bool attPosi=true)
    {
        if (who!= null)
        for (int i = 0; i < popUpDamageList.Length; i++)
        {
            if (!popUpDamageList[i].activeInHierarchy)
            {
                popUpDamageList[i].GetComponent<Fade>().timeToFade = popUpTime;

                popUpDamageList[i].SetActive(true);

                popUpDamageList[i].GetComponentInChildren<Text>().color     = Color.white;


                if (time == -1)
                    time = popUpTime;

                if (PlayerPrefs.GetInt("MoveCameraArrow") == 0)
                    positionY = 0;              

                popUpDamageList[i].GetComponentInChildren<Effects>().target      = who;               

                Vector3 _position = new Vector3(who.transform.position.x, (who.transform.position.y + positionY), who.transform.position.z);
                popUpDamageList[i].GetComponentInChildren<Effects>().MyPosition = _position;

                Vector2 screenPoint = Camera.main.WorldToScreenPoint(_position);               

                Text textInst = popUpDamageList[i].GetComponentInChildren<Text>();

                textInst.text = _text;
                textInst.transform.position = screenPoint;

                popUpDamageList[i].GetComponentInChildren<Effects>().attPosition = attPosi;
                //iTween.MoveTo(popUpDamageList[i].GetComponentInChildren<Effects>().gameObject,
                //       iTween.Hash( "x", who.transform.position.x,
                //                    "y", who.transform.position.y + 7,
                //                    "z", who.transform.position.z,
                //                    "delay", 0,
                //                    "easetype", iTween.EaseType.easeOutQuint,
                //                    "time", popUpTime));                            
                //popUpDamageAtived[i] = Time.time + time;

                //popUpAtive = true;               
                return;
            }
        }
    }

    //void PopUpDamageReset()//Falta otimizar
    //{
    //    if (popUpAtive)
    //    {
    //        for (int i = 0; i < popUpDamageAtived.Length; i++)
    //        {
    //            if (Time.time >= popUpDamageAtived[i] || popUpDamageList[i].GetComponentInChildren<Effects>().target == null)
    //            {
    //                popUpDamageList[i].GetComponentInChildren<Text>().text = "";
    //                popUpDamageList[i].GetComponentInChildren<Effects>().target = null;
    //                popUpDamageList[i].SetActive(false);
    //                popUpDamageAtived[i] = -1;
    //                break;
    //            }
    //        }

    //        foreach (var item in popUpDamageAtived)
    //        {
    //            if (item != -1)
    //            {                  
    //                return;
    //            }                 
    //        }

    //        popUpAtive = false;
    //    }           
    //}
    #endregion

    #region Effect Show Enemy
    [Space(7), Header("Show Enemy")]
    [SerializeField] GameObject showEnemyPrefab;                // Prefab do obj
    [SerializeField] float showEnemyTime;
    //[SerializeField] int showEnemySize = 7;
    [SerializeField] GameObject[] showEnemyList;               // list de prefab do obj  
    //[SerializeField] float[]      showEnemyAtived;             // bool pra ve quem esta ativo

    public void ShowEnemyEffect(GameObject who)
    {
        //if (showEnemyAtived != null && who!=null)
            for (int i = 0; i < showEnemyList.Length; i++)
        {
            if (!showEnemyList[i].activeSelf)
            {
                //showEnemyAtived[i] = Time.time + showEnemyTime;

                showEnemyList[i].GetComponent<Fade>().timeToFade = showEnemyTime;

                showEnemyList[i].SetActive(true);

                showEnemyList[i].GetComponentInChildren<Effects>().target = who;

                if (showEnemyList[i].GetComponentInChildren<ParticleSystem>())
                {
                    if (who.GetComponent<MobManager>().MesmoTime(respawmob.PlayerTime))
                        showEnemyList[i].GetComponentInChildren<ParticleSystem>().startColor = isGood;
                    else
                        showEnemyList[i].GetComponentInChildren<ParticleSystem>().startColor = isBad;
                }

                if (showEnemyList[i].GetComponentInChildren<Light>())
                {
                    if (who.GetComponent<MobManager>().MesmoTime(respawmob.PlayerTime))
                        showEnemyList[i].GetComponentInChildren<Light>().color = isGood;
                    else
                        showEnemyList[i].GetComponentInChildren<Light>().color = isBad;
                }
                //Vector2 screenPoint = Camera.main.WorldToScreenPoint(who.transform.position);

                //Invoke("ShowEnemyReset()", showEnemyTime+0.25f);
                return;
            }
        }
    }

    //void ShowEnemyReset()//Falta otimizar
    //{
    //    if (showEnemyAtived != null)
    //        for (int i = 0; i < showEnemyAtived.Length; i++)
    //        {
    //            if (showEnemyList[i] != null)
    //                if (showEnemyList[i].activeSelf)
    //                {
    //                    if (Time.time > showEnemyAtived[i])
    //                    {
    //                        showEnemyAtived[i] = -1;
    //                        showEnemyList[i].GetComponent<Effects>().target = null;
    //                        showEnemyList[i].SetActive(false);
    //                        break;
    //                    }
    //                }
    //        }
    //}
    #endregion

    #region Effect dead
    [Space(7), Header("Dead")]
    [SerializeField] GameObject deadPrefab; // Prefab do obj
    //[SerializeField] int deadSize = 4;
    [SerializeField] bool deadAtive;        // bool pra ve si tem alguem ativo
    [SerializeField] GameObject[] deadList; // list de prefab do obj   
    [SerializeField] bool[] deadAtived;     // bool pra ve quem esta ativo

    public void DeadEffect(GameObject who)
    {
        if (!deadAtive)
            StartCoroutine(DeadResetCoroutine());

        for (int i = 0; i < deadList.Length; i++)
        {
            if (!deadList[i].activeSelf)
            {                
                deadList[i].SetActive(true);
                deadList[i].transform.position = who.transform.position;
                deadList[i].transform.rotation = who.transform.rotation;
                deadList[i].GetComponent<Effects>().target = who;
                deadAtived[i] = true;
                deadAtive = true;
                return;
            }
        }
    }

    IEnumerator DeadResetCoroutine()
    {
        while (deadAtive)
        {
            yield return waitUpdate;

            for (int i = 0; i < deadAtived.Length; i++)
            {
                if (deadAtived[i])
                {
                    if (!deadList[i].GetComponentInChildren<ParticleSystem>().isPlaying)
                    {
                        deadAtived[i] = false;
                        deadList[i].GetComponent<Effects>().target = null;
                        deadList[i].SetActive(false);
                    }
                }
            }

            for (int i = 0; i < deadList.Length; i++)
            {
                if (deadList[i].activeSelf)
                {
                    deadAtive = true;
                    break;
                }

                if (i == deadList.Length - 1)
                {
                    deadAtive = false;
                }
            }
        }
    }
    #endregion

    #region Effect Respaw
    [Space(7), Header("Respaw Effect")]
    [SerializeField]
    GameObject respawPrefab; // Prefab do obj
    [SerializeField]
    GameObject[] respawList; // list de prefab do obj   

    public void RespawEffect(GameObject who,float timeActive)
    {
        for (int i = 0; i < respawList.Length; i++)
        {
            if (!respawList[i].activeSelf)
            {
                respawList[i].GetComponent<Effects>().ChangeTimerActive(timeActive);
                respawList[i].SetActive(true);
                respawList[i].transform.position = who.transform.position;
                respawList[i].transform.rotation = who.transform.rotation;
                respawList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }
    #endregion

    #region Effect Dbuff Arma
    [Space(7)]
    #region Fire
    [Header("Dbuff Arma Effect - Fire")]
    [SerializeField]
    GameObject dbuffFireArmaPrefab; // Prefab do obj
    [SerializeField]
    GameObject[] dbuffFireArmaList; // list de prefab do obj   

    public void FireArmaEffect(GameObject who)
    {
        if (who==null)
            return;

        for (int i = 0; i < dbuffFireArmaList.Length; i++)
        {
            if (dbuffFireArmaList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < dbuffFireArmaList.Length; i++)
        {
            if (!dbuffFireArmaList[i].activeSelf)
            {
                dbuffFireArmaList[i].transform.position = who.transform.position;
                dbuffFireArmaList[i].SetActive(true);
                dbuffFireArmaList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void FireArmaReset(GameObject who)
    {
        if (who != null)
            for (int i = 0; i < dbuffFireArmaList.Length; i++)
            {
                if (dbuffFireArmaList[i].GetComponent<Effects>().target == who)
                {
                    dbuffFireArmaList[i].GetComponent<Effects>().target = null;
                    dbuffFireArmaList[i].SetActive(false);
                    dbuffFireArmaList[i].transform.position = new Vector3(-999, -999, -99);
                }
            }
    }
    #endregion

    #region Poison
    [Header("Dbuff Arma Effect - Poison")]
    [SerializeField]
    GameObject dbuffPoisonArmaPrefab; // Prefab do obj
    [SerializeField]
    GameObject[] dbuffPoisonArmaList; // list de prefab do obj   

    public void PoisonArmaEffect(GameObject who)
    {
        if (who == null)
            return;

        for (int i = 0; i < dbuffPoisonArmaList.Length; i++)
        {
            if (dbuffPoisonArmaList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < dbuffPoisonArmaList.Length; i++)
        {
            if (!dbuffPoisonArmaList[i].activeSelf)
            {
                dbuffPoisonArmaList[i].transform.position = who.transform.position;
                dbuffPoisonArmaList[i].SetActive(true);
                dbuffPoisonArmaList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void PoisonArmaReset(GameObject who)
    {
        if (who != null)
            for (int i = 0; i < dbuffPoisonArmaList.Length; i++)
            {
                if (dbuffPoisonArmaList[i].GetComponent<Effects>().target == who)
                {
                    dbuffPoisonArmaList[i].GetComponent<Effects>().target = null;
                    dbuffPoisonArmaList[i].SetActive(false);
                    dbuffPoisonArmaList[i].transform.position = new Vector3(-999, -999, -99);
                }
            }
    }
    #endregion

    #region Stun
    [Header("Dbuff Arma Effect - Stun")]
    [SerializeField]
    GameObject dbuffStunArmaPrefab; // Prefab do obj
    [SerializeField]
    GameObject[] dbuffStunArmaList; // list de prefab do obj   

    public void StunArmaEffect(GameObject who)
    {
        if (who == null)
            return;
        for (int i = 0; i < dbuffStunArmaList.Length; i++)
        {
            if (dbuffStunArmaList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < dbuffStunArmaList.Length; i++)
        {
            if (!dbuffStunArmaList[i].activeSelf)
            {
                dbuffStunArmaList[i].transform.position = who.transform.position;
                dbuffStunArmaList[i].SetActive(true);
                dbuffStunArmaList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void StunArmaReset(GameObject who)
    {
        if (who != null)
            for (int i = 0; i < dbuffStunArmaList.Length; i++)
            {
                if (dbuffStunArmaList[i].GetComponent<Effects>().target == who)
                {
                    dbuffStunArmaList[i].GetComponent<Effects>().target = null;
                    dbuffStunArmaList[i].SetActive(false);
                    dbuffStunArmaList[i].transform.position = new Vector3(-999, -999, -99);
                }
            }
    }
    #endregion

    #region Petrify
    [Header("Dbuff Arma Effect - Petrify")]
    [SerializeField]
    GameObject dbuffPetrifyArmaPrefab; // Prefab do obj
    [SerializeField]
    GameObject[] dbuffPetrifyArmaList; // list de prefab do obj   

    public void PetrifyArmaEffect(GameObject who)
    {
        if (who == null)
            return;
        for (int i = 0; i < dbuffPetrifyArmaList.Length; i++)
        {
            if (dbuffPetrifyArmaList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < dbuffPetrifyArmaList.Length; i++)
        {
            if (!dbuffPetrifyArmaList[i].activeSelf)
            {
                dbuffPetrifyArmaList[i].transform.position = who.transform.position;
                dbuffPetrifyArmaList[i].SetActive(true);
                dbuffPetrifyArmaList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void PetrifyArmaReset(GameObject who)
    {
        if (who != null)
            for (int i = 0; i < dbuffPetrifyArmaList.Length; i++)
            {
                if (dbuffPetrifyArmaList[i].GetComponent<Effects>().target == who)
                {                   
                    dbuffPetrifyArmaList[i].GetComponent<Effects>().target = null;
                    dbuffPetrifyArmaList[i].transform.position = new Vector3(-999, -999, -99);
                    dbuffPetrifyArmaList[i].SetActive(false);
                }
            }
    }
    #endregion

    #region Bleed
    [Header("Dbuff Arma Effect - Bleed")]
    [SerializeField]
    GameObject dbuffBleedArmaPrefab; // Prefab do obj
    [SerializeField]
    GameObject[] dbuffBleedArmaList; // list de prefab do obj   

    public void BleedArmaEffect(GameObject who)
    {
        if (who == null)
            return;
        for (int i = 0; i < dbuffBleedArmaList.Length; i++)
        {
            if (dbuffBleedArmaList[i].GetComponent<Effects>().target == who)
                return;
        }

        for (int i = 0; i < dbuffBleedArmaList.Length; i++)
        {
            if (!dbuffBleedArmaList[i].activeSelf)
            {
                dbuffBleedArmaList[i].transform.position = who.transform.position;
                dbuffBleedArmaList[i].SetActive(true);
                dbuffBleedArmaList[i].GetComponent<Effects>().target = who;
                return;
            }
        }
    }

    public void BleedArmaReset(GameObject who)
    {
        if (who != null)
            for (int i = 0; i < dbuffBleedArmaList.Length; i++)
            {
                if (dbuffBleedArmaList[i].GetComponent<Effects>().target == who)
                {                 
                    dbuffBleedArmaList[i].GetComponent<Effects>().target = null;
                    dbuffBleedArmaList[i].transform.position = new Vector3(-999, -999, -99);
                    dbuffBleedArmaList[i].SetActive(false);
                }
            }
    }
    #endregion
    #endregion
}
