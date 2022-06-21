using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;
using UnityEngine;
using UnityEngine.Events;
using static Riten.Windinator.LayoutBuilder.Layout;

public class MaterialButtonSegment : MonoBehaviour
{
    [SerializeField, HideInInspector] MaterialButton[] m_buttons;
    [SerializeField, HideInInspector] MaterialCircleClick[] m_circles;

    [SerializeField, HideInInspector] int m_selected;

    public UnityEvent<int> onSelectionChanged = new UnityEvent<int>();

    bool m_selectionInsideRange => (m_selected >= 0 && m_selected < m_buttons.Length);

    public int SelectedItemID
    {
        get { return m_selected; }
        set {
            Select(value);
        }
    }

    public MaterialButton SelectedButton => m_selectionInsideRange ? m_buttons[m_selected] : null;

    public string SelectedText => m_selectionInsideRange ? m_buttons[m_selected].GetText() : null;

    private void Start()
    {
        for (int i = 0; i < m_buttons.Length; i++)
        {
            var btn = m_buttons[i];
            int index = i;
            btn.onClick.AddListener(() =>
            {
                OnButtonClicked(index);
            });
        }
    }

    internal void Initialize(int selectedIndex = -1, Reference<MaterialButton>[] buttons = null)
    {
        if (buttons == null) return;

        m_selected = selectedIndex;
        m_buttons = new MaterialButton[buttons.Length];
        m_circles = new MaterialCircleClick[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i].Value;

            m_buttons[i] = btn;
            m_circles[i] = btn.GetComponent<MaterialCircleClick>();
        }

        if (m_selectionInsideRange)
        {
            m_circles[m_selected].ForceClick(true);
            m_circles[m_selected].SnapAnimation();
        }
    }

    public void OnButtonClicked(int index)
    {
        Select(index);
    }
    
    public void Select(int index)
    {
        if (m_selectionInsideRange)
            m_circles[m_selected].ForceClick(false);

        if (m_selected != index)
            onSelectionChanged?.Invoke(index);
        m_selected = index;

        if (m_selectionInsideRange)
            m_circles[m_selected].ForceClick(true);
    }
}
