using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] CSSCursors m_onHover = CSSCursors.pointer;

    public void OnPointerEnter(PointerEventData _)
    {
        Cursorinator.SetCursor(m_onHover);
    }

    public void OnPointerExit(PointerEventData _)
    {
        Cursorinator.SetCursor(CSSCursors.@default);
    }
}
