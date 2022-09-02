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
    float4 uv1 : TEXCOORD1;
    float4 WH_OutlineSize_Alpha : TEXCOORD2;
    float4 OutlineColor : TEXCOORD3;
    float4 tangent : TANGENT;
    float3 normals : NORMAL;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float4 vertex           : SV_POSITION;
    float4 color            : COLOR;
    float2 texcoord         : TEXCOORD0;
    float4 uv1 : TEXCOORD1;
    float4 WH_OutlineSize_Alpha : TEXCOORD2;
    float4 OutlineColor : TEXCOORD3;
    float4 tangent : TANGENT;
    float3 normals : NORMAL;
    UNITY_VERTEX_OUTPUT_STEREO
};

sampler2D _MainTex;
float4 _MainTex_ST;

v2f vert (appdata v)
{
    v2f OUT;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

    OUT.vertex = UnityObjectToClipPos(v.vertex);
    OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
    OUT.WH_OutlineSize_Alpha = v.WH_OutlineSize_Alpha;
    OUT.OutlineColor = v.OutlineColor;
    OUT.tangent = v.tangent;
    OUT.uv1 = float4(v.uv1.xy, v.vertex.y, v.uv1.w);
    OUT.color = v.color;
    OUT.normals = float3(v.normals.xy, v.vertex.x);
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

float _Union;
int _Operation;

float Encode(float dist)
{
    float maxLen = length(_Size * 0.5 + _Padding);
    return ((dist / maxLen) + 1) * 0.5;
}

float Decode(float value)
{
    float maxLen = length(_Size * 0.5 + _Padding);
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

    bool shouldMask = ((_MaskRect.z > 0 && _MaskRect.w > 0 &&
        position.x >= _MaskRect.x && position.x <= _MaskRect.x + _MaskRect.z &&
        position.y >= _MaskRect.y && position.y <= _MaskRect.y + _MaskRect.w) ||
        pos.x < _MaskOffset.x - _Padding + 1.5 || pos.x > _Size.x + _MaskOffset.z + _Padding - 0.5 ||
        pos.y < _MaskOffset.y - _Padding + 1.5 || pos.y > _Size.y + _MaskOffset.w + _Padding - 0.5);

    _Alpha = min(_Alpha, 1 - shouldMask);
}

float4 DecodeColor(float value)
{
    int v = asint(value);

    float r = v & 0xFF;
    float g = (v >> 8) & 0xFF;
    float b = (v >> 16) & 0xFF;
    float a = (v >> 24) & 0xFF;

    return float4(r / 255.0f, g / 255.0f, b / 255.0f, a / 126.0f);
}

float4 DecodeCoords(float value)
{
    int v = asint(value);

    float r = v & 0xFF;
    float g = (v >> 8) & 0xFF;
    float b = (v >> 16) & 0xFF;
    float a = (v >> 24) & 0xFF;

    return float4(r, g, b, a);
}

void LoadData(v2f v, out float2 worldPos)
{
    float4 uv2 = v.WH_OutlineSize_Alpha;
    float4 uv1 = v.uv1;

    _Alpha = uv2.w;
    _Size = uv2.xy;
    _OutlineSize = uv2.z;
    _Padding = uv1.w;

    _ShadowSize = uv1.x;
    _ShadowBlur = uv1.y;
    _ShadowPow = 1;

    _OutlineColor = DecodeColor(v.OutlineColor.x);
    _ShadowColor = DecodeColor(v.OutlineColor.y);
    _CircleColor = DecodeColor(v.OutlineColor.z);
    _CircleAlpha = _CircleColor.a;

    _CirclePos = v.normals.xy;
    _CircleRadius = v.OutlineColor.w;

    worldPos = float2(v.normals.z, uv1.z);
}

fixed4 fragFunctionRaw(float2 uv, float2 worldPosition, float4 color, float dist, float2 position, float2 halfSize)
{
    float4 effects;

    float outlineDist = dist - _OutlineSize;
    float shadowDist = dist - _ShadowSize;
    
    float delta = fwidth(dist * 0.5);
    float outlineDelta = fwidth(outlineDist);
    float shadowDelta = fwidth(shadowDist);

    // Calculate the different masks based on the SDF
    float graphicAlpha = smoothstep(delta, -delta, dist);
    float outlineAlpha = smoothstep(outlineDelta, -outlineDelta, outlineDist);
    float shadowAlpha = smoothstep(shadowDelta, -shadowDelta - _ShadowBlur, shadowDist);

    float4 graphic = float4(color.rgb, color.a);
    float4 outline = float4(_OutlineColor.rgb, _OutlineColor.a);
    float4 shadow = float4(_ShadowColor.rgb, _ShadowColor.a);

    float circleSDF = distance(position, _CirclePos) - _CircleRadius;
    float circleDelta = fwidth(circleSDF);
    float circleAASDF = smoothstep(circleDelta, -circleDelta, circleSDF);

    graphic = lerp(graphic, float4(_CircleColor.rgb, _CircleAlpha), _CircleRadius > 0 ? circleAASDF * _CircleAlpha : 0);
    effects = lerp(graphic, outline, _OutlineSize > 0 ? 1 - graphicAlpha : 0);
    effects = lerp(effects, shadow, _ShadowSize > _OutlineSize ? 1 - outlineAlpha : 0);

    effects.a *= max(graphicAlpha, max(outlineAlpha, shadowAlpha));
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

fixed4 fragFunction(float2 uv, float2 worldPosition, float4 color, float dist, float2 position, float2 halfSize)
{
    return (tex2D(_MainTex, uv) + _TextureSampleAdd) * fragFunctionRaw(uv, worldPosition, color, dist, position, halfSize);
}