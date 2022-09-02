using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.Material;

public class GenericModalDialog : WindinatorBehaviour
{
    [SerializeField] TMPro.TMP_Text m_title;

    [SerializeField] TMPro.TMP_Text m_message;

    [SerializeField] MaterialButton m_button1, m_button2;

    [Header("Sections")]

    [SerializeField] GameObject m_headerHolder;
    [SerializeField] GameObject m_action1GO, m_action2GO, m_bothButtons;

    System.Action m_okcb, m_cancelcb;

    public void Setup(
        string title = null, string message = null,
        string action1 = null, string action2 = null, bool requireInput = false,
        System.Action action1evt = null, System.Action action2evt = null
    )
    {
        m_okcb = action1evt;
        m_cancelcb = action2evt;

        m_title.text = title;
        m_message.text = message;

        m_button1.SetText(action1);
        m_button2.SetText(action2);

        m_action1GO.SetActive(action1 != null);
        m_action2GO.SetActive(action2 != null);

        m_bothButtons.SetActive(action1 != null || action2 != null);

        m_headerHolder.SetActive(title != null);
        SetCanExit(!requireInput);
    }

    public void Ok()
    {
        m_okcb?.Invoke();
        ForcePopWindow();
    }

    public void Cancel()
    {
        m_cancelcb?.Invoke();
        ForcePopWindow();
    }
}
