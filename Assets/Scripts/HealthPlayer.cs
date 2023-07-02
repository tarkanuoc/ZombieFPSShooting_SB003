using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPlayer : Health
{
    public override void Die()
    {
        base.Die();
        GameHelper.Instance.GamePlayUI.OnGameOver();
    }
}
