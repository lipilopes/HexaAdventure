using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class EffectGhost : MonoBehaviour
{
    protected GameObject _ghostPrefab;


    protected GameObject target;
    protected Mesh       _mesh;
    protected Material[] _mat;


    [SerializeField, Tooltip("Tempo que efeito dura, 0 para infinito")]
    protected float _timer            = 5;
    [SerializeField,Tooltip("Tempo que a progreção fica ativa")]
    protected float _timerGhost       = 1;
    [SerializeField, Tooltip("Tempo para criar nova progreção")]
    protected float _timeToActive     = 0.5f;

    [SerializeField, Tooltip("Maximo de ghosts que podem aparecer")]
    protected int _maxGhosts = 5;

    protected int _maxGhostsCurrent = 0;

    public bool StartAwake;

    //public bool AttTimer;

    private void OnEnable()
    {
        _maxGhostsCurrent = 0;

        if (StartAwake && GetComponent<MeshFilter>() && GetComponent<MeshRenderer>())
        {
            StartGhostEffect(gameObject);

           // StartGhostEffect(gameObject, GetComponent<MeshFilter>().mesh, GetComponent<MeshRenderer>().materials);
        }
    }   

   /* private void Update()
    {
        if (AttTimer)
        {
            Start();

            AttTimer = false;
        }
    }
    */

    //List<GameObject> ghostList = new List<GameObject>();

    WaitForSeconds Timer;

    WaitForSeconds TimeToActive;

    bool effectActive=false;

    protected virtual void Start()
    {
        _ghostPrefab = (GameObject)Resources.Load("Ghost_Prefab", typeof(GameObject));

        Timer       = new WaitForSeconds(_timer);

        TimeToActive = new WaitForSeconds(_timeToActive);

        if (StartAwake && GetComponent<MeshFilter>() && GetComponent<MeshRenderer>())
        {
            StartGhostEffect(gameObject);

            // StartGhostEffect(gameObject, GetComponent<MeshFilter>().mesh, GetComponent<MeshRenderer>().materials);
        }
    }

    public void StartGhostEffect(GameObject Target)
    {
        target = Target;

        _mesh = Target.GetComponent<MeshFilter>().mesh;

        _mat = Target.GetComponent<MeshRenderer>().materials;

        foreach (var m in _mat)
        {
            m.color = new Color(m.color.r, m.color.g, m.color.b, 0.5f);
        }

        effectActive = true;

        StartCoroutine(TimerEffectDurationCoroutine());
    }

    public void StartGhostEffect(GameObject Target,Mesh mesh, Material[] material)
    {
        target = Target;

        mesh = _mesh;

        _mat = material;

        foreach (var m in _mat)
        {
            m.color = new Color(m.color.r, m.color.g, m.color.b, 0.5f);
        }

        effectActive = true;

        StartCoroutine(TimerEffectDurationCoroutine());
    }

    protected virtual IEnumerator TimerEffectDurationCoroutine()
    {       
        StartCoroutine(TimeCreateGhostEffectCoroutine());

        if (_timer > 0)
        {
            while (effectActive)
            {
                yield return Timer;

                effectActive = false;
            }
        }
    }

    protected virtual IEnumerator TimeCreateGhostEffectCoroutine()
    {
        while (effectActive)
        {
            if (_maxGhostsCurrent < _maxGhosts || _maxGhosts <= 0)
                CreateEffect();

            yield return TimeToActive;

        }
    }

    protected virtual void CreateEffect()
    {
        if (_ghostPrefab == null)
        {
            Start();
        }

        #region otimizado
        //foreach (GameObject g in ghostList)
        //{
        //    if (!g.activeSelf)
        //    {
        //        g.GetComponent<MeshRenderer>().materials = _mat;

        //        g.GetComponent<MeshFilter>().mesh = _mesh;

        //        g.transform.position = target.transform.position;

        //        g.transform.rotation = target.transform.rotation;

        //        g.transform.localScale = target.transform.localScale;

        //        g.SetActive(true);
        //    }
        //}

        //ghostList.Add(Instantiate(gameObject, target.transform.position, target.transform.rotation));

        //int index = ghostList.Count - 1;

        //ghostList[index].name = "Ghost of " + target.name;

        //ghostList[index].GetComponent<MeshRenderer>().materials = _mat;

        //ghostList[index].GetComponent<MeshFilter>().mesh       = _mesh;

        //ghostList[index].transform.localScale                  = target.transform.localScale;

        //ghostList[index].SetActive(true);
        #endregion

        _maxGhostsCurrent++;

        GameObject Ghost = Instantiate(_ghostPrefab, target.transform.position, target.transform.rotation);

        Ghost.name = "Ghost of " + target.name;

        Ghost.GetComponent<MeshRenderer>().materials = _mat;

        Ghost.GetComponent<MeshFilter>().mesh        = _mesh;

        Ghost.transform.localScale                   = target.transform.localScale;

        Ghost.SetActive(true);

        Destroy(Ghost,_timerGhost);

        Invoke("DesactiveGhost", _timerGhost);
    }

    protected virtual void DesactiveGhost()
    {
        _maxGhostsCurrent--;
    }
}
