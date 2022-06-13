using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;
using static Riten.Windinator.LayoutBuilder.Material;

public class LoginForm : LayoutBaker
{
    [SerializeField, HideInInspector] Reference<MaterialInputField> m_email;
    [SerializeField, HideInInspector] Reference<MaterialInputField> m_password;

    [SerializeField, HideInInspector] Reference<MaterialButton> m_loginButton;

    public override Element Bake()
    {
        return new Vertical(
            new Element[]
            {
                new Label("Login Form", MaterialLabelStyle.Title),

                new InputField(labelText: "Email", regexExpression: MaterialInputField.Rules.Email, errorText: "Email invalid", icon: MaterialIcons.email)
                    .GetReference(out m_email),

                new InputField(labelText: "Password", regexExpression: MaterialInputField.Rules.StrongPassword,
                    errorText: "Weak password", icon: MaterialIcons.pin, contentType: TMPro.TMP_InputField.ContentType.Password)
                    .GetReference(out m_password),

                new Horizontal(
                    new Element[] {
                        new FlexibleSpace(),
                        new Button("Login", MaterialIcons.login, type: MaterialButtonType.Text)
                            .GetReference(out m_loginButton)
                    }
                )
            },
            spacing: 35f
        );
    }

    private void OnEnable()
    {
        m_loginButton.Value.onClick.AddListener(OnLogin);
    }

    private void OnDisable()
    {
        m_loginButton.Value.onClick.RemoveListener(OnLogin);
    }

    public void OnLogin()
    {
        bool validForm = true;

        validForm = m_email.Value.ValidateField() && validForm;
        validForm = m_password.Value.ValidateField() && validForm;

        if (validForm)
        {
            Windinator.Push<GenericModalDialog>().Setup(title: "Huh?", message: "SOmething, I guess u missed it", action1: "Welp, I'm gone!");
        }
    }
}
