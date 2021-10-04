using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillTeleport : MobSkillManager
{
    protected  WaitForSeconds waitToTeleport;
    protected  WaitForSeconds waitToEndTeleport;
    [Space]
    [Space, Header("Teleport")]
    [SerializeField, Tooltip("Tempo q efeito do teleport fica ativo")]
    protected float _waitToTeleport = 1;
    [SerializeField, Tooltip("Tempo q efeito do teleporte Fica ativo para acabar")]
    protected float _waitToEndTeleport = 1;
    [Space]
    [SerializeField,Range(0,100), Tooltip("% do valor base do Dbuff $P0")]
    protected int _baseBuffRecHp = 1;
    [SerializeField, Tooltip("limita a quantidade maxima do dbuff $P0")]
    protected int _bonusBuffRecHpMin = 0;
    [SerializeField, Tooltip("limita a quantidade maxima do dbuff $P0")]
    protected int _bonusBuffRecHpMax = 0;
    [SerializeField, Tooltip("Se cura ao usar skill em vc mesmo $P1")]
    protected bool _autoCure = false;
    [Space]
    [SerializeField,Tooltip("Cria areas de teleport em volta dos amigos")]
    protected bool _friendArea=false;
    [SerializeField, Range(0, 6),Tooltip("Tamanho da area em volta dos amigos")]
    protected int rangeFriends = 2;

    protected int _buff = 0;

    protected override void Start()
    {
        waitToTeleport    = new WaitForSeconds(_waitToTeleport);
        waitToEndTeleport = new WaitForSeconds(_waitToEndTeleport);
            
        AttDescription();

        base.Start();
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        currentdamage = /*BaseDamage + */User.GetComponent<SkillManager>().CalculePorcent(mobManager.damage, porcentDamage, dividedDamage, baseDamage, MaxHpProcent);

        _buff = currentdamage * _baseBuffRecHp / 100;

        currentdamage = 0;

        int _buffTooltip = _buff + _bonusBuffRecHpMax;

        AttDescription("$P0", "<b>" + (_buff) + " - " + (_buffTooltip) + "</b>");

        AttDescription("$P1", _autoCure ?
              "<color=blue>" + XmlMenuInicial.Instance.Get(151) + "</color>" //Se cura Caso seja o alvo.
            : " <color=red>" + XmlMenuInicial.Instance.Get(152) + "</color>");//NÃO se cura Caso seja o alvo.

    }

    public override void AttDamage()
    {
        base.AttDamage();

        Start();
    }
    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        if (!SelectTouch)
        {
            useSkill = true;

            hexList.Clear();

            if (_friendArea)
            {
                //Se o Target For um inimigo pega area dos inimigos caso seja amigo pega dos amigos
                List<GameObject> friends = TurnSystem.Instance.GetMob(mobManager.TimeMob, !mobManager.MesmoTime(Target));

                int count = friends.Count;

                if (count != 0)
                    for (int i = 0; i < count; i++)
                    {
                        MoveController mc = friends[i].GetComponent<MoveController>();

                        if (mc != null)
                        {
                            RegisterOtherHex(mc.hexagonX, mc.hexagonY, rangeFriends);
                        }
                    }
            }
            else
                RegisterOtherHex(Target.GetComponent<MoveController>().hexagonX, Target.GetComponent<MoveController>().hexagonY, rangeFriends);

            RegisterOtherHex(_range: Range);

            RegisterOtherHexOnlyFree();
        }

        if (mobManager.isPlayer)
        {
            if (HexList.Count <= 0)
            {
                ToolTip.Instance.TargetTooltipCanvas(Nome, "<color=red>"+XmlMenuInicial.Instance.Get(148)+"</color>");//Não ha Casas proximas o sufuciente!!!

                ButtonManager.Instance.ClearHUD(false);

                useSkill    = false;

                SelectTouch = false;

                return;
            }

            if (!SelectTouch)
            {
                if (target != null)
                {
                    ToolTip.Instance.TargetTooltipCanvas(Nome,
                        GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(153), target.GetComponent<ToolTipType>()._name));//Selecione alguma casa Verde para poder teleportar o(a) {0}!!!
                }

                SelectTouch = true;

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
                StartCoroutine(TeleportIA());
        }
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        CheckUseTouch();
    }

    protected void CheckUseTouch()
    {
        Debug.LogError("UseTouchSkill() - EndTeleport");

        if (objectSelectTouch == null)
        {
            ColorHex(3);

            if (target != null && mobManager.isPlayer)
            {
                ToolTip.Instance.TargetTooltipCanvas(Nome,
                        GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(153), target.GetComponent<ToolTipType>()._name));//Selecione alguma casa Verde para poder teleportar o(a) {0}!!!
            }
            return;
        }

        HexManager ground = objectSelectTouch.GetComponent<HexManager>();

        if (!ground.free)
        {
            ColorHex(3);

            if (target != null && mobManager.isPlayer)
            {
                ToolTip.Instance.TargetTooltipCanvas(Nome,
                        GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(153), target.GetComponent<ToolTipType>()._name));//Selecione alguma casa Verde para poder teleportar o(a) {0}!!!
            }
            return;
        }

        HexList.Clear();

        SelectTouch = false;

        HexManager hex = moveController.Solo;

        if (hex == null)
        {
            GameObject obj = GameObject.Find("Hex" + (moveController.hexagonX) + "x" + moveController.hexagonY);

            if (obj != null)
                hex = obj.GetComponent<HexManager>();
        }

        StartCoroutine(Teleport(hex, ground));
    }

    public virtual IEnumerator TeleportIA()
    {
        Debug.LogError(name + " usou a skill " + Nome + "IA");

        int random        = Random.Range(0, HexList.Count);

        HexManager hex    = null;

        HexManager ground = HexList[random];

        if (ground != null && ground.free)
        {
                hex = ground;
        }
        if (hex != null)
        {
            objectSelectTouch = hex.gameObject;

            SelectTouch = true;

            yield return null;//wait;

            UseTouchSkill();
        }
        else
        {
            HexList.Remove(ground);
            StartCoroutine(TeleportIA());
        }
    }

    protected virtual IEnumerator Teleport(HexManager _old, HexManager _new)
    {
        if (skillAttackOther!=null && skillAttackOther.GetComponent<TeleportEffect>())
            skillAttackOther.GetComponent<TeleportEffect>().ActiveTeleport(Target);
        else
            EffectManager.Instance.TeleportEffect(target);

        yield return waitToTeleport;

        if (!target.GetComponent<MoveController>().Walk(null, _new.x, _new.y, 1, true))
        {
            if (skillAttackOther == null)
                EffectManager.Instance.TeleportReset(target);

            SelectTouch = true;
        }
        else
        {
            CameraOrbit.Instance.ChangeTarget(target);

            iTween.LookTo(Camera.main.gameObject, target.transform.position, 0.5f);

            StartCoroutine(TimeEffect());
        }

        yield return waitToEndTeleport;

        EndTeleport();
    }

    protected virtual IEnumerator TimeEffect()
    {
        yield return waitToTeleport;
    }

    protected virtual void EndTeleport()
    {
        if (useSkill && Target.GetComponent<MobManager>().Alive)
        {
            Hit(false,Target);

            User.transform.LookAt(target.transform);
        }
        else
        {

        }

        if (mobManager.isPlayer)
            ToolTip.Instance.AttTooltip();

        iTween.ShakePosition(Camera.main.gameObject, iTween.Hash("z", -0.5f, "y", 0.5f, "time", 1, "easetype", iTween.EaseType.easeInOutBounce));

        enemyAttack.timeAttack--;

        if (skillAttackOther != null && skillAttackOther.GetComponent<TeleportEffect>())
            skillAttackOther.GetComponent<TeleportEffect>().DesactiveTeleport(Target);
else
            EffectManager.Instance.TeleportReset(target);

        //ResetCoolDownManager();

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        //useSkill = false;

        SelectTouch = false;

        //mobManager.EndTurn();

        EndSkill();
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        if (mobManager.MesmoTime(targetDbuff))
        {
            int maxBuff = _buff + (Random.Range(_bonusBuffRecHpMin, _bonusBuffRecHpMax + 1));

            CreateDbuff(targetDbuff, Dbuff.Recupera_HP, targetDbuff == User && _autoCure ? true : false, 1, _buff, maxBuff);
        }

        base.Hit(endTurn, targetDbuff);
    }
}
