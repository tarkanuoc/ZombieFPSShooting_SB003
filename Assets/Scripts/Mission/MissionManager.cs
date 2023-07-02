using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public int requiredKill;
    [SerializeField] private TextMeshProUGUI textMission;
    [SerializeField] private TextMeshProUGUI textCount;
    [SerializeField] private Transform exitDoor;


    private int currentKill;

    private void Start()
    {
        exitDoor.gameObject.SetActive(false);
        StartCoroutine(VerifyMissions());
    }

    IEnumerator VerifyMissions() 
    {
        yield return VerifyZombieKill();
        yield return VerifyPlayerExit();
        yield return new WaitForSeconds(3);
        GameHelper.Instance.GamePlayUI.OnMissionCompleted();
    }

    IEnumerator VerifyZombieKill() 
    {
        currentKill = 0;
        textMission.text = $"Kill {requiredKill} zombies";
        yield return new WaitUntil(() => currentKill >= requiredKill);
    }

    IEnumerator VerifyPlayerExit() 
    {
        exitDoor.gameObject.SetActive(true);
        textMission.text = $"Find exit door";
        yield return new WaitUntil(IsPlayerExit);
    }

    bool IsPlayerExit()
    {
        float distance = Vector3.Distance(GameHelper.Instance.Player.PlayerFoot.position, exitDoor.position);
        Debug.Log("========= distance = " + distance);
        return distance < 1f;
    }

    public void OnZombieKilled()
    {
        currentKill++;
        textCount.text = $"Zombies killed : {currentKill}";

    }
}
