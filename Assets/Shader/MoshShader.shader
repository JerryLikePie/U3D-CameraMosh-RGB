Shader "Unlit/MoshShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrevFrame ("", 2D) = ""{}
    }
    
    SubShader
    {
        GrabPass {
            //############### Me Applying buffer ###############
            "_PrevFrame"
            // using this grab pass it grabs the frame into a buffer.
        }
        Pass
        {
            //############### Default code when creating a shader ###############
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            // one pseudo random function I modified from:
            // https://stackoverflow.com/questions/4200224/random-noise-functions-for-glsl
            // either I have to pass in from the CPU side or use this kind of stuff
            // shaders don't have random(), returns between 0.0 and 1.0
            float random(float x, float y)
            {
                return frac(sin(dot(float2(x, y), float2(12.9898f, 78.233f))) * 43758.5453123f);
            }
            
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
            
            //############### Me Applying mosh ###############
            sampler2D _CameraMotionVectorsTexture;
            // read from the buffer
            sampler2D _PrevFrame;
            // also the global var from Datamosh.cs
            float moshAmount;
            float rgbAmount;
            int isRGB;
            fixed4 outColor;
            float4 screen;
            float4 buffer;
 
            fixed4 frag(v2f i) : SV_Target
            {
                
                // first, get the motion from the motion vector that was passed in.
                screen = tex2D(_MainTex, i.uv);
                // round the coord for moshing using the screen's size.
                // so that pixelated effects can be shown.
                float2 roundedCoord = round(i.uv * (_ScreenParams.xy / 50)) / (_ScreenParams.xy / 50);
                float4 motionVector = tex2D(_CameraMotionVectorsTexture, roundedCoord);
                // generate a random number for noise
                float randNum = random(_Time.x,roundedCoord.x+roundedCoord.y*_ScreenParams.x);
                // add the noise in
                motionVector = max(abs(motionVector) - round(randNum / 1.4f), 0) * sign(motionVector);
                // round the motion vector to full pixels.
                motionVector.x = round(motionVector.x * _ScreenParams.x) / _ScreenParams.x;
                motionVector.y = round(motionVector.y * _ScreenParams.y) / _ScreenParams.y;
                // then, directly add the motion to the uv position
                // so now, each pixel will be "dragging along the direction of motion"
                // the grabbed texture is upside down compared to mainTex
                float2 newFragCoord = float2(i.uv.x - motionVector.x, 1.0f - i.uv.y + motionVector.y);
                float2 newFragCoordMain = float2(i.uv.x - motionVector.x, i.uv.y - motionVector.y);
                
                buffer = tex2D(_PrevFrame, newFragCoord);
                // now, transition between the screen and the buffer
                // this will drag the buffer along with the new updates on the screen
                moshAmount = clamp(moshAmount, 0.0f, 1.0f);
                //############### Me Applying RGB split ###############
                if (length(motionVector) < isRGB)
                {
                    buffer = tex2D(_MainTex, newFragCoordMain);
                }
                outColor = (1.0f - moshAmount) * screen + moshAmount * buffer;
                if (isRGB == 1)
                {
                    // technically I can get another global variable passed in as the "offset"
                    // but hardcoded 0.007 works fine
                    // plus, this isn't procedual modeling.
                    float2 offset = float2(rgbAmount * (0.5f - i.uv.x), rgbAmount * (0.5f - i.uv.y));
                    float4 shiftedColorR = (1.0f - moshAmount) * tex2D(_MainTex, i.uv + offset)
                                            + moshAmount * tex2D(_MainTex, newFragCoordMain + offset);
                    float4 shiftedColorB = (1.0f - moshAmount) * tex2D(_MainTex, i.uv + offset)
                                            + moshAmount * tex2D(_MainTex, newFragCoordMain + offset);
                    outColor = float4(shiftedColorR.r, outColor.g, shiftedColorB.b, outColor.a);
                }
                // render this pixel
                return outColor;
            }
            ENDCG
        }
        
    }
}
