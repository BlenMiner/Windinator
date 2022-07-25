using UnityEngine;
using Riten.Windinator;
using Riten.Windinator.LayoutBuilder;

using static Riten.Windinator.LayoutBuilder.Layout;
using UnityEngine.UI;

public class Benchmark02Controls : LayoutBaker
{
    [SerializeField] ComplexShapesBenchmark2 m_subject;

    [SerializeField, HideInInspector] Reference<Slider> m_count;

    [SerializeField, HideInInspector] Reference<Slider> m_step;

    [SerializeField, HideInInspector] Reference<Slider> m_wheight;

    [SerializeField, HideInInspector] Reference<Slider> m_quality;

    [SerializeField, HideInInspector] Reference<Slider> m_blend;

    public override Element Bake()
    {
        return new Rectangle(
            new Vertical(
                children: new Element[] {
                    new MaterialUI.Label("Benchmark 02", style: MaterialSize.Title),

                    new MaterialUI.Separator(false),

                    new MaterialUI.LabeledControl(
                        "Count", new MaterialUI.Slider(5000, 0, 10000, true).GetReference(out m_count).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Step", new MaterialUI.Slider(0.1f, 0.01f, 0.5f).GetReference(out m_step).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Weight", new MaterialUI.Slider(2f, 0f, 10f).GetReference(out m_wheight).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Quality", new MaterialUI.Slider(1f, 0f, 1f).GetReference(out m_quality).Flexible(2f, -1)
                    ),
                    new MaterialUI.LabeledControl(
                        "Blend", new MaterialUI.Slider(0f, 0f, 5f).GetReference(out m_blend).Flexible(2f, -1)
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
        m_subject.m_lineCount = Mathf.RoundToInt(m_count.Value.value);
        m_subject.m_lineStep = m_step.Value.value;
        m_subject.m_lineWeight = m_wheight.Value.value;

        if (m_subject.Canvas.Quality != m_quality.Value.value)
        {
            m_subject.Canvas.Quality = m_quality.Value.value;
            m_subject.Canvas.SetAllDirty();
        }

        m_subject.m_blend = m_blend.Value.value;
    }
}
