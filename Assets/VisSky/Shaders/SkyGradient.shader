// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "VisSky/SkyGradient"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Pass
        {
            Fog { Mode Off }
            Tags { "Queue" = "Background" "RenderType" = "Opaque" }
            Lighting Off
            ZWrite Off
            
            CGPROGRAM
    
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
    
            #include "UnityCG.cginc"
    
            float  _CloudDensity;
            float3 _SunDirection;
            float4 _SunColor;
            float4 _SunDisk;
    
            float4    _Color;
            sampler2D _MainTex;
    
            struct VertexOutput
            {
                float4 position : POSITION;
                float2 uvCoords : TEXCOORD0;
                float3 viewDir  : TEXCOORD1;
            };
    
            VertexOutput vert(appdata_full IN)
            {
                VertexOutput OUT;
    
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uvCoords = IN.texcoord;
                OUT.viewDir  = IN.vertex.xyz;
    
                return OUT;
            }
    
            float CalcSunIntensity(float cosTheta, float height)
            {
                return _SunDisk.x / pow(_SunDisk.y - _SunDisk.z * cosTheta, _SunDisk.w);
            }
    
            float4 frag(VertexOutput IN) : COLOR
            {
                half3 viewDir = normalize(IN.viewDir);
    
                float cosTheta  = saturate(dot(-_SunDirection, viewDir));
                float sunIntens = CalcSunIntensity(cosTheta, viewDir.y); // * _SunColor.a;
    
                half3 skyColor = tex2D(_MainTex, float2(_CloudDensity, IN.uvCoords.y)).rgb * _Color.rgb;
                half3 sunColor = sunIntens * _SunColor.rgb;
    
                return float4(saturate(skyColor + sunColor), 1.0f);
            }
    
            ENDCG
        }
    }
    
    Fallback "None"
}
