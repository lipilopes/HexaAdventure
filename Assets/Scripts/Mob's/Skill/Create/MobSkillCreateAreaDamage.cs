using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillCreateAreaDamage : MobSkillManager
{
    [Space]
    [Header("Propriedades AreaDamage")]
    [SerializeField,Tooltip("Tempo que ficara ativo $P0")]
    protected int maxTimeAreaDamage;
    [SerializeField, Range(0, 1), Tooltip("Porcentagem de dano ao andar sobe a area $P1")]
    protected float porcenWalkDamageAreaDamage = 0.20f;
    [Space]
    [SerializeField, Range(0, 1), Tooltip("Chance de Dar mais hits $P2")]
    protected float chanceMaxHitAreaDamage;
    [SerializeField, Tooltip("Maximo de hits Extras $P3")]
    protected int maxHitAreaDamage;
    [Space]
    [Header("Real Damage")]
    [SerializeField, Tooltip("Dano real")]
    protected bool AreaDamageRealDamage       = false;
    [SerializeField, Tooltip("Dano real no hit extra")]
    protected bool AreaDamageMaxHitRealDamage = false;
    [SerializeField, Tooltip("Dano real ao andar sobe")]
    protected bool AreaDamageWalkRealDamage   = false; 
    [Space]
    [SerializeField, Tooltip("Apos Morrer, Area Damage sera Desativada  $P4")]
    protected bool _desactiveAreaDamage = true;

    [Header("")]

    protected int walkDamage = 0;

    protected List<MobSkillAreaDamage> _AreaDamageList = new List<MobSkillAreaDamage>();

    protected MobSkillAreaDamage MyAreaDamage;

    protected virtual MobSkillAreaDamage GetAreaDamage
    {
        get
        {
            if (MyAreaDamage != null && /*!MyAreaDamage.gameObject.activeInHierarchy &&*/ MyAreaDamage.TimeOn <= 0)
                return MyAreaDamage;
            else
                foreach (var s in _AreaDamageList)
                {
                    if (s.TimeOn >= 0 && !s.gameObject.activeInHierarchy)
                    {
                        return s;
                    }
                }

            CreateSkills();

            return GetAreaDamage;
        }
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + maxTimeAreaDamage + "</b>");
        AttDescription("$P1", "<b>" + porcenWalkDamageAreaDamage * 100 + "%</b>");
        AttDescription("$P2", "<b>" + chanceMaxHitAreaDamage * 100 + "%</b>");
        AttDescription("$P3", "<b>" + maxHitAreaDamage + "</b>");
        AttDescription("$P4",
            _desactiveAreaDamage ?
            "<color=red>" + XmlMenuInicial.Instance.Get(141) + "</color>" //Apos Morrer Efeito Acaba
            : "");
    }

    protected override void CreateSkills()
    {
        AttDescription();

        if (skillPrefab != null)
        {
            MobSkillAreaDamage  _AreaDamage = Instantiate(skillPrefab, new Vector3(0, 0.057f, 0), new Quaternion()).GetComponent<MobSkillAreaDamage>();
            
            _AreaDamage.RealDamage       = AreaDamageRealDamage;
            _AreaDamage.MaxHitRealDamage = AreaDamageMaxHitRealDamage;
            _AreaDamage.WalkRealDamage   = AreaDamageWalkRealDamage;

            _AreaDamage._desactiveAreaDamage = _desactiveAreaDamage;

            _AreaDamage.User = User;

            _AreaDamage.name = Nome + " - " + _AreaDamageList.Count + 1;
            //_AreaDamage.transform.SetParent(transform);
            _AreaDamage.gameObject.SetActive(false);
          
            _AreaDamageList.Add(_AreaDamage);

            MyAreaDamage = _AreaDamage;
            
            if (_AreaDamageList.Count == 1)
            {
             /*   MobAttack mobAttack = User.GetComponent<MobAttack>();

                if (Skill == 1)
                {
                    mobAttack.nameSkill1       = Nome;
                    mobAttack.distanceSkill1   = Range;
                    mobAttack.damageSkill1     = PorcentDamage;
                    mobAttack.MaxCooldownSkill1 = CooldownMax;

                    mobAttack.Skill1NeedTarget = NeedTarget;
                    mobAttack.Skill1TargetFriend = TargetFriend;
                    mobAttack.Skill1TargetMe = TargetMe;
                }
                if (Skill == 2)
                {
                    mobAttack.nameSkill2 = Nome;
                    mobAttack.distanceSkill2 = Range;
                    mobAttack.damageSkill2 = PorcentDamage;
                    mobAttack.MaxCooldownSkill2 = CooldownMax;

                    mobAttack.Skill2NeedTarget = NeedTarget;
                    mobAttack.Skill2TargetFriend = TargetFriend;
                    mobAttack.Skill2TargetMe = TargetMe;
                }
                if (Skill == 3)
                {
                    mobAttack.nameSkill3 = Nome;
                    mobAttack.distanceSkill3 = Range;
                    mobAttack.damageSkill3 = PorcentDamage;
                    mobAttack.MaxCooldownSkill3 = CooldownMax;

                    mobAttack.Skill3NeedTarget = NeedTarget;
                    mobAttack.Skill3TargetFriend = TargetFriend;
                    mobAttack.Skill3TargetMe = TargetMe;
                }*/                
            }
            else
                if (GameManagerScenes._gms.Adm)
                EffectManager.Instance.PopUpDamageEffect("<color=pink>Nova Area Criada</color>", User, 5);
        }
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        MyAreaDamage = GetAreaDamage;

        walkDamage = (int)(currentdamage * (porcenWalkDamageAreaDamage * 100) / 100);

        if (!mobManager.isPlayer)
        {
            if (walkDamage > 0)
                InfoTable.Instance.GetComponent<InfoTable>().NewInfo(
                    GameManagerScenes._gms.AttDescriçãoMult(
                        XmlMenuInicial.Instance.Get(142)//<color=red><b>{0}</b></color> da <b>{1}</b> do dano em inimigos que andam sobe a skill.
                        , Nome, ""+walkDamage)
                    , 10);
        }
        else
        {
            CameraOrbit.Instance.ChangeTarget(User);
            CameraOrbit.Instance.MaxOrbitCamera();
        }

        ShootSkill();

        if (MyAreaDamage.gameObject.activeInHierarchy)
            iTween.LookFrom(Camera.main.gameObject, MyAreaDamage.transform.position, 0.25f);       

        enemyAttack.timeAttack--;

        //ResetCoolDownManager();

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        //mobManager.EndTurn();

        EndSkill();
    }

    protected override void ShootSkill()
    {
        Debug.LogError(Nome + " ShotSkill(" + target + ")");

        if (GameManagerScenes._gms.Adm)
        {
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);
        }

        if (target.activeInHierarchy && User != target)
            User.transform.LookAt(target.transform);

        mobManager.ActivePassive(Passive.ShootSkill, target);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        HexManager _hex = target.GetComponent<MoveController>().Solo;

        MyAreaDamage.gameObject.SetActive(true);

        MyAreaDamage.RespawSkill(User, target, _hex, currentdamage,walkDamage, maxHitAreaDamage, chanceMaxHitAreaDamage, maxTimeAreaDamage+1, mobManager.chanceCritical);

        if (MyAreaDamage.GetComponent<PassiveManager>()!=null)
            MyAreaDamage.GetComponent<PassiveManager>().User = User;

        Hit(false,target);       
    }

    public override void EndTurnAttack()
    {
        //GameManagerScenes._gms.NewInfo("Override EndTurnAttack - "+Nome,3,true);

        base.EndTurnAttack();

        //mobManager.ActivePassive(Passive.EndSkill,Target);


        bool areaDamageActive = false;   

        foreach (var area in _AreaDamageList)
        {
           // GameManagerScenes._gms.NewInfo("Check - " + area.name, 3, true);

            if (area.gameObject.activeInHierarchy && area.User == User)
            {
                //GameManagerScenes._gms.NewInfo("Attack- " + area.name, 3, true);

                mobManager.ActivePassive(Passive.AreaAttack, Target);
                EffectManager.Instance.PopUpDamageEffect(nome, User, 2);
                area.Attack();
                areaDamageActive = true;
            }
        }  
        
        if(areaDamageActive)
            EndSkill();
    }

    /// <summary>
    /// Geralmente usado quando o User Morre
    /// </summary>
    public override void DesactiveTurnAttack()
    {
        if (!_desactiveAreaDamage)
            return;
        
        foreach (var area in _AreaDamageList)
        {
            if (area.gameObject.activeInHierarchy && area.User == User)
            {
                EffectManager.Instance.PopUpDamageEffect(nome, User, 2);
                area.Desactive();
            }
        }
    }


    public void ShootSkillTeste()
    {
        MyAreaDamage.gameObject.SetActive(true);

        MyAreaDamage.SkillRespawnedTeste(User, target, null, currentdamage, walkDamage, maxHitAreaDamage, chanceMaxHitAreaDamage, maxTimeAreaDamage + 1, mobManager.chanceCritical);

        if (MyAreaDamage.GetComponent<PassiveManager>() != null)
            MyAreaDamage.GetComponent<PassiveManager>().User = User;
    }
}
