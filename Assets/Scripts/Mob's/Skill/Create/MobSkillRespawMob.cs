using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClonesControl
{
    public GameObject _clone;
    public int        _time;
}

public class MobSkillRespawMob : MobSkillManager
{
    [Space]
    [Header("Clone")]
    [SerializeField, Tooltip("Prefab do clone $P0")]
    protected GameObject _prefab;
    [Space]
    [SerializeField]
    protected ParticleSystem _effectPrefab;
    protected ParticleSystem _effect;
    [SerializeField, Tooltip("Tempo de vida dos Clones,-1 para ser ate a vida acabar  $P1")]
    protected int _maxTimeClone = -1;
    [SerializeField, Tooltip("Range Respaw Clones $P2"), Range(0, 6)]
    protected int _range = 3;
    [SerializeField, Tooltip("dano do mob -1 para pegar do pai $P3")]
    protected int _damage = -1;
    [Space]
    [SerializeField, Tooltip("Vida do mob -1 para pegar do pai $P4")]
    protected int _health = -1;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de dano com base no 'pai' $P3")]
    protected int minPorcentDamage = 2;
    [SerializeField, Range(0, 100)]
    protected int maxPorcentDamage = 5;
    [Space]
    [SerializeField, Range(0, 100), Tooltip("Porcentagem de vida com base no 'pai' $P4")]
    protected int minPorcentHealth = 2;
    [SerializeField, Range(0, 100)]
    protected int maxPorcentHealth = 5;
    [Space]
    [SerializeField, Tooltip("Maximo de clones ativos, -1 para ilimitado  $P5")]
    protected int _maxClonesActive = -1;
    [SerializeField, Tooltip("Apos Morrer Clone sera Desativado")]
    protected bool _desactiveClones = true;
    [Space]
    [SerializeField, Tooltip("Caso for o player controla o clone")]
    protected bool _controlClone = true;
    [Space]
    [SerializeField, Tooltip("Troca de Posição com o clone")]
    protected bool _changePosition = false;

    protected List<ClonesControl> clones = new List<ClonesControl>();

    protected int current = 0;

    public void TesteFindClone()
    {
        FindClone();
    }

    public void DesactiveEffect()
    {
        _effect.Stop();
        _effect.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        print("START " + name);

        base.Start();

        StartCoroutine(StartCoroutine());

        if (_effect==null && _effectPrefab!=null)
        {
            _effect = Instantiate(_effectPrefab,new Vector3(-99,-99,-99),User.transform.rotation,User.transform);
            DesactiveEffect();          
        }     
    }

