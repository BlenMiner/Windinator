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

            #define GETSDF sdRoundedBox

            float2 getNormal(float2 p, float2 b, float4 r)
            {
                float x = p.x;
                float y = p.y;

                float d = GETSDF(p, b, r);
                float sign = d >= 0 ? 1.0f : -1.0f;

                //read neighbour distances, ignoring border pixels
                float x0 = GETSDF(p + float2(-1, 0), b, r);
                float x1 = GETSDF(p + float2(+1, 0), b, r);
                float y0 = GETSDF(p + float2(0, -1), b, r);
                float y1 = GETSDF(p + float2(0, +1), b, r);

                //use the smallest neighbour in each direction to calculate the partial deriviates
                float xgrad = sign * x0 < sign* x1 ? -(x0 - d) : (x1 - d);
                float ygrad = sign * y0 < sign* y1 ? -(y0 - d) : (y1 - d);

                //combine partial derivatives to get gradient
                return float2(xgrad, ygrad);
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                float2 position;
                float2 halfSize;
                float2 worldPos;

                LoadData(IN, worldPos);
                float4 tangent = IN.tangent;

                _Roundness = float4(tangent.x, tangent.y, tangent.z, tangent.w);

                GetRawRect(IN.texcoord, position, halfSize, 0);

                // Signed distance field calculation
                float dist = sdRoundedBox(position, halfSize, _Roundness);
                float2 normal = getNormal(position, halfSize, _Roundness);

                return fragFunction(IN.texcoord, worldPos, IN.color, dist, position, halfSize, normal);

                // return IN.color * (1 - dist / length(_Size));
            }
            ENDCG
        }
    }
}
