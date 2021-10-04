using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDbuffList
{
    List<DbuffBuff> _DbuffBuff { get; set; }

    void CheckDbuff (GameObject targetDbuff, int i);

    void DbuffActive(GameObject targetDbuff, int index);

    void DbuffFail  (GameObject targetDbuff, int index);

    bool CheckChance(float chance);
}
