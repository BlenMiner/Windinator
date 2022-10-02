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
    float4 texcoord : TEXCOORD0;
    float4 uv1 : TEXCOORD1;
    float4 uv2 : TEXCOORD2;
    float4 uv3 : TEXCOORD3;
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
    float4 uv2 : TEXCOORD2;
    float4 uv3 : TEXCOORD3;
    float4 uv4 : TEXCOORD4;
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

    OUT.uv4.xy = v.texcoord.zw;
    OUT.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
    OUT.uv2 = v.uv2;
    OUT.uv3 = v.uv3;
    OUT.tangent = v.tangent;
    OUT.uv1 = v.uv1;
    OUT.color = v.color;
    OUT.normals = float3(v.normals.x, v.vertex.y, v.vertex.x);
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

float2 rotate(float2 pos, float angle)
{
    float sinX = sin(angle);
    float cosX = cos(angle);
    float2x2 rotationMatrix = float2x2(cosX, -sinX, sinX, cosX);
    return mul(pos, rotationMatrix);
}

inline float Encode(float dist)
{
    float maxLen = length(_Size * 0.5 + _Padding);
    return ((dist / maxLen) + 1) * 0.5;
}

inline float Decode(float value)
{
    float maxLen = length(_Size * 0.5 + _Padding);
    return ((value * 2) - 1) * maxLen;
}

inline float opSmoothUnion( float d1, float d2, float k ) {
    float h = clamp( 0.5 + 0.5*(d2-d1)/k, 0.0, 1.0 );
    return lerp( d2, d1, h ) - k*h*(1.0-h); 
}


inline float opSmoothSubtraction( float d1, float d2, float k ) {
    float h = clamp( 0.5 - 0.5*(d2+d1)/k, 0.0, 1.0 );
    return lerp( d2, -d1, h ) + k*h*(1.0-h);
}

inline float opSmoothIntersection( float d1, float d2, float k ) {
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
    position = (uv - 0.5) * _Size - 0.5 + 0.5;
}

void GetRect(float2 uv, out float2 position, out float2 halfSize, float extra)
{
    GetRawRect(uv, position, halfSize, extra);

    float2 pos = ((position + 1) + halfSize - 0.5);

    bool shouldMask = ((_MaskRect.z > 0 && _MaskRect.w > 0 &&
        position.x >= _MaskRect.x && position.x <= _MaskRect.x + _MaskRect.z &&
        position.y >= _MaskRect.y && position.y <= _MaskRect.y + _MaskRect.w) ||
        pos.x < _MaskOffset.x - _Padding + 1.5 || pos.x > _Size.x + _MaskOffset.z + _Padding - 0.5 ||
        pos.y < _MaskOffset.y - _Padding + 1.5 || pos.y > _Size.y + _MaskOffset.w + _Padding - 0.5);

    _Alpha = min(_Alpha, 1 - shouldMask);
}

#define LOSE 20
#define WIN (255.0f - (LOSE * 2))
#define WIN_2 (127.0f - (LOSE * 2))

float2 DecodeVector2(float value)
{
    float x = floor(value) / 16384;
    float y = floor(value) % 16384.0;

    return float2((x / 4), (y / 4));
}

float2 DecodeVector2(float value, uint xmax)
{
    float x = (uint)(value) / xmax;
    float y = (uint)(value) % xmax;

    return float2((x / 4), (y / 4));
}

float4 DecodeColor(float value)
{
    uint v = asuint(value);

    float r = v & 0xFF;
    float g = (v >> 8) & 0xFF;
    float b = (v >> 16) & 0xFF;
    float a = (v >> 24) & 0xFF;

    return float4(
        (r - LOSE) / WIN,
        (g - LOSE) / WIN,
        (b - LOSE) / WIN, 
        (a - LOSE) / WIN_2
    ); 
}

float Encode01(float4 c)
{
    float max = 32768;

    uint r = (uint)(c.r * 31);
    uint g = (uint)(c.g * 31) << 5;
    uint b = (uint)(c.b * 15) << 10;

    uint res = (r | g | b);

    return (float)res / max;
}

float4 Decode01(float c)
{
    float max = 32768;

    c = c * max;

    uint color = (uint)c;

    // 0x1F
    float r = color & 0x1F;
    float g = (color >> 5) & 0x1F;
    float b = (color >> 10) & 0x1F;

    return float4(r / 31.0f, g / 31.0f, b / 15, 1);
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

float _EmbossDirection;
float _EmbossStrength;
float _EmbossBlurTop;
float _EmbossBlurBottom;
float _EmbossSize;
float _EmbossDistance;

float4 _EmbossHColor;
float4 _EmbossLColor;
float _EmbossHPower;
float _EmbossLPower;

void LoadData(v2f v, out float2 worldPos)
{
    _Padding = v.uv4.y;

    float4 uv2 = v.uv2;
    float4 uv1 = v.uv1;
    float4 uv3Data = DecodeColor(v.uv3.w);

    _Size = uv2.xy;

    float4 emData = DecodeColor(v.normals.x);
    float4 emColorData = DecodeColor(uv2.w);

    _EmbossSize = uv1.z;
    _EmbossDistance = uv2.z;
    _EmbossDirection = emData.r * 6.28318530;
    _EmbossStrength = emData.a;
    _EmbossHColor = float4(emColorData.rgb, 1);
    _EmbossHPower = emColorData.a;

    float4 emLColorData = DecodeColor(v.uv4.x);

    _EmbossLColor = float4(emLColorData.rgb, 1);
    _EmbossLPower = emLColorData.a;

    _EmbossBlurTop = ((emData.y * 2) - 1) * _EmbossSize;
    _EmbossBlurBottom = ((emData.z * 2) - 1) * _EmbossSize;

    float2 padBlur = DecodeVector2(uv1.x);
    float2 outCirc = DecodeVector2(uv1.w);

    _ShadowSize = uv1.x;
    _ShadowBlur = uv1.y;
    _ShadowPow = 1;
    _ShadowColor = DecodeColor(v.uv3.y);

    _OutlineColor = DecodeColor(v.uv3.x);
    _OutlineSize = uv1.w;
    _Alpha = uv3Data.y;

    /*_CircleColor = float4(colDecoded.xyz, 1);
    _CircleAlpha = _CircleColor.a;
    _CirclePos = v.uv1.yz;
    _CircleRadius = uv2.z;*/

    worldPos = float2(v.normals.z, uv1.z);
}

fixed4 fragFunctionRaw(float2 uv, float2 worldPosition, float4 color, float dist, float2 position, float2 halfSize, float2 normal)
{
    float4 effects;

    float embossDist = dist + _EmbossDistance;
    float embossSizeDist = dist + _EmbossDistance + _EmbossSize;

    float outlineDist = dist - _OutlineSize;
    float shadowDist = dist - _ShadowSize;
    
    float delta = fwidth(dist * 0.5);
    float outlineDelta = fwidth(outlineDist);
    float shadowDelta = fwidth(shadowDist);
    float embossDelta = fwidth(embossDist);
    float embossSizeDelta = fwidth(embossSizeDist);

    // Calculate the different masks based on the SDF
    float graphicAlpha = smoothstep(delta, -delta, dist);
    float outlineAlpha = smoothstep(outlineDelta, -outlineDelta, outlineDist);
    float shadowAlpha = smoothstep(shadowDelta, -shadowDelta - _ShadowBlur, shadowDist);

    float embossStartAlpha = smoothstep(embossDelta - min(0, _EmbossBlurTop), -embossDelta - max(0, _EmbossBlurTop), embossDist);
    float embossEndAlpha = smoothstep(embossSizeDelta - min(0, _EmbossBlurBottom), -embossSizeDelta - max(0, _EmbossBlurBottom), embossSizeDist);

    float4 graphic = float4(color.rgb, color.a);
    float4 outline = float4(_OutlineColor.rgb, _OutlineColor.a);
    float4 shadow = float4(_ShadowColor.rgb, _ShadowColor.a);

    float circleSDF = distance(position, _CirclePos) - _CircleRadius;
    float circleDelta = fwidth(circleSDF);
    float circleAASDF = smoothstep(circleDelta, -circleDelta, circleSDF);

    float light = dot(normal, rotate(float2(0, 1), _EmbossDirection));

    if (light > 0)
         light *= _EmbossHPower;
    else light *= _EmbossLPower;

    light = sign(light) * pow(abs(light), (_EmbossStrength * 50) + 1);


    float4 lightTint = float4(_EmbossHColor.r, _EmbossHColor.g, _EmbossHColor.b, 1);
    float4 shadowTint = float4(_EmbossLColor.r, _EmbossLColor.g, _EmbossLColor.b, 1);
    float4 lightColor = lerp(graphic, (light > 0 ? lightTint : shadowTint), abs(light));

    graphic = lerp(graphic, lightColor, _EmbossSize > 0 ? (embossStartAlpha - embossEndAlpha) : 0);
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

fixed4 fragFunction(float2 uv, float2 worldPosition, float4 color, float dist, float2 position, float2 halfSize, float2 normal)
{
    float2 textUV = (position + _Size.xy * 0.5) / _Size.xy;
    float4 textCol = tex2D(_MainTex, textUV);

    return fragFunctionRaw(uv, worldPosition, color * textCol, dist, position, halfSize, normal);
}