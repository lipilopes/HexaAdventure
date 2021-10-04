using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalTime : MonoBehaviour
{
    public static TotalTime Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    [SerializeField]  Text txtTime;

   bool? gameOver = false;
   float seg, min, hour,startTime;

   float totalSeg, totalMin,totalStartTime;

   GameManagerScenes _gms;

   float Cooldown;

   float recordSeg,recordMin;

    string txtRecord = null;

    string _tempoDeJogo,_tempoRecord,_semRecord;

    private void Start()
    {
        //    StartTime();       
    }

    private void Load()
    {
        _gms = GameManagerScenes._gms;

        if (_gms != null)
        {

            if (!GameManagerScenes.BattleMode)
            {
                recordSeg = _gms.FaseSeg(-1, 0, false);

                recordMin = _gms.FaseMin(-1, 0, false);
            }
            
            _tempoDeJogo = /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(17) /*: "Tempo de jogo:"*/;

            _tempoRecord = /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(7) /*: "Tempo Record"*/;

            _semRecord =/* !_gms.IsMobile ? */XmlMenuInicial.Instance.Get(40)/* : "Sem Recorde"*/;            

            txtTime.text = _tempoDeJogo + "\n<color=green>Aguarde os turnos serem iniciados.</color>";

        }
        else
        {
            recordSeg = -1;

            recordMin = -1;
        }

        Debug.LogError("Rec_Seg: "+recordSeg+" Rec_Min: "+recordMin);


        if (recordSeg == -1)
            recordSeg = 999;

        if (recordMin == -1)
            recordMin = 999;
    }    

    public void StartTime()
    {
        Load();

        Cooldown       = Time.time + 61;//Salve Time Game

        startTime      = Time.time;

        totalStartTime = Time.time;

        if (/*recordSeg > 0 &&  */recordSeg != 999 /*&& recordMin > 0 */&& recordSeg != 999)
        {
            string seg, min;

            if (recordSeg <= 9)
                seg = "0" + recordSeg;
            else
                seg = recordSeg.ToString("F0");

            if (recordMin != 999)
            {
                if (recordMin <= 9)
                    min = "0" + recordMin;
                else
                    min = recordMin.ToString("F0");
            }
            else
                min = "0";


            txtRecord = "\n\n<b>"+_tempoRecord+"\n<color=green>" + min + ":" + seg + "</color></b>";
        }
        else
            txtRecord = "\n\n<b>"+_tempoRecord+"\n<color=red>"+ _semRecord + "</color></b>";

        if (GameManagerScenes.BattleMode)
            txtRecord = "";

            gameOver = null;       
    }

    public void GameOver(bool? lose=null)
    {
        gameOver = lose;

        SalveTotalTime();

        if (lose == false)
            RecordTime();
    }

    /// <summary>
    /// Para de contar o tempo (usado para passaro modo historia)
    /// </summary>
    public void PauseTimer()
    {
        gameOver = true;
    }

    private void Update()
    {
        if (gameOver == null)      
            CalculeTime();          
    }

    void CalculeTime()
    {       
        float time      = Time.time -  startTime;
        float totalTime = Time.time - totalStartTime;
        string cor      = "green";

        seg = (int)time%60;
        min =      time/60;

        if (min >= 60)
            hour++;
      
        if (Time.time > Cooldown)
            SalveTotalTime();

        totalSeg = (int)totalTime % 60;
        totalMin = (int)totalTime / 60;

        if (totalMin >= 60)
            totalMin=0;

        string _seg, _min;

        if (totalSeg >= 60)
        {
            totalSeg = 0;
            totalMin++;
        }

        if (totalSeg <= 9)
            _seg = "0" + totalSeg;
        else
            _seg = totalSeg.ToString("F0");

        if (totalMin <= 9)
            _min = "0" + totalMin;
        else
            _min = totalMin.ToString("F0");

        if (recordSeg >= totalSeg && recordMin >= totalMin || recordMin > totalMin)
        {
            cor = "green";
        }
        else
            cor = "red";

            txtTime.text = _tempoDeJogo+"\n<color=" +cor+">" + _min + ":" + _seg +"</color>"+ txtRecord;
    }

    public void SalveTotalTime()
    {
        if (GameManagerScenes.BattleMode)
            return;

            if (_gms==null)
            Load();

        startTime = Time.time;

        if (_gms!=null)
        {
            _gms.TotalTimeSeg     = ((int)seg);
            _gms.TotalTimeMinutes = ((int)min);
            _gms.TotalTimeHour    = ((int)hour);
        }

        seg  = 0;
        min  = 0;
        hour = 0;

        Cooldown = Time.time + 61;
    }

    public void RecordTime()
    {
        if (GameManagerScenes.BattleMode)
            return;

        if (recordSeg > totalSeg && recordMin > totalMin)
        {
            string _seg, _min;

            if (totalSeg <= 9)
                _seg = "0" + totalSeg;
            else
                _seg = totalSeg.ToString("F0");

            if (totalMin <= 9)
                _min = "0" + totalMin;
            else
                _min = totalMin.ToString("F0");

            GetComponent<ButtonManager>().painelGameOver.GetComponentInChildren<Text>().text += "\nPARABENS!!!\nNovo Record!!!\n" + _min + " : " + _seg;
            _gms.NewInfo("PARABENS!\nNovo Record!!!\n" + _min + ":" + _seg, 5);
            _gms.TimeRecord(-1, totalSeg, totalMin);         
        }
    }
}
