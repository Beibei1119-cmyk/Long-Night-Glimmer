using UnityEngine;
using UnityEngine.Playables;

public class DialogueSignalReceiver : MonoBehaviour, INotificationReceiver
{
    public PlayableDirector director;

    public void OnNotify(Playable origin, INotification notification, object context)
    {
        var dialogueSignal = notification as DialogueSignal;
        if (dialogueSignal != null && dialogueSignal.dialogueList != null)
        {
            // ÔÝÍ£ Timeline
            if (director != null)
                director.playableGraph.GetRootPlayable(0).SetSpeed(0);

            DialogueManager.Instance.StartDialogue(dialogueSignal.dialogueList);
        }
    }
}