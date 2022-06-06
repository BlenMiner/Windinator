using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.MaterialLayout;
using static Riten.Windinator.LayoutBuilder.Layout;

public class ButtonsScreen : LayoutBaker
{
    [SerializeField] Reference<MaterialButton> m_firstButton;

    public override Element Bake()
    {
        return new Vertical(
            children: new Element[] {
                new Horizontal(
                    children: new Element[]
                    {
                        new Vertical(
                            children: new Element[] {
                                new Button("Something that will get changed", type: MaterialButtonType.Elevated, icon: MaterialIcons.plus).GetReference(out m_firstButton),
                                new ActionButton(icon: MaterialIcons.numeric_1),
                            },
                            spacing: 5,
                            alignment: TextAnchor.MiddleCenter
                        ),
                        new Vertical(
                            children: new Element[] {
                                new Button("Filled", type: MaterialButtonType.Filled),
                                new ActionButton(icon: MaterialIcons.numeric_2),
                            },
                            spacing: 5,
                            alignment: TextAnchor.MiddleCenter
                        ),
                        new Vertical(
                            children: new Element[] {
                                new Button("Tonal", type: MaterialButtonType.Tonal, icon: MaterialIcons.reddit),
                                new ActionButton(icon: MaterialIcons.numeric_3),
                            },
                            spacing: 5,
                            alignment: TextAnchor.MiddleCenter
                        ),
                        new Vertical(
                            children: new Element[] {
                                new Button("Outlined", type: MaterialButtonType.Outlined),
                                new ActionButton(icon: MaterialIcons.numeric_4),
                            },
                            spacing: 5,
                            alignment: TextAnchor.MiddleCenter
                        ),
                        new Vertical(
                            children: new Element[] {
                                new Button("Text", type: MaterialButtonType.Text),
                                new ActionButton(icon: MaterialIcons.numeric_5),
                            },
                            spacing: 5,
                            alignment: TextAnchor.MiddleCenter
                        ),
                    },
                    alignment: TextAnchor.MiddleCenter,
                    spacing: 10f
                ),
                new Vertical(
                    children: new Element[] {
                        new SegmentedButton(
                            new string[]
                            {
                                "Good", "Bad", "Ok"
                            }
                        ),
                        new SegmentedButton(
                            new string[]
                            {
                                "Hour", "Minute", "Seconds", "Milliseconds", "Nanoseconds"
                            },
                            startSelectedIndex: 2
                        ),
                        new ActionButton(icon: MaterialIcons.numeric_6),
                    },
                    spacing: 5,
                    alignment: TextAnchor.MiddleCenter
                ),
                new Vertical(
                    children: new Element[] {
                        new InputField(
                            labelText: "Email",
                            errorText: "Invalid Email Format",
                            regexExpression: MaterialInputField.Rules.Email,
                            icon: MaterialIcons.email,
                            contentType: TMPro.TMP_InputField.ContentType.EmailAddress
                        ),
                        new ActionButton(icon: MaterialIcons.numeric_7),
                    },
                    spacing: 35,
                    alignment: TextAnchor.MiddleCenter
                ) 
            },
            alignment: TextAnchor.MiddleCenter,
            spacing: 50f
        );
    }

    void Start()
    {
        var btn = m_firstButton.Value;
        btn.SetText("Elevated");

        btn.onClick.AddListener(() =>
        {
            Windinator.Push<SnackBar>().Setup("Snackbar example with action", action: "SOME ACTION");
        });
    }
}
