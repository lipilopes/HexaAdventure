using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillJumpAttack : MobSkillManager
{
    [Header("Jump Attack")]
    [SerializeField, Tooltip("Pivor do jump")]
    protected bool _pivotTarget = true;
    [SerializeField,Range(1,6), Tooltip("Distancia do jump $P0")]
    protected int _rangeJump = 1;

    protected int jump = 0;

    protected override void Start()
    {
        base.Start();        

        jump = _rangeJump;
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + _rangeJump.ToString("F0") + "</b>");
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        enemyAttack.timeAttack--;

        int X = target.GetComponent<MoveController>().hexagonX,
            Y = target.GetComponent<MoveController>().hexagonY;

        if (!_pivotTarget)
        {
            X = moveController.hexagonX;
            Y = moveController.hexagonY;
        }

        RegisterOtherHex(X, Y, jump, _clearList:true);
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
            ToolTip.Instance.TargetTooltipCanvas(Nome,XmlMenuInicial.Instance.Get(150));//Selecione uma das casas Verde para poder pular até ela!!!
            CameraOrbit.Instance.MaxOrbitCamera();
        }
        else
        {
            UseTouchIA();
        }
    }

    protected virtual  void UseTouchIA()
    {
        foreach (var item in HexList)
        {
            if (item.free && item.currentMob == null)
            {
                objectSelectTouch = item.gameObject;
                UseTouchSkill();
                break;
            }
        }
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        if (objectSelectTouch.GetComponent<HexManager>())
        {
            Jump(objectSelectTouch.GetComponent<HexManager>());
        }
    }

    protected virtual  void Jump(HexManager ground)
    {
        if (User.GetComponent<MoveController>().Walk(null, ground.x, ground.y, 0, true))
        {
            HexList.Clear();
            SelectTouch = false;
            Down();
        }
    }

    protected virtual  void Down()
    {
        User.transform.LookAt(target.transform);

        //ResetCoolDownManager();

        if (mobManager.isPlayer)
        {
            if (Target.activeInHierarchy == false)
                ToolTip.Instance.TargetTooltip(Target, prop: false);
            else
                ToolTip.Instance.TargetTooltip(User, prop: false);
        }

        skillAttack.UseSkill(target, this);

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        EndSkill();
    }
}
