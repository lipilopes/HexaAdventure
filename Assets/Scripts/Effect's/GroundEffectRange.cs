using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GroundEffectRange : MonoBehaviour
{
    [HideInInspector]
    public bool CheckCompativelMode = false;
    [SerializeField]
    protected Game_Mode _mode = Game_Mode.History;
    [SerializeField,Tooltip("Caso for efeito diferente, colocar outro nome")]
    string Nome = "(1)";
    [Space]
    [Header("SetUP")]
    [SerializeField, Tooltip("Ja deixa alguns grounds criados")]
    protected int createDefault;
    [SerializeField, Tooltip("Inicia assim que obj é criado")]
    protected bool autoStart = true;
    [Space]
    [SerializeField, Tooltip("Esse Obj Anda enquanto coloca os grounds")]
    protected bool walk;
    [SerializeField, Tooltip("Tamanho do raio")]
    protected int rangeRadio;
    [SerializeField, Tooltip("Tempo de espera pra ativar")]
    protected float delay;
    [Space]
    [Header("Propriedades  Dbuff Ground")]
    [SerializeField, Tooltip("Prefab do effect Dbuff Ground")]
    protected GameObject DbuffGround;
    [Space]
    [SerializeField, Tooltip("Chance Critico bonus soma com a do mob user"),Range(0,100)]
    protected float bonusCritical;
    [Space]
    [SerializeField, Tooltip("Tempo que ficara ativo")]
    protected int maxTimeDbuffGround = 3;
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de dano do ground com base no dano da skill")]
    protected float porcenDamageDbuffGround = 50f;
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de dano ao andar sobe o ground")]
    protected float porcenWalkDamageDbuffGround = 25f;
    [Space]
    [SerializeField, Range(0, 1), Tooltip("Chance de Dar mais hits $P2")]
    protected float chanceMaxHitDbuffGround;
    [SerializeField, Tooltip("Maximo de hits Extras $P3")]
    protected int maxHitAreaDbuffGround;

    protected int _damageDbuffGround;
    protected int _damageWalkDbuffGround;

    protected List<GameObject> DbuffGroundList = new List<GameObject>();

    protected bool active = false;

    protected GameObject user;
    protected CheckGrid  checkGrid;

    [HideInInspector]
    public int baseDamage;

    protected GameObject lastHit;    

    protected virtual void Awake()
    {
        if (!CheckCompativelMode)
        {
            int count = 0;

            foreach (var item in GetComponents<GroundEffectRange>())
                if (item.Nome == Nome && item._mode != _mode)
                {
                    count++;
                    Debug.LogWarning(name + " Encontrou um possivel Incompativel com o modo!!!");
                }

            if (GameManagerScenes._gms.GameMode != _mode
                && count > 0)
            {               
                Debug.LogWarning(name + " Incompativel com o modo!!!");

                Destroy(this);
                count--;              
                return;
            }

            if (count > 1)
            {
                Awake();
                return;
            }

            CheckCompativelMode = true;

            Debug.Log(name + " total de GroundEffectRange iguais " + count);

            name = name /*+ " - " + Skill*/ + " [" + GameManagerScenes._gms.GameMode + "]";
        }
    }

    private void Start()
    {
            checkGrid = CheckGrid.Instance;

        if (GetComponent<SkillAttack>())
        {
            user = GetComponent<SkillAttack>().who;
            baseDamage = GetComponent<SkillAttack>().damage;
        }
        else
            if (GetComponent<MobManager>())
        {
            user = gameObject;
        }
        else
        if (GetComponent<MobSkillManager>())
        {
            user       = GetComponent<MobSkillManager>().User;
            baseDamage = GetComponent<MobSkillManager>().CurrentDamage;
        }

        for (int i = 0; i < createDefault; i++)
        {
            ActiveDbuffGround().gameObject.SetActive(true);
        }

        for (int i = 0; i < DbuffGroundList.Count; i++)
        {
            DbuffGroundList[i].SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (autoStart)
            Invoke("Active", delay);
    }

    private void OnDisable()
    {
        active = false;

        lastHit = null;
    }

    public void Active()
    {
        Debug.LogError("<color=green>Ground Effect Active</color>");


        checkGrid = CheckGrid.Instance;


        if (GetComponent<SkillAttack>())
        {
            user = GetComponent<SkillAttack>().who;
            baseDamage = GetComponent<SkillAttack>().damage;
        }
        else
            if (GetComponent<MobManager>())
        {
            user = gameObject;
        }
        else
        if (GetComponent<MobSkillManager>())
        {
            user = GetComponent<MobSkillManager>().User;
            baseDamage = GetComponent<MobSkillManager>().CurrentDamage;
        }

        _damageDbuffGround     = (int)((baseDamage * porcenDamageDbuffGround) / 100);
        _damageWalkDbuffGround = (int)((baseDamage * porcenWalkDamageDbuffGround) / 100);

        active = true;

        if (GetComponent<BoxCollider>())
        {
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = true;
        }
        else
           if (GetComponent<CapsuleCollider>())
        {
            GetComponent<CapsuleCollider>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = true;
        }
        else
           if (GetComponent<SphereCollider>())
        {
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<SphereCollider>().enabled = true;
        }

        if (GetComponent<SkillAttackRange>() && GetComponent<SkillAttackRange>().GroundCheck != null)
            HitCollider(GetComponent<SkillAttackRange>().GroundCheck.gameObject);
    }

    protected GroundEffect ActiveDbuffGround()
    {
        if (DbuffGroundList.Count > 0)
        {
            foreach (var i in DbuffGroundList)
            {
                if (!i.gameObject.activeInHierarchy && i.GetComponent<GroundEffect>().TimeOn <= 0)
                { 
                    Debug.LogError("<color=green>ActiveDbuffGround -> <b>Find</b> " + i + "</color>");
                    return i.GetComponent<GroundEffect>();
                }
            }
        }

        GameObject obj = Instantiate(DbuffGround, new Vector3(-99, -99, -99),new Quaternion());

        obj.name = DbuffGround.name + " " + (DbuffGroundList.Count + 1) + "  (" + (user!=null ? (user.GetComponent<ToolTipType>() ? user.GetComponent<ToolTipType>()._name : user.name) : "") + ")";

        DbuffGroundList.Add(obj);

        if (RespawMob.Instance)
            RespawMob.Instance.allRespaws.Add(obj.gameObject);

        obj.gameObject.SetActive(false);

        Debug.LogError("<color=green>ActiveDbuffGround -> <b>Create</b> " + obj + "</color>");

        return obj.GetComponent<GroundEffect>();
    }

    protected void HitCollider(GameObject hitted)
    {
        //Debug.LogError("<color=green>HitCollider in " + hitted + "</color>");

        GameManagerScenes._gms.NewInfo("<color=green>HitCollider in " + hitted + "</color>", 2, true);

        if (active)
            if (DbuffGround != null && hitted != null && hitted.GetComponent<HexManager>() && hitted.GetComponent<HexManager>().currentItem == null)
            {
                if (CheckGround(hitted.GetComponent<HexManager>()))
                {
                    //Debug.LogError("<color=green>HitCollider in " + hitted.name + "</color>");

                    GameManagerScenes._gms.NewInfo("<color=green>HitCollider in " + hitted.name + "</color>", 2, true);

                    if (walk)
                        WalkHit(hitted);
                    else
                        StaticHit(hitted);
                }
            }
    }

    protected void WalkHit(GameObject hitted)
    {
        if (!active)
            return;

        lastHit = hitted;

        Debug.LogError("<color=green>Ground Effect WalkHit in " + hitted + "</color>");

            GameManagerScenes._gms.NewInfo("<color=green>Ground Effect WalkHit in " + hitted + "</color>", 2, true);

        ActiveDbuffGround().RespawSkill(
            (user != null ? (user) : gameObject),
            (user != null ? (user.GetComponent<EnemyAttack>() ? user.GetComponent<EnemyAttack>().target : null) : null),
            hitted.GetComponent<HexManager>(),
            _damageDbuffGround,
            _damageWalkDbuffGround,
            maxHitAreaDbuffGround,
            chanceMaxHitDbuffGround,
            maxTimeDbuffGround,
            (user != null ? (user.GetComponent<MobManager>() ? user.GetComponent<MobManager>().chanceCritical + bonusCritical : bonusCritical) : bonusCritical)
            );
    }

    public void CreateWalkHit()
    {
        if (lastHit == null)
        {
            return;
        }

        GameObject hitted = lastHit;

        Debug.LogError("<color=green>CreateWalkHit in " + hitted + "</color>");

        if (GameManagerScenes._gms.Adm)
            GameManagerScenes._gms.NewInfo("<color=green>CreateWalkHit in " + hitted + "</color>", 2);

        ActiveDbuffGround().RespawSkill(
            (user != null ? (user) : gameObject),
            (user != null ? (user.GetComponent<EnemyAttack>() ? user.GetComponent<EnemyAttack>().target : null) : null),
            hitted.GetComponent<HexManager>(),
            _damageDbuffGround,
            _damageWalkDbuffGround,
            maxHitAreaDbuffGround,
            chanceMaxHitDbuffGround,
            maxTimeDbuffGround,
            (user != null ? (user.GetComponent<MobManager>() ? user.GetComponent<MobManager>().chanceCritical + bonusCritical : bonusCritical) : bonusCritical)
            );
    }

    /// <summary>
    /// Cria um raio em volta e coloca os grounds
    /// </summary>
    /// <param name="hitted"></param>
    protected void StaticHit(GameObject hitted)
    {
        if (!active)
            return;

        lastHit = hitted;

        Debug.LogError("<color=green>Ground Effect StaticHit in " + hitted + "</color>");

            GameManagerScenes._gms.NewInfo("<color=green>Ground Effect StaticHit in " + hitted + "</color>", 2, true);

        List<HexManager> listStaticHit = CheckGrid.Instance.RegisterRadioHex(hitted.GetComponent<HexManager>().x, hitted.GetComponent<HexManager>().y, rangeRadio, true, 2);

        foreach (HexManager hexList in listStaticHit)
        {
            if (CheckGround(hexList))
            {
                Debug.LogError("<color=green>Ground StaticHit ground " + hexList.name + "</color>");

                GameManagerScenes._gms.NewInfo("<color=green>Ground StaticHit ground " + hexList.name + "</color>", 2, true);

                ActiveDbuffGround().RespawSkill(
                (user != null ? (user) : gameObject),
                (user != null ? (GetComponent<SkillAttack>() ? GetComponent<SkillAttack>().target : (user.GetComponent<EnemyAttack>() ? user.GetComponent<EnemyAttack>().target : null)) : null),
                hexList,
                _damageDbuffGround,
                _damageWalkDbuffGround,
                maxHitAreaDbuffGround,
                chanceMaxHitDbuffGround,
                maxTimeDbuffGround,
                (user != null ? (user.GetComponent<MobManager>() ? user.GetComponent<MobManager>().chanceCritical + bonusCritical : bonusCritical) : bonusCritical)
                );
            }
        }
    }

    public void CreateStaticHit(int range)
    {
        if (lastHit == null)
        {
            return;
        }

        GameObject hitted = lastHit;

        Debug.LogError("<color=green>CreateStaticHit in " + hitted + "</color>");

        GameManagerScenes._gms.NewInfo("<color=green>CreateStaticHit in " + hitted + "</color>", 2, true);

        List<HexManager> listStaticHit = CheckGrid.Instance.RegisterRadioHex(hitted.GetComponent<HexManager>().x, hitted.GetComponent<HexManager>().y, range, true, 2);

        foreach (HexManager hexList in listStaticHit)
        {
            if (CheckGround(hexList))
            {
                Debug.LogError("<color=green>CreateStaticHit ground " + hexList.name + "</color>");

                    GameManagerScenes._gms.NewInfo("<color=green>CreateStaticHit ground " + hexList.name + "</color>", 2, true);

                ActiveDbuffGround().RespawSkill(
                (user != null ? (user) : gameObject),
                (user != null ? (user.GetComponent<EnemyAttack>() ? user.GetComponent<EnemyAttack>().target : null) : null),
                hexList,
                _damageDbuffGround,
                _damageWalkDbuffGround,
                maxHitAreaDbuffGround,
                chanceMaxHitDbuffGround,
                maxTimeDbuffGround,
                (user != null ? (user.GetComponent<MobManager>() ? user.GetComponent<MobManager>().chanceCritical + bonusCritical : bonusCritical) : bonusCritical)
                );
            }
        }
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (active)
        {
                   GameManagerScenes._gms.NewInfo("<color=green>Ground Effect OnCollisionEnter in " + collision + "</color>", 2, true);
            //Debug.LogError("<color=green>Ground Effect OnCollisionEnter in " + collision + "</color>");
            HitCollider(collision.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active)
        {
            Debug.LogError("<color=green>Ground Effect OnTriggerEnter in " + other + "</color>");
            HitCollider(other.gameObject);
        }
    }

    protected bool CheckGround(HexManager hex)
    {
        if (hex != null)
        {
            if (hex.currentItem != null)
            {
                Debug.LogError("<color=red>CheckWalk has a item ->" + hex + "</color>");

                GameManagerScenes._gms.NewInfo("<color=red>CheckWalk  has a item ->" + hex + "</color>", 2, true);

                return false;
            }
            else
            if (hex.free)
            {
                Debug.LogError("<color=green>CheckWalk ->" + hex + "</color>");

                GameManagerScenes._gms.NewInfo("<color=green>CheckWalk free ->" + hex + "</color>", 2, true);
                return true;
            }
            else
                if (hex.currentMob.GetComponent<MobManager>())
            {
                GameManagerScenes._gms.NewInfo("<color=green>CheckWalk mob ->" + hex + "</color>", 2, true);
                return true;
            }
        }

        GameManagerScenes._gms.NewInfo("<color=red>CheckWalk fail ->" + hex + "</color>", 2, true);

        return false;
    }
}
