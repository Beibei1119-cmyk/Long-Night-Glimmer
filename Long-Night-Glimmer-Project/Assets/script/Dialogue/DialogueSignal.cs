using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

[System.Serializable]
public class DialogueSignal : Marker, INotification
{
    public List<DialogueData> dialogueList;

    public PropertyName id => new PropertyName("DialogueSignal");
}