#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif
float zoom = 1;
Texture2D SpriteTexture;
int width, height;
int xcooInt1, ycooInt1;
int xcooInt2, ycooInt2;
float PI = 3.1415f;
float aspectratio;
int maxiterations;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{

	double xcoo = asdouble((uint)xcooInt1, (uint)xcooInt2);
	double ycoo = asdouble((uint)ycooInt1, (uint)ycooInt2);
	double currenta = (input.Position.x / (float)width) * 2 * aspectratio - 1 * aspectratio;
	double currentb = (input.Position.y / (float)height) * 2 - 1;
	currenta *= (1 / zoom);
	currentb *= (1 / zoom);

	currenta += xcoo;

	currentb += ycoo;

	double a = 0;
	double b = 0;
	int n = 0;
	float factor;
	double aa, bb;
	double asqr = a * a, bsqr = b * b;
	double dif, dif_1, dif_2;
	while (n < maxiterations)
	{
		aa = asqr - bsqr + currenta;
		bb = a * b;
		b = bb + bb + currentb;
		a = aa;
		asqr = a * a;
		bsqr = b * b;
		/*if (n == 1000)
			dif_1 = asqr + bsqr;
		if (n == 1050)
		{
			dif_2 = asqr + bsqr;
			dif = dif_1 - dif_2;
			if (dif*1000000 < 0.00000001f)
			{
				//n = maxiterations;
				//break;
			}
		}*/
		if ((asqr + bsqr) > 4)
			break;
		n++;
	}
	double smoothed = log2(log2(asqr + bsqr) / 2.0f);
	double nn = sqrt(n + 5 - smoothed) * 6 + 1;
	float4 color = float4(0, 0, 0, 1);
	color.r = (sin((nn * PI * 0.5f) / 256.0f) + 1.0f) / 2.0f;
	color.g = (sin((nn * 16.0f * PI * 0.5f) / 256.0f) + 1.0f) / 2.0f;
	color.b = (sin((nn * 256.0f * PI * 0.5f) / 256.0f) + 1.0f) / 2.0f;
	if (n == maxiterations)
		color.rgb = 0;

	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color + color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};