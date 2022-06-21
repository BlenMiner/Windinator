Shader "UI/Windinator/TriangleRenderer"
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

            float sdTriangleIsosceles(in float2 p, in float2 q )
            {
                p.x = abs(p.x);
                float2 a = p - q*clamp( dot(p,q)/dot(q,q), 0.0, 1.0 );
                float2 b = p - q*float2( clamp( p.x/q.x, 0.0, 1.0 ), 1.0 );
                float s = -sign( q.y );
                float2 d = min( float2( dot(a,a), s*(p.x*q.y-p.y*q.x) ),
                            float2( dot(b,b), s*(p.y-q.y)  ));
                return -sqrt(d.x)*sign(d.y);
            }

            float sdTriangle( in float2 p, in float2 p0, in float2 p1, in float2 p2 )
            {
                float2 e0 = p1-p0, e1 = p2-p1, e2 = p0-p2;
                float2 v0 = p -p0, v1 = p -p1, v2 = p -p2;
                float2 pq0 = v0 - e0*clamp( dot(v0,e0)/dot(e0,e0), 0.0, 1.0 );
                float2 pq1 = v1 - e1*clamp( dot(v1,e1)/dot(e1,e1), 0.0, 1.0 );
                float2 pq2 = v2 - e2*clamp( dot(v2,e2)/dot(e2,e2), 0.0, 1.0 );
                float s = sign( e0.x*e2.y - e0.y*e2.x );
                float2 d = min(min(float2(dot(pq0,pq0), s*(v0.x*e0.y-v0.y*e0.x)),
                                float2(dot(pq1,pq1), s*(v1.x*e1.y-v1.y*e1.x))),
                                float2(dot(pq2,pq2), s*(v2.x*e2.y-v2.y*e2.x)));
                return -sqrt(d.x)*sign(d.y);
            }


            fixed4 frag (v2f IN) : SV_Target
            {
                float2 position;
                float2 halfSize;

                GetRect(IN.texcoord, position, halfSize);

                float2 size = halfSize - 0.5;

                float roundness = max(min(min(size.x, size.y + 0.0001), _Roundness.x), 0);

                size -= roundness;

                float2 dsize = float2(size.x, -size.y);
                float2 dpos = position;

                float2 p0 = position - dsize;
                float2 p1 = p0 + float2(0, dsize.y * 2);
                float2 p2 = position + float2(dsize.x * 2, -dsize.y);

                // Signed distance field calculation
                
                float dist = sdTriangle(dpos, float2(-dsize.x, dsize.y), float2(dsize.x * (_Roundness.y - 1), -dsize.y), dsize) - roundness;

                // float dist = sdTriangleIsosceles(dpos, dsize) - roundness;

                return fragFunction(IN.texcoord, IN.worldPosition, IN.color, dist, position, halfSize);
            }
            ENDCG
        }
    }
}
