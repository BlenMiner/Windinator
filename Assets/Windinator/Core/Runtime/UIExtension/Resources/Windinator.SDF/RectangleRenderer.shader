Shader "UI/Windinator/RectangleRenderer"
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
            #include "shared.cginc"

            float sdRoundedBox(float2 p, float2 b, float4 r )
            {
                r.xy = (p.x>0.0)?r.xy : r.zw;
                r.x  = (p.y>0.0)?r.x  : r.y;
                float2 q = abs(p)-b+r.x;
                return min(max(q.x,q.y),0.0) + length(max(q,0.0)) - r.x;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                float2 position;
                float2 halfSize;

                GetRect(IN.texcoord, position, halfSize, 0);

                // Signed distance field calculation
                float dist = sdRoundedBox(position, halfSize, _Roundness);

                return fragFunction(IN.texcoord, IN.worldPosition, IN.color, dist, position, halfSize);
            }
            ENDCG
        }
    }
}
