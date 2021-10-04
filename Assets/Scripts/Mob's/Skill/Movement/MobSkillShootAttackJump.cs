using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillShootAttackJump : MobSkillJumpShootAttack
{
    [Space]
    [Header("ShootAttackJump")]
    [SerializeField, Tooltip("Attack apos o Jump")]
    protected GameObject _prefabAttackBeforeDown;
    protected SkillAttack _attackBeforeDown;
    [Space]
    [SerializeField,Tooltip("Range para o script SkillAttackRange, $SAR1")]
    protected int rangeSkillAttackRangeBeforeDown = -1;


    protected override void Start()
    {
        base.Start();

        if (skillAttack != null)
            skillAttack.endTurn = false;
    }

    protected override void AttDescription()
    {
        base.AttDescription();


        string sar = ("$SAR1");
        if (Description.Contains(sar))
            AttDescription(sar, "<b>" + rangeSkillAttackRangeBeforeDown + "</b>");
    }

    public override void AttDamageAndDescription()
    {
        base.AttDamageAndDescription();

        if (_attackBeforeDown != null)
        {
            _attackBeforeDown.damage = currentdamage;
            _attackBeforeDown.who = User;
            _attackBeforeDown.AreaDamage = AreaDamage;
            _attackBeforeDown.DamageAreaDamage = DamageAreaDamage;
            _attackBeforeDown.OtherTargetDbuff = OtherTargetDbuff;
            _attackBeforeDown.skill = Skill;
            _attackBeforeDown.SkillManager = this;
            _attackBeforeDown.TakeRealDamage = RealDamage;
            _attackBeforeDown.AreaRealDamage = AreaRealDamage;
            _attackBeforeDown.DamageCountPassive = DamageCountPassive;

            if (_attackBeforeDown.GetComponent<SkillAttackRange>())
                _attackBeforeDown.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRangeBeforeDown;
        }
    }

    protected override void CreateSkills()
    {
        base.CreateSkills();

        if (_attackBeforeDown == null && _prefabAttackBeforeDown != null)
        {
            _attackBeforeDown = Instantiate(_prefabAttackBeforeDown).GetComponent<SkillAttack>();

            if (RespawMob.Instance)
            RespawMob.Instance.allRespaws.Add(_attackBeforeDown.gameObject);

            if (autoCorrectCollider)
            {
                if (_attackBeforeDown.GetComponent<BoxCollider>())
                {
                    float valueSizeCollider = (Range * 4) + Range;
                    Vector3 sizeCollider = new Vector3(valueSizeCollider, 2, valueSizeCollider);
                    _attackBeforeDown.GetComponent<BoxCollider>().size = sizeCollider;

                    Debug.LogError("Ajustado BoxCollider[" + _prefabAttackBeforeDown.name + "] para " + sizeCollider);
                }
                else
                 if (_attackBeforeDown.GetComponent<CapsuleCollider>())
                {
                    float valueSizeCollider = (Range / 10) + 0.05f;

                    _attackBeforeDown.GetComponent<CapsuleCollider>().radius = valueSizeCollider;

                    Debug.LogError("Ajustado CapsuleCollider[" + _prefabAttackBeforeDown.name + "] para " + valueSizeCollider);
                }
                else
                 if (_attackBeforeDown.GetComponent<SphereCollider>())
                {
                    float valueSizeCollider = (Range / 10) + 0.05f;

                    _attackBeforeDown.GetComponent<SphereCollider>().radius = valueSizeCollider;

                    Debug.LogError("Ajustado SphereCollider[" + _prefabAttackBeforeDown.name + "] para " + valueSizeCollider);
                }
            }
            if (_attackBeforeDown != null)
            {
                _attackBeforeDown.name = _prefabAttackBeforeDown.name + " - " + User.name;

                _attackBeforeDown.transform.SetParent(transform);
                _attackBeforeDown.damage = currentdamage;
                _attackBeforeDown.who = User;
                _attackBeforeDown.AreaDamage = AreaDamage;
                _attackBeforeDown.DamageAreaDamage = DamageAreaDamage;
                _attackBeforeDown.OtherTargetDbuff = OtherTargetDbuff;
                _attackBeforeDown.transform.rotation = new Quaternion(0, 0, 0, 0);
                _attackBeforeDown.skill = Skill;
                _attackBeforeDown.SkillManager = this;
                _attackBeforeDown.gameObject.SetActive(false);
                _attackBeforeDown.TakeRealDamage = RealDamage;
                _attackBeforeDown.AreaRealDamage = AreaRealDamage;
                _attackBeforeDown.DamageCountPassive = DamageCountPassive;

                if (_attackBeforeDown.GetComponent<SkillAttackRange>())
                    _attackBeforeDown.GetComponent<SkillAttackRange>().Range = rangeSkillAttackRangeBeforeDown;
            }
        }
    }

    protected override void Down()
    {
        iTween.ShakeRotation(Camera.main.gameObject, iTween.Hash("z", 0.3f, "x", 0.3f, "time", 0.05f, "easetype", iTween.EaseType.easeOutCirc));

        User.transform.LookAt(target.transform);

        //ResetCoolDownManager();

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        if (mobManager.isPlayer)
        {
            if (Target.activeInHierarchy == false)
                ToolTip.Instance.TargetTooltip(Target, prop: false);
            else
                ToolTip.Instance.TargetTooltip(User, prop: false);
        }

        if (_attackBeforeDown != null)
        {
            _attackBeforeDown.damage = currentdamage;

            _attackBeforeDown.endTurn = true;

            if (_attackBeforeDown.GetComponent<SkillAttackRange>())
            {
                if (jump<2)
                    _attackBeforeDown.GetComponent<SkillAttackRange>().Range = 2;
                else
                _attackBeforeDown.GetComponent<SkillAttackRange>().Range = jump;
            }

            _attackBeforeDown.transform.position = User.transform.position;

            _attackBeforeDown.UseSkill(Target, this);

            EndSkill();
        }
        else
        {
            Target.GetComponent<MobHealth>().Damage(User, currentdamage, mobManager.chanceCritical);
            Hit(true, null);
        }

        //mobManager.EndTurn();        
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        skillAttack.transform.position = User.transform.position;

        skillAttack.transform.rotation = User.transform.rotation;

        skillAttack.UseSkill(Target, this);

        enemyAttack.timeAttack--;

        int X = target.GetComponent<MoveController>().hexagonX,
            Y = target.GetComponent<MoveController>().hexagonY;

        if (!_pivotTarget)
        {
            X = moveController.hexagonX;
            Y = moveController.hexagonY;
        }

        RegisterOtherHex(X, Y, jump, _clearList: true);
        RegisterOtherHexOnlyFree();

        if (HexList.Count <= 0)
        {
            EndSkill();
            return;
        }

        //Sai do antigo Ground
        moveController.Solo.free       = true;
        moveController.Solo.currentMob = null;

        SelectTouch = true;

        if (!_pivotTarget)
            CameraOrbit.Instance.ChangeTarget(User);
        else
            CameraOrbit.Instance.ChangeTarget(Target);

        if (mobManager.isPlayer)
        {
            ToolTip.Instance.TargetTooltipCanvas(Nome, XmlMenuInicial.Instance.Get(150));//Selecione uma das casas Verde para poder pular até ela!!!
            CameraOrbit.Instance.MaxOrbitCamera();
        }
        else
        {
            UseTouchIA();
        }
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        endTurn = false;
        base.Hit(endTurn, targetDbuff);
    }
}
