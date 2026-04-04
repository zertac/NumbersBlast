Shader "NumbersBlast/TutorialOverlay"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Overlay Color", Color) = (0,0,0,0.7)
        _CutoutCenter ("Cutout Center", Vector) = (0.5, 0.5, 0, 0)
        _CutoutSize ("Cutout Size", Vector) = (0.2, 0.2, 0, 0)
        _CutoutRadius ("Cutout Radius", Float) = 0.008
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _CutoutCenter;
            float4 _CutoutSize;
            float _CutoutRadius;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = _CutoutCenter.xy;
                float2 halfSize = _CutoutSize.xy * 0.5;

                float2 dist = abs(i.uv - center) - halfSize + _CutoutRadius;
                float d = length(max(dist, 0.0)) - _CutoutRadius;

                float cutoutAlpha = smoothstep(-0.005, 0.005, d);

                return float4(_Color.rgb, _Color.a * cutoutAlpha);
            }
            ENDCG
        }
    }
}
