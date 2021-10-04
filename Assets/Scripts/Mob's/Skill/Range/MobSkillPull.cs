using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillPull : MobSkillManager
{
    WaitForSeconds wait = new WaitForSeconds(0.5f);

    [Space, Header("Pull")]
    [SerializeField,Tooltip("Range de Casas verdes para puxar o target para uma delas")]
    protected int rangeToPull = 2;
    [SerializeField, Tooltip("Pode Puxar Itens")]
    protected bool pullItem = true;
    public    bool PullItem { get { return pullItem; } }

    protected HexManager hex;

    protected override void Start()
    {
        base.Start();

        if (_DbuffBuff.Count >= 1 && GameManagerScenes._gms.GameMode == Game_Mode.History)
        {
            foreach (var d in _DbuffBuff)
            {
                if (d._buff == Dbuff.Stun)
                {
                    float newChance = 0.5f;

                    if (mobManager.isPlayer)
                        newChance = 1f;

                    if (d._dbuffChance < newChance)
                        d._dbuffChance = newChance;
                }
            }
        }       
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        if (pullItem && mobManager.isPlayer)
            description += " <color=blue>" + XmlMenuInicial.Instance.Get(158) + "</color>";//Puxa Item
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        hex = null;
        SelectTouch = false;
        objectSelectTouch = null;

        base.UseSkill();

        useSkill = true;

        if (target.GetComponent<ItemRecHp>() && PullItem)
        {
            ShootSkill();
            SelectTouch = false;
            return;
        }
        else
        if (!SelectTouch)
        {
            RegisterOtherHex(_range: rangeToPull, _clearList: true);
            RegisterOtherHexOnlyFree();
        }

        if (mobManager.isPlayer)
        {
            if (HexList.Count <= 0)
            {
                useSkill = false;

                ToolTip.Instance.TargetTooltipCanvas(Nome, "<color=red>"+XmlMenuInicial.Instance.Get(148)+"</color>");//Não ha Casas proximas o sufuciente!!!

                return;
            }

            if (!SelectTouch)
            {
                mobManager.ActivePassive(Passive.StartSkill,Target);               

                SelectTouch = true;

                ToolTip.Instance.TargetTooltipCanvas(Nome, 
                    GameManagerScenes._gms.AttDescriçãoMult(
                       XmlMenuInicial.Instance.Get(159)//Selecione uma das casas Verdes para puxar, o(a) <b>{0}</b> para ela!!!
                    , target.GetComponent<ToolTipType>()._name)
                    );

                CameraOrbit.Instance.ChangeTarget(User);
                CameraOrbit.Instance.MaxOrbitCamera();
            }
        }
        else
        {
            if (HexList.Count <= 0)
            {
                useSkill = false;
                enemyAttack.canSkill1 = false;
                enemyAttack.CheckInList();
                return;
            }
            else
                StartCoroutine(PullIA());
        }
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        Debug.LogError("UseTouchSkill() - Pull");

        hex = objectSelectTouch.GetComponent<HexManager>();

        if (objectSelectTouch == null)
        {
            ColorHex(3);

            if (mobManager.isPlayer)
                ToolTip.Instance.TargetTooltipCanvas(Nome,
                   GameManagerScenes._gms.AttDescriçãoMult(
                      XmlMenuInicial.Instance.Get(159)//Selecione uma das casas Verdes para puxar, o(a) <b>{0}</b> para ela!!!
                   , target.GetComponent<ToolTipType>()._name)
                   );

            return;
        }

        if (!hex.free || !HexList.Contains(hex))
        {
            ColorHex(3);

            if (mobManager.isPlayer)
                ToolTip.Instance.TargetTooltipCanvas(Nome,
                   GameManagerScenes._gms.AttDescriçãoMult(
                      XmlMenuInicial.Instance.Get(159)//Selecione uma das casas Verdes para puxar, o(a) <b>{0}</b> para ela!!!
                   , target.GetComponent<ToolTipType>()._name)
                   );

            return;
        }

        SelectTouch = false;

        ShootSkill();

        CameraOrbit.Instance.ChangeTarget(Target);
    }

    public IEnumerator PullIA()
    {
        Debug.LogError(User.name + " usou a skill " + Nome + "IA");

        int X      = moveController.hexagonX, Y = moveController.hexagonY;

        int random = Random.Range(0, HexList.Count);

        hex = null;

        HexManager ground = HexList[random];

        if (ground != null && ground.free && ground.x != X && ground.y != Y)
        {
            if (HexList.Contains(ground))
                hex = ground;
        }
        if (hex != null)
        {
            objectSelectTouch = hex.gameObject;

            yield return wait;

            UseTouchSkill();
        }
        else
            StartCoroutine(PullIA());
    }

    protected virtual void Pull(GameObject _target)
    {
        if (Target != _target)
        {
            if (_target.GetComponent<MoveController>())
            {
                _target.GetComponent<MoveController>().EnemyWalk(moveController, true,true);
            }

            return;
        }

        #region Puxar Mob
            if (target.GetComponent<MobHealth>())
        {
            if (!mobManager.isPlayer || hex == null)
            {
                int Rr = Random.Range(0, HexList.Count);

                hex = HexList[Rr];
            }

            if (HexList.Count > 0)
            {
                if (hex.gameObject != null)
                {
                    if (hex.free)
                    {
                        if (target.GetComponent<MoveController>().Walk(hex.gameObject, hex.x, hex.y, Dbuff: true))
                        {
                            target.transform.LookAt(User.transform);
                            HexList.Clear();
                            hex = null;
                        }
                    }
                }
            }
            else
            {
                HexList.Remove(hex);
                hex = null;
                Pull(target);
                return;
            }
        }
        #endregion

        #region Puxar Item
        if (target.GetComponent<ItemRecHp>())
        {           
            target.GetComponent<ItemRecHp>().Pull(moveController.Solo);

            //Bonus No cooldown
            for (int i = 0; i < skillManager.Skills.Count; i++)
            {
                CreateDbuff(User, Dbuff.Cooldown, true, 100, i, -2);
            }         
        }
        #endregion

        //mobManager.ActivePassive(Passive.EndSkill, Target);       

        CameraOrbit.Instance.ChangeTarget(_target);

        //ResetCoolDownManager();
        //useSkill    = false;
        SelectTouch = false;

        if (target != User)
            User.transform.LookAt(target.transform);

        if (mobManager.isPlayer)
            ToolTip.Instance.AttTooltip(Target);

        EndSkill();
    }

    public override void Hit(bool endTurn, GameObject target)
    {
        Debug.LogError("HIT - " + Nome);

        Pull(target);

        base.Hit(skillAttack.endTurn, target);

        if (mobManager.isPlayer)
        {
            if (Target.activeInHierarchy == false)
                ToolTip.Instance.TargetTooltip(Target, prop: false);
            else
                ToolTip.Instance.TargetTooltip(User, prop: false);
        }
    }
}
