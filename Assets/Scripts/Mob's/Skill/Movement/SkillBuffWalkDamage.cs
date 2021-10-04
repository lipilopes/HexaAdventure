using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBuffWalkDamage : SkillBuffWalk
{
    [Space]
    [Header("Damage")]
    [SerializeField, Tooltip("_prefab Attack")]
    protected GameObject _prefabSkillWalk;
    protected SkillAttack skillAttackWalk;

    [SerializeField, Tooltip("Distancia do attack $P1")]
    protected int _rangeAttack=1;

    [SerializeField, Tooltip("Attack e lançado")]
    protected bool _shootAttack = false;


    [SerializeField, Tooltip("Range para o script SkillAttackRange, $SAR1")]
    protected int rangeSkillAttackRangeWalk = -1;
    
    protected List<HexManager> _rangeAttackList = new List<HexManager>();

    /// <summary>
    /// Já Atacou
    /// </summary>
    protected bool _attack = false;

    public override void AttDamageAndDescription()
    {
        base.AttDamageAndDescription();
        if (skillAttackWalk != null)
        {
            skillAttackWalk.damage = currentdamage;
            skillAttackWalk.who = User;
            skillAttackWalk.AreaDamage = AreaDamage;
            skillAttackWalk.DamageAreaDamage = DamageAreaDamage;
            skillAttackWalk.OtherTargetDbuff = OtherTargetDbuff;
            skillAttackWalk.skill = Skill;
            skillAttackWalk.SkillManager = this;
            skillAttackWalk.TakeRealDamage = RealDamage;
            skillAttackWalk.AreaRealDamage = AreaRealDamage;
            skillAttackWalk.DamageCountPassive = DamageCountPassive;

            if (skillAttackWalk.GetComponent<SkillAttackRange>())
                skillAttackWalk.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRangeWalk;
        }
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        description += GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(155), "" + _rangeAttack);//<color=blue>Pode Atacar alvo em ate <b>{0}</b> de distancia.</color><color=red>Apos atacar efeito acaba.</color>

        string sar = ("$SAR1");
        if (Description.Contains(sar))
            AttDescription(sar, "<b>" + rangeSkillAttackRangeWalk + "</b>");
    }

    protected override void CreateSkills()
    {
        base.CreateSkills();

        if (skillAttackWalk == null && _prefabSkillWalk != null)
        {
            skillAttackWalk = Instantiate(_prefabSkillWalk).GetComponent<SkillAttack>();

            if (RespawMob.Instance)
                RespawMob.Instance.allRespaws.Add(skillAttackWalk.gameObject);

            if (autoCorrectCollider)
            {
                if (skillAttackWalk.GetComponent<BoxCollider>())
                {
                    float valueSizeCollider = (Range * 4) + Range;
                    Vector3 sizeCollider = new Vector3(valueSizeCollider, 2, valueSizeCollider);
                    skillAttackWalk.GetComponent<BoxCollider>().size = sizeCollider;

                    Debug.LogError("Ajustado BoxCollider[" + _prefabSkillWalk.name + "] para " + sizeCollider);
                }
                else
                 if (skillAttackWalk.GetComponent<CapsuleCollider>())
                {
                    float valueSizeCollider = (Range / 10) + 0.05f;

                    skillAttackWalk.GetComponent<CapsuleCollider>().radius = valueSizeCollider;

                    Debug.LogError("Ajustado CapsuleCollider[" + _prefabSkillWalk.name + "] para " + valueSizeCollider);
                }
                else
                 if (skillAttackWalk.GetComponent<SphereCollider>())
                {
                    float valueSizeCollider = (Range / 10) + 0.05f;

                    skillAttackWalk.GetComponent<SphereCollider>().radius = valueSizeCollider;

                    Debug.LogError("Ajustado SphereCollider[" + _prefabSkillWalk.name + "] para " + valueSizeCollider);
                }
            }
            if (skillAttackWalk != null)
            {
                skillAttackWalk.name = _prefabSkillWalk.name + " - " + User.name;

                skillAttackWalk.transform.SetParent(transform);
                skillAttackWalk.damage             = currentdamage;
                skillAttackWalk.who                = User;
                skillAttackWalk.AreaDamage         = AreaDamage;
                skillAttackWalk.DamageAreaDamage   = DamageAreaDamage;
                skillAttackWalk.OtherTargetDbuff   = OtherTargetDbuff;
                skillAttackWalk.transform.rotation = new Quaternion(0, 0, 0, 0);
                skillAttackWalk.skill = Skill;
                skillAttackWalk.SkillManager = this;
                skillAttackWalk.TakeRealDamage = RealDamage;
                skillAttackWalk.AreaRealDamage = AreaRealDamage;
                skillAttackWalk.DamageCountPassive = DamageCountPassive;
                skillAttackWalk.gameObject.SetActive(false);


                if (skillAttackWalk.GetComponent<SkillAttackRange>())
                    skillAttackWalk.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRangeWalk;
            }
        }
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        _attack = false;

        counter = 0;

        base.UseSkill();
    }

    protected override IEnumerator WalkCoroutine()
    {       
        CheckTarget(true);

        return base.WalkCoroutine();
    }

    /// <summary>
    /// Atacar Target, Cancela Buff
    /// </summary>
    protected virtual  void Attack()
    {
        if (_attack == false)
        {
            if (Target != null)
            {                
                _attack = true;

                SelectTouch = false;

                EffectWalk(false);              

                StopCoroutine(WalkCoroutine());                            

                HexList.Clear();

                _rangeAttackList.Clear();

                if (mobManager.isPlayer)
                    EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(156)/*Atacar Inimigo*/, User, 3);

                mobManager.ActivePassive(Passive.ShootSkill,Target);

                User.transform.LookAt(Target.transform);

                if (skillAttackWalk != null)
                {
                    skillAttackWalk.damage = currentdamage;

                    skillAttackWalk.transform.position = _shootAttack == true ? User.transform.position : Target.transform.position;

                    skillAttackWalk.transform.LookAt(Target.transform);

                    skillAttackWalk.endTurn = true;

                    skillAttackWalk.UseSkill(Target, this);
                }
                else
                {
                    target.GetComponent<MobHealth>().Damage(User, CurrentDamage, mobManager.chanceCritical);
                    Hit(true, null);
                }               
            }
        }
    }

    protected override void AfterWalk()
    {
        if (_attack == false)
        {
            base.AfterWalk();

            if (CheckTarget(true))
            {
                if (!mobManager.isPlayer ||
                    counter >= _buff)
                    Attack();
            }
        }
    }

    protected override void BeforeWalk()
    {
        if (_attack==false)
        {
            base.BeforeWalk();

            if (CheckTarget(true))
            {
                if (!mobManager.isPlayer ||
                    counter >= _buff)
                    Attack();
            }
        }     
    }

    /// <summary>
    /// Target Esta Dentro da Range de Walk
    /// </summary>
    /// <returns>Pode Ataca-lo</returns>
    protected virtual  bool CheckTarget(bool attList=false)
    {
        if (_attack==true)
            return false;

        if (attList)
        {
            _rangeAttackList.Clear();
            _rangeAttackList = enemyAttack.RegisterOtherHex(X: -1, Y: -1, range: _rangeAttack, colore: _rangeAttack > _buff, color: 2);
        }
           
        if (_rangeAttackList.Count != 0)
        {
            enemyAttack.ColorHex(_rangeAttackList, 2);

            ColorHex(3);

            foreach (var t in _rangeAttackList)
            {
                if (t.currentMob == Target)
                {
                    EffectManager.Instance.TargetEffect(Target);

                    if (mobManager.isPlayer)
                        EffectManager.Instance.PopUpDamageEffect("<color=yellow>"+XmlMenuInicial.Instance.Get(157)/*Pode atacar Inimigo*/+"</color>", User, 3);

                    enemyAttack.ColorHex(t,3);

                    HexList.Add(t);

                    return true;
                }
            }
        }

        EffectManager.Instance.TargetReset(Target);
        return false;
    }

    protected override void UseTouchSkill()
    {
        if (_attack == false)
        {
            if (mobManager.isPlayer && CheckTarget())
            {
                if (objectSelectTouch != null)
                {
                    if (objectSelectTouch.GetComponent<HexManager>())
                    {
                        if (objectSelectTouch.GetComponent<HexManager>().currentMob == Target)
                        {                          
                            Attack();
                            return;
                        }
                    }
                    else
                    if (objectSelectTouch == Target)
                    {                      
                        Attack();
                        return;
                    }
                }
            }

            base.UseTouchSkill();
        }
    }

    protected override void IAWalk()
    {
        if (!mobManager.isPlayer)
        {
            if (_attack == false)
            {
                //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " IAWalk() - _attack == false", 6);

                if (CheckTarget())
                {
                    //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " IAWalk() - Attack!!!", 6);
                    Attack();

                    return;
                }

                //GameManagerScenes._gms.NewInfo(User.name + "- " + nome + " IAWalk() - Base", 6);
                base.IAWalk();
            }
        }
    }

    public    override void Hit(bool endTurn, GameObject targetDbuff)
    {
        base.Hit(endTurn, targetDbuff);

        if (_attack && targetDbuff == Target)
        {
            StopCoroutine(WalkCoroutine());

            counter = 0;

            HexList.Clear();

            _rangeAttackList.Clear();

            EffectWalk(false);

            if (mobManager.isPlayer)
            {
                if (Target.activeInHierarchy == false)
                    ToolTip.Instance.TargetTooltip(Target, prop: false);
                else
                    ToolTip.Instance.TargetTooltip(User, prop: false);
            }

            EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(132), User);//Acabou

            //mobManager.ActivePassive(Passive.EndSkill, target);

            EndSkill();
        }
    }
}
