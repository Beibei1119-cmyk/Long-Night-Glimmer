using UnityEngine;

public class ClickableItem : MonoBehaviour
{
    private InteractableObject parent;

    private void Start()
    {
        parent = GetComponentInParent<InteractableObject>();
    }

    private void OnMouseDown()
    {
        if (parent == null) return;

        UIManager.Instance.ShowHint(parent.detailHint);
        UIManager.Instance.ShowDetail(parent.clickableItemDetailImage, parent.clickableItemDetailDescription);
    }
}