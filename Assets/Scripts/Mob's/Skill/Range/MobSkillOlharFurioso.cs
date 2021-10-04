using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillOlharFurioso : MobSkillBasicDamage
{
    [Header("Olhar Furioso")]
    [SerializeField]
    protected GameObject  skillPrefabBack;
    protected SkillAttack skillAttackBack;
    [SerializeField]
    protected float speedRotate = 150;
    [SerializeField]
    protected float timeRotate  = 2;

    protected override void Start()
    {
        base.Start();
    }

    public override void AttDamageAndDescription()
    {
        base.AttDamageAndDescription();

        if (skillAttackBack != null)
        {
            skillAttackBack.damage = currentdamage;
            skillAttackBack.who = User;
            skillAttackBack.AreaDamage = AreaDamage;
            skillAttackBack.DamageAreaDamage = DamageAreaDamage;
            skillAttackBack.OtherTargetDbuff = OtherTargetDbuff;
            skillAttackBack.skill = Skill;
            skillAttackBack.SkillManager = this;
            skillAttackBack.TakeRealDamage = RealDamage;
            skillAttackBack.AreaRealDamage = AreaRealDamage;
            skillAttackBack.DamageCountPassive = DamageCountPassive;

            if (skillAttackBack.GetComponent<SkillAttackRange>())
                skillAttackBack.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRange;
        }
    }

    protected override void CreateSkills()
    {
        base.CreateSkills();

        if (skillAttack!=null)        
            skillAttack.transform.SetParent(transform);       

        if (skillAttackBack == null && skillPrefabBack != null)
        {
            skillAttackBack = Instantiate(skillPrefabBack).GetComponent<SkillAttack>();

            if (RespawMob.Instance)
            RespawMob.Instance.allRespaws.Add(skillAttackBack.gameObject);

            if (autoCorrectCollider)
            {
                if (skillAttackBack.GetComponent<BoxCollider>())
                {
                    float valueSizeCollider = (Range * Range) + Range;
                    Vector3 sizeCollider = new Vector3(valueSizeCollider, 2, valueSizeCollider);
                    skillAttackBack.GetComponent<BoxCollider>().size = sizeCollider;

                    Debug.LogError("Ajustado BoxCollider[" + skillPrefabBack.name + "] para " + sizeCollider);
                }
                else
                 if (skillAttackBack.GetComponent<CapsuleCollider>())
                {
                    float valueSizeCollider = (Range / 10) + 0.05f;

                    skillAttackBack.GetComponent<CapsuleCollider>().radius = valueSizeCollider;

                    Debug.LogError("Ajustado CapsuleCollider[" + skillPrefabBack.name + "] para " + valueSizeCollider);
                }
                else
                 if (skillAttackBack.GetComponent<SphereCollider>())
                {
                    float valueSizeCollider = (Range / 10) + 0.05f;

                    skillAttackBack.GetComponent<SphereCollider>().radius = valueSizeCollider;

                    Debug.LogError("Ajustado SphereCollider[" + skillPrefabBack.name + "] para " + valueSizeCollider);
                }
            }
            if (skillAttackBack != null)
            {
                skillAttackBack.name = skillPrefabBack.name + " - " + User.name;

                skillAttackBack.transform.SetParent(transform);
                skillAttackBack.damage           = currentdamage;
                skillAttackBack.who              = User;
                skillAttackBack.AreaDamage       = AreaDamage;
                skillAttackBack.DamageAreaDamage = DamageAreaDamage;
                skillAttackBack.OtherTargetDbuff = OtherTargetDbuff;
                //skillAttackBack.transform.rotation = new Quaternion(0, -180, 0, 0);
                skillAttackBack.skill            = Skill;
                skillAttackBack.SkillManager     = this;
                skillAttackBack.TakeRealDamage   = RealDamage;
                skillAttackBack.AreaRealDamage   = AreaRealDamage;
                skillAttackBack.DamageCountPassive = DamageCountPassive;
                skillAttackBack.gameObject.SetActive(false);

                if (skillAttackBack.GetComponent<SkillAttackRange>())
                    skillAttackBack.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRange;
            }        
        }
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        target = enemyAttack.target;
        
        GameManagerScenes._gms.NewInfo(User.name+" usou a skill "+nome+", no "+Target,3, true);

        //if (mobManager.isPlayer)
        //    if (!CheckPlayerUseSkill())
        //    {
        //        if (GameManagerScenes._gms.Adm)
        //            GameManagerScenes._gms.NewInfo("skill " + nome + " -> CheckPlayerUseSkill false", 3);

        //        ButtonManager.Instance.SkillInUseCanceled();

        //        return;
        //    }


        useSkill = true;

        mobManager.ActivePassive(Passive.StartSkill, target);

        alvosListSkill.Clear();

        HexList.Clear();

        mobManager.currentTimeAttack--;

        objectSelectTouch = null;

        if (target == null)
            target = User;

        Debug.LogError(User + " usou a skill " + Nome + " no(a) " + target.name);

        if (skillAttack != null)
            skillAttack.Duration = 0.5f + timeRotate;

        if (skillAttackBack != null)
        skillAttackBack.Duration = 0.5f + timeRotate;

        ShootSkill();        
    }

    protected override void ShootSkill()
    {
        if (GameManagerScenes._gms.Adm && EffectManager.Instance!=null)
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);

        if (target != null && User != target)
            User.transform.LookAt(target.transform);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);      

        iTween.ShakeRotation(Camera.main.gameObject, iTween.Hash("z", 0.4f, "x", 0.6f, "time", 5 + 0.25f, "easetype", iTween.EaseType.easeOutCirc));

        mobManager.ActivePassive(Passive.ShootSkill, target);

        skillAttack.UseSkill(target, this);

        skillAttackBack.UseSkill(target, this);

       // skillAttackBack.transform.rotation = new Quaternion(0, 0, 0, 0);
       //skillAttackBack.transform.LookAt(gameObject.transform.position * -1);

        StartCoroutine(TimerCoroutine());
    }

    protected override IEnumerator TimerCoroutine()
    {
        float _Time = Time.time, _TimeMax = Time.time + timeRotate;

        while (_Time < _TimeMax)
        {
            User.transform.Rotate(0, Time.deltaTime * speedRotate, 0);

            _Time += Time.time - _Time;

            yield return null;
        }

        yield return wait;

        if (target != null && User != target)
            User.transform.LookAt(target.transform);

        EndSkill();
    }

    //public void TesteRotate() { StartCoroutine(TesteRotateCoroutine()); }

    //IEnumerator TesteRotateCoroutine()
    //{
    //    skillAttack.gameObject.SetActive(true);

    //    skillAttackBack.gameObject.SetActive(true);
    //    skillAttackBack.transform.LookAt(User.transform.forward / -1);

    //    float _Time = Time.time, _TimeMax = Time.time + timeRotate;

    //    while (_Time < _TimeMax)
    //    {
    //        User.transform.Rotate(0, Time.deltaTime * speedRotate, 0);

    //        _Time += Time.time - _Time;

    //        yield return null;
    //    }

    //    yield return wait;

    //    skillAttack.gameObject.SetActive(false);

    //    skillAttackBack.gameObject.SetActive(false);

    //    print("Skill["+Nome+"] Acabou");
    //}
}
