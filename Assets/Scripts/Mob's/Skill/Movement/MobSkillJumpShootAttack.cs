using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillJumpShootAttack : MobSkillJumpAttack
{
    [Space]
    [Header("JumpShootAttack")]
    [SerializeField,Range(0,6), Tooltip("Randomiza a distancia do pulo entre o valor do 'Range Jump' e esse valor do 'Max Range Jump'")]
    protected int _maxRangeJump = 6;

    protected override void Start()
    {

        base.Start();
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        if (_rangeJump == (_maxRangeJump - 1))
            AttDescription("$P0", "<b>" + (_maxRangeJump).ToString("F0") + "</b>");
        else
            AttDescription("$P0", "<b>" + _rangeJump + " - " + (_maxRangeJump).ToString("F0") + "</b>");

    }

    protected virtual void randomJump()
    {
        jump = Random.Range(_rangeJump,_maxRangeJump+1);

        EffectManager.Instance.PopUpDamageEffect(jump.ToString("F0"),User.GetComponent<MoveController>().Solo.gameObject, 1.5f);
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        randomJump();

        base.UseSkill();
    }

    protected override void Down()
    {
        iTween.ShakeRotation(Camera.main.gameObject, iTween.Hash("z", 0.3f, "x", 0.3f, "time", 0.05f, "easetype", iTween.EaseType.easeOutCirc));

        ShootAttack();
    }

    protected virtual void ShootAttack()
    {
        //ResetCoolDownManager();

        User.transform.LookAt(target.transform);

        if (mobManager.isPlayer)
        {
            CameraOrbit.Instance.ChangeTarget(target);

            if (Target.activeInHierarchy == false)
                ToolTip.Instance.TargetTooltip(Target, prop: false);
            else
                ToolTip.Instance.TargetTooltip(User, prop: false);
        }

        ShootSkill();
    }
}
