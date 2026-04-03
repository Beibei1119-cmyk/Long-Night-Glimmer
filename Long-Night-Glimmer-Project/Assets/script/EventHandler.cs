using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{



    //场景切换：
    public static event Action<string, Vector3> TransitionEvent;
    public static void CallTransitionEvent(string sceneName, Vector3 pos)
    {
        TransitionEvent?.Invoke(sceneName, pos);
        Debug.Log("TransitionEvent执行");
    }


    //场景卸载之前要执行Event事件（卸载之前告诉场景要保存数据）：        
    public static event Action BeforeSceneUnloadEvent;
    public static void CallBeforeSceneUnloadEvent()
    {
        BeforeSceneUnloadEvent?.Invoke();
        Debug.Log("场景加载后执行");
    }


    //场景卸载之后要执行Event事件（加载场景后）：
    public static event Action AfterSceneLoadEvent;
    public static void CallAfterSceneLoadEvent()//场景加载之后要执行Event事件
    {
        AfterSceneLoadEvent?.Invoke();
        Debug.Log("场景加载后执行");
    }

    //人物移动
    public static event Action<Vector3> MoveToPosition;
    public static void CallMoveToPosition(Vector3 targetPosition)
    {
        MoveToPosition?.Invoke(targetPosition);
    }

}
