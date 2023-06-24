using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private AutoFade leftScratch;
    [SerializeField] private AutoFade rightScratch;

    public void ShowLeftScratch()
    { 
        leftScratch.Show();
    }

    public void ShowRightScratch()
    {
        rightScratch.Show();
    }
}
