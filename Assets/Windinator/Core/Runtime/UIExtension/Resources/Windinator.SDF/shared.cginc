#include "UnityCG.cginc"
#include "UnityUI.cginc"

#pragma vertex vert
#pragma fragment frag
#pragma target 2.0

#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

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
    float4 color            : COLOR;
    float2 texcoord         : TEXCOORD0;
    float4 worldPosition    : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _Color;

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

fixed4 _CircleColor;
fixed4 _TextureSampleAdd;
float4 _ClipRect;

fixed4 _Roundness;
float _Padding;
float _OutlineSize;
float _GraphicBlur;
fixed4 _OutlineColor;

float _ShadowSize;
float _ShadowBlur;
float _ShadowPow;
fixed4 _ShadowColor;

float _CircleRadius;
float2 _CirclePos;
float _CircleAlpha;
float4 _MaskRect;
float4 _MaskOffset;

float _Alpha;
float2 _Size;

float4 _LU;
float4 _RU;
float4 _LD;
float4 _RD;

float _Union;
int _Operation;

#define MIN_VAL 4096

float Encode(float dist)
{
    float maxLen = sqrt(_Size.x * _Size.x * 0.5 + _Size.y * _Size.y * 0.5f);
    return ((dist / maxLen) + 1) * 0.5;
}

float Decode(float value)
{
    float maxLen = sqrt(_Size.x * _Size.x * 0.5 + _Size.y * _Size.y * 0.5f);
    return ((value * 2) - 1) * maxLen;
}

float opSmoothUnion( float d1, float d2, float k ) {
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) - k*h*(1.0-h); 
}


float opSmoothSubtraction( float d1, float d2, float k ) {
    float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
    return lerp( d2, -d1, h ) + k*h*(1.0-h);
}

float opSmoothIntersection( float d1, float d2, float k ) {
    float h = clamp( 0.5 - 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) + k*h*(1.0-h); 
}

float AddSDF(float sdf, float old) {
    if (_Operation == 0) {
        return opSmoothUnion(sdf, old, _Union);
    } else if (_Operation == 1) {
        return opSmoothSubtraction(sdf, old, _Union);
    } else {
        return opSmoothIntersection(sdf, old, _Union);
    }
}

float AddSDF(float sdf, float old, float k) {
    if (_Operation == 0) {
        return opSmoothUnion(sdf, old, k);
    } else if (_Operation == 1) {
        return opSmoothSubtraction(sdf, old, k);
    } else {
        return opSmoothIntersection(sdf, old, k);
    }
}

void GetRawRect(float2 uv, out float2 position, out float2 halfSize, float extra)
{
    float2 normalizedPadding = float2(_Padding / _Size.x, _Padding / _Size.y);

    halfSize = (_Size + extra) * 0.5;


    // Transform UV based on padding so image stays inside its container
    uv = uv * (1 + normalizedPadding * 2) - normalizedPadding;

    // For simplicity, convert UV to pixel coordinates
    position = (uv - 0.5) * _Size - 0.5;
}

void GetRect(float2 uv, out float2 position, out float2 halfSize, float extra)
{
    GetRawRect(uv, position, halfSize, extra);

    float2 pos = position + halfSize - 0.5;

    if ((_MaskRect.z > 0 && _MaskRect.w > 0 &&
        position.x >= _MaskRect.x && position.x <= _MaskRect.x + _MaskRect.z &&
        position.y >= _MaskRect.y && position.y <= _MaskRect.y + _MaskRect.w) ||
        pos.x < _MaskOffset.x - _Padding + 1.5 || pos.x > _Size.x + _MaskOffset.z + _Padding - 0.5 ||
        pos.y < _MaskOffset.y - _Padding + 1.5 || pos.y > _Size.y + _MaskOffset.w + _Padding - 0.5)
    {
        clip(-1);
    }
}

fixed4 fragFunctionRaw(float2 uv, float4 worldPosition, float4 color, float dist, float2 position, float2 halfSize)
{
    half4 _GraphicColor = color;

    float4 top = lerp(_LU, _RU, uv.x);
    float4 bottom = lerp(_LD, _RD, uv.x);

    _GraphicColor *= lerp(bottom, top, uv.y);

    float delta = fwidth(dist);
    float delta1 = fwidth(dist + 1);

    // Calculate the different masks based on the SDF
    float graphicAlpha = 1 - smoothstep(-delta, delta * 0.5, dist);
    float graphicAlphaOutline = 1 - smoothstep(-delta1, 0, dist + 1);
    float outlineAlpha = (1 - smoothstep(_OutlineSize - 1 - delta, _OutlineSize - 1 + delta * 0.5, dist)) * _OutlineColor.a;
    float shadowAlpha = (1 - smoothstep(_ShadowSize - _ShadowBlur - delta, _ShadowSize, dist));

    float circleSDF = distance(position, _CirclePos) - _CircleRadius;
    float circleAASDF = 1 - smoothstep(-fwidth(circleSDF), 0, circleSDF);

    // Start with the background most layer, aka shadows
    shadowAlpha = pow(shadowAlpha, _ShadowPow) * step(0.001, _ShadowSize);
    outlineAlpha = outlineAlpha * step(0.001, _OutlineSize);

    float4 shadowColor = float4(_ShadowColor.rgb, shadowAlpha);
    float4 outlineColor = float4(_OutlineColor.rgb, outlineAlpha);
    float4 graphicColor = float4(_GraphicColor.rgb, graphicAlpha * _GraphicColor.a);

    float shadowInvisible = step(shadowAlpha, 0.001);
    float shapeInvisible = step(0.001, graphicAlphaOutline);

    float4 baseColor = lerp(float4(_ShadowColor.rgb, 0), float4(_GraphicColor.rgb, 0), max(shadowInvisible, shapeInvisible));

    float4 graphic = lerp(
        baseColor,
        graphicColor,
        graphicColor.a
    );
    graphic = lerp(graphic, float4(_CircleColor.rgb, 1), circleAASDF * _CircleAlpha * graphicAlpha);

    float4 shadowWithGraphic = lerp(
        graphic,
        _ShadowColor,
        shadowAlpha * (1 - graphicAlphaOutline)
    );

    shapeInvisible = step(0.001, shadowWithGraphic.a);

    float4 shapeColor = lerp(float4(_OutlineColor.rgb, 0), shadowWithGraphic, shapeInvisible);

    float4 effects = lerp(
        shapeColor,
        _OutlineColor,
        outlineAlpha * (1 - graphicAlphaOutline)
    );


    effects.a *= _Alpha;
    // Unity stuff
    #ifdef UNITY_UI_CLIP_RECT
    effects.a *= UnityGet2DClipping(worldPosition.xy, _ClipRect);
    #endif

    #ifdef UNITY_UI_ALPHACLIP
    clip (effects.a - 0.001);
    #endif

    return effects;
}

fixed4 fragFunction(float2 uv, float4 worldPosition, float4 color, float dist, float2 position, float2 halfSize)
{
    return (tex2D(_MainTex, uv) + _TextureSampleAdd) * fragFunctionRaw(uv, worldPosition, color, dist, position, halfSize);
}