Shader "UI/Windinator/CanvasProceduralRenderer"
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

            float GETSDF(in float2 p)
            {
                float2 fullsize = (_Size.xy + _Padding * 2);
                float2 textUV = (p + (fullsize) * 0.5) / fullsize;
                float4 color = tex2D(_MainTex, textUV);
                return Decode(color.r);
            }

            float2 getNormal(in float2 p)
            {
                float x = p.x;
                float y = p.y;

                float d = GETSDF(p);
                float sign = d >= 0 ? 1.0f : -1.0f;

                //read neighbour distances, ignoring border pixels
                float x0 = GETSDF(p + float2(-1, 0));
                float x1 = GETSDF(p + float2(+1, 0));
                float y0 = GETSDF(p + float2(0, -1));
                float y1 = GETSDF(p + float2(0, +1));

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

                GetRect(IN.texcoord, position, halfSize, 1);

                float dist = GETSDF(position);
                float2 norm = getNormal(position);

                return fragFunctionRaw(IN.texcoord, worldPos, IN.color, dist, position, halfSize, norm);
            }
            ENDCG
        }
    }
}
