sampler2D _MainTex;
fixed4 _Color;
fixed4 _CircleColor;
fixed4 _TextureSampleAdd;
float4 _ClipRect;
float4 _MainTex_ST;

fixed4 _Roundness;
float _Padding;
float _OutlineSize;
float _GraphicBlur;
fixed4 _OutlineColor;

float _ShadowSize;
float _ShadowBlur;
float _ShadowPow;
fixed4 _ShadowColor;

float _CircleRadius;
float2 _CirclePos;
float _CircleAlpha;
float4 _MaskRect;

float2 _Size;

void GetRect(float2 uv, out float2 position, out float2 halfSize)
{
    float2 normalizedPadding = float2(_Padding / _Size.x, _Padding / _Size.y);

    // Transform UV based on padding so image stays inside its container
    uv = uv * (1 + normalizedPadding * 2) - normalizedPadding;

    // For simplicity, convert UV to pixel coordinates
    position = (uv - 0.5) * _Size;
    halfSize = (_Size + 1) * 0.5;

    if (_MaskRect.z > 0 && _MaskRect.w > 0 &&
        position.x >= _MaskRect.x && position.x <= _MaskRect.x + _MaskRect.z &&
        position.y >= _MaskRect.y && position.y <= _MaskRect.y + _MaskRect.w)
    {

        clip(-1);
    }
}

fixed4 fragFunction(float2 uv, float4 worldPosition, float4 color, float dist, float2 position, float2 halfSize)
{
    half4 _GraphicColor = (tex2D(_MainTex, uv) + _TextureSampleAdd) * color;

    float delta = fwidth(dist);
    float delta1 = fwidth(dist + 1);

    // Calculate the different masks based on the SDF
    float graphicAlpha = 1 - smoothstep(-delta, 0, dist);
    float graphicAlphaOutline = 1 - smoothstep(-delta1, 0, dist + 1);
    float outlineAlpha = (1 - smoothstep(_OutlineSize - 1 - delta, _OutlineSize - 1 + delta, dist)) * _OutlineColor.a;
    float shadowAlpha = (1 - smoothstep(_ShadowSize - _ShadowBlur - delta, _ShadowSize, dist));

    float circleSDF = distance(position, _CirclePos) - _CircleRadius;
    float circleAASDF = 1 - smoothstep(-fwidth(circleSDF), 0, circleSDF);

    // Start with the background most layer, aka shadows
    shadowAlpha = shadowAlpha * step(0.001, _ShadowSize);
    outlineAlpha = outlineAlpha * step(0.001, _OutlineSize);

    float4 shadowColor = float4(_ShadowColor.rgb, shadowAlpha);
    float4 outlineColor = float4(_OutlineColor.rgb, outlineAlpha);
    float4 graphicColor = float4(_GraphicColor.rgb, graphicAlpha * _GraphicColor.a);

    float shadowInvisible = step(shadowAlpha, 0.001);
    float shapeInvisible = step(0.001, graphicAlphaOutline);

    float4 baseColor = lerp(float4(_ShadowColor.rgb, 0), float4(_GraphicColor.rgb, 0), max(shadowInvisible, shapeInvisible));

    float4 graphic = lerp(
        baseColor,
        graphicColor,
        graphicColor.a
    );

    float4 shadowWithGraphic = lerp(
        graphic,
        _ShadowColor,
        shadowAlpha * (1 - graphicAlphaOutline)
    );

    shapeInvisible = step(0.001, shadowWithGraphic.a);

    float4 shapeColor = lerp(float4(_OutlineColor.rgb, 0), shadowWithGraphic, shapeInvisible);

    float4 effects = lerp(
        shapeColor,
        _OutlineColor,
        outlineAlpha * (1 - graphicAlphaOutline)
    );

    effects = lerp(effects, _CircleColor, circleAASDF * _CircleAlpha * graphicAlphaOutline);

    // Unity stuff
    #ifdef UNITY_UI_CLIP_RECT
    effects.a *= UnityGet2DClipping(worldPosition.xy, _ClipRect);
    #endif

    #ifdef UNITY_UI_ALPHACLIP
    clip (effects.a - 0.001);
    #endif

    return effects;
}