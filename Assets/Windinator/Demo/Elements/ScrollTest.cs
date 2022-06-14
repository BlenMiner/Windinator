using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using System.Collections.ObjectModel;

using static Riten.Windinator.LayoutBuilder.Layout;
using static Riten.Windinator.LayoutBuilder.MaterialUI;

public class ScrollTest : LayoutBaker
{
    ObservableCollection<int> m_data = new ObservableCollection<int>();

    public Reference<ScrollViewDynamicRuntime> Scrollview;

    public override Element Bake()
    {
        return new Vertical(
            new Element[] { 
                new Rectangle(
                    child: new ScrollViewDynamic().GetReference(out Scrollview),
                    new Vector2(500, 400),
                    padding: new Vector4(30, 30, 30, 30),
                    new ShapeProperties {
                        Color = new Color(0.9f, 0.9f, 0.9f),
                        Roundness = new Vector4(20, 20, 20, 20)
                    }
                ),
                new Stack(
                    children: new Element[] {
                        new Rectangle(size: new Vector2(150, 50), shape: new ShapeProperties {Color = Color.black}),
                        new Button("Hello")
                    }
                ),
                new Button("Huh")
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

    private void UpdateCell(int index, SimpleButton instance, int data)
    {
        instance.ButtonReference.Value.SetText($"I'm the button {data}");
    }
}
