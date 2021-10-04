using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeSkillMetralhadoraDeFritas : MobSkillShootable
{
    protected WaitForSeconds wait = new WaitForSeconds(0.25f);

    [Space]
    [Header("Metralhadora De Fritas")]
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de dano do primeiro hit, $P0%")]
    protected int PorcentDamageFirstHits = 50;
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de dano nos hits subsequente, $P1%")]
    protected int PorcentDamageOtherHits = 50;
    [SerializeField, Tooltip("Quantos hit para mudar o dano $P2")]
    protected int maxHitToChangeDamage = 1;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("Dano bonus em alvo unico, $P3")]
    protected int PorcentBonusAlvoUnico = 0;

    int maxShoot             = 0,
        currentShoot         = 0,
        DamageFirstHit       = 0,
        DamageOtherHit       = 0,
        DamageBonusAlvoUnico = 0;

   protected bool alvoUnico = false;

    protected override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0%", "<color=red><b>" + PorcentDamageFirstHits + "</b>%</color>");

        int dp1 = CurrentDamage * PorcentDamageOtherHits / 100;


        AttDescription("$P1%", "<color=red><b>" + dp1.ToString("F0") + "</b></color>");

        AttDescription("$P2", "<color=blue><b>" + maxHitToChangeDamage + "</b></color>");

        AttDescription("$P3", "<color=red><b>" + ((CurrentDamage * PorcentDamageFirstHits / 100) + (CurrentDamage * PorcentBonusAlvoUnico) / 100) + "</b></color>");
    }

    public override void AttDamage()
    {
        AttDescription("<color=red><b>" + (CurrentDamage * PorcentDamageOtherHits / 100) + "</b></color>", "$P1%");

        base.AttDamage();

        Start();
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        target = enemyAttack.target;

        AlvosNaListSkill();

        if (!CheckPlayerUseSkill())
            return;

        useSkill = true;

        mobManager.ActivePassive(Passive.StartSkill, Target);

        HexList.Clear();

        mobManager.currentTimeAttack--;

        SelectTouch      = false;

        objectSelectTouch = Target;     

        DamageOtherHit = (CurrentDamage * PorcentDamageOtherHits) / 100;

        DamageFirstHit = (CurrentDamage * PorcentDamageFirstHits) / 100;

        DamageBonusAlvoUnico  = (CurrentDamage * PorcentBonusAlvoUnico) / 100;

        currentShoot = 0;
        maxShoot     = alvosListSkill.Count;

        if (alvosListSkill.Count > 1)
        {
            EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(134)+"</color>", target, 3);//Alvo

            for (int i = 0; i < alvosListSkill.Count; i++)
            {
                if (alvosListSkill[i] != null && alvosListSkill[i] != Target)
                {
                    EffectManager.Instance.TargetEffect(alvosListSkill[i]);
                    EffectManager.Instance.PopUpDamageEffect("<color=white>"+XmlMenuInicial.Instance.Get(134)+"</color>", alvosListSkill[i], 3);//Alvo
                }
            }

            alvoUnico = false;

            skillAttack.damage = DamageFirstHit;
        }
        else
        {
            EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(135)+"</color>", target, 3);//Alvo Unico

            alvoUnico = true;

            skillAttack.damage = DamageFirstHit + /*(alvoUnico ?*/ DamageBonusAlvoUnico /*: 0)*/;
        }            

        EffectManager.Instance.TargetTargeteado(target);       

        ShootSkill();
    }

    protected virtual void Check()
    {
        if (!CheckList())
            return;

        if (currentShoot < maxHitToChangeDamage)
            skillAttack.damage = DamageFirstHit;
        else
            skillAttack.damage = DamageOtherHit;

        if (alvosListSkill.Count == 1)
        {
            Target = alvosListSkill[0];
           
            skillAttack.endTurn = true;

            EffectManager.Instance.TargetTargeteado(target);

            EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(136)+"</color>", target, 3);//Ultimo

            mobManager.transform.LookAt(target.transform);

            skillAttack.transform.LookAt(target.transform);

            ShootSkill();
            return;
        }

        SelectTouch = true;

        if (mobManager.isPlayer)
        {
            ToolTip.Instance.TargetTooltipCanvas(Nome, XmlMenuInicial.Instance.Get(137));//Selecione um dos Mob's <color=yellow>Mirados</color> para ataca-los!!!
        }
        else
        {
            foreach (var t in alvosListSkill)
            {
                objectSelectTouch = t;
            }

            if (RulesClickTarget(objectSelectTouch))
            {
                UseTouchSkill();
            }
            else
                Check();
        }
    }

    protected override bool RulesClickTarget(GameObject hitObject)
    {
        if (!CheckList())
            return false;

        GameObject hitted = null;

        #region Caso Click no Hexagono                             
        #region Mob in Hex
        if (hitObject.GetComponent<HexManager>())
            if (hitObject.GetComponent<HexManager>().currentMob != null)
                if (hitObject.GetComponent<HexManager>().currentMob.GetComponent<MobHealth>())
                    if (hitObject.GetComponent<HexManager>().currentMob.GetComponent<MobHealth>().Alive)
                        hitted = hitObject.GetComponent<HexManager>().currentMob;
        #endregion

        #region Mob
        if (hitObject.GetComponent<MobHealth>())
            if (hitObject.GetComponent<MobHealth>().Alive)
                hitted = hitObject;
        #endregion
        #endregion

        if (hitted == null)
        {
            if (alvosListSkill.Contains(hitObject))
                alvosListSkill.Remove(hitObject);

            return false;
        }

        if (!alvosListSkill.Contains(hitted))
        {
            return false;
        }

        objectSelectTouch = hitted;

        User.transform.LookAt(hitted.transform);

        EffectManager.Instance.TargetEffect    (hitted);
        EffectManager.Instance.TargetTargeteado(hitted);

        CheckGrid.Instance.ColorGrid(0, 0, 0, clear: true);
        return true;
    }

    protected override void RulesClickTargetElse()
    {
        if (!CheckList())
            return;

        base.RulesClickTargetElse();

        enemyAttack.CheckDistance(_skill);

        for (int i = 0; i < alvosListSkill.Count; i++)
        {
            if (alvosListSkill[i].GetComponent<MoveController>())
            {
                CheckGrid.Instance.ColorGrid(3,
                                    alvosListSkill[i].GetComponent<MoveController>().hexagonX,
                                    alvosListSkill[i].GetComponent<MoveController>().hexagonY);
            }
        }
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        if (objectSelectTouch == null)
        {
            RulesClickTargetElse();
            return;
        }

        SelectTouch = false;

        target      = objectSelectTouch;    

        EffectManager.Instance.TargetTargeteado(target);

        EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(138)+"</color>", objectSelectTouch, 3);//Esse

        mobManager.transform.LookAt(target.transform);

        ShootSkill();
    }

    protected override void ShootSkill()
    {
        currentShoot++;

        if (target != null && mobManager.attackTurn && (alvosListSkill.Count) > 0)
        {
            mobManager.transform.LookAt(target.transform);

            skillAttack.transform.position = mobManager.transform.position;

            skillAttack.transform.LookAt(Target.transform);

            skillAttack.endTurn = alvosListSkill.Count <= 1 ? true : false;

            base.ShootSkill();

            alvosListSkill.Remove(Target);
        }
        else
        {
            EndSkill();
            return;
        }

        EffectManager.Instance.PopUpDamageEffect((currentShoot) + "/" + maxShoot, User);
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        alvoUnico = false;

        base.Hit(endTurn, targetDbuff);

        if (!CheckList())
            return;

        if (targetDbuff == Target)
        {
            skillAttack.ResetOtherTarget();

            EffectManager.Instance.TargetReset(Target);            

            if (endTurn == false)
               Invoke("Check",0.2f);
        }
    }

    protected override void EndSkill()
    {
        EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(132), User);//Acabou

        selectTouch = false;

        base.EndSkill();

        /*if (mobManager.isPlayer)
        {
            if (Target.activeInHierarchy==false)
                ToolTip.Instance.TargetTooltip(Target, prop: false);
            else
                ToolTip.Instance.TargetTooltip(User, prop: false);
        }*/

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        //ResetCoolDownManager();             

        //mobManager.EndTurn();       
    }

    protected virtual bool CheckList()
    {
        if (alvosListSkill.Count > 1)
            for (int i = 0; i < alvosListSkill.Count; i++)
            {
                if (alvosListSkill[i] == null || !alvosListSkill[i].activeInHierarchy || !alvosListSkill[i].GetComponent<MobHealth>().Alive)
                {
                    alvosListSkill.Remove(alvosListSkill[i]);
                    currentShoot++;
                    EffectManager.Instance.PopUpDamageEffect((currentShoot) + "/" + maxShoot, User);
                }
            }


        if (mobManager.attackTurn && (alvosListSkill.Count) == 0)
        {
            EndSkill();
            return false;
        }

        return true;
    }
}
