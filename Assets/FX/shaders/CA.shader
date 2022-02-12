Shader "Amit/CA"
{
    //https://reshade.me/forum/shader-presentation/1133-yaca-yet-another-chromatic-aberration
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SampleCount ("Amount of samples (only even numbers are accepted, odd numbers will be clamped)", Int) = 48
        _Aberration ("Aberration scale in pixels", Int) = 1
        _RadialIntensity("radial length intensity domain:[0.-1.]", Float) = 1
        [MaterialToggle] AutoSample ("Auto Gen Sample or take input", Float) = 0
        _RedShiftX ("Red Shift X", Range(-10, 10)) = 3.0
        _RedShiftY ("Red Shift Y", Range(-10, 10)) = -2.0
        _GreenShiftX ("Green Shift X", Range(-10, 10)) = 4.0
        _GreenShiftY ("Green Shift Y", Range(-10, 10)) = 0.0
        _BlueShiftX ("Blue Shift X", Range(-10, 10)) = 0.0
        _BlueShiftY ("Blue Shift Y", Range(-10, 10)) = 0.0
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;     
            uniform int _SampleCount;
            uniform int _Aberration;
            float _RadialIntensity;
            #pragma multi_compile DUMMY AUTOSAMPLE_ON

            half _RedShiftX;
            half _RedShiftY;
            half _GreenShiftX;
            half _GreenShiftY;
            half _BlueShiftX;
            half _BlueShiftY;
            half2 _MainTex_TexelSize;            
            // Special Hue generator by JMF
            float3 Spectrum(float Hue)
            {
	            float3 HueColor = abs(Hue * 4.0 - float3(1.0, 2.0, 1.0));
	            HueColor = saturate(1.5 - HueColor);
	            HueColor.xz += saturate(Hue * 4.0 - 3.5);
	            HueColor.z = 1.0 - HueColor.z;
	            return HueColor;
            }
            v2f vert (appdata v)
            {
                float x = _ScreenParams.x;
                float y = _ScreenParams.y;
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //height*(1+1/width -1)
                //https://docs.unity3d.com/Manual/SL-UnityShaderVariables.html?_ga=2.141394103.400275972.1642226348-562698595.1631056537
                float Aspect = _ScreenParams.y*(1.-_ScreenParams.z);
	                // Grab Pixel V size
	            float Pixel = .5;
                int Samples=0;
            #ifdef AUTOSAMPLE_ON
                Samples = ceil(_Aberration * 0.5) * 2;
		        Samples += 2;
                // Minimum 6 samples for right color split
                Samples = max(Samples, 6);
            #else
                Samples = floor(_SampleCount * 0.5) * 2;
            #endif
                // sample the texture
                // Clamp maximum sample count
	            //Samples = min(Samples, 48);

                

	            // Convert UVs to radial coordinates with correct Aspect Ratio
	            float2 RadialCoord = i.uv * 2.0 - 1.0;
	            RadialCoord.x *= Aspect;


                
	            // Generate radial mask from center (0) to the corner of the screen (1)
            	//float Mask = length(RadialCoord) / length(float2(Aspect, 1.0));
	            float Mask = length(RadialCoord) * rsqrt(Aspect * Aspect + 1.0) * _RadialIntensity;

	            // Reset values for each pixel sample
	            float3 BluredImage = float3(0.0, 0.0, 0.0);
	            float OffsetBase = Mask * _Aberration * Pixel ;



                
	            // Each loop represents one pass
	            for(int P = 0; P < Samples ; P++)
	            {
		            // Calculate current sample
		            float CurrentSample = float(P) / float(Samples);

		            float Offset = OffsetBase * CurrentSample + 1.0;

		            // Scale UVs at center
		            float2 Position = RadialCoord / Offset;
		            // Convert aspect ratio back to square
		            Position.x /= Aspect;
		            // Convert radial coordinates to UV
		            Position = Position * 0.5 + 0.5;

		            // Multiply texture sample by HUE color
                    //tex2Dlod(ReShade::BackBuffer, float4(Position, 0, 0))
		            BluredImage += Spectrum(CurrentSample) * tex2D(_MainTex, float4(Position,0,0)).rgb;
	            }
	            BluredImage = BluredImage / Samples * 2.0;
	            return fixed4(BluredImage,0.);

//                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
//                UNITY_APPLY_FOG(i.fogCoord, col);
//                return col;
            }
            ENDCG
        }
    }
}