    IEnumerator StartCoroutine()
    {
        if (_prefab != null && _prefab.GetComponent<MobManager>())
            _prefab.GetComponent<MobManager>().classe = MobManager.Classe.manual;

        yield return new WaitForSeconds(2);

        if (_damage == -1)
            _damage = User.GetComponent<MobManager>().damage + CurrentDamage;

        if (_health == -1)
            _health = (int)User.GetComponent<MobManager>().health + CurrentDamage;

        if (_prefab != null)
        {
            if (_prefab.GetComponent<ToolTipType>())
                AttDescription("$P0", "<b>" + _prefab.GetComponent<ToolTipType>()._name + "</b>");
            else
                AttDescription("$P0", "<b>" + _prefab.name + "</b>");
        }
        else
            AttDescription("$P0", "<b>"+XmlMenuInicial.Instance.Get(77)+"</b>");//Clone

        AttDescription("$P1", _maxTimeClone == -1 ? "<b>" + XmlMenuInicial.Instance.Get(78)/*Até morrer*/+ "</b>" : "<b>" + _maxTimeClone + "</b>");

        AttDescription("$P2", "<b>" + _range + "</b>");

        if (minPorcentDamage != maxPorcentDamage)
            AttDescription("$P3", "<b>" + (_damage * minPorcentDamage / 100).ToString("F0") + " - " + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");
        else
            AttDescription("$P3", "<b>" + ((_damage * maxPorcentDamage / 100)).ToString("F0") + "</b>");

        if (minPorcentHealth != maxPorcentHealth)
            AttDescription("$P4", "<b>" + (_health * minPorcentHealth / 100).ToString("F0") + " - " + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");
        else
            AttDescription("$P4", "<b>" + ((_health * maxPorcentHealth / 100)).ToString("F0") + "</b>");

        AttDescription("$P5", _maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)+/*Ilimitado*/"</b>" : "<b>" + _maxClonesActive + "</b>");

        if (_maxClonesActive > 0)
            CreateClone(_maxClonesActive);

        currentdamage = 0;
    }

    public override void AttDamage()
    {
        AttDescription("<b>" +
            (minPorcentDamage == maxPorcentDamage ?
            ((User.GetComponent<MobManager>().damage * maxPorcentDamage / 100)).ToString("F0") :
            (User.GetComponent<MobManager>().damage * minPorcentDamage / 100).ToString("F0") + " - " + ((User.GetComponent<MobManager>().damage * maxPorcentDamage / 100)).ToString("F0")) + "</b>", "$P3");

        AttDescription("<b>" +
            (minPorcentHealth == maxPorcentHealth ?
            (User.GetComponent<MobManager>().health * minPorcentHealth / 100).ToString("F0") :
            (User.GetComponent<MobManager>().health * minPorcentHealth / 100).ToString("F0") + " - " + ((User.GetComponent<MobManager>().health * maxPorcentHealth / 100)).ToString("F0")) + "</b>", "$P4");

        if (User.GetComponent<MobManager>().isPlayer && _controlClone)
            description += "\n<color=blue>"+XmlMenuInicial.Instance.Get(143)+"</color>";//Pode controlar o(a) $P0 em seu turno

        if (_changePosition)
            description += "\n<color=blue>"+XmlMenuInicial.Instance.Get(144)+"</color>";//Pode trocar de lugar com o $P0, Assim que criado.


        base.AttDamage();

        //Start();
    }

    #region Create Clones
    public override void UseSkill()
    {
        if (!CheckUseSkill())
            return;

        base.UseSkill();

        RegisterOtherHex(_range: _range, _clearList: true);
        RegisterOtherHexOnlyFree();

        //if (CountClonesActives >= clones.Count - 1)
        //    RegisterClones();

        current = 0;

        EffectManager.Instance.PopUpDamageEffect(current + "/" + _maxClonesActive, User);

        if (!mobManager.isPlayer)
        {
            if (HexList.Count <= 0)
            {
                useSkill = false;
                enemyAttack.canSkill1 = false;
                enemyAttack.CheckInList();
                return;
            }

            IASelectGround();
        }
        else
        {
            if (HexList.Count <= 0)
            {
                useSkill = false;

                //if (mobManager.isPlayer)
                    ToolTip.Instance.TargetTooltipCanvas(Nome, "<color=red>"+ XmlMenuInicial.Instance.Get(145)+"</color>");//Não ha Casas proximas o sufuciente!!!
                return;
            }

            //if (mobManager.isPlayer)
            ToolTip.Instance.TargetTooltipCanvas(Nome,
                GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(146), ""+current)+//Selecione outra casa Verde para Criar outro(a).\n<color=green>" + current + "</color>/
                (_maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)+/*Ilimitado*/"</b>" : _maxClonesActive + ""));

            SelectTouch = true;
        }
    }

    protected virtual void IASelectGround()
    {
        ///grounds livre
        List<HexManager> IaList      = new List<HexManager>();

        ///Inimigos nos grounds
        List<HexManager> IaEnimyList = new List<HexManager>();

        foreach (var h in hexList)
        {
            if (h.free == false && h.currentMob!=null)
            {
                if (mobManager.MesmoTime(h.currentMob) == false)
                {
                    IaEnimyList.Add(h);
                }
            }
            else
                IaList.Add(h);
        }

        if (IaList.Count != 0)
        {
            objectSelectTouch = IaList[Random.Range(0, IaList.Count)].gameObject;

            UseTouchSkill();
            return;
        }
        else
        if (IaEnimyList.Count != 0 && CheckGrid.Instance != null)
        {
            objectSelectTouch = null;

            while (objectSelectTouch == null)
            {
                if (IaEnimyList.Count > 0)
                {
                    int current = Random.Range(0, IaEnimyList.Count);

                    MoveController Enemy = IaEnimyList[current].currentMob.GetComponent<MoveController>();

                    IaEnimyList.Remove(IaEnimyList[current]);

                    objectSelectTouch = IAFindHex(Enemy).gameObject;

                    UseTouchSkill();
                    break;
                }
                else
                    break;
            }                  
        }
        else
            EndSkill();
    }

    /// <summary>
    /// Procura Hex No raio de _Find q esteja no raio da skill
    /// </summary>
    /// <param name="_find"></param>
    /// <returns></returns>
    protected virtual HexManager IAFindHex(MoveController _find)
    {

        HexManager _hexFind           = null;

        List<HexManager> _hexFindList = CheckGrid.Instance.RegisterRadioHex(X: _find.hexagonX, Y: _find.hexagonY, colore: false);

        int countHL = HexList.Count,
            count   = _hexFindList.Count;

        for (int _hL = 0; _hL < countHL; _hL++)
        {
            if (_hexFindList != null)            
                break;
            
            for (int i = 0; i < count; i++)
            {
                if (HexList[_hL] == _hexFindList[i])
                {
                    _hexFind = _hexFindList[i];
                    break;
                }
            }
        }

        return _hexFind;
    }

    protected override void UseTouchSkill()
    {
        base.UseTouchSkill();

        Debug.LogError("UseTouchSkill() - RespawMob");

        if (objectSelectTouch != null)
        {

            ////Trocar de Lugar
            //if (objectSelectTouch.GetComponent<MobHealth>() || objectSelectTouch.GetComponent<HexManager>() && !objectSelectTouch.GetComponent<HexManager>().free)
            //{
            //    foreach (var c in clones)
            //    {
            //        if (c._clone == objectSelectTouch)
            //        {
            //            Debug.LogError("UseTouchSkill() - ChangePosition");
            //            if (ChangePosition(objectSelectTouch))
            //            {
            //                //objectSelectTouch = null;
            //                //RegisterOtherHex(_range: range, _clearList: true);
            //                //RegisterOtherHexOnlyFree();
            //                return;
            //            }
            //            else
            //                break;
            //        }
            //    }
            //}
            //else 
            if (objectSelectTouch.GetComponent<HexManager>() && objectSelectTouch.GetComponent<HexManager>().free)
            {
                SelectTouch = false;
                Debug.LogError("UseTouchSkill() - RespawClone");

                RespawClone(objectSelectTouch.GetComponent<HexManager>());

                objectSelectTouch = null;
                return;
            }
                      
        }

        Debug.LogError("UseTouchSkill() - TargetElse");
        RulesClickTargetElse();
    }

    protected override bool RulesClickTarget(GameObject hitObject)
    {

        Debug.LogError("RulesClickTarget("+ hitObject + ")");

        GameObject hitted = null;

        #region Caso Click no Hexagono                             
        #region Mob in Hex
        if (hitObject.GetComponent<HexManager>())
            if (hitObject.GetComponent<HexManager>().free)
                 hitted = hitObject.GetComponent<HexManager>().gameObject;
        #endregion

        #region Mob
        if (hitObject.GetComponent<MobHealth>())
            if (hitObject.GetComponent<MobHealth>().Alive)
                hitted = hitObject;
        #endregion
        #endregion

        if (hitted == null)
        {
            Debug.LogError("RulesClickTarget() is null");
            return false;
        }

        //Trocar de Lugar
        if (_changePosition)
        {
            if (hitObject.GetComponent<MobHealth>() || hitObject.GetComponent<HexManager>() && !hitObject.GetComponent<HexManager>().free)
            {
                //foreach (var c in clones)
                //{
                //    if (c._clone == hitObject)
                //    {
                Debug.LogError("UseTouchSkill() - ChangePosition");
                if (ChangePosition(hitObject))
                {
                    //objectSelectTouch = null;
                    //RegisterOtherHex(_range: range, _clearList: true);
                    //RegisterOtherHexOnlyFree();
                    return false;
                }
                //  else
                //      break;
                //  }
                // }
            }
        }

        if (hitObject.GetComponent<HexManager>() && !HexList.Contains(hitObject.GetComponent<HexManager>()))
        {
            Debug.LogError("RulesClickTarget() hitted not found");

            RegisterOtherHex(_range: _range, _clearList: true);
            RegisterOtherHexOnlyFree();

            //for (int i = 0; i < HexList.Count; i++)
            //    CheckGrid.Instance.ColorGrid(3, HexList[i].x, HexList[i].y);
            return false;
        }

        ////Trocar de Lugar
        //if (hitObject.GetComponent<MobHealth>() || hitObject.GetComponent<HexManager>() && hitObject.GetComponent<HexManager>().free == false)
        //{
        //    foreach (var c in clones)
        //    {
        //        if (c._clone == hitted)
        //        {
        //            if (ChangePosition(hitted))
        //            {
        //                objectSelectTouch = null;
        //                RegisterOtherHex(_range: range, _clearList: true);
        //                RegisterOtherHexOnlyFree();
        //                return false;
        //            }
        //            else
        //                break;
        //        }
        //    }
        //}

        objectSelectTouch = hitted;

        User.transform.LookAt(hitted.transform);

        EffectManager.Instance.TargetEffect(hitted);
        EffectManager.Instance.TargetTargeteado(hitted);

        CheckGrid.Instance.ColorGrid(0, 0, 0, clear: true);
        return true;
    }

    GameObject _changePos      = null;
    int        _changePosNumber = 0;

    /// <summary>
    /// User troca de posição com clone
    /// </summary>
    /// <returns></returns>
    protected virtual bool ChangePosition(GameObject t)
    {
        if (_changePosition && t != null)
        {
            Debug.LogError("ChangePosition()");

            if (t.GetComponent<HexManager>() && t.GetComponent<HexManager>().currentMob != null)
                t = t.GetComponent<HexManager>().currentMob;
            else
            if (t.GetComponent<MobManager>() == null)
                return false;

            Debug.LogError("ChangePosition() - " + t.name);

            if (t != null)
            {
                foreach (var c in clones)
                {
                    if (c._clone == t)
                    {
                        _changePosNumber++;
                        _changePos = t;

                        if (_changePosNumber < 3 || _changePosition != t)
                        {
                            objectSelectTouch = null;

                            if (_changePos != t && _changePosNumber>0)
                                _changePosNumber = 1;

                            break;
                        }

                        SelectTouch       = false;
                        objectSelectTouch = null;

                        HexManager _hexUser = moveController.Solo;
                        int userX = _hexUser.x,
                            userY = _hexUser.y;
                        _hexUser.free       = true;
                        _hexUser.currentMob = null;

                        MoveController _cloneMc = t.GetComponent<MoveController>();
                        HexManager _hexClone    = _cloneMc.Solo;
                        _hexClone.free          = true;
                        _hexClone.currentMob    = null;
                       
                        moveController.transform.position = _hexClone.transform.position;                        
                        moveController.Walk(null, _hexClone.x, _hexClone.y, 0, true);
                        _hexClone.free = false;
                        _hexClone.currentMob = User;

                        _cloneMc.transform.position = _hexUser.transform.position;                                              
                        _cloneMc.Walk(null, userX, userY, 0, true);
                        _hexUser.free       = false;
                        _hexUser.currentMob = t;

                        if (mobManager.isPlayer)
                        {
                            EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(147), _hexClone.gameObject);//Trocou
                            EffectManager.Instance.PopUpDamageEffect(XmlMenuInicial.Instance.Get(147), _hexUser.gameObject);//Trocou
                        }

                        objectSelectTouch = null;

                        //RegisterOtherHex(_range: range, _clearList: true);
                        //RegisterOtherHexOnlyFree();

                        SelectTouch = true;

                        _changePosNumber = 0;
                        _changePos       = null;
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Respawna os clones no jogo
    /// </summary>
    /// <param name="_hex"></param>
    protected virtual void RespawClone(HexManager _hex)
    {
        SelectTouch = false;       

        Debug.LogError("RespawClone(" + _hex + ")");

        if (_hex.free == false)
        {
            Debug.LogError("RespawClone(" + _hex + ") -> Não esta free");
            SelectTouch = true;
            return;
        }

        current++;

        GameObject clone = FindClone();

        clone.tag = "Clone";

        mobManager.ActivePassive(Passive.ShootSkill,Target);

        _hex.free       = false;
        _hex.currentMob = clone;
        _hex.WalkInHere();

        clone.GetComponent<MoveController>().hexagonX = _hex.x;
        clone.GetComponent<MoveController>().hexagonY = _hex.y;

        clone.transform.position = _hex.transform.position;

        if (skillAttack != null)
        {
            skillAttack.endTurn = false;

            skillAttack.UseSkill(_hex.gameObject, this);
        }

        if (mobManager.isPlayer)
            ToolTip.Instance.TargetTooltipCanvas(Nome,
                GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(146), "" + current)//Selecione outra casa Verde para Criar outro(a).\n<color=green>" + current + "</color>/
                + (_maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)+/*Ilimitado*/"</b>" : _maxClonesActive + ""));
        else
          if (Random.value >= 0.5f)
        {
            ChangePosition(clone);
        }

        GameObject t = TurnSystem.Instance.GetRandomMob(User.GetComponent<MobManager>().TimeMob);

        if (t != null)
            clone.transform.LookAt(t.transform.position);
        else
            clone.transform.LookAt(User.transform.position);

        if (_effect != null)
        {
            _effect.transform.position = clone.transform.position;
            _effect.gameObject.SetActive(true);
            _effect.GetComponentInChildren<ParticleSystem>().Play(true);

            Invoke("DesactiveEffect", 5);
        }
        else
            EffectManager.Instance.RespawEffect(clone, 3);

        clone.GetComponent<MobHealth>().ReBorn(/*_controlClone*/true);

        EffectManager.Instance.PopUpDamageEffect(
            "<color=green>" + current + "</color>/" 
            + (_maxClonesActive == -1 ? "<b>"+XmlMenuInicial.Instance.Get(79)+/*Ilimitado*/"</b>" : _maxClonesActive + ""), clone);

        if (current == _maxClonesActive)
        {
            EndSkill();
        }
        else
        {
            RegisterOtherHex(_range: _range, _clearList: true);
            RegisterOtherHexOnlyFree();

            if (mobManager.isPlayer)
                SelectTouch = true;
            else
                IASelectGround();
        }           
    }

    /// <summary>
    /// Cria os clones e adciona na lista
    /// </summary>
    /// <param name="create">numeros de clones a serem criados</param>
    /// <returns></returns>
    protected virtual GameObject CreateClone(int create = 2)
    {
        int count = 0;

        Debug.LogError("CreateClone(" + create + ")");

        while (create > count)
        {
            GameObject obj = null;

            /*  if (RespawMob.Instance != null)
                  obj = RespawMob.Instance.CreateMob(_prefab, User.GetComponent<MoveController>().hexagonX, User.GetComponent<MoveController>().hexagonY, false, false, mobM.TimeMob);
              else*/
            // {
            obj = Instantiate(_prefab, new Vector3(-99, -99, -99), User.transform.rotation);
            //}

            obj.AddComponent<EffectStigma>();
            obj.GetComponent<EffectStigma>().desactiveThis     = _desactiveClones;
            obj.GetComponent<EffectStigma>().desativaSemTarget = false;            

            if (obj.GetComponent<MobManager>())
            {
                obj.GetComponent<MobManager>().classe = MobManager.Classe.manual;

                obj.GetComponent<MobManager>().TimeMob = mobManager.TimeMob;

                obj.GetComponent<MobManager>().getBonusPlayer = false;

                if (mobManager.isPlayer && _controlClone)
                {
                    obj.GetComponent<MobManager>().isPlayer = true;
                    obj.AddComponent<PlayerControl>();
                }
            }

            if (!obj.activeInHierarchy)
                obj.SetActive(true);

            Debug.LogError("CreateClone (" + (clones.Count + 1) + "/" + create + ") Criado " + obj.name);

            obj.name = _prefab.name + " " + (clones.Count + 1) + "  (" + User.GetComponent<ToolTipType>()._name + ")";

            ToolTipType tt = obj.GetComponent<ToolTipType>();

            if (tt != null)
            {
                if (RespawMob.Instance && mobManager.TimeMob == (RespawMob.Instance.PlayerTime))
                {
                    tt._name = obj.GetComponent<ToolTipType>()._name + " Clone";
                }
                else
                {
                    tt._name = obj.GetComponent<ToolTipType>()._name;

                    int length = obj.GetComponentsInChildren<Effects>().Length;

                    if (length>0)
                    for (int i = 0; i < length; i++)
                    {
                        if (obj.GetComponentsInChildren<Effects>()[i] != null &&
                            obj.GetComponentsInChildren<Effects>()[i].name == "Effect_Clone"       &&
                            obj.GetComponentsInChildren<Effects>()[i].gameObject.activeInHierarchy &&
                           (int)GameManagerScenes._gms.Dificuldade() >= 2)
                            obj.GetComponentsInChildren<Effects>()[i].gameObject.SetActive(false);
                    }                  
                }
            }

            if (obj.GetComponent<MoveController>())
            {
                obj.GetComponent<MoveController>().hexagonX = -1;

                obj.GetComponent<MoveController>().hexagonY = -1;

                obj.transform.position = new Vector3(0, -999, 0);
            }

            if (obj.GetComponent<MobHealth>())
            {
                obj.GetComponent<MobHealth>().Health = 0;
                // obj.GetComponent<MobHealth>().CheckHp(false);
            }

            if (obj.GetComponent<IaAttackMob>())
                obj.GetComponent<IaAttackMob>().AttAttack();

            ClonesControl c = new ClonesControl();
            c._clone = obj;
            c._time  = -1;
            clones.Add(c);
            //  c._clone.GetComponent<MobManager>().maxTimeWalk = mobM.maxTimeWalk;

            count++;

            if (RespawMob.Instance)
                RespawMob.Instance.allRespaws.Add(obj);
        }

        //FindClone();
        
        return clones[clones.Count - 1]._clone;
    }

    /// <summary>
    /// Procura um Clone não ativo
    /// </summary>
    /// <returns></returns>
    protected virtual GameObject FindClone()
    {
        GameObject clone = null;

        Debug.LogError("FindClone()");
      
        foreach (var c in clones)
        {
            if (!c._clone.GetComponent<MobHealth>().Alive || c._time<=0)
            {
                clone = c._clone;

                c._time = _maxTimeClone;

                Debug.LogError("FindClone() -> " + c._clone);
                break;
            }
        }

        if (clone==null)
        {
            CreateClone(1);

            //clone = clones[clones.Count - 1]._clone;

            //clones[clones.Count-1]._time = _maxTimeClone;

            //Debug.LogError("FindClone() Create -> " + clone);

            return FindClone();
        }

        int porcentDamage = Random.Range(minPorcentDamage, maxPorcentDamage + 1),
            porcentHealth = Random.Range(minPorcentHealth, maxPorcentHealth + 1);

        int health = (_health * porcentHealth) / 100;

        if (clone.GetComponent<MobManager>())
        {
            if (GameManagerScenes._gms.Adm && InfoTable.Instance)
            {
                InfoTable.Instance.NewInfo(clone.name + " Damage[" + porcentDamage + "% - " + ((_damage * porcentDamage) / 100) + "]", 10);
                InfoTable.Instance.NewInfo(clone.name + " Health[" + porcentHealth + "% - " + ((_health * porcentHealth) / 100) + "]", 10);
            }

            clone.GetComponent<MobManager>().damage = (_damage * porcentDamage / 100);

            clone.GetComponent<MobManager>().health = health;
        }

        if (clone.GetComponent<MobHealth>())
        {           
            clone.GetComponent<MobHealth>().MaxHealth = health;

            clone.GetComponent<MobHealth>().Health = clone.GetComponent<MobHealth>().MaxHealth;          
        }


        if (clone.GetComponent<SkillManager>())
        {
            clone.GetComponent<SkillManager>().AttDamageSkills();

            int value = 0;
            if (clone.GetComponent<SkillManager>().Skills.Count >= 1)
            {
                //clone.GetComponent<MobCooldown>().AttCooldown(0, 0);

                clone.GetComponent<MobCooldown>().timeCooldownSkill[0] = value;

                if (clone.GetComponent<SkillManager>().Skills[0] != null)
                    clone.GetComponent<SkillManager>().Skills[0].CooldownCurrent = value;
            }

            if (clone.GetComponent<SkillManager>().Skills.Count >= 2)
            {
                //clone.GetComponent<MobCooldown>().AttCooldown((int)Random.Range(0, clone.GetComponent<SkillManager>().Skills[1].CooldownMax), 1);


                if (clone.GetComponent<SkillManager>().Skills[1] != null)
                {
                    value = Random.Range(0, clone.GetComponent<SkillManager>().Skills[1].CooldownMax);
                    clone.GetComponent<MobCooldown>().timeCooldownSkill[1] = value;
                    clone.GetComponent<SkillManager>().Skills[1].CooldownCurrent = value;
                }
            }

            if (clone.GetComponent<SkillManager>().Skills.Count >= 3)
            {

                //clone.GetComponent<MobCooldown>().AttCooldown((int)Random.Range(0, clone.GetComponent<SkillManager>().Skills[2].CooldownMax), 2);

                if (clone.GetComponent<SkillManager>().Skills[2] != null)
                {
                    value = Random.Range(0, clone.GetComponent<SkillManager>().Skills[2].CooldownMax);
                    clone.GetComponent<MobCooldown>().timeCooldownSkill[2] = value;
                    clone.GetComponent<SkillManager>().Skills[2].CooldownCurrent = value;
                }
            }
        }

        if (clone.GetComponent<EffectStigma>() == null)
        {
            clone.AddComponent<EffectStigma>();
            clone.GetComponent<EffectStigma>().desactiveThis = _desactiveClones;
        }

        clone.GetComponent<EffectStigma>().StartStigma(null, User, clone, _maxTimeClone, false);


        Debug.LogError("FindClone() return - >" + clone + " HP:" + clone.GetComponent<MobHealth>().Health);

        return clone;
    }

    /// <summary>
    /// Contador de Clones Vivos na scena
    /// </summary>
    /// <param name="rule">Regra</param>
    /// <returns></returns>
    protected int CountClonesActives()
    {
        int count = 0;

        foreach (var c in clones)
        {
            if (c._clone.GetComponent<MobHealth>().Alive && c._time>0)
            {
                count++;
            }
        }

        Debug.LogError("CountClonesActives() -> " + count);

        return count;
    }

    protected override void EndSkill()
    {
        if (mobManager.isPlayer)
        {
            if (Target.activeInHierarchy == false)
                ToolTip.Instance.TargetTooltip(Target, prop: false);
            else
                ToolTip.Instance.TargetTooltip(User, prop: false);
        }

        Debug.LogError("EndSkill()");

        EffectManager.Instance.PopUpDamageEffect("Acabou", User);        

        selectTouch = false;

        HexList.Clear();

        base.EndSkill();

        //mobManager.ActivePassive(Passive.EndSkill, Target);

        //ResetCoolDownManager();

        //useSkill = false;

        //mobManager.EndTurn();
    }

    protected override void RegisterOtherHexOnlyFree()
    {
        base.RegisterOtherHexOnlyFree();

        HexList.Remove(moveController.Solo);
    }
    #endregion

    #region SetUp Clones
    /// <summary>
    /// Conta tempo de vida dos Clones
    /// </summary>
    public override void EndTurnAttack()
    {
        base.EndTurnAttack();

        if (_controlClone == false && _prefab.GetComponent<MobSkillAreaDamage>())
        {
            foreach (var area in clones)
            {
                if (area._clone.GetComponent<MobSkillAreaDamage>())
                {
                    if (area._clone.gameObject.activeInHierarchy && area._clone.GetComponent<MobSkillAreaDamage>().User == User)
                    {
                        EffectManager.Instance.PopUpDamageEffect(nome, User, 2);
                        area._clone.GetComponent<MobSkillAreaDamage>().Attack();
                    }
                }
            }
        }

        TimerClones();
    }

    /// <summary>
    /// Quando user Desactive Seus clones
    /// </summary>
    public override void DesactiveTurnAttack()
    {
        KillAllClones();
    }

    /// <summary>
    /// Contador de tempo dos clones
    /// </summary>
    protected virtual void TimerClones()
    {
        /*if (clones.Count > 0 && _maxTimeClone != -1)
        {
            Debug.LogError("TimerClones()");

            for (int index = 0; index < clones.Count; index++)
            {
                Debug.LogError(User.name + " clones[" + index + "]");

                if (clones[index]._clone.activeInHierarchy && clones[index]._clone.GetComponent<MobHealth>().Alive && clones[index]._time >= 1)
                {
                    clones[index]._time--;

                    Debug.LogError("DeadClone(" + index + ") - time " + clones[index]._time);

                    if (clones[index]._time <= 0)
                    {
                        EffectManager.Instance.PopUpDamageEffect("PUFF", clones[index]._clone);

                        clones[index]._clone.GetComponent<MobHealth>().HitKill(clones[index]._clone, false);
                    }
                    else
                        if (User.GetComponent<MobManager>().TimeMob == (RespawMob.Instance.PlayerTime))
                        EffectManager.Instance.PopUpDamageEffect((_maxTimeClone - clones[index]._time) + "/" + _maxTimeClone, clones[index]._clone);

                    if (_effect != null)
                    {
                        _effect.gameObject.SetActive(false);
                        _effect.GetComponentInChildren<ParticleSystem>().Play(false);
                    }
                }
            }
        }   
        */     
    }

    /// <summary>
    /// Quando user Morre clones tbm morrem
    /// </summary>
    protected virtual void KillAllClones()
    {
      /*  if (_desactiveClones)
        {
            Debug.LogError("KillAllClones()");

            for (int index = 0; index < clones.Count; index++)
            {
                if (clones[index] != null && clones[index]._clone.GetComponent<MobHealth>().Alive)
                {
                    EffectManager.Instance.PopUpDamageEffect("PUFF", clones[index]._clone);

                    clones[index]._clone.GetComponent<MobHealth>().HitKill(clones[index]._clone, false);
                }
            }
        }
        */
    }
    #endregion
}
