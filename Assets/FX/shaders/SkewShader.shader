//https://www.sector12games.com/skewshear-vertex-shader/
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Adapted from the built-in Sprites-Default.shader
// You can download the source for the built-in shaders from:
// http://unity3d.com/unity/download/archive
Shader "Sector12/SpriteSkewShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [Toggle(USE_MASK)] _UseMask("Use Mask", Float) = 0
        _Mask ("MaskColor", Color) = (1,1,1,0)
        _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        
        _HorizontalSkew ("Horizontal Skew", Float) = 0
        _VerticalSkew ("Vertical Skew", Float) = 0
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
        Cull Front
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #pragma shader_feature USE_MASK
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _Mask;
            float _HorizontalSkew;
            float _VerticalSkew;
            fixed _Cutoff;
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                
                // Create a skew transformation matrix
                float h = _HorizontalSkew;
                float v = _VerticalSkew;
                float4x4 transformMatrix = float4x4(
                    1,h,0,0,
                    v,1,0,0,
                    0,0,1,0,
                    0,0,0,1);
                
                float4 skewedVertex = mul(transformMatrix, IN.vertex);
                OUT.vertex = UnityObjectToClipPos(skewedVertex);
                
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
                return OUT;
            }
            fixed4 frag(v2f IN) : SV_Target
            {

                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                clip(c.a - _Cutoff);
      #ifdef USE_MASK
                return _Mask;
      #else
                c.rgb *= c.a;
                return c;
      #endif
            }
        ENDCG
        }
    }
}