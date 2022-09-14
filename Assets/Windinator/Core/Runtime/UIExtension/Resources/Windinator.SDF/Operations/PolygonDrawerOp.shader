Shader "UI/Windinator/DrawPoly"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM

            #include "../shared.cginc"

            uniform float2 _Points[2048];

            int _PointsCount;

            float2 GetPoint(int id, float2 size)
            {
                return _Points[id];
            }

            float sdPolygon(in float2 p, float r, float2 size)
            {
                float d = dot(p - GetPoint(0, size) * r, p - GetPoint(0, size) * r);
                float s = 1.0;

                for (int i = 0, j = _PointsCount - 1; i < _PointsCount; j = i, i++)
                {
                    float2 e = GetPoint(j, size) * r - GetPoint(i, size) * r;
                    float2 w = (p - GetPoint(i, size) * r);
                    float2 b = w - e * clamp(dot(w, e) / dot(e, e), 0.0, 1.0);
                    d = min(d, dot(b, b));
                    bool3 c = bool3(p.y >= (GetPoint(i, size).y * r), p.y<(GetPoint(j, size).y* r), e.x* w.y>e.y * w.x);
                    if (all(c) || all(!(c))) s *= -1.0;
                    // if (all(c) || all(not(c))) s *= -1.0;
                }
                return s * sqrt(d);
            }

            float4 frag (v2f IN) : SV_Target
            {
                float2 position;
                float2 halfSize;

                GetRawRect(IN.texcoord, position, halfSize, 1);

                half4 color = tex2D(_MainTex, IN.texcoord);

                float roundness = max(0, _Roundness.x);
                float2 dpos = position;

                float p = 1 + roundness / max(halfSize.x, halfSize.y);

                float dist = sdPolygon(dpos * p, 1, halfSize) - roundness;

                return float4(Encode(dist), 1, 1, 1);
            }
            ENDCG
        }
    }
}
