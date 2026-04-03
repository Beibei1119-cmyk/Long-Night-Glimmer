using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


namespace Transition
{

public class DoorTeleport : MonoBehaviour
{
    [Header("目标场景名称")]
    public string targetSceneName;

    [Header("玩家出生位置")]
    public Vector3 spawnPosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.CompareTag("Player"))
            {
                EventHandler.CallTransitionEvent(targetSceneName, spawnPosition);
            }
            else 
            {
                Debug.Log("未检测到player");

            }
    }

}

}