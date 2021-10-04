using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MobManager))]
public class PlayerControl : MonoBehaviour
{
    public void PlayerControlThis()
    {       
        if (ButtonManager.Instance.player != gameObject)
        {
            Debug.LogError("PlayerControlThis() -> " + gameObject);
            ButtonManager.Instance.PlayerInf(gameObject);
        }   
    }
}
