Shader "UI/Windinator/PolygonRenderer"
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
            
            uniform float2 _Points[2048];

            int _PointsCount;

            float2 GetPoint(int id, float2 size)
            {
                return (_Points[id] - 0.5f) * size * 2;
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

            #define GETSDF sdPolygon

            float2 getNormal(in float2 p, float r, float2 size, float roundness)
            {
                float x = p.x;
                float y = p.y;

                float d = GETSDF(p, r, size) - roundness;
                float sign = d >= 0 ? 1.0f : -1.0f;

                //read neighbour distances, ignoring border pixels
                float x0 = GETSDF(p + float2(-1, 0), r, size) - roundness;
                float x1 = GETSDF(p + float2(+1, 0), r, size) - roundness;
                float y0 = GETSDF(p + float2(0, -1), r, size) - roundness;
                float y1 = GETSDF(p + float2(0, +1), r, size) - roundness;

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

                float2 size = halfSize - 0.5;

                float maxSize = min(size.x , size.y);
                float roundness = max(min(maxSize, _Roundness.x), 0);
                size -= roundness;

                float2 dsize = float2(size.x, -size.y);
                float2 dpos = position;

                // Signed distance field calculation
                halfSize -= roundness;
                
                float dist = sdPolygon(dpos, 1, halfSize) - roundness;
                float2 norm = getNormal(dpos, 1, halfSize, roundness);

                // float dist = sdTriangleIsosceles(dpos, dsize) - roundness;

                return fragFunction(IN.texcoord, worldPos, IN.color, dist, position, halfSize, norm);
            }
            ENDCG
        }
    }
}
