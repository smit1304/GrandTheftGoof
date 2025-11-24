using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSfx : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public bool playHover = true;
    public bool playClick = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (playHover && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIHover();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (playClick && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIClick();
        }
    }
}
