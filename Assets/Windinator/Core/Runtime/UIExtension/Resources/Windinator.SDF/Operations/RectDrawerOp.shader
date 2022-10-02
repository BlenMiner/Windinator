Shader "UI/Windinator/DrawRect"
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

            uniform float4 _Points[512];
            uniform float4 _PointsExtra[512];
            uniform float4 _PointsExtra2[512];
            uniform float4 _Transform[512];

            int _PointsCount;

            float sdRoundedBox(float2 p, float2 b, float4 r )
            {
                r.xy = (p.x>0.0)?r.xy : r.zw;
                r.x  = (p.y>0.0)?r.x  : r.y;
                float2 q = abs(p)-b+r.x;
                return min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
            }

            float4 frag (v2f IN) : SV_Target
            {
                float2 position;
                float2 halfSize;

                GetRawRect(IN.texcoord, position, halfSize, 1);

                half4 color = tex2D(_MainTex, IN.texcoord);

                float dist = Decode(color.r);

                for (int i = 0; i < 512; ++i)
                {
                    if (i >= _PointsCount) break;

                    float rotation = _Transform[i].x;

                    float2 pos = _Points[i].xy;
                    float2 size = _Points[i].zw;
                    float4 round = _PointsExtra[i];
                    float blend = _PointsExtra2[i].x;

                    float2 localPos = position - pos;

                    localPos = rotate(localPos, rotation);

                    float d = sdRoundedBox(localPos, size, round);
                    dist = AddSDF(d, dist, blend);
                }

                return float4(Encode(dist), 1, 1, 1);
            }
            ENDCG
        }
    }
}
