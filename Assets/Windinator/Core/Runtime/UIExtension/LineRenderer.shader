Shader "UI/Windinator/LineRenderer"
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
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "shared.cginc"
            
            struct appdata
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex           : SV_POSITION;
                fixed4 color            : COLOR;
                float2 texcoord         : TEXCOORD0;
                float4 worldPosition    : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            uniform float2 _Points[2048];

            int _PointsCount;

            float2 GetPoint(int id, float2 size)
            {
                return (_Points[id] - 0.5f) * size * 2;
            }

            float sdSegment( in float2 p, in float2 a, in float2 b )
            {
                float2 pa = p-a, ba = b-a;
                float h = clamp( dot(pa,ba)/dot(ba,ba), 0.0, 1.0 );
                return length( pa - ba*h );
            }
            
            float _LineThickness;

            float sdPolygon(in float2 p, float r, float2 size)
            {
                float d = 999999;

                for (int i = 0; i < _PointsCount - 1; i++)
                {
                    float2 a = GetPoint(i,     size);
                    float2 b = GetPoint(i + 1, size);

                    d = min(d,sdSegment(p, a, b) - _LineThickness);
                }

                return d;
            }

            v2f vert (appdata v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                OUT.color = v.color * _Color;
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                float2 position;
                float2 halfSize;

                GetRect(IN.texcoord, position, halfSize, 1);

                float2 size = halfSize - 0.5;

                float maxSize = min(size.x , size.y);
                float roundness = max(min(maxSize, _Roundness.x), 0);
                size -= roundness;

                float2 dsize = float2(size.x, -size.y);
                float2 dpos = position;

                // Signed distance field calculation

                float multiplier = 1 - (roundness / maxSize);
                halfSize -= roundness;
                
                float dist = sdPolygon(dpos, 1, halfSize) - roundness;

                // float dist = sdTriangleIsosceles(dpos, dsize) - roundness;

                return fragFunction(IN.texcoord, IN.worldPosition, IN.color, dist, position, halfSize);
            }
            ENDCG
        }
    }
}
