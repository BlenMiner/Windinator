using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;
using System.Collections.Generic;

public class GoogleResultsContent : LayoutBaker
{
    [SerializeField] Reference<MaterialLabel> m_title;
    [SerializeField] Reference<ScrollViewDynamicRuntime> m_scrollview;

    List<string> m_data = new List<string>();

    private void Awake()
    {
        m_data.Add("Hello World");
        m_data.Add("Wazzap");
        m_data.Add("Some stuff");
    }

    public override Element Bake()
    {
        return new Vertical(
            new Element[]
            {
                new MaterialUI.Label(
                    "Sample Text",
                    color: Colors.OnBackground,
                    style: MaterialSize.Title
                ).GetReference(out m_title),

                new Rectangle(
                    new ScrollViewDynamic().GetReference(out m_scrollview).Flexible(),
                    padding: new Vector4(30f, 30f, 30f, 30f),
                    shape: new ShapeProperties
                    {
                        Color = Colors.PrimaryContainer,
                        Roundness = new Vector4(20f, 20f, 20f, 20f)
                    } 

                ).Flexible()
            },
            alignment: TextAnchor.MiddleCenter,
            padding: new Vector4(20, 20, 20, 20),
            spacing: 20f
        ).Flexible();
    }

    public void Setup(string searchStr)
    {
        for (int i = 0; i < 100; ++i)
            m_data.Add(searchStr + " " + i);

        m_title.Value.LabelText = searchStr;
        m_scrollview.Value.Setup<GoogleResultEntry, string>(m_data, 50f, UpdateCell);
    }

    public void UpdateCell(int index, GoogleResultEntry element, string data)
    {
        var label = element.Label.Value;

        label.LabelText = data;
        label.ForceUpdate();

        var delete = element.DeleteButton.Value;

        delete.onClick.RemoveAllListeners();
        delete.onClick.AddListener(() => {
            m_data.RemoveAt(index);
            m_scrollview.Value.SetDirty();
        });
    }
}
