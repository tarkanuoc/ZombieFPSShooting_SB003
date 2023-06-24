using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private Image imgHealthValue;

    public void UpdateUI(int currentHP, int maxHP)
    { 
        var ratio = (float) currentHP / (float) maxHP;
        imgHealthValue.fillAmount = ratio;
    }
}
