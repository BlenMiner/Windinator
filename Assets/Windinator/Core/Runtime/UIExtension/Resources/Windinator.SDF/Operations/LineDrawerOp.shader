Shader "UI/Windinator/DrawLine"
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
        CGINCLUDE

        #include "../shared.cginc"

        uniform float4 _Points[512];
        int _PointsCount;
        float _LineThickness;

        float sdSegment( in float2 p, in float2 a, in float2 b )
        {
            float2 pa = p-a, ba = b-a;
            float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
            return length( pa - ba*h );
        }

        ENDCG

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

            float4 frag (v2f IN) : SV_Target
            {
                half4 color = tex2D(_MainTex, IN.texcoord);
                
                float2 position;
                float2 halfSize;

                GetRawRect(IN.texcoord, position, halfSize, 1);

                float2 size = halfSize - 0.5;

                float maxSize = min(size.x , size.y);
                float roundness = max(min(maxSize, _Roundness.x), 0);
                size -= roundness;

                float2 dsize = float2(size.x, -size.y);

                float multiplier = 1 - (roundness / maxSize);
                halfSize -= roundness;

                float dist = Decode(color.r);

                for (int i = 0; i < 512; ++i)
                {
                    if (i >= _PointsCount) break;

                    float2 p0 = _Points[i].xy;
                    float2 p1 = _Points[i].zw;

                    float d = sdSegment(position, p0, p1) - _LineThickness;
                    dist = min(d, dist);
                }

                float result = AddSDF(dist, dist);

                return float4(Encode(result), 1, 1, 1);
            }
            ENDCG
        }
    }
}
