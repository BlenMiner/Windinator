using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;

public class GoogleTemplate : LayoutBaker
{
    [SerializeField] Sprite m_logo;

    [SerializeField] Reference<MaterialInputField> m_field;

    [SerializeField] Reference<MaterialButton> m_searchButton;

    WindinatorBehaviour RootWindow;

    public override Element Bake()
    {
        return new Vertical(
            new Element[]
            {
                new Graphic(sprite: m_logo, color: new Swatch(Colors.OnBackground)),
                new MaterialUI.Label("Guuglio", style: MaterialSize.Headline, fontStyle: TMPro.FontStyles.Bold, color: Colors.OnBackground),

                new Spacer(25f),
 
                new MaterialUI.InputField(labelText: "Search", style: MaterialTextFieldType.Filled)
                    .GetReference(out m_field),

                new Spacer(25f),

                new Horizontal(
                    children: new Element[]
                    {
                        new MaterialUI.Button("Search", type: MaterialButtonStylePresets.Text)
                            .GetReference(out m_searchButton),
                        new MaterialUI.Button("Feeling Lucky", type: MaterialButtonStylePresets.Text)
                    },
                    alignment: TextAnchor.MiddleCenter,
                    spacing: 20f
                )
            },
            alignment: TextAnchor.MiddleCenter
        );
    }
 
    private void Awake()
    {
        RootWindow = GetComponentInParent<WindinatorBehaviour>();
    }

    private void Start()
    {
        m_searchButton.Value.onClick.RemoveAllListeners();
        m_searchButton.Value.onClick.AddListener(OnSearch);
    }

    private void OnSearch()
    {
        string searchStr = m_field.Value.Text;

        if (string.IsNullOrEmpty(searchStr))
        {
            var snackbar = Windinator.Push<SnackBar>();
            snackbar.Setup("Search parameter is empty.");
        }
        else if (RootWindow != null)
        {
            var resultPage = Windinator.Push<GoogleResults>();
            resultPage.Setup(searchStr);
        }
        else
        {
            Debug.LogError("Missing Root Window");
        }
    }
}
