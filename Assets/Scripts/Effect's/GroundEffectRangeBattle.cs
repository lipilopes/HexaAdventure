using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEffectRangeBattle : GroundEffectRange
{
    private void Reset()
    {
        _mode = Game_Mode.Battle;
    }

    protected override void Awake()
    {
        _mode = Game_Mode.Battle;
    }
}
