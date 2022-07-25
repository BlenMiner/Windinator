using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;
using Riten.Windinator.Material;

using static Riten.Windinator.LayoutBuilder.Layout;
using UnityEngine.UI;

public class Benchmark01Controls : LayoutBaker
{
    [SerializeField] ComplexShapesBenchmark1 m_subject;

    [SerializeField, HideInInspector] Reference<Slider> m_count;

    [SerializeField, HideInInspector] Reference<Slider> m_quality;

    [SerializeField, HideInInspector] Reference<Slider> m_blend;

    [SerializeField, HideInInspector] Reference<Slider> m_speed;

    public override Element Bake()
    {
        return new Rectangle(
            new Vertical(
                children: new Element[] {
                    new MaterialUI.Label("Benchmark 01", style: MaterialSize.Title),

                    new MaterialUI.Separator(false),

                    new MaterialUI.LabeledControl(
                        "Count", new MaterialUI.Slider(100, 0, 771, true).GetReference(out m_count).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Quality", new MaterialUI.Slider(1f, 0f, 1f).GetReference(out m_quality).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Blend", new MaterialUI.Slider(1f, 0f, 5f).GetReference(out m_blend).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Speed", new MaterialUI.Slider(1f, 0f, 10f).GetReference(out m_speed).Flexible(2f, -1)
                    ),
                },
                spacing: 15f
            ).Flexible(), 
            padding: Vector4.one * 20f,
            shape: new ShapeProperties{
                Roundness = Vector4.one * 20f,
                Shadow = new ShadowProperties{
                    Blur = 50f,
                    Size = 20f
                },
                Color = new Swatch(Colors.Background)
            }
        ).Flexible(1, 1);
    }

    void Update()
    {
        m_subject.BallCount = Mathf.RoundToInt(m_count.Value.value);

        if (m_subject.Canvas.Quality != m_quality.Value.value)
        {
            m_subject.Canvas.Quality = m_quality.Value.value;
            m_subject.Canvas.SetAllDirty();
        }

        m_subject.Blend = m_blend.Value.value;
        m_subject.Speed = m_speed.Value.value;
    }
}
