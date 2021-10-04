using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillTeleportSupremo : MobSkillTeleport
{
    [Space]
    [Header("Teleporte Supremo")]
    [SerializeField, Tooltip("dano ao terminar efeito de teleport")]
    protected bool _damageBeforeEffect = true;
    [Space]
    [SerializeField, Tooltip("Dbuff ao teleportar inimigo $BuffE + ElementNumber, todos igual ao do outro com 'E' no final")]
    protected List<DbuffBuff> _dbuffBuffEnemy;
    public    List<DbuffBuff> _DbuffBuffEnemy { get { return _dbuffBuffEnemy; } }
    [Space]
    [SerializeField, Tooltip("Dbuff ao teleportar Amigo $BuffF + ElementNumber, todos igual ao do outro com 'F' no final")]
    protected List<DbuffBuff> _dbuffBuffFriend;
    public List<DbuffBuff>    _DbuffBuffFriend { get { return _dbuffBuffFriend; } }

    protected override void Start()
    {
        waitToTeleport    = new WaitForSeconds(_waitToTeleport);
        waitToEndTeleport = new WaitForSeconds(_waitToEndTeleport);

        _buff = currentdamage * _baseBuffRecHp / 100;

        AttDescription();

        //currentdamage = 0;

        //AttDescription("$P0", "<b>" + _buff + " - " + (_buff + _bonusBuffRecHpMax) + "</b>");
        //AttDescription("$P1", _autoCure ? "<color=blue>Se cura Caso seja o alvo.</color>" : " <color=red>NÃO se cura Caso seja o alvo.</color>");

        if (User != null)
        {
            if (mobManager == null)
                mobManager = User.GetComponent<MobManager>();
            if (enemyAttack == null)
                enemyAttack = User.GetComponent<EnemyAttack>();
            if (mobCooldown == null)
                mobCooldown = User.GetComponent<MobCooldown>();
            if (moveController == null)
                moveController = User.GetComponent<MoveController>();
        }

        if (passive == null)
            passive = GetComponent<PassiveManager>();
    }

    protected override void AttDescription()
    {
        base.AttDescription();
      
        #region Dbuff Enemy
        int count = _DbuffBuffEnemy.Count;

        if (count > 0)
        for (int i = 0; i < count; i++)
        {
            if (_DbuffBuffEnemy[i]._dbuffDuracaoMin == -1)
                    _DbuffBuffEnemy[i]._dbuffDuracaoMin = currentdamage;

            if (_DbuffBuffEnemy[i]._dbuffDuracaoMax == -1)
                    _DbuffBuffEnemy[i]._dbuffDuracaoMax = currentdamage;

            string buff = ("$BuffE" + i);
            if (Description.Contains(buff))
            {
                AttDescription(buff, "<b>" + XmlMenuInicial.Instance.DbuffTranslate(_DbuffBuffEnemy[i]._buff) + "</b>");

                if (Description.Contains("_"))
                    AttDescription("_", " $DurE" + i + " de ");
            }

            string chance = ("$%E" + i);
            if (Description.Contains(chance))
                AttDescription(chance, "<b>" + (_DbuffBuffEnemy[i]._dbuffChance * 100).ToString() + "%</b>");

            string min = ("$MinE" + i);
            if (Description.Contains(min))
                AttDescription(min, "<b>" + _DbuffBuffEnemy[i]._dbuffDuracaoMin + "</b>");

            string max = ("$MaxE" + i);
            if (Description.Contains(max))
                AttDescription(max, "<b>" + _DbuffBuffEnemy[i]._dbuffDuracaoMax + "</b>");

            string dur = ("$DurE" + i);
            if (Description.Contains(dur))
            {
                string Dur = "<b>" + _DbuffBuffEnemy[i]._dbuffDuracaoMin + " - " + _DbuffBuffEnemy[i]._dbuffDuracaoMax + "</b>";

                if (_DbuffBuffEnemy[i]._dbuffDuracaoMin == _DbuffBuffEnemy[i]._dbuffDuracaoMax)
                    Dur = "<b>" + _DbuffBuffEnemy[i]._dbuffDuracaoMax + "</b>";

                AttDescription(dur, Dur);
            }

            string acumule = ("$AcE" + i);
            if (Description.Contains(acumule))
                AttDescription(acumule, _DbuffBuffEnemy[i]._acumule
                    ? "(<color=blue><b>" + XmlMenuInicial.Instance.Get(193) + "</b></color>" +//Acumula
                        (_DbuffBuffEnemy[i]._acumuleMax > 1 ? " - x" + _DbuffBuffEnemy[i]._acumuleMax + ")" : "")
                    : ""//"<color=red><b>"  + XmlMenuInicial.Instance.Get(194) + "</b></color>"//Não acumula
                    );

        }
        #endregion

        #region Dbuff Friend
        count = _DbuffBuffFriend.Count;
        if (count > 0)
            for (int i = 0; i < count; i++)
            {
                if (_DbuffBuffFriend[i]._dbuffDuracaoMin == -1)
                    _DbuffBuffFriend[i]._dbuffDuracaoMin = currentdamage;

                if (_DbuffBuffFriend[i]._dbuffDuracaoMax == -1)
                    _DbuffBuffFriend[i]._dbuffDuracaoMax = currentdamage;

                string buff = ("$BuffF" + i);
                if (Description.Contains(buff))
                {
                    AttDescription(buff, "<b>" + XmlMenuInicial.Instance.DbuffTranslate(_DbuffBuffFriend[i]._buff) + "</b>");

                    if (Description.Contains("_"))
                        AttDescription("_", " $DurF" + i + " de ");
                }

                string chance = ("$%F" + i);
                if (Description.Contains(chance))
                    AttDescription(chance, "<b>" + (_DbuffBuffFriend[i]._dbuffChance * 100).ToString() + "%</b>");

                string min = ("$MinF" + i);
                if (Description.Contains(min))
                    AttDescription(min, "<b>" + _DbuffBuffFriend[i]._dbuffDuracaoMin + "</b>");

                string max = ("$MaxF" + i);
                if (Description.Contains(max))
                    AttDescription(max, "<b>" + _DbuffBuffFriend[i]._dbuffDuracaoMax + "</b>");

                string dur = ("$DurF" + i);
                if (Description.Contains(dur))
                {
                    string Dur = "<b>" + _DbuffBuffFriend[i]._dbuffDuracaoMin + " - " + _DbuffBuffFriend[i]._dbuffDuracaoMax + "</b>";

                    if (_DbuffBuffFriend[i]._dbuffDuracaoMin == _DbuffBuffFriend[i]._dbuffDuracaoMax)
                        Dur = "<b>" + _DbuffBuffFriend[i]._dbuffDuracaoMax + "</b>";

                    AttDescription(dur, Dur);
                }

                string acumule = ("$AcF" + i);
                if (Description.Contains(acumule))
                    AttDescription(acumule, _DbuffBuffFriend[i]._acumule
                        ? "(<color=blue><b>" + XmlMenuInicial.Instance.Get(193) + "</b></color>" +//Acumula
                            (_DbuffBuffFriend[i]._acumuleMax > 1 ? " - x" + _DbuffBuffFriend[i]._acumuleMax + ")" : "")
                        : ""//"<color=red><b>"  + XmlMenuInicial.Instance.Get(194) + "</b></color>"//Não acumula
                        );

            }
        #endregion        
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        if (!SelectTouch)
        {
            RegisterOtherHex(Target.GetComponent<MoveController>().hexagonX, Target.GetComponent<MoveController>().hexagonY, rangeFriends, _clearList: false);

            RegisterOtherHex(_range: Range, _clearList: true);

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
                            RegisterOtherHex(mc.hexagonX, mc.hexagonY, rangeFriends,_clearList:false);
                        }
                    }
            }           

            RegisterOtherHexOnlyFree();
        }

        if (mobManager.isPlayer)
        {
            if (HexList.Count <= 0)
            {
                ToolTip.Instance.TargetTooltipCanvas(Nome, "<color=red>" + XmlMenuInicial.Instance.Get(148) + "</color>");//Não ha Casas proximas o sufuciente!!!

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

        if (mobManager.MesmoTime(Target))
            UseInFriend();
        else
            UseInEnimy();
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        if (mobManager.MesmoTime(targetDbuff))
            HitInFriend(targetDbuff);
        else
        {
            HitInEnimy(targetDbuff);

            if (targetDbuff == Target && Target.GetComponent<MobHealth>())
                Target.GetComponent<MobHealth>().Damage(User, currentdamage, mobManager.chanceCritical);
        }

        base.Hit(endTurn, targetDbuff);
    }

    protected override IEnumerator TimeEffect()
    {
        base.TimeEffect();

        if (!mobManager.MesmoTime(Target))
        {
            if (_damageBeforeEffect && Target.GetComponent<MobHealth>())
                Target.GetComponent<MobHealth>().Damage(User, currentdamage, mobManager.chanceCritical);
        }

        yield return null;
    }

    protected virtual void UseInFriend()
    {

    }

    protected virtual void UseInEnimy()
    {

    }

    protected virtual void HitInFriend(GameObject _target)
    {
        if (_target == User ||
            _target.GetComponent<MobManager>() && _target == mobManager.MesmoTime(_target.GetComponent<MobManager>().TimeMob))
        {
            int count = _DbuffBuffFriend.Count;
            if (count > 0)
                for (int i = 0; i < count; i++)
                    CreateDbuff(_target, _DbuffBuffFriend[i]._buff, _DbuffBuffFriend[i]._forMe, _DbuffBuffFriend[i]._dbuffChance, _DbuffBuffFriend[i]._dbuffDuracaoMin, _DbuffBuffFriend[i]._acumuleMax, _DbuffBuffFriend[i]._acumule, _DbuffBuffFriend[i]._acumuleMax);
        }
    }

    protected virtual void HitInEnimy(GameObject _target)
    {
        if (/*otherTargetDbuff &&*/_target == Target ||
            otherTargetDbuff)
        {
            int count = _DbuffBuffEnemy.Count;
            if (count > 0)
                for (int i = 0; i < count; i++)               
                    CreateDbuff(_target, _DbuffBuffEnemy[i]._buff, _DbuffBuffEnemy[i]._forMe, _DbuffBuffEnemy[i]._dbuffChance, _DbuffBuffEnemy[i]._dbuffDuracaoMin, _DbuffBuffEnemy[i]._acumuleMax, _DbuffBuffEnemy[i]._acumule, _DbuffBuffEnemy[i]._acumuleMax);                    
        }
    }

    protected override void RegisterOtherHexOnlyFree()
    {
        if (HexList.Count > 0)
            for (int i = 0; i < HexList.Count; i++)
            {
                if (!HexList[i].free && HexList[i].currentMob != null)
                    HexList.Remove(HexList[i]);
            }
    }
}
