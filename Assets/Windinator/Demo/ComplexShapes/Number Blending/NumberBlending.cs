using Riten.Windinator.Shapes;
using UnityEngine;

public class NumberBlending : CanvasDrawer
{
    [SerializeField] float m_thickness = 15f;

    [SerializeField] float m_animPower = 10f;

    [SerializeField] AnimationCurve m_animation = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    Vector2[][] numbers = new Vector2[][]
    {
        // 0
        new Vector2[] {
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        },
        // 1
        new Vector2[] {
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
        },
        // 2
        new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
        },
        // 3
        new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        },
        // 4
        new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
        },
        // 5
        new Vector2[] {
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        },
        // 6
        new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(0f, 0f),
            new Vector2(1f, 0f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0.5f),
        },
        // 7
        new Vector2[] {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(0.5f, 0f),
        },
        // 8
        new Vector2[] {
            new Vector2(0f, 0f),
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0.5f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        },
        // 9
        new Vector2[] {
            new Vector2(1f, 0.5f),
            new Vector2(1f, 1f),
            new Vector2(0f, 1f),
            new Vector2(0f, 0.5f),
            new Vector2(1f, 0.5f),
            new Vector2(1f, 0f),
            new Vector2(0.5f, 0f),
        },
    };

    LayerGraphic m_numberA, m_numberB;

    void DrawNumer(CanvasGraphic canvas, Vector2 size, LayerGraphic layer, int number)
    {
        var nb = numbers[number];
        var hs = size / 2f;

        for (int i = 0; i < nb.Length - 1; ++i)
        {
            canvas.LineBrush.AddBatch(nb[i] * size - hs, nb[i + 1] * size - hs);
        }

        canvas.LineBrush.DrawBatch(m_thickness, layer: layer);
    }

    void OnEnable()
    {
        m_numberA = Canvas.GetNewLayer();
        m_numberB = Canvas.GetNewLayer();
    }

    void OnDisable()
    {
        m_numberA?.Dispose();
        m_numberB?.Dispose();
    }

    protected override void Draw(CanvasGraphic canvas, Vector2 size)
    {
        int time = Mathf.FloorToInt(Time.time) % 10;
        float lerp = Time.time % 1f;

        canvas.Clear(m_numberA);
        canvas.Clear(m_numberB);

        DrawNumer(canvas, size, m_numberA, time);
        DrawNumer(canvas, size, m_numberB, (time + 1) % 10);

        canvas.Blend(m_numberA, m_numberB, Mathf.Pow(m_animation.Evaluate(lerp), m_animPower), canvas.MainLayer);
    }
}
