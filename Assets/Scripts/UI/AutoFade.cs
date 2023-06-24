using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AutoFade : MonoBehaviour
{
   // [SerializeField] private Image image;
   [SerializeField] private CanvasGroup canvasGroup;

    public float visibleDuration;
    public float fadingDuration;

    private float startTime;

    public void Show()
    { 
        startTime = Time.time;
        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }
        
    private void Update()
    {
        float elapsedTime = Time.time - startTime;
        if (elapsedTime < visibleDuration) 
        {
            return;
        }

        elapsedTime -= visibleDuration;
        if (elapsedTime < fadingDuration)
        {
            //elapsedTime : chạy từ 0 đến fadingDuration
            //1f - elapsedTime / fadingDuration sẽ chạy từ 1f -> 0
            canvasGroup.alpha = (1f - elapsedTime / fadingDuration);
        }
        else
        { 
            Hide();
        }
    }

    public void Hide()
    { 
        gameObject.SetActive(false);
    }

}
