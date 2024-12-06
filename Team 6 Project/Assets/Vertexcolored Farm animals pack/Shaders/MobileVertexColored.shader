 Shader "Custom/Mobile (Vertex colored)" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
         LOD 200
         
         CGPROGRAM
         #pragma surface surf Standard vertex:vert fullforwardshadows
         
         struct Input {
             float2 uv_MainTex;
             float3 vertexColor;
         };
         
         struct v2f {
           float4 pos : SV_POSITION;
           fixed4 color : COLOR;
         };
 
         void vert (inout appdata_full v, out Input o)
         {
             UNITY_INITIALIZE_OUTPUT(Input,o);
             o.vertexColor = v.color;
         }
 
         sampler2D _MainTex;
 
         half _Glossiness;
         half _Metallic;
         fixed4 _Color;
 
         void surf (Input IN, inout SurfaceOutputStandard o) 
         {
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb * IN.vertexColor;
             o.Alpha = c.a;
         }
         ENDCG
     } 
     FallBack "Diffuse"
 }