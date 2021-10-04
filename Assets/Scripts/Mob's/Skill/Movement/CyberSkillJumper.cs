using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberSkillJumper : MobSkillManager
{
    protected WaitForSeconds wait = new WaitForSeconds(0.5f);

    [Space, Header("Jumper")]
    [SerializeField]
    protected float waitTimeEndTeleport       = 0.5f;
    protected WaitForSeconds TimeEndTeleport;
    [Space]
    [SerializeField]
    protected GameObject _effectEndTeleportPrefab;
    protected GameObject effectEndTeleport;

    protected int minX = 0;
    protected int maxX = 11;
    [Space]
    protected int minY = 0;
    protected int maxY = 11;

    protected override void Start()
    {
        base.Start();

        if (GridMap.Instance!=null)
        {
            maxX = GridMap.Instance.width - 1;
            maxY = GridMap.Instance.height - 1;
        }

        WaitForSeconds TimeEndTeleport = new WaitForSeconds(waitTimeEndTeleport);

        if (_effectEndTeleportPrefab)
        {
            effectEndTeleport = Instantiate(_effectEndTeleportPrefab, _effectEndTeleportPrefab.transform.position, _effectEndTeleportPrefab.transform.rotation);

            effectEndTeleport.transform.SetParent(transform);
            effectEndTeleport.gameObject.SetActive(false);
        }
    }

    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        useSkill = true;

        base.UseSkill();       

        if (!SelectTouch)
        {           
            if (Target.GetComponent<MeshRenderer>().enabled)
                Target.GetComponent<MeshRenderer>().enabled = false;

           RegisterOtherHex(_range: Range,_clearList: true);
           RegisterOtherHexOnlyFree();
        }

        if (mobManager.isPlayer)
        {
            if (HexList.Count <= 0)
            {
                if (ToolTip.Instance)
                    ToolTip.Instance.TargetTooltipCanvas(Nome, "<color=red>"+XmlMenuInicial.Instance.Get(148)+"</color>");//Não ha Casas proximas o sufuciente!!!
                return;
            }

            if (!SelectTouch)
            {
                if (ToolTip.Instance)
                    //{
                    ToolTip.Instance.TargetTooltipCanvas(Nome,
                    GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(149),//{0}, Selecione uma das casas Verdes!!!
                    User.GetComponent<ToolTipType>()._name));
                //  }

                SelectTouch = true;

                CameraOrbit.Instance.ChangeTarget(User);
                CameraOrbit.Instance.MaxOrbitCamera();
            }
        }
        else
        {
            if (HexList.Count <= 0)
            {
                EndSkill();
                return;
            }
            else
            StartCoroutine(TeleportIA());
        }
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        Debug.LogError("UseTouchSkill() - UseTouchSkill");

        if (objectSelectTouch==null)
        {
            if (Target.GetComponent<MeshRenderer>().enabled)
                Target.GetComponent<MeshRenderer>().enabled = true;

            ColorHex(3);

            if (mobManager.isPlayer && (ToolTip.Instance))
                ToolTip.Instance.TargetTooltipCanvas(Nome,
                        GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(149),//{0}, Selecione uma das casas Verdes!!!
                        User.GetComponent<ToolTipType>()._name));
            return;
        }

        HexManager ground = objectSelectTouch.GetComponent<HexManager>();

        if (ground.x < minX && ground.x > maxX &&
            ground.y < minY && ground.y > maxY)
        {
            if (!Target.GetComponent<MeshRenderer>().enabled)
                Target.GetComponent<MeshRenderer>().enabled = true;

            ColorHex(3);

            if (mobManager.isPlayer && ToolTip.Instance)
                ToolTip.Instance.TargetTooltipCanvas(Nome,
                        GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(149),//{0}, Selecione uma das casas Verdes!!!
                        User.GetComponent<ToolTipType>()._name));

            return;
        }              

        HexList.Clear();

        SelectTouch = false;

        HexManager hex    = moveController.Solo;

        if (hex==null)
        {
            GameObject obj = GameObject.Find("Hex" + (moveController.hexagonX) + "x" + moveController.hexagonY);

            if (obj!=null)
                hex = obj.GetComponent<HexManager>();
        }

        CheckGrid.Instance.ColorGrid(0, 0, 0, clear: true);

        CheckGrid.Instance.ColorGrid(1, hex.x, hex.y);

        if (Target.GetComponent<MeshRenderer>().enabled)
            Target.GetComponent<MeshRenderer>().enabled = false;

        Teleport(hex, ground);
    }

    public IEnumerator TeleportIA()
    {
        Debug.LogError(name + " usou a skill " + Nome+"IA");

        int X      = User.GetComponent<MoveController>().hexagonX, Y = User.GetComponent<MoveController>().hexagonY;

        int random = Random.Range(0, HexList.Count);

        HexManager hex = null;

        HexManager ground = HexList[random];

        if (ground!=null && ground.free && ground.x != X && ground.y!=Y)
        {           
            if (ground.x >= minX && ground.x <= maxX &&
                ground.y >= minY && ground.y <= maxY)
                hex = ground;
        }
        if (hex != null)
        {
            objectSelectTouch = hex.gameObject;

            yield return wait;

            UseTouchSkill();
        }
        else
        StartCoroutine(TeleportIA());
    }

    protected void Teleport(HexManager _old, HexManager _new)
    {
        Debug.LogError("Old -> " + _old.name + " / New -> " + _new.name);

        Target.GetComponent<MoveController>().Walk(null, _new.x, _new.y, Dbuff: true);

        Target.transform.position = _new.transform.position;

        Target.transform.LookAt(_old.transform);

        skillAttack.positionWho = false;

        skillAttack.rotationWho = false;

        skillAttack.transform.position = (_old.transform.position);

        skillAttack.transform.LookAt(_new.transform);

        if (skillAttack.GetComponent<SkillAttackGoTo>())
            skillAttack.GetComponent<SkillAttackGoTo>().UseSkillGoTo(Target, this, _old);
        else
            skillAttack.UseSkill(Target, this);       
    }

    public override void Hit(bool endTurn,GameObject target)
    {
        Debug.LogError(name + " HIT(" + endTurn + "," + target + ")");

        if (GameManagerScenes._gms)
            GameManagerScenes._gms.NewInfo(name + " HIT(" + endTurn + "," + target + ")", 4, true);
       
        base.Hit(false, target);

        if (Target == target)
        {
            Target.GetComponent<MoveController>().Walk(objectSelectTouch, objectSelectTouch.GetComponent<HexManager>().x, objectSelectTouch.GetComponent<HexManager>().y, 0, true);

            StartCoroutine(EndTeleport());
        }
    }

    protected virtual IEnumerator EndTeleport()
    {
        if (useSkill)
        {
            UseEffectEndTeleport(true);

            yield return TimeEndTeleport;

            enemyAttack.timeAttack--;

            //ResetCoolDownManager();

            UseEffectEndTeleport(false);

            if (!Target.GetComponent<MeshRenderer>().enabled)
                Target.GetComponent<MeshRenderer>().enabled = true;

           // User.GetComponent<MobManager>().EndTurn();

            //useSkill = false;

            User.transform.LookAt(target.transform);

            //mobManager.ActivePassive(Passive.EndSkill, Target);

            EndSkill();
        }
    }

    protected virtual void UseEffectEndTeleport(bool active=true)
    {
        if (_effectEndTeleportPrefab)
        {
            effectEndTeleport.transform.position = Target.transform.position;
            SkillAttack sk = effectEndTeleport.GetComponent<SkillAttack>();

            if (sk)
            {
                if (active)
                {
                    sk.damage = currentdamage;
                    sk.who = User;
                    sk.AreaDamage = AreaDamage;
                    sk.DamageAreaDamage = DamageAreaDamage;
                    sk.OtherTargetDbuff = OtherTargetDbuff;
                    sk.TakeRealDamage = RealDamage;
                    sk.AreaRealDamage = AreaRealDamage;
                    sk.DamageCountPassive = DamageCountPassive;
                    sk.TakeDamage = !mobManager.MesmoTime(Target);

                    sk.UseSkill(Target, this);
                }
                else
                {
                    effectEndTeleport.SetActive(false);
                }
            }
            else
            if (effectEndTeleport.GetComponent<ParticleSystem>())
            {
                if (active)
                {                  
                    if (!effectEndTeleport.activeInHierarchy)
                        effectEndTeleport.SetActive(true);

                    effectEndTeleport.GetComponent<ParticleSystem>().Play();
                }
                else
                    effectEndTeleport.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}

