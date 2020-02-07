Shader "Custom/CrossScan"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Phase ("InitialPhase", Range(0,360)) = 0
        _Blocked ("Blocked Position",int)=0 
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IngnoreProjector" = "True" "RenderType" = "Transparent" "ActiveDistrict" = "True"}
        Cull off
        LOD 200
        Pass{
            Tags { "LightMode" = "ForwardBase" }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Lighting.cginc"

            fixed4 _Color;
            float _Phase;
            struct a2v {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 position : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };
            float sigmoid(float x) {
                return 1 / (1 + exp(-20 * x));
            }

            float f(float x) {
                return sigmoid(x) * sigmoid(1 / 3 - x);
            }

            float p2a(float x){
                float _x=frac(x);
                return f(_x)+f(_x-1);
            }


            v2f vert(a2v v)
            {
                v2f f;
                f.position = UnityObjectToClipPos(v.vertex);

                //计算世界空间下的法线
                f.worldNormal = UnityObjectToWorldNormal(v.normal);

                //计算世界空间下的顶点
                f.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                //计算变换后的纹理坐标
                f.uv = v.texcoord;//TRANSFORM_TEX(v.texcoord, _MainTex);

                return f;
            }
            int _Blocked;
            static const float PI=3.1415926535898;
            static int blocked[5][5]={-1,-1,8,-1,-1,-1,-1,4,-1,-1,2,3,0,1,5,-1,-1,2,-1,-1,-1,-1,6,-1,-1};
            fixed4 frag(v2f i) : SV_Target
            {
                if(1<< (blocked[int(i.uv.x*5)][(int)(i.uv.y*5)]-1) & _Blocked)
                return fixed4(0,0,0,0);
                if( (blocked[int(i.uv.x*5)][(int)(i.uv.y*5)])==-1)
                return fixed4(0,0,0,0);

                //归一
                fixed3 worldNormal = normalize(i.worldNormal);
                fixed3 worldLightDir = normalize(UnityWorldSpaceLightDir(i.worldPos));
                // return fixed4(1,1,1,gaussian(i.uv.x - .5));
                //纹理颜色
                ///*tex2D(_Tex1,i.uv)*/fixed4(_Color.rgb,1);
                int phase=(_Phase+_Time.y*360/3)%360; 
                float x=i.uv.x-0.5;
                float y=i.uv.y-0.5;
                float theta=(abs(x)+abs(y))/0.5*360;

                float alpha=(1-((phase-theta)/360) )%1;
                // return fixed4(alpha,alpha,alpha,1);
                // alpha=pow(alpha,2);
                fixed4 textColor = _Color;
                //反射颜色
                fixed3 albedo = textColor.rgb;//* _Color.rgb;

                //环境光
                fixed3 ambient = (1-(1-UNITY_LIGHTMODEL_AMBIENT.xyz )/2)* albedo;

                //漫反射
                fixed3 diffuse = _LightColor0.rgb * albedo * max(0, dot(worldNormal, worldLightDir));

                return fixed4(ambient + diffuse, textColor.a*alpha);
            }

            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}
