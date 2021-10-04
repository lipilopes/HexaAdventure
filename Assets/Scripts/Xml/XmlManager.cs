using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;


[System.Serializable]
public class CodesHistory
{
    public string _key;
    public string _changer;      
}

[System.Serializable]
public class StringXml
{
    public string _nameX;
}

[System.Serializable]
public class StringDatabase
{
    [XmlArray("String")]//Nome da tag no XML
    public List<StringXml> list = new List<StringXml>();
}

public enum TypeArq
{
 xml,json,yaml
}

public class XmlManager : MonoBehaviour
{    
    [SerializeField]
    protected string _nameXml = "HexaAdventureHistory";
    [SerializeField]
    protected TypeArq _typeArq;
    [Space, SerializeField]
    protected CodesHistory[] _codeWords; 

    /// <summary>
    /// Tempo de espera para esperar o Game Manager
    /// </summary>
    WaitForSeconds wait = new WaitForSeconds(1);

    protected virtual IEnumerator Start()
    {       
        while (GameManagerScenes._gms == null &&
               GameManagerScenes._gms.LoadComplete == false)
        {
            yield return wait;
        }

        //if (GameManagerScenes._gms.IsMobile)
        //    Destroy(this);
    }

    protected string CheckAndChangeWords(string list)
    {
        string _S = list;

        if (_S.Length > 0 && _S != null)
        //foreach (var i in list)
        {           
            foreach (var c in _codeWords)
            {
                if (c != null && c._key != "")
                    if (_S.Contains(c._key))
                {
                    string _old = _S;

                    _S = _old.Replace(c._key, c._changer);
                }
            }
        }

        return _S;
    }


    public void CreateXML(string name)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(HistoryDatabase));

        string nameStream = "";

#if UNITY_ANDROID && !UNITY_EDITOR
          nameStream ="jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/PT_BR/" + _nameXml + ".xml";  
        //nameStream = (Application.streamingAssetsPath + "/StreamingAssets/Assets/Resources/Xml/PT_BR/"+name+".xml");        
#else
        //#if UNITY_EDITOR
        nameStream = (Application.streamingAssetsPath + "/Xml/PT_BR/" + name + ".xml");            
#endif

        #region Create XML
#if UNITY_EDITOR
        if (!File.Exists(nameStream))
        {
            FileStream stream = new FileStream(nameStream, FileMode.Create);

            History _new = new History();
            _new._fase = 0;
            _new._mobID = 0;
            _new._left = true;
            _new._startFase = true;
            _new._mobName = "Test";
            _new._chat = "1...2...3... Test";
            /*_new._ID        = -1;
            _new._nameX      = "new create";
            _new._description = "descricao";            
            _new._type      = "gold";
            _new._dlc       = "dlc";*/

            serializer.Serialize(stream, null);

            stream.Close();

            Debug.LogError(name + ".xml Criada [ PT_BR ]");
        }
#endif
        #endregion
    }
}
