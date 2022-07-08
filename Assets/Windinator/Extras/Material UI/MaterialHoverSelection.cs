using Riten.Windinator.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

public class MaterialHoverSelection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectangleGraphic m_hoverGraphic;

    [SerializeField] float m_animationSpeed = 10f;

    [SerializeField] AnimationCurve m_animationCurve;

    VarAnimator<float> m_selected;

    void Awake()
    {
        m_selected = new VarAnimator<float>(
            Mathf.Lerp,
            v => {
                if (m_hoverGraphic != null) {
                    m_hoverGraphic.rectTransform.localScale = Vector3.one * v * 2f;
                }
            },
            m_animationSpeed,
            m_animationCurve
        );

        m_selected.SnapToTarget(0f);
    }

    void Update()
    {
        m_selected.Update(Time.deltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_selected.AnimateToTarget(1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_selected.AnimateToTarget(0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_selected.AnimateToTarget(1f);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_selected.AnimateToTarget(0f);
    }
}
