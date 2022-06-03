using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using System.Collections.ObjectModel;

using static Riten.Windinator.LayoutBuilder.Layout;
using static Riten.Windinator.LayoutBuilder.LayoutMaterial;

public class ScrollTest : LayoutBaker
{
    ObservableCollection<int> m_data = new ObservableCollection<int>();

    public Reference<ScrollViewDynamicRuntime> Scrollview;

    public override Element Bake()
    {
        return new Vertical(
            new Element[] {
                new Container(
                    new ScrollViewDynamic().GetReference(out Scrollview),
                    new Vector2(300, 200)
                )
            },
            alignment: TextAnchor.MiddleCenter
        );
    }

    private void Awake()
    {
        for (int i = 0; i < 70; ++i)
            m_data.Add(i);

        Scrollview.Value.Setup<SimpleButton, int>(m_data, 40f, UpdateCell);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_data.Add(69);
            Scrollview.Value.SetDirty();
        }
    }

    private void UpdateCell(SimpleButton instance, int data)
    {
        instance.ButtonReference.Value.SetText($"I'm the button {data}");
    }
}
