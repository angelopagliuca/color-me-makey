Shader "Custom/PixelShader"
{
    Properties
    {
        _Color ("Fill Color", Color) = (1,1,1,1)
        _BorderColor ("Border Color", Color) = (0,0,0,1)
        _BorderThickness ("Border Thickness", Range(0,0.5)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float4 _BorderColor;
            float _BorderThickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float borderX = step(uv.x, _BorderThickness) + step(1.0 - uv.x, _BorderThickness);
                float borderY = step(uv.y, _BorderThickness) + step(1.0 - uv.y, _BorderThickness);
                float border = saturate(borderX + borderY);

                fixed4 col = _Color;

                col.rgb = lerp(col.rgb, _BorderColor.rgb, border);
                return col;
            }
            ENDCG
        }
    }
}
