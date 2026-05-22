Shader "UI/TelescopeMask"
{
    Properties
    {
        _Color ("Mask Color", Color) = (0, 0, 0, 0.9)
        _Radius ("Radius", Range(0, 1)) = 0.33
        _Softness ("Softness", Range(0, 0.2)) = 0.025
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Radius;
            float _Softness;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 center = float2(0.5, 0.5);
                float2 p = i.uv - center;

                // 修正屏幕宽高比，避免圆形被拉成椭圆
                p.x *= _ScreenParams.x / _ScreenParams.y;

                float dist = length(p);

                // 圆内透明，圆外黑色
                float alpha = smoothstep(_Radius, _Radius + _Softness, dist);

                fixed4 col = _Color;
                col.a *= alpha;

                return col;
            }
            ENDCG
        }
    }
}