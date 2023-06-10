using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMuzzle : MonoBehaviour
{
    [SerializeField] private Transform muzzleImage;
    public float duration;

    private void Start()
    {
        HideMuzzle();
    }
    public void ShowMuzzle()
    {
        Debug.Log("=========== Show Muzzle");
        muzzleImage.gameObject.SetActive(true);
        float angle = Random.Range(0f, 360f);
        muzzleImage.localEulerAngles = new Vector3(0, 180, angle);
        CancelInvoke();
        Invoke(nameof(HideMuzzle), duration);
    }

    private void HideMuzzle()
    { 
        muzzleImage.gameObject.SetActive(false);
    }
}
