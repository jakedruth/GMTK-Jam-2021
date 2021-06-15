Shader "Custom/PostProcess" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
		_FillAmount("Fill Amount", float) = 0
		_ScreenWidth ("Screen Width", Float) = 0
        _ScreenHeight ("Screen Height",Float) = 0  
	}

		SubShader{
			Pass {
				CGPROGRAM
				#pragma vertex vert_img
				#pragma fragment frag
				#include "UnityCG.cginc" // required for v2f_img

				// Properties
				sampler2D _MainTex;
				float _FillAmount;
				float _ScreenWidth;
	            float _ScreenHeight;

				float4 frag(v2f_img input) : COLOR {
					// sample texture for color

					float d = (_ScreenWidth - _ScreenHeight)/2;

					half u = (input.uv.x+input.pos.x - d)/_ScreenHeight;
					half v = (input.uv.y+input.pos.y)/_ScreenHeight;

					float distFromCenter = distance(float2(u, v), float2(0.5, 0.5));

					float mask = 0;
					if (distFromCenter < _FillAmount)
						mask = 1;

					float4 frag = float4(mask, mask, mask, 1.0);
					float4 screen = tex2D(_MainTex, input.uv);

					return frag * screen;
				}


				ENDCG
			  }
		}
}