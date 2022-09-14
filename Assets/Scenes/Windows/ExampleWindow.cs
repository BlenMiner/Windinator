using UnityEngine;
using Riten.Windinator;
using TMPro;

public class ExampleWindow : WindinatorBehaviour
{
    [SerializeField] TMP_Text m_label;

    public void Setup(string username)
    {
        m_label.text = "Hello " + username + " !";
    }

    public void CloseWindow()
    {
        PopWindow();
    }
}
