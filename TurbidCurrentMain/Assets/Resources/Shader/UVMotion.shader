// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "MyShader/UI/UVMotion"
{
    Properties
    {
        _MainTex ("Texture Sheet", 2D) = "white" {}
        _Color("Color",color)=(1,1,1,1)
        _HorizontalAmount("Horizontal Num",float)=4
        _VerticalAmount("Vertical Num",float)=4
        _Speed("Play Speed",Range(0,150))=30
    }
    SubShader
    {
        Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        Pass
        {
            Tags{"LightModel"="ForwardBase"}

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
        
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _HorizontalAmount;
			float _VerticalAmount;
			float _Speed;

            struct a2v
            {
                float4 vertex:POSITION;
                float4 texcoord:TEXCOORD0;
            };
            struct v2f
            {
                float4 pos:SV_POSITION;
                float2 uv:TEXCOORD0;
            };
            v2f vert(a2v v)
            {
                v2f o;
                o.pos=UnityObjectToClipPos(v.vertex);
                o.uv=TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag(v2f i):SV_Target
            {
                float2 time=floor(_Time.y*_Speed);
                float row=floor(time/_HorizontalAmount);
                float column=time-row*_HorizontalAmount;
                half2 uv=float2(i.uv.x/_HorizontalAmount,i.uv.y/_VerticalAmount);
                uv.x+=column/_HorizontalAmount;
                uv.y-=row/_VerticalAmount;
                fixed4 c=tex2D(_MainTex,uv);
                c.rgb*=_Color;
                return c;
            }
            ENDCG
            
        }
    }
    FallBack"Transparent/VertexLit"
    
}
