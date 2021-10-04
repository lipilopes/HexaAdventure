using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OthersTarget
{
    public GameObject _target;
    public int        _hit;
}

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class SkillAttack : MonoBehaviour
{   
   protected MobSkillManager   Skill;
   public    MobSkillManager   SkillManager { get { return Skill;} set { Skill = value; } }

    [Tooltip("Modelo/Particula da skill")]
    public GameObject Fx;
    [Header("Props")]
    [ContextMenuItem("Testar Velocidade", "TestVelocidade")]
    public float velocidade;

    protected float vel;

    [SerializeField]
    float timeDestroy;
    public float Duration { get { return timeDestroy; } set { timeDestroy = value; } }

    [SerializeField]
    protected int _maxHit = 1;
    public    float MaxHit { get { return _maxHit; } }

    protected int _currentHit = 0;

    //[HideInInspector]
    public int damage;

    [HideInInspector]
    public float critical;

    //[HideInInspector]
    public GameObject target;

    /// <summary>
    /// Quem esta lançando a skill
    /// </summary>
    //[HideInInspector]
    public GameObject who; 
       
    [HideInInspector]
    public int skill = -1;
    

    [HideInInspector]
    public Rigidbody rb;

    protected bool mirado = false;
    [Header("On hit Target")]
    [Tooltip("Acaba turn ao colidir")]
    public bool endTurn     = true;
    public bool positionWho = true;
    public bool rotationWho = true;

    [Header("Effects")]
    [Tooltip("Skill Da Dano")]
    [SerializeField]
    protected bool _takeDamage = true;
    public    bool TakeDamage { get { return _takeDamage; } set { _takeDamage = value; } }

    [SerializeField,Tooltip("Dano da skill é dano real")]
    protected bool _takeRealDamage = false;
    public   bool TakeRealDamage { get { return _takeRealDamage; } set { _takeRealDamage = value; } }

    [SerializeField, Tooltip("Dano da skill Ativa Passiva do inimigo")]
    protected bool _damageCountPassive = true;
    public    bool DamageCountPassive { get { return _damageCountPassive; } set { _damageCountPassive = value; } }
    [Space]
    [Tooltip("Skill Da Dano em Outros inimigos")]
    [SerializeField]
    protected bool _areaDamage;
    protected bool _currentAreaDamage;
    public    bool AreaDamage { set { _currentAreaDamage = value; } get { return _currentAreaDamage; } }

    [SerializeField, Tooltip("Dano em area da skill é dano real")]
    protected bool _areaRealDamage=false;
    public    bool AreaRealDamage { get { return _areaRealDamage; } set { _areaRealDamage = value; } }

    [Tooltip("Skill aplica dbuff da skill em Outros inimigos")]
    [SerializeField]
    protected bool _otherTagertDbuff = true;
    public    bool OtherTargetDbuff { set { _otherTagertDbuff = value; } get { return _otherTagertDbuff; } }

    //
    [Range(0, 100), Tooltip("% do dano da skill em outros inimigos")]
    [SerializeField]
    protected float _damageAreaDamage;
    public    float DamageAreaDamage { set { if (value < 0) value = 0; _damageAreaDamage = value; } get { return _damageAreaDamage; } }

    [Space]
    [Header("MultTarget")]
    [SerializeField,Tooltip("Quando skill recomeçar começa de quem a usou")]
    protected bool _startMultTargetInUser = true;
    [SerializeField, Tooltip("Maximos de kicks que pode fazer")]
    protected int _maxMultTargetHit = 3;
    [SerializeField, Range(1, 6), Tooltip("Range para o kick")]
    protected int _rangeMultTarget = 1;
    [SerializeField, Range(0, 100), Tooltip("Reduz dano em inimigo q ja levou kick")]
    protected int _reduzMultTargetHitDamage = 10;
    [SerializeField]
    protected List<OthersTarget> _multTargetList = new List<OthersTarget>();
    public    List<OthersTarget> MultTargetList { get { return _multTargetList; } }

    //Target da Vez
    protected int _multTargetTarget = 0;

    /// <summary>
    /// Dano Calculado da AreaDamage
    /// </summary>
    int TotalDamageAreaDamage
    {
        get
        {
            return (int)(damage * DamageAreaDamage) / 100;
        }
    }

    protected static WaitForSeconds _Time = new WaitForSeconds(10);

    protected List<OthersTarget> _otherTarget = new List<OthersTarget>();

    protected WaitForSeconds waitHitTarget = new WaitForSeconds(4);

    protected bool _hit = false;

    protected WaitForSeconds waitDesative;

    [SerializeField,Space]
    protected UnityEvent startSkillEvent;
    [SerializeField]
    protected UnityEvent hitMainTargetEvent;
    [SerializeField]
    protected UnityEvent endSkillEvent;

    protected virtual void Start()
    {
        if (timeDestroy == 0)
            timeDestroy = 0.2f;

        waitDesative = new WaitForSeconds(timeDestroy);
    }

    protected virtual void OnEnable()
    {
        if(rb!=null)
        rb.Sleep();
        ResetOtherTarget();

        waitDesative = new WaitForSeconds(timeDestroy);

        _hit = false;
        _currentHit = 0;

        StopCoroutine(DesactiveCouroutine());
        StopCoroutine(WaitHitTargetCoroutine());
        StartCoroutine(WaitHitTargetCoroutine());

        vel = velocidade;

        AreaDamage = _areaDamage;

        if (_otherTarget.Count > 0)
            _otherTarget.Clear();

            
        //StartCoroutine(UpdateCoroutine());
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
    }

    WaitForSeconds update = new WaitForSeconds(3);
    protected virtual IEnumerator UpdateCoroutine()
    {
        while (gameObject.activeSelf)
        {
            if (gameObject.activeSelf && mirado)
            {
                rb.velocity = (transform.forward) * (2+vel);
                transform.LookAt(target.transform);
            }

            if (!gameObject.activeSelf && mirado)
                mirado = false;

            if (gameObject.activeSelf && target != null)
                if (!target.activeSelf)
                    Desactive();

            yield return update;
            ///Caso erre o alvo
            mirado = true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_who"></param>
    /// <param name="_damage"></param>
    /// <param name="_target"></param>
    /// <param name="_dbuff">Recuar = -2, Fire = 0 , Poison = 1 , Petryf = 2,Stun = 3,Bleed = 4</param>
    /// <param name="_dbuffChance"></param>
    /// <param name="_dbuffDuracao"></param>
    public virtual void UseSkill(GameObject _target,MobSkillManager _skill)
    {
        if (GetComponent<ShakeCamera>())
            GetComponent<ShakeCamera>().StartEffect();

        who = _skill.User;

        Debug.LogError(who.name + " UseSkill() -> " + _target);      

        Skill = _skill;

        mirado = false;

        target = _target;


        gameObject.SetActive(true);


        if (positionWho)
            transform.position = who.GetComponent<MoveController>().Solo.transform.position;

        if (rotationWho)
        {
            if (target != null)
                transform.LookAt(target.transform.position);
            else
                transform.rotation = (who.transform.rotation);

        }

        Debug.LogWarning("<color=magenta>Skill [" + name + "] comecou na position = " + (transform.position == Skill.User.transform.position) + " rotation[" + (transform.rotation == Skill.User.transform.rotation) + "]</color>");


        //if (positionWho)
        //    transform.position = who.transform.position;

        //if (rotationWho)
        //    transform.LookAt(target.transform.position);


        if (who.GetComponent<MobManager>())
            critical = who.GetComponent<MobManager>().chanceCritical;

        if (rb == null)
            rb = this.GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * (velocidade*2));

        mirado = true;

        _currentHit = 0;

        startSkillEvent.Invoke();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Mob" || other.gameObject.layer == 8)
        Debug.LogError(name+ " OnTriggerEnter -> " + other.gameObject);


        if (other.GetComponent<HexManager>())
        {
            if (other.GetComponent<HexManager>().currentMob != null &&
                other.GetComponent<HexManager>().currentMob.GetComponent<MobManager>() &&
                other.GetComponent<HexManager>().currentMob.GetComponent<MobManager>().Alive)
            {
                Debug.LogError(name + " OnTriggerEnter() [on Hex] " + other.gameObject);
                HittedSkill(other.GetComponent<HexManager>().currentMob);
            }
        }
        else
            HittedSkill(other.gameObject);
    }

    /// <summary>
    /// Take Damage
    /// </summary>
    /// <param name="hitted">Who take Damage</param>
    protected virtual void HittedSkill(GameObject hitted)
    {
        if (hitted == null)
            return;

        Debug.LogError("HittedSkill() -> Main Target is " + target.name + "/" + hitted.name);

        if (AreaDamage && Contains(hitted) && //Skill da dano em area e hitted exist na lista
            !OtherTargetCanHit(hitted) && hitted != target)//hitted ja atingiu o maximo de hit levados e ele nao é o alvo principal
            return;

        MobHealth mobHealth = hitted.GetComponent<MobHealth>();

        if (hitted != target)//Não é o alvo principal
        {           
            if (AreaDamage && Contains(hitted) && OtherTargetCanHit(hitted) || //Skill da dano em area e alvo nao existe e pode levar hit
                AreaDamage && !Contains(hitted))//Skill em area e alvo nao esta na lista
            {
                MobManager mm = hitted.GetComponent<MobManager>();

                if (mm != null)
                {
                    bool friend = who.GetComponent<MobManager>().MesmoTime(mm.TimeMob);
                    if (!friend ||//Não sao do msm time
                        friend && Skill.TargetFriend || //São do msm time e skill libera isso
                        hitted == who && Skill.TargetMe)//Quem usou a skill e skill libera isso
                    {
                        if (!OtherTargetHit(hitted))//Aumenta hit do alvo na lista
                            return;

                        if (Skill != null && OtherTargetDbuff)//Skill aplica dbuff em alvo não principal
                            Skill.Hit(false, hitted);

                        if (!friend)
                        {
                            if (mobHealth != null && TakeDamage && damage > 0)//skill da dano
                                if (_areaRealDamage)
                                    mobHealth.RealDamage(who, TotalDamageAreaDamage, critical, _damageCountPassive);
                                else
                                    mobHealth.Damage(who, TotalDamageAreaDamage, critical, _damageCountPassive);
                        }                      
                       
                        Debug.LogError(hitted + " levou hit do " + name);
                    }
                }
            }
        }
        else
            if (OtherTargetHit(hitted) && hitted == target)//Alvo principal
        {
            mirado = false;

            if (!_hit)//Não levou todos os hit maximos
            {
                int _damage = damage;

                // MultTarget
                if (AttMultTarget() && target != MultTargetList[_multTargetTarget]._target)//Existem alvos na lista e Target não é o primeiro
                {
                    return;
                }
                else
                if(MultTargetList.Count>=1)
                {
                    int _reduzDamage = MultTargetList[_multTargetTarget]._hit <= 0 ? 1 : MultTargetList[_multTargetTarget]._hit;

                    Debug.LogError(name + ": dano é " + _damage + " - " + _reduzDamage + "%");

                    _damage -= (_damage * (_maxMultTargetHit * _reduzDamage)) / 100;

                    Debug.LogError(name + ": dano atualizado é " + _damage);
                }

                _currentHit++;

                Hit();

                if (TakeDamage && mobHealth != null && _damage > 0)
                {
                    if (!who.GetComponent<MobManager>().MesmoTime(mobHealth.gameObject))
                    {
                        if (_takeRealDamage)
                            mobHealth.RealDamage(who, _damage, critical, _damageCountPassive);
                        else
                            mobHealth.Damage(who, _damage, critical, _damageCountPassive);
                    }
                    
                }                
            }

        }
    }

    protected virtual IEnumerator DesactiveCouroutine()
    {
        Debug.LogError(name + " TimerDesactive()");

        StopCoroutine(WaitHitTargetCoroutine());

        yield return waitDesative;

        Desactive();
    }

    public virtual void Desactive()
    {
        endSkillEvent.Invoke();

        if (positionWho)
            transform.position = new Vector3(who.transform.localPosition.x, transform.position.y, who.transform.localPosition.z);

        transform.rotation = new Quaternion(0, 0, 0, 0);

        //caso isso bug apaga
        if (GetComponent<EnemyAttack>() != null && endTurn)
            GetComponent<EnemyAttack>().StartAttackTurn();
        else
        if (GetComponent<EnemyAttack>() == null && endTurn)
            who.GetComponent<MobManager>().EndTurn();

        StopAllCoroutines();

        _hit = false;
        _currentHit = 0;

        if (_otherTarget.Count > 0)
            _otherTarget.Clear();

        this.gameObject.SetActive(false);

        Debug.LogError(who.name + " TimerDesactive() -> " + name);
    }

    protected virtual void Hit()
    {
        if (!AttMultTarget()  && _currentHit <= MaxHit ||
             AttMultTarget()  && _currentHit <= _maxMultTargetHit)
        {
            hitMainTargetEvent.Invoke();

            Debug.LogError(name + " Hit()");

            if (target.GetComponent<MobManager>())
                target.transform.LookAt(who.transform);

            if (Skill != null)
                //who.GetComponent<SkillManager>().HitSkill(target);
            Skill.Hit(false,target);

            if (AttMultTarget())
            {
                 NextMultTarget();

                UseSkill(MultTargetList[_multTargetTarget]._target, Skill);

            }
            else
            if (_currentHit == MaxHit)
                _hit = true;

            if (_hit)
                StartCoroutine(DesactiveCouroutine());
        }
    }

    protected IEnumerator TimerOverSkillCoroutine()
    {
        yield return _Time;

        Debug.LogError(name + " TimerOverSkillCoroutine()");

        Desactive();
    }

    /// <summary>
    /// Go existe na lista do OtherTarget
    /// </summary>
    /// <param name="Go"></param>
    /// <returns></returns>
    protected bool Contains(GameObject Go)
    {       
        foreach (var item in _otherTarget)
            if (item._target == Go)
                return true;

        return false;
    }

    /// <summary>
    /// Limpa lista de OtherTarget
    /// </summary>
    public void ResetOtherTarget()
    {
        _otherTarget.Clear();
    }

    /// <summary>
    /// Outro target Que ainda pode levar hit
    /// </summary>
    /// <param name="Go"></param>
    /// <returns></returns>
    protected bool OtherTargetCanHit(GameObject Go)
    {
        foreach (var item in _otherTarget)
        {
            if (item._target != who)                     
            if (item._target == Go && item._hit < MaxHit)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Aumenta os hits do OtherTarget
    /// </summary>
    /// <param name="Go"></param>
    /// <returns></returns>
    protected bool OtherTargetHit(GameObject Go)
    {
        if (!Contains(Go))
        {

            OthersTarget T = new OthersTarget();

            T._target = Go;
            T._hit++;

            _otherTarget.Add(T);

            return true;
        }
        else
        {
            if (OtherTargetCanHit(Go))
            {
                foreach (var item in _otherTarget)
                {
                    if (item._target != who)
                        if (item._target == Go)
                    {
                        item._hit++;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Retira os alvos mortos da lista
    /// </summary>
    /// <returns>Existem alvos na lista</returns>
    protected bool AttMultTarget()
    {
        int count = _multTargetList.Count;

        if (count >= 1)
            for (int i = 0; i < count; i++)
            {
                if (!_multTargetList[i]._target.GetComponent<MobManager>().Alive)
                {
                    _multTargetList.Remove(_multTargetList[i]);

                    return AttMultTarget();
                }
            }

        return count > 0;
    }

    /// <summary>
    /// Go existe na lista do MultTarget
    /// </summary>
    /// <param name="Go"></param>
    /// <returns></returns>
    protected bool MultContains(GameObject Go)
    {
        foreach (var item in MultTargetList)
            if (item._target == Go)
                return true;

        return false;
    }

    /// <summary>
    /// Aumenta os hits do MultTarget
    /// </summary>
    /// <param name="Go"></param>
    /// <returns></returns>
    protected bool MultOtherTargetHit(GameObject Go)
    {
        if (!MultContains(Go))
        {
            //OthersTarget T = new OthersTarget();

            //T._target = Go;
            //T._hit++;

            //_otherTarget.Add(T);

            return false;
        }
        else
        {
            if (OtherTargetCanHit(Go))
            {
                foreach (var item in MultTargetList)
                {
                    if (item._target != who)
                        if (item._target == Go)
                    {
                        item._hit++;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Pega o proximo da lista ou busca algum novo
    /// </summary>
    protected void NextMultTarget()
    {
        int count = _multTargetList.Count;
        int index;
        for (int i = 0; i < count; i++)
        {
            if (i == _multTargetTarget)
            {

                index = i++;

                if (index >= count &&
                    _currentHit <= MaxHit)//Acabou a lista, procura Alvos proximos
                {
                    index = FindNewMultTarget(i);

                    if (_multTargetTarget != index)
                    {
                        _multTargetTarget = index;
                        return;
                    }
                }

                _multTargetTarget = index;
                return;
            }
        }
    }

    protected int FindNewMultTarget(int currentTarget)
    {
        int        _return     = _multTargetTarget;
        float      _currentHp = -1;

        GameObject gO =null;

     List<HexManager> g =   CheckGrid.Instance.RegisterRadioHex(
         MultTargetList[_multTargetTarget]._target.GetComponent<MoveController>().hexagonX,
         MultTargetList[_multTargetTarget]._target.GetComponent<MoveController>().hexagonY,
         _rangeMultTarget, 
         true,
         1,
         true);

        foreach (var i in g)
        {
            if (!SkillManager.GetComponent<MobManager>().MesmoTime(i.currentMob))
            {
                MobHealth h = i.currentMob.GetComponent<MobHealth>();

                if (_currentHp==-1)
                {
                    gO = i.currentMob;
                }
                else
                if (h.Health <= _currentHp)
                {
                    gO = i.currentMob;

                    _currentHp = h.Health;
                }
            }
        }

        if (gO!=null)
        {
            OthersTarget T = new OthersTarget();

            T._target = gO;
            T._hit    = 0;

            MultTargetList.Add(T);

            _return = MultTargetList.Count-1;
        }

        return _return;
    }

    protected  virtual IEnumerator WaitHitTargetCoroutine()
    {
        while (gameObject.activeSelf)
        {
            yield return waitHitTarget;

            if (target != null && !_hit && vel<9000)
            {
                if (velocidade == 0)
                    AreaDamage = false;

                //vel++;

                //vel *= 2;

                if (!mirado && target != null)
                    transform.LookAt(target.transform.position);

                //rb.AddForce(transform.forward * velocidade);

                StartCoroutine(WaitHitTargetCoroutine());
            }
        }      
    }


    void TestVelocidade()
    {
        vel = velocidade;
        GetComponent<Rigidbody>().velocity = (transform.forward) * (2 + vel);
    }
}
