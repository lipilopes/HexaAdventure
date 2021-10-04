using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobSkillOndaDefensiva : MobSkillManager
{
    [Space]
    [Header("Onda Defensiva")]
    [SerializeField, Tooltip("Recua todos a sua volta")]
    protected bool _todosNaArea             = false;

    [Space]
    [SerializeField,Range(0,1),Tooltip("Chance de Recuar o Alvo $P0")]
    protected float _chanceRecuo;

    [SerializeField, Tooltip("Total de vezes que alvo sera recuado $P1")]
    protected int _totalRecuo;

    [SerializeField,Range(0, 100), Tooltip("Porcentagem do dano da skill,Dano extra no alvo ao ser recuado $P2")]
    protected int _extraDamageRecuo;

    [SerializeField, Tooltip("maximo de falha no recuo para dar dano extra $P3")]
    protected int _maxFalhaDanoExtra;

    protected WaitForSeconds wait       = new WaitForSeconds(0.25f);
    protected WaitForSeconds waitNaArea = new WaitForSeconds(0.25f);

    protected int _countFalha=0;
    protected int _countRecuo=0;
    protected int _damageRecuo;
    protected bool _firstHit=false;

    protected override void Start()
    {
        base.Start();

        _damageRecuo = currentdamage * _extraDamageRecuo / 100;       
    }

    protected override void AttDescription()
    {
        base.AttDescription();

        AttDescription("$P0", "<b>" + _chanceRecuo * 100 + "</b>%");
        AttDescription("$P1", "<b>" + _totalRecuo + "</b>");
        AttDescription("$P2", "<b>" + _damageRecuo + "</b>");
        AttDescription("$P3", "<b>" + _maxFalhaDanoExtra + "</b>");
    }

    public override void AttDamage()
    {
        AttDescription("<b>" + (currentdamage * _extraDamageRecuo / 100) + "</b>", "$P3");

        base.AttDamage();

        Start();
    }

    protected override void CreateSkills()
    {
        base.CreateSkills();

        if(skillAttack!=null)
        skillAttack.endTurn = false;
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        target = enemyAttack.target;

        if (mobManager.isPlayer)
            if (!CheckPlayerUseSkill())
            {
                /* if (!mobManager.isPlayer)
                     enemyAttack.CheckInList();*/
                return;
            }

        base.UseSkill();

        useSkill = true;

        mobManager.ActivePassive(Passive.StartSkill, target);

        alvosListSkill.Clear();

        HexList.Clear();

        mobManager.currentTimeAttack--;

        objectSelectTouch = null;

        //if (target == null)
        //    target = User;

        if (skillAttack != null)
        {
            skillAttack.transform.position = Vector3.zero;

            Debug.LogError("Prefab -> " + skillAttack.name);
        }

        if (GameManagerScenes._gms.Adm)
        {
            if (mobManager.MesmoTime(RespawMob.Instance.PlayerTime))
                EffectManager.Instance.PopUpDamageEffect("<color=#055b05>" + Nome + "</color>", User);
            else
                EffectManager.Instance.PopUpDamageEffect("<color=#962209>" + Nome + "</color>", User);
        }

        _countFalha = 0;
        _countRecuo = 0;
        _firstHit   = false;   

        if (_todosNaArea)
        {
            AlvosNaListSkill(removeTargetSkill: true);

            //InfoTable.Instance.NewInfo(Nome + ", Tem " + (alvosListSkill.Count + 1).ToString("F0") + " alvos!!!", 10);

            foreach (var a in alvosListSkill)
                EffectManager.Instance.PopUpDamageEffect("<color=white>"+XmlMenuInicial.Instance.Get(134)+"</color>",a);//alvo

            EffectManager.Instance.PopUpDamageEffect("<color=black>"+XmlMenuInicial.Instance.Get(134)+"</color>", Target);//alvo
        }

        if (target != null)
            Debug.LogWarning(User.name + " usou a skill " + Nome + " no " + target);      

        ShootSkill();
    }

    protected override void ShootSkill()
    {
        if (target.activeInHierarchy && User != target)
            User.transform.LookAt(target.transform);

        if (GameManagerScenes._gms.Adm)
        {
            EffectManager.Instance.PopUpDamageEffect(mobManager.MesmoTime(RespawMob.Instance.PlayerTime) ? "<color=#055b05>" + Nome + "</color>" : "<color=#962209>" + Nome + "</color>", User);
        }

        mobManager.ActivePassive(Passive.ShootSkill, target);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);

        //Não mudar
        if (skillAttack != null)
        {
            skillAttack.velocidade = 0;
            skillAttack.damage     = 0;
            skillAttack.positionWho = false;
            skillAttack.transform.position = target.transform.position;
            skillAttack.UseSkill(target, this);
        }
        else
        {
            target.GetComponent<MobHealth>().Damage(User, CurrentDamage, mobManager.chanceCritical);
            Hit(true, target);
        }       
    }

    public override void Hit(bool endTurn, GameObject targetDbuff)
    {
        if (Target.GetComponent<MobManager>())
            Target.GetComponent<MobManager>().ActivePassive(Passive.EnimyHitSkill, User);

        if (targetDbuff == Target)
            mobManager.ActivePassive(Passive.TargetHitSkill, targetDbuff);

        mobManager.ActivePassive(Passive.HitSkill, targetDbuff);        

        if (_firstHit == false)
            Calcule();
    }

    protected virtual void Calcule()
    {
        _firstHit = true;
        StartCoroutine(CalculeCoroutine());
    }
    protected virtual IEnumerator CalculeCoroutine()
    {       
        while (_countRecuo < _totalRecuo && target.activeInHierarchy)
        {
            //if (skillAttack != null)
            //    if (target != null)
            //        skillAttack.transform.position = target.transform.position;

            _countRecuo++;
            
            if (target != null && target.GetComponent<MobHealth>().Alive)
            {
                if (CreateDbuff(target, Dbuff.Recuar, false, _chanceRecuo, 1, 1))
                {
                    mobManager.ActivePassive(Passive.ShootSkill, target);
                    RecuoDamage(Target);
                }
                else
                 if (_maxFalhaDanoExtra > 0)
                {
                    _countFalha++;

                    if (target != null && target.GetComponent<MobHealth>().Alive)
                        EffectManager.Instance.PopUpDamageEffect(_countFalha + "/" + _maxFalhaDanoExtra, target.GetComponent<MoveController>().Solo.gameObject);
                }                           
            }

            if (_todosNaArea)
            {
                int count = alvosListSkill.Count;

               // if (GameManagerScenes._gms.Adm)
               //InfoTable.Instance.NewInfo("Todos na Area[ "+count+" ]",3);

                for (int i = 0; i < count; i++)
                {
                    //if (GameManagerScenes._gms.Adm)
                    //    InfoTable.Instance.NewInfo("Todos na Area[ " + i + " ] - "+ alvosListSkill[i].name, 3);

                    EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(134)/*Alvo*/,alvosListSkill[i],2);

                    //yield return waitNaArea;

                    if (CreateDbuff(alvosListSkill[i], Dbuff.Recuar, false, _chanceRecuo, 1, 1))
                    {
                        RecuoDamage(alvosListSkill[i]);
                    }
                }
            }

            EffectManager.Instance.PopUpDamageEffect(_countRecuo + "/" + _totalRecuo, User);

            yield return wait;

            if (_maxFalhaDanoExtra > 0)
                if (_countFalha == _maxFalhaDanoExtra)
                {
                    RecuoDamage(Target);

                    _countFalha = 0;
                }
                    
        }

        if (target.activeInHierarchy && target.GetComponent<MobHealth>().Alive)
        {
            target.GetComponent<MobHealth>().Damage(User, currentdamage, mobManager.chanceCritical);

            yield return wait;
        }

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        //ResetCoolDownManager();

        //mobManager.EndTurn();

        _countFalha = 0;
        _countRecuo = 0;
        _firstHit = false;

        EndSkill();
    }

    protected virtual void RecuoDamage(GameObject _Target)
    {
        if (_Target.activeInHierarchy && target.GetComponent<MobHealth>().Alive)
        {
            mobManager.ActivePassive(Passive.ShootSkill, target);

            if (_Target.GetComponent<MobHealth>())
                _Target.GetComponent<MobHealth>().GetDamage(User, _damageRecuo, mobManager.chanceCritical,true,false);

            Hit(false, _Target);
        }
    }
}
