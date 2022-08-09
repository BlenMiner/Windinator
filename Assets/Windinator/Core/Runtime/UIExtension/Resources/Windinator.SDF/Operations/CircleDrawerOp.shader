Shader "UI/Windinator/DrawCircle"
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
            int _PointsCount;
            float _LineThickness;

            float circleSDF(float2 center, float2 pos, float radius)
            {
                return distance(center, pos) - radius;
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

                    float2 pos = _Points[i].xy;
                    float size = _Points[i].z;
                    float blend = _Points[i].w;

                    float d = circleSDF(pos, position, size);
                    dist = AddSDF(d, dist, blend);
                }

                return float4(Encode(dist), 0, 0, 1);
            }
            ENDCG
        }
    }
}
