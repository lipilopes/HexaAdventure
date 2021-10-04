using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyAdm : MonoBehaviour
{

    private void Awake()
    {
        GameManagerScenes gms = GameManagerScenes._gms;

        if (gms==null || !gms.Adm)
            Destroy(gameObject);
    }
}
