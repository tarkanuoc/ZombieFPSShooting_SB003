using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    [SerializeField] private GunAmmo[] guns;

    private void Update()
    {
        if (!GameManager.Instance.IsGameReady)
        {
            return;
        }

        for (int i = 0; i < guns.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)
                || Input.GetKeyDown(KeyCode.Keypad1 + i))
            {
                SetActiveGun(i);
            }
        }
    }

    public void SetActiveGun(int gunIndex)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            bool isAcvtive = (i == gunIndex);
            guns[i].gameObject.SetActive(isAcvtive);

            if (isAcvtive)
            {
                guns[i].OnGunSelected();
            }
        }
    }
}
