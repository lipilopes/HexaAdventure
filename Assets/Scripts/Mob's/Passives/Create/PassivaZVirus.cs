using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivaZVirus : PassiveManager
{
    [Space]
    [Header("Z Virus")]
    //[SerializeField, Tooltip("Ja cria X prefabs por padrão")]
    //protected int            _createDefault;
    [SerializeField, Tooltip("Prefab do clone $P0")]
    protected GameObject     _prefab;
    [Space]
    [SerializeField]
    protected ParticleSystem _effect;
    [Space]
    [SerializeField]
    protected int _createDefault = 3;
    [Space]
    [SerializeField, Tooltip("Tempo de vida dos Clones,-1 para ser ate a vida acabar  $P1")]
    protected int _maxTimeClone = -1;
    [SerializeField, Tooltip("dano do mob -1 para pegar do pai $P2")]
    protected int _damage = -1;
    [Space]
    [SerializeField, Tooltip("Vida do mob -1 para pegar do pai $P3")]
    protected int _health = -1;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de dano com base no 'pai' $P2")]
    protected int minPorcentDamage = 2;
    [SerializeField, Range(0, 100)]
    protected int maxPorcentDamage = 5;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de vida com base no 'pai' $P3")]
    protected int minPorcentHealth = 2;
    [SerializeField, Range(0, 100)]
    protected int maxPorcentHealth = 5;
    [Space]
    [SerializeField, Tooltip("Maximo de clones ativos, -1 para ilimitado  $P4")]
    protected int _maxClonesActive   = -1;
    [SerializeField, Tooltip("Apos Morrer Clone sera Desativado")]
    protected bool _desactiveClones = true;
    [Space]
    [SerializeField, Tooltip("Caso for o player controla o clone")]
    protected bool _controlClone = true;

    protected List<ClonesControl> clones = new List<ClonesControl>();

    //int teste = 0;

    protected override void Start()
    {
        //if (_AttDescriptonOnEnable)        
        //    _AttDescriptonOnEnable = false;        

        print("START " + name);

        if (GetUserStatus())
            base.Start();
    }

    protected virtual bool GetUserStatus()
    {
        print("GetUserStatus");

        GetUser();

        if (User == null)
            return false;

        print("GetUserStatus -> User!=null -> " + User.name);

        GameManagerScenes._gms.NewInfo(name + " user é " + User.name, 3, true);

        if (_damage == -1)
            _damage = User.GetComponent<MobManager>().damage;

        if (_health == -1)
            _health = (int)User.GetComponent<MobManager>().health;

        if (_createDefault > 0)
            CreateClone(_createDefault);

        return true;
    }

    protected override void DescriptionPost()
    {
        if (descriptionPost || User == null || _AttDescriptonOnTooltip)
            return;

        AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

        AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

        if (_prefab != null)
        {
            if (_prefab.GetComponent<ToolTipType>())
                AttDescription("$P0", "<b>" + _prefab.GetComponent<ToolTipType>()._name + "</b>");
            else
                AttDescription("$P0", "<b>" + _prefab.name + "</b>");
        }
        else
            AttDescription("$P0", "<b>"+XmlMenuInicial.Instance.Get(77)+"</b>");//Clone

        AttDescription("$P1", _maxTimeClone == -1 ?
            "<b>" + XmlMenuInicial.Instance.Get(78)+ "</b>"/*"Até morrer"*/ : 
            "<b>" + _maxTimeClone + "</b>");

        if (minPorcentDamage != maxPorcentDamage)
            AttDescription("$P2", "<b>" + (_damage * minPorcentDamage / 100).ToString("F0") + " - " + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");
        else
            AttDescription("$P2", "<b>" + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");

        if (minPorcentHealth != maxPorcentHealth)
            AttDescription("$P3", "<b>" + (_health * minPorcentHealth / 100).ToString("F0") + " - " + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");
        else
            AttDescription("$P3", "<b>" + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");

        AttDescription("$P4", _maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)/*Ilimitado*/+"</b>" : "<b>" + _maxClonesActive + "</b>");
   

        _Description = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

        if (_mobSkillManager && !_AttDescriptonOnTooltip)
        {
            _mobSkillManager.Description += _Description;
            descriptionPost = true;
        }
    }

    public override string DescriptionToolType
    {
        get
        {
            string _r = "";

            if (User == null || !_AttDescriptonOnTooltip)
                return _r;

            _Description = XmlMobPassive.Instance.GetDescription(_XmlID);


            AttDescription("$%", "<b>" + _chanceActivePassive + "%</b>");

            AttDescription("$User", User.GetComponent<ToolTipType>() ? "<b>" + User.GetComponent<ToolTipType>()._name + "</b>" : "");

            if (_prefab != null)
            {
                if (_prefab.GetComponent<ToolTipType>())
                    AttDescription("$P0", "<b>" + _prefab.GetComponent<ToolTipType>()._name + "</b>");
                else
                    AttDescription("$P0", "<b>" + _prefab.name + "</b>");
            }
            else
                AttDescription("$P0", "<b>" + XmlMenuInicial.Instance.Get(77) + "</b>");//Clone

            AttDescription("$P1", _maxTimeClone == -1 ?
                "<b>" + XmlMenuInicial.Instance.Get(78) + "</b>"/*"Até morrer"*/ :
                "<b>" + _maxTimeClone + "</b>");

            if (minPorcentDamage != maxPorcentDamage)
                AttDescription("$P2", "<b>" + (_damage * minPorcentDamage / 100).ToString("F0") + " - " + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");
            else
                AttDescription("$P2", "<b>" + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");

            if (minPorcentHealth != maxPorcentHealth)
                AttDescription("$P3", "<b>" + (_health * minPorcentHealth / 100).ToString("F0") + " - " + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");
            else
                AttDescription("$P3", "<b>" + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");

            AttDescription("$P4", _maxClonesActive == -1 ? "<b>" + XmlMenuInicial.Instance.Get(79)/*Ilimitado*/+ "</b>" : "<b>" + _maxClonesActive + "</b>");

            _r = "<i><b>" + XmlMenuInicial.Instance.Get(67)/*PASSIVA*/+ "</b>: <color=red>" + _Nome + "</color>\n" + _Description + "</i>";

            if (GetComponent<ToolTipType>())
                GetComponent<ToolTipType>()._passiveDesc = _r;

            return _r;
        }
    }

    public override void AttDescription()
    {
        base.AttDescription();

         if (_prefab != null)
        {
            if (_prefab.GetComponent<ToolTipType>())
                AttDescription("$P0", "<b>" + _prefab.GetComponent<ToolTipType>()._name + "</b>");
            else
                AttDescription("$P0", "<b>" + _prefab.name + "</b>");
        }
        else
            AttDescription("$P0", "<b>"+XmlMenuInicial.Instance.Get(77)+"</b>");//Clone

        AttDescription("$P1", _maxTimeClone == -1 ?
            "<b>" + XmlMenuInicial.Instance.Get(78)+ "</b>"/*"Até morrer"*/ : 
            "<b>" + _maxTimeClone + "</b>");

        if (minPorcentDamage != maxPorcentDamage)
            AttDescription("$P2", "<b>" + (_damage * minPorcentDamage / 100).ToString("F0") + " - " + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");
        else
            AttDescription("$P2", "<b>" + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");

        if (minPorcentHealth != maxPorcentHealth)
            AttDescription("$P3", "<b>" + (_health * minPorcentHealth / 100).ToString("F0") + " - " + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");
        else
            AttDescription("$P3", "<b>" + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");

        AttDescription("$P4", _maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)/*Ilimitado*/+"</b>" : "<b>" + _maxClonesActive + "</b>");
    }

    protected virtual GameObject CreateClone(int create=1)
    {
        int current = 0;

        Debug.LogError("CreateClone("+create+") - User ("+User+")");

        MobManager mobM = User.GetComponent<MobManager>();

        while (create > current)
        {
            GameObject obj = null;

          /*  if (RespawMob.Instance != null)
                obj = RespawMob.Instance.CreateMob(_prefab, User.GetComponent<MoveController>().hexagonX, User.GetComponent<MoveController>().hexagonY, false, false, mobM.TimeMob);
            else*/
            {
                obj = Instantiate(_prefab, User.transform.position, User.transform.rotation);

                obj.GetComponent<MobManager>().classe = MobManager.Classe.manual;

                obj.GetComponent<MobManager>().TimeMob = mobM.TimeMob;

                obj.tag = "Clone";

                obj.AddComponent<EffectStigma>();
                obj.GetComponent<EffectStigma>().desactiveThis = _desactiveClones;
            }

            obj.GetComponent<MobManager>().getBonusPlayer = false;

            if (obj.GetComponent<PassivaZVirus>())
                Destroy(obj.GetComponent<PassivaZVirus>());

            Debug.LogError("CreateClone (" + (current + 1) + "/" + create + ") Criado " + obj.name);

            obj.name = _prefab.name + " " + (current + 1) + "  (" + User.GetComponent<ToolTipType>()._name + ")";

            ToolTipType tt = obj.GetComponent<ToolTipType>();

            if (tt != null)
            {
                if (RespawMob.Instance!=null && mobM.TimeMob == (RespawMob.Instance.PlayerTime))
                {
                    tt._name = obj.GetComponent<ToolTipType>()._name+" "+ XmlMenuInicial.Instance.Get(77)/*Clone*/;
                }
                else
                {
                    tt._name = obj.GetComponent<ToolTipType>()._name;

                    if (obj.GetComponentInChildren<Effects>() && (int)GameManagerScenes._gms.Dificuldade() >= 2)
                        obj.GetComponentInChildren<Effects>().gameObject.SetActive(false);
                }
            }

            ClonesControl c = new ClonesControl();

            c._clone = obj;
            c._time  = _maxTimeClone;

            clones.Add(c);

            c._clone.GetComponent<MoveController>().hexagonX = -1;

            c._clone.GetComponent<MoveController>().hexagonY = -1;

            c._clone.transform.position = new Vector3(0, -999, 0);
            
            c._clone.GetComponent<MobHealth>().MaxHealth = (_health * Random.Range(minPorcentHealth, maxPorcentHealth + 1)) / 100;

            c._clone.GetComponent<MobHealth>().Health = c._clone.GetComponent<MobHealth>().MaxHealth;

            c._clone.GetComponent<MobManager>().health = c._clone.GetComponent<MobHealth>().MaxHealth;

            if (mobM.isPlayer)
            {               
                if (_controlClone)
                {
                    c._clone.GetComponent<MobManager>().isPlayer = true;

                    c._clone.AddComponent<PlayerControl>();
                }
            }

            c._clone.GetComponent<IaAttackMob>().AttAttack();

            c._clone.GetComponent<MobHealth>().HitKill(null,false);

            if (RespawMob.Instance)
                RespawMob.Instance.allRespaws.Add(c._clone);

            current++;
        }

        //FindClone();

        return clones[clones.Count-1]._clone;
    }

    public override void StartPassive(GameObject target, params Passive[] passive)
    {
        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        foreach (var i in passive)
        {
            if (i == Passive.EndTurn)
            {
                TimerClones();
            }

            if (i == Passive.Kill)
            {
                KillAllClones();
                break;
            }

            foreach (var y in _Passive)
            {
                if (y._startPassive == i)
                {
                    Debug.LogError("StartPassiveZVirus("+target+")");

                    if (ChanceActivePassive() && CountClonesActives() <= _maxClonesActive)
                    {
                        if (target.GetComponent<MoveController>())
                        {
                            EffectManager.Instance.PopUpDamageEffect(_Nome+" Active", User);
                            StartCoroutine(RespawClone(target.GetComponent<MoveController>().Solo));
                        }
                    }
                    else
                        EffectManager.Instance.PopUpDamageEffect(_Nome+" Maximo", User);
                }               
           }          
        }
    }
 
    /// <summary>
    /// Cria clone no jogo
    /// </summary>
    /// <param name="_hex"></param>
    protected virtual IEnumerator RespawClone(HexManager _hex)
    {
        Debug.LogError("RespawClone(" + _hex + ")");

        if (CountClonesActives() >= _maxClonesActive)
        {
            Debug.LogError("RespawClone(" + _hex + ") -> Maximo de clones alcançado");
            yield break;
        }

        while (_hex.free==false)
        {
            Debug.LogError("RespawClone(" + _hex + ") -> Não esta free");
            yield break;
        }

        CooldownReset();

        GameManagerScenes._gms.NewInfo(_Nome+" RespawClone("+_hex+")",3,true);

        GameObject clone = FindClone();

        while (clone.GetComponent<MobHealth>().MaxHealth <= 0)
        {
            clone = FindClone();
        }

        _hex.free       = false;
        _hex.currentMob = clone;
        _hex.WalkInHere();

        clone.GetComponent<MoveController>().hexagonX = _hex.x;
        clone.GetComponent<MoveController>().hexagonY = _hex.y;

        clone.transform.position = _hex.transform.position;

        if (TurnSystem.Instance!=null)
        {
            GameObject t = TurnSystem.Instance.GetRandomMob(User.GetComponent<MobManager>().TimeMob);

            if (t != null)
                clone.transform.LookAt(t.transform.position);
            else
                clone.transform.LookAt(User.transform.position);
        }
        
        if (_effect != null)
        {
            _effect.transform.position = clone.transform.position;
            _effect.gameObject.SetActive(true);
            _effect.GetComponentInChildren<ParticleSystem>().Play(true);
        }
        else
            EffectManager.Instance.RespawEffect(clone, 3);

        clone.GetComponent<MobHealth>().ReBorn(/*_controlClone*/true);

        if (!clone.GetComponent<MobManager>().Alive || clone.GetComponent<MobHealth>().Health<=0)
        {
            clone.GetComponent<MobHealth>().HitKill(null,false);

            StartCoroutine(RespawClone(_hex));
        }
        else
        EffectManager.Instance.PopUpDamageEffect("<color=green>" + CountClonesActives(false) + "</color>/" + (_maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)/*Ilimitado*/+"</b>" : _maxClonesActive+""), clone);
    }

    /// <summary>
    /// Procura um Clone não ativo
    /// </summary>
    /// <returns></returns>
    protected virtual GameObject FindClone()
    {
        GameObject clone = null;

        Debug.LogError("FindClone()");

        foreach (var c in clones)
        {
            if (!c._clone.GetComponent<MobHealth>().Alive || c._time <= 0)
            {
                clone   = c._clone;

                c._time = _maxTimeClone;

                Debug.LogError("FindClone() -> "+c);
                break;
            }
        }

        if (clone == null)
            clone = CreateClone();

        if (_damage == -1)
            _damage = User.GetComponent<MobManager>().damage;

        if (_health == -1)
            _health = (int)User.GetComponent<MobManager>().health;

        int porcentDamage = Random.Range(minPorcentDamage, maxPorcentDamage + 1),
            porcentHealth = Random.Range(minPorcentHealth, maxPorcentHealth + 1);

        if (GameManagerScenes._gms!=null && GameManagerScenes._gms.Adm && InfoTable.Instance!=null)
        {
            InfoTable.Instance.NewInfo(clone.name + " Damage("+_damage+")[" + porcentDamage + "% - " + ((_damage * porcentDamage) / 100) + "]", 10);
            InfoTable.Instance.NewInfo(clone.name + " Health("+_health+")[" + porcentHealth + "% - " + ((_health * porcentHealth) / 100) + "]", 10);
        }
               
        int  health                               = (_health * porcentHealth) / 100;

        clone.GetComponent<MobManager>().health = health;

        clone.GetComponent<MobManager>().damage = (_damage * porcentDamage / 100);

        //clone.GetComponent<MobHealth>().MaxHealth = health;

        clone.GetComponent<MobHealth>().StartHealth(health);

        clone.GetComponent<SkillManager>().AttDamageSkills();

        clone.GetComponent<EffectStigma>().StartStigma(null,User,clone,_maxTimeClone,false);

        if (health != 0)
            if (clone.GetComponent<MobHealth>().MaxHealth <= 0 ||
               clone.GetComponent<MobManager>().health <= 0    ||
               clone.GetComponent<MobManager>().damage <= 0 && _damage>0)
            {
                Debug.LogError("FindClone() reStart clone dont att status - >" + clone);
                return FindClone();
            }

        Debug.LogError("FindClone() return - >" + clone);

        return clone;
    }

    /// <summary>
    /// Contador de tempo dos clones
    /// </summary>
    protected virtual void TimerClones()
    {
      /*  if (clones.Count > 0 && _maxTimeClone>0)
        {
            Debug.LogError("TimerClones()");

            for (int index = 0; index < clones.Count; index++)
            {
                Debug.LogError(User.name + " clones[" + index + "]");

                if (clones[index]._clone.activeInHierarchy && clones[index]._clone.GetComponent<MobHealth>().Alive && clones[index]._time >= 1)
                {
                    clones[index]._time--;

                    Debug.LogError("DeadClone(" + index + ") - time " + clones[index]._time);

                    if (clones[index]._time <= 0)
                    {
                        EffectManager.Instance.PopUpDamageEffect("PUFF", clones[index]._clone);

                        clones[index]._clone.GetComponent<MobHealth>().HitKill(clones[index]._clone, false);
                    }
                    else
                        if (User.GetComponent<MobManager>().TimeMob == (RespawMob.Instance.PlayerTime))
                        EffectManager.Instance.PopUpDamageEffect((_maxTimeClone - clones[index]._time) + "/" + _maxTimeClone, clones[index]._clone);

                    if (_effect != null)
                    {
                        _effect.gameObject.SetActive(false);
                        _effect.GetComponentInChildren<ParticleSystem>().Play(false);
                    }
                }
            }
        }*/
    }

    /// <summary>
    /// Quando user Morre clones tbm morrem
    /// </summary>
    protected virtual void KillAllClones()
    {
     /*   if (_desactiveClones)
        {
            Debug.LogError("KillAllClones()");

            for (int index = 0; index < clones.Count; index++)
            {
                if (clones[index] != null && clones[index]._clone.GetComponent<MobHealth>().Alive)
                {
                    EffectManager.Instance.PopUpDamageEffect("PUFF", clones[index]._clone);

                    clones[index]._clone.GetComponent<MobHealth>().HitKill(clones[index]._clone, false);
                }
            }
        }
        */
    }

    protected  int CountClonesActives(bool rule=true)
    {
       int count = 0;

        if (_maxClonesActive==-1 && rule)
        {
            count = -99;
        }
        else
        foreach (var c in clones)
        {
            if (c._clone.GetComponent<MobHealth>().Alive)
            {
                count++;
            }
        }

        Debug.LogError("CountClonesActives(rule) -> "+ count);

        return count;
    }

    public void TesteStatusClones()
    {
        FindClone().GetComponent<MobHealth>().ReBorn(_controlClone);
    }
}