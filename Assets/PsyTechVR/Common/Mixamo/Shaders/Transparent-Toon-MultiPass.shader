// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'glstate.matrix.modelview[0]' with 'UNITY_MATRIX_MV'
// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'glstate.matrix.projection' with 'UNITY_MATRIX_P'

// Upgrade NOTE: replaced 'glstate.matrix.modelview[0]' with 'UNITY_MATRIX_MV'
// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'
// Upgrade NOTE: replaced 'glstate.matrix.projection' with 'UNITY_MATRIX_P'

Shader "Mixamo/Transparent/Toon MultiPass" {
  Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _Color ("Tint (RGB)", Color) = (1,1,1,1)
    
    _BumpMap ("Normal (RGB)", 2D) = "bump" {}
 
 
    _SpecMap ("Specular Map (R)", 2D) = "white" {}
    _SpecularColor("Specular Color", Color) = (1,1,1,1)
    _SpecAmount ("Specular Amount", Float) = 1.0
    _SpecCutoff ("SpecCutoff", Float) = 0.5

    
    _RampValues ("Ramp Values (RGB)", Color) = (0.83, 0.5, 0.0, 1.0)
    _RampThresholds ("Ramp Thresholds (RGB)", Color) = (0.83, 0.5, 0.0, 1.0)
    _RampBlend ("Ramp Blend", Float) = 0.08
    
    //Gloss Map
    _GlossMap("Gloss",2D) = "white" {}
    
    _LightCoefficient ("Light Coefficient", Float) = 1.0
    
    _Fresnel ("Fresnel Amount", Float) = 0.1
    _FresnelCutoff ("Fresnel Cutoff", Float) = 0.4
    
    _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    _Outline ("Outline width", Range (.00, 0.03)) = .005
//    _Upness ("Upness", Range(0, 1)) = 0.2
  }
  SubShader { 
  Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}   
  LOD 200
    Name "TOON"
    
    CGINCLUDE
    #include "UnityCG.cginc"
    #include "Lighting.cginc"

    sampler2D _MainTex; 
    sampler2D _Ramp;
    sampler2D _BumpMap;
    sampler2D _SpecMap;
    sampler2D _GlossMap;
    
    //samplerCUBE _Cube;
    half4 _Color;
    half _SpecAmount;
    half4 _SpecularColor;

    half _LightCoefficient;
    
    half4 _RampValues;
    half4 _RampThresholds;
    half _RampBlend;
    
    half _Fresnel;
    half _FresnelCutoff;
    
    float _SpecCutoff;

    struct Input {
      float2 uv_MainTex;
      float2 uv_BumpMap;
      float2 uv_SpecMap;
      float2 uv_GlossMap;
      float3 worldRefl;
      float3 viewDir;
      float3 worldNormal;
      float3 sphericalHarmonic;
      INTERNAL_DATA
    };
    
       
    
    struct MyOutput {
        half3 Albedo;
        half3 Normal;
        half3 Ambient;
        half3 Emission;
        half Specular;
        half Gloss;
        half Alpha;
    };
    
    void vert (inout appdata_full v, out Input o) {
    #if (defined (SHADER_API_D3D11)) || (defined (SHADER_API_D3D11_9X))
 
      o = (Input)0;

    #endif

            // evaluate SH light
            float3 worldN = mul ((float3x3)unity_ObjectToWorld, SCALED_NORMAL);
            o.worldNormal = worldN;
            o.sphericalHarmonic = ShadeSH9 (float4 (worldN, 1.0));
        }
    
    half4 LightingRampLambert (MyOutput s, half3 lightDir, half3 viewDir, half atten) {
      half NdotL = dot (s.Normal, lightDir);
      half diff = (NdotL * 0.5 + 0.5);
      
      // ramp
      half rampScalar = _RampValues.b;
      half mixB = clamp((diff-_RampThresholds.g)/_RampBlend, 0.0, 1.0);
      half mixA = clamp((diff-_RampThresholds.r)/_RampBlend, 0.0, 1.0);
      rampScalar = lerp(rampScalar, _RampValues.g, mixB);
      half3 ramp = lerp(rampScalar, _RampValues.r, mixA);
      
      half4 c;
      
      // spec highlight
      half3 h = normalize (lightDir + viewDir);
      float nh = max (0.0, dot (s.Normal, h));
      float spec = smoothstep(_SpecCutoff, 1.0, pow (nh, s.Gloss * 128.0)) * s.Specular;
      half3 highlight = clamp(spec * atten, 0.0, 1.0) * _SpecAmount;
      
      // ramp lighting
      c.rgb = (s.Albedo * _LightColor0.rgb * ramp * 2  + highlight * _SpecularColor.rgb) * atten + s.Ambient;
      
      c.a = s.Alpha; 
      
      return c;
    }
        
    void surf (Input IN, inout MyOutput o) {
      
      fixed4 gls = lerp(fixed4(0.5), fixed4(0.25), tex2D(_GlossMap, IN.uv_MainTex)); //gloss map
      half4 c = tex2D (_MainTex, IN.uv_MainTex);
      o.Normal = UnpackNormal (tex2D(_BumpMap, IN.uv_BumpMap));     
      //half3 worldNorm = WorldNormalVector(IN, o.Normal);

      // rim coefficients
      half fresnel =_Fresnel * smoothstep(_FresnelCutoff, 1, (1 - min(pow(dot(o.Normal, normalize(IN.viewDir)), 2), 1)));
      half3 rim = fresnel * IN.sphericalHarmonic;
      // rim from cube map
      
      //half3 rim = fresnel * upness * texCUBE(_Cube, worldNorm).rgb;
      //o.Rim = rim;
      //o.Emission = tex2D(_GlowTex, IN.uv_GlowTex).rgb * _GlowStrength;
            
      o.Gloss = gls.r;
      o.Albedo = _Color.rgb * c.rgb; //texCUBE(_Cube, worldNorm).rgb;
      o.Specular = tex2D(_SpecMap, IN.uv_SpecMap).r;
      o.Ambient = (_LightCoefficient * IN.sphericalHarmonic * o.Albedo) + rim;
      o.Alpha = c.a;
    }
    ENDCG
    
    
     // prime the alpha values
  ZWrite On
  ZTest Less
  Cull Off
  AlphaTest Equal 1
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask A

  CGPROGRAM
  #pragma surface alphaPass Lambert noforwardadd alpha

  void alphaPass(Input IN, inout SurfaceOutput o) {
    half4 c = tex2D (_MainTex, IN.uv_MainTex);
    o.Albedo = c.rgb * _Color.rgb;
    o.Alpha = c.a;
  }

  ENDCG

  // CORE PASS - this is the main pass
  ZWrite Off
  ZTest Equal
  Cull Back
  AlphaTest Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGBA

  CGPROGRAM
  #pragma surface surf RampLambert nolightmap noambient noforwardadd vertex:vert 
  #pragma target 3.0

  ENDCG

  // fringe back face pass
  ZWrite Off
  ZTest Less
  Cull Front
  AlphaTest Off
  Blend SrcAlpha OneMinusSrcAlpha
  
  CGPROGRAM
  #pragma surface surf RampLambert nolightmap noambient noforwardadd vertex:vert 
  #pragma target 3.0

  ENDCG

  // fringe alpha pass, just the front details
  ZWrite On
  ZTest Less
  Cull Back
  AlphaTest Off
  Blend SrcAlpha OneMinusSrcAlpha

  CGPROGRAM
  #pragma surface surf RampLambert nolightmap noambient noforwardadd vertex:vert 
  #pragma target 3.0
  ENDCG
  
  }
  FallBack "Transparent/VertexLit"
}
