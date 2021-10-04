using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivaZVirusAreaBattle : PassivaZVirusArea
{

    private void Reset()
    {
        _balanceMode = Game_Mode.Battle;
    }

    protected override void Awake()
    {
        _balanceMode = Game_Mode.Battle;
    }
}
