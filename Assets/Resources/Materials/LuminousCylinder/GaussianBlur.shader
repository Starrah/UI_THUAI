Shader "Hidden/GaussianBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CoreTex ("Scene Tex", 2D)="black"{}
        _GlowTex ("Glow Tex", 2D)="white"{}
        _GlowColor ("Glow Color",Color)=(1,1,1,1)
        // _ScreenWidth("Screen width",int)=1000
        // _IterationNumber("Iteration Number",Range(0,20))=5
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name  "Pass Y"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;

            static const float g[]={0.00225755, 0.00244362, 0.00264079, 0.00284932, 0.00306939,
                0.00330118, 0.00354479, 0.0038003 , 0.0040677 , 0.00434697,
                0.00463798, 0.00494056, 0.00525447, 0.00557939, 0.00591493,
                0.00626062, 0.00661592, 0.00698021, 0.00735278, 0.00773286,
                0.00811958, 0.00851202, 0.00890915, 0.00930991, 0.00971313,
                0.01011762, 0.01052211, 0.01092527, 0.01132575, 0.01172213,
                0.012113  , 0.01249688, 0.01287232, 0.01323783, 0.01359197,
                0.01393326, 0.01426029, 0.01457167, 0.01486603, 0.0151421 ,
                0.01539864, 0.01563448, 0.01584856, 0.01603989, 0.01620758,
                0.01635083, 0.01646898, 0.01656147, 0.01662785, 0.0166678 ,
                0.01668114, 0.0166678 , 0.01662785, 0.01656147, 0.01646898,
                0.01635083, 0.01620758, 0.01603989, 0.01584856, 0.01563448,
                0.01539864, 0.0151421 , 0.01486603, 0.01457167, 0.01426029,
                0.01393326, 0.01359197, 0.01323783, 0.01287232, 0.01249688,
                0.012113  , 0.01172213, 0.01132575, 0.01092527, 0.01052211,
                0.01011762, 0.00971313, 0.00930991, 0.00890915, 0.00851202,
                0.00811958, 0.00773286, 0.00735278, 0.00698021, 0.00661592,
                0.00626062, 0.00591493, 0.00557939, 0.00525447, 0.00494056,
                0.00463798, 0.00434697, 0.0040677 , 0.0038003 , 0.00354479,
                0.00330118, 0.00306939, 0.00284932, 0.00264079, 0.00244362,
            0.00225755};

            fixed4 frag (v2f p) : SV_Target{
                float alpha=0;
                float _ScreenWidth=1000;
                int _Radius=50;
                // return tex2D(_MainTex, fixed2(p.uv.x,p.uv.y))+fixed4(0,_ScreenWidth/1000,0,0);
                for(int i=-_Radius;i<=_Radius;i++){
                    alpha+=tex2D(_MainTex, fixed2(p.uv.x,p.uv.y+(float(i))/_ScreenWidth)).r*g[(i+_Radius)];
                }
                // alpha-=tex2D(_MainTex, fixed2(p.uv.x,p.uv.y)).r;
                return fixed4(fixed3(1,1,1)*alpha,1);
            }
            ENDCG
        }
        Pass
        {
            Name  "Pass X"
            Blend one one
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;

            static const float g[]={0.00225755, 0.00244362, 0.00264079, 0.00284932, 0.00306939,
                0.00330118, 0.00354479, 0.0038003 , 0.0040677 , 0.00434697,
                0.00463798, 0.00494056, 0.00525447, 0.00557939, 0.00591493,
                0.00626062, 0.00661592, 0.00698021, 0.00735278, 0.00773286,
                0.00811958, 0.00851202, 0.00890915, 0.00930991, 0.00971313,
                0.01011762, 0.01052211, 0.01092527, 0.01132575, 0.01172213,
                0.012113  , 0.01249688, 0.01287232, 0.01323783, 0.01359197,
                0.01393326, 0.01426029, 0.01457167, 0.01486603, 0.0151421 ,
                0.01539864, 0.01563448, 0.01584856, 0.01603989, 0.01620758,
                0.01635083, 0.01646898, 0.01656147, 0.01662785, 0.0166678 ,
                0.01668114, 0.0166678 , 0.01662785, 0.01656147, 0.01646898,
                0.01635083, 0.01620758, 0.01603989, 0.01584856, 0.01563448,
                0.01539864, 0.0151421 , 0.01486603, 0.01457167, 0.01426029,
                0.01393326, 0.01359197, 0.01323783, 0.01287232, 0.01249688,
                0.012113  , 0.01172213, 0.01132575, 0.01092527, 0.01052211,
                0.01011762, 0.00971313, 0.00930991, 0.00890915, 0.00851202,
                0.00811958, 0.00773286, 0.00735278, 0.00698021, 0.00661592,
                0.00626062, 0.00591493, 0.00557939, 0.00525447, 0.00494056,
                0.00463798, 0.00434697, 0.0040677 , 0.0038003 , 0.00354479,
                0.00330118, 0.00306939, 0.00284932, 0.00264079, 0.00244362,
            0.00225755};

            fixed4 frag (v2f p) : SV_Target{
                float alpha=0;
                float _ScreenWidth=1000;
                int _Radius=50;
                // return tex2D(_MainTex, fixed2(p.uv.x,p.uv.y))+fixed4(0,_ScreenWidth/1000,0,0);
                for(int i=-_Radius;i<=_Radius;i++){
                    alpha+=tex2D(_MainTex, fixed2(p.uv.x+(float(i))/_ScreenWidth,p.uv.y)).r*g[(i+_Radius)];
                }
                // alpha-=tex2D(_MainTex, fixed2(p.uv.x,p.uv.y)).r;
                return fixed4(fixed3(1,1,1)*alpha,1);
            }
            ENDCG
        }
        
        Pass
        {
            Name  "Pass Last"
            // Blend one one
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            sampler2D _CoreTex;
            fixed4 frag (v2f p) : SV_Target{
                // return tex2D(_MainTex,p.uv);
                return fixed4(tex2D(_MainTex,p.uv).rgb-tex2D(_CoreTex,p.uv).rgb,1);
            }
            ENDCG
        }
        pass{
            Name  "Pass Final"
            // Blend one one
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            sampler2D _MainTex;
            sampler2D _GlowTex;
            fixed4 _GlowColor;
            fixed4 frag (v2f p) : SV_Target{
                // return tex2D(_MainTex,p.uv);
                float alpha_glow=_GlowColor.a*tex2D(_GlowTex,p.uv).r;
                return fixed4(_GlowColor.rgb*(alpha_glow)+tex2D(_MainTex,p.uv).rgb*(1-alpha_glow),1);
            }
            ENDCG

        }
    }
}
