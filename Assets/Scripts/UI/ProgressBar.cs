using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private RectTransform rtfmLoadingbar;
    [SerializeField] private RectTransform rtfmMask;
    [SerializeField] private RectTransform rtfmFill;

    private float maxWidth;
    private float maxHeight;

    private void Awake()
    {
        maxWidth = rtfmMask.rect.width;
        maxHeight = rtfmMask.rect.height;
    }

    public void SetProgressValue(float progress)
    { 
        float currentWidth = Mathf.Clamp01(progress) * maxWidth;
        rtfmMask.sizeDelta = new Vector2(currentWidth, maxHeight);
    }
}
