using System.Collections.Generic;
using OpenGL.Core;

namespace OpenGL.Helper
{
	public class TwentyTwoSegmentDisplayRenderer
	{
		#region Font texture 36x52 pixel
		static readonly byte[] fontTexture= {
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 22, 22, 22, 0, 0, 0, 0, 0,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 22, 22, 22, 0, 0, 0, 0,
				0, 0, 0, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 0, 0, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 0, 0, 22, 22, 22, 0, 0, 0, 0,
				0, 0, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 16, 0, 0, 22, 0, 17, 17, 17, 0,
				0, 0, 10, 0, 0, 15, 15, 15, 15, 15, 15, 15, 15, 15, 0, 12, 0, 16, 16, 16, 16, 16, 16, 16, 16, 16, 0, 0, 14, 0, 0, 17, 17, 17, 17, 17,
				0, 10, 10, 0, 11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 13, 0, 14, 14, 0, 17, 17, 17, 17, 17,
				0, 10, 10, 0, 11, 11, 11, 0, 0, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 0, 0, 13, 13, 13, 0, 14, 14, 0, 17, 17, 17, 17, 17,
				0, 10, 10, 10, 0, 11, 11, 0, 0, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 0, 0, 13, 13, 0, 14, 14, 14, 0, 0, 17, 17, 17, 0,
				0, 10, 10, 10, 0, 11, 11, 11, 0, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 0, 13, 13, 13, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 11, 11, 11, 0, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 0, 13, 13, 13, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 11, 11, 11, 11, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 13, 13, 13, 13, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 11, 11, 11, 0, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 0, 13, 13, 13, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 11, 11, 11, 11, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 13, 13, 13, 13, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 11, 11, 11, 0, 0, 0, 0, 12, 12, 12, 0, 0, 0, 0, 13, 13, 13, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 11, 11, 11, 11, 0, 0, 0, 12, 12, 12, 0, 0, 0, 13, 13, 13, 13, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 11, 11, 11, 0, 0, 0, 12, 12, 12, 0, 0, 0, 13, 13, 13, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 11, 11, 11, 11, 0, 0, 12, 12, 12, 0, 0, 13, 13, 13, 13, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 11, 11, 11, 0, 0, 12, 12, 12, 0, 0, 13, 13, 13, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 11, 11, 11, 11, 0, 12, 12, 12, 0, 13, 13, 13, 13, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 0, 11, 11, 11, 0, 12, 12, 12, 0, 13, 13, 13, 0, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 0, 11, 11, 11, 0, 12, 12, 12, 0, 13, 13, 13, 0, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 0, 0, 11, 11, 11, 12, 12, 12, 13, 13, 13, 0, 0, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 0, 0, 11, 11, 11, 12, 12, 12, 13, 13, 13, 0, 0, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 0, 0, 0, 0,
				0, 10, 10, 10, 0, 0, 0, 0, 0, 0, 0, 0, 0, 11, 11, 12, 13, 13, 0, 0, 0, 0, 0, 0, 0, 0, 0, 14, 14, 14, 0, 0, 18, 18, 18, 0,
				0, 0, 10, 0, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 11, 12, 13, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 0, 14, 0, 0, 18, 18, 18, 18, 18,
				0, 0, 0, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 0, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 0, 0, 0, 18, 18, 18, 18, 18,
				0, 0, 3, 0, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 4, 5, 6, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 0, 7, 0, 0, 18, 18, 18, 18, 18,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 5, 6, 6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 18, 18, 18, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 5, 5, 5, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 5, 5, 5, 6, 6, 6, 0, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 4, 4, 4, 0, 5, 5, 5, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 0, 4, 4, 4, 0, 5, 5, 5, 0, 6, 6, 6, 0, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 4, 4, 4, 4, 0, 5, 5, 5, 0, 6, 6, 6, 6, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 0, 4, 4, 4, 0, 0, 5, 5, 5, 0, 0, 6, 6, 6, 0, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 4, 4, 4, 4, 0, 0, 5, 5, 5, 0, 0, 6, 6, 6, 6, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 0, 4, 4, 4, 0, 0, 0, 5, 5, 5, 0, 0, 0, 6, 6, 6, 0, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 4, 4, 4, 4, 0, 0, 0, 5, 5, 5, 0, 0, 0, 6, 6, 6, 6, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 0, 4, 4, 4, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 6, 6, 6, 0, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 4, 4, 4, 4, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 6, 6, 6, 6, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 0, 4, 4, 4, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 6, 6, 6, 0, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 4, 4, 4, 4, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 6, 6, 6, 6, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 4, 4, 4, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 4, 4, 4, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 3, 0, 4, 4, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 6, 6, 0, 7, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 0, 4, 4, 4, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 6, 6, 6, 0, 7, 7, 0, 0, 0, 0, 0, 0,
				0, 3, 3, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5, 5, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 7, 7, 0, 19, 21, 21, 20, 0,
				0, 0, 3, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 5, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 7, 0, 19, 19, 21, 21, 20, 20,
				0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 19, 19, 0, 0, 20, 20,
				0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0, 19, 19, 0, 0, 20, 20,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 19, 19, 21, 21, 20, 20,
				0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 19, 21, 21, 20, 0
			};
		#endregion

		public readonly int FontWidth=36;
		public readonly int FontHeight=52;

		#region Bitpatterns for numbers letters and some special stuff
		static readonly int[] lookUpTable= { // 0-127 (do we need more? => if so extend)
				0, 0xF66F, 0x1C0000, 0xEBD7, 0x10, 0x20, 0x100, 0x1000, 0x800, 0x400, 0x80, 0x08, 0x810, 0x420, 0x180, 0x1008,
				0x60C, 0x3060, 0xE257, 0xE267, 0xE347, 0xF247, 0xEA47, 0xE647, 0xE2C7, 0xE24F, 0xEA57, 0xE667, 0xE3C7, 0xF24F, 0xD400, 0x2B,
				0, 0x83B, 0xC0000, 0xA347, 0xE997, 0xADB5, 0xF432, 0x40000, 0x1020, 0x408, 0x15A8, 0x990, 0x210000, 0x180, 0x10000, 0x420,
				0xE667, 0x2060, 0xC3C3, 0xE1C3, 0x21C4, 0xD087, 0xE387, 0x2043, 0xE3C7, 0xE1C7, 0x30000, 0x230000, 0x1020, 0xC180, 0x408, 0x943,
				0x7347, 0x23C7, 0xE953, 0xC207, 0xE853, 0xC387, 0x387, 0xE307, 0x23C4, 0xC813, 0xE240, 0x12A4, 0xC204, 0x226C, 0x324C, 0xE247,
				0x3C7, 0xF247, 0x13C7, 0xE187, 0x813, 0xE244, 0x624, 0x3644, 0x1428, 0x9C4, 0xC423, 0x8812, 0x1008, 0x4811, 0xC, 0xC000,
				0x8, 0xCA80, 0x4A84, 0x4280, 0x4A90, 0x4680, 0x992, 0x4D95, 0xA84, 0x800, 0x4A10, 0x1910, 0x4204, 0x2B80, 0xA80, 0x4A80,
				0x295, 0x895, 0x280, 0x9100, 0x4284, 0x4A00, 0x600, 0xEA00, 0xC30, 0xB000, 0x4480, 0x8892, 0x810, 0x4911, 0x155, 0xFFFF
			};
		#endregion

		static readonly float[] Identity= { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };

		#region Shader (inlined)
		const string vertexShaderSource=
			"#version 330\n"+
			"uniform mat4 projectionMatrix;"+
			"uniform mat4 modelViewMatrix;"+
			"layout (location=0) in vec2 inPosition;"+
			"layout (location=1) in int inBitmask;"+
			"out vec2 texCoord;"+
			"flat out int bitmask;"+
			"void main()"+
			"{"+
			"	gl_Position=projectionMatrix*modelViewMatrix*vec4(inPosition, 0.0, 1.0);"+
			"	bitmask=inBitmask;"+
			"	int v=gl_VertexID%6;"+
			"	if(v==0) texCoord=vec2(0, 0);"+
			"	else if(v==1||v==4) texCoord=vec2(36, 0);"+
			"	else if(v==2||v==3) texCoord=vec2(0, 52);"+
			"	else if(v==5) texCoord=vec2(36, 52);"+
			"}";

		const string fragmentShaderSource=
			"#version 330\n"+
			"uniform usampler2DRect sampler;"+
			"uniform vec4 color;"+
			"uniform vec4 offColor;"+
			"in vec2 texCoord;"+
			"flat in int bitmask;"+
			"out vec4 outputColor;"+
			"void main()"+
			"{"+
			"	int r=int(texture(sampler, texCoord).r)-1;"+
			"	if(r<0) outputColor=vec4(0, 0, 0, 0);"+
			"	else if((bitmask&(1<<r))!=0) outputColor=color;"+
			"	else outputColor=offColor;"+
			"}";
		#endregion

		uint vao, vbo, texture;
		ShaderProgram program;
		int uniformIndexProjectionMatrix, uniformIndexModelViewMatrix, uniformIndexSampler, uniformIndexColor, uniformIndexOffColor;

		public TwentyTwoSegmentDisplayRenderer()
		{
			#region Init shader program and get uniform locations
			program=new ShaderProgram(vertexShaderSource, fragmentShaderSource);
			uniformIndexProjectionMatrix=program.GetUniformLocation("projectionMatrix");
			uniformIndexModelViewMatrix=program.GetUniformLocation("modelViewMatrix");
			uniformIndexSampler=program.GetUniformLocation("sampler");
			uniformIndexColor=program.GetUniformLocation("color");
			uniformIndexOffColor=program.GetUniformLocation("offColor");
			#endregion

			#region Init program constants (uniform that should never change)
			program.UseProgram();

			// Tell shader which texture unit to use
			gl.Uniform1i(uniformIndexSampler, 0);
			gl.Uniform4f(uniformIndexColor, 1f, 0.3f, 0.1f, 0.8f);
			gl.Uniform4f(uniformIndexOffColor, 1f, 0.3f, 0.1f, 0.1f);
			#endregion

			// Get a name for the array, buffer and texture
			vao=gl.GenVertexArray();
			vbo=gl.GenBuffer();
			texture=gl.GenTexture();

			gl.PixelStorei(glPixelStoreParameter.UNPACK_ALIGNMENT, 1);

			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, texture);

			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_RECTANGLE, 0, glInternalFormat.R8UI, FontWidth, FontHeight, 0, glPixelFormat.RED_INTEGER, glPixelDataType.UNSIGNED_BYTE, fontTexture);

			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_WRAP_S, glTextureWrapMode.CLAMP_TO_EDGE); // we need this line for rectangle textures to work
			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_WRAP_T, glTextureWrapMode.CLAMP_TO_EDGE); // we need this line for rectangle textures to work
			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_MAG_FILTER, glFilter.NEAREST);
			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_MIN_FILTER, glFilter.NEAREST);
		}

		public void SetProjectionMatrix(float[] projectionMatrix)
		{
			program.UseProgram();
			gl.UniformMatrix4fv(uniformIndexProjectionMatrix, 1, false, projectionMatrix);
		}

		public void SetColor(float red, float green, float blue, float alpha)
		{
			program.UseProgram();
			gl.Uniform4f(uniformIndexColor, red, green, blue, alpha);
		}

		public void SetOffColor(float red, float green, float blue, float alpha)
		{
			program.UseProgram();
			gl.Uniform4f(uniformIndexOffColor, red, green, blue, alpha);
		}

		public void SetColor(float red, float green, float blue, float alpha, float alphaOff)
		{
			program.UseProgram();
			gl.Uniform4f(uniformIndexColor, red, green, blue, alpha);
			gl.Uniform4f(uniformIndexOffColor, red, green, blue, alphaOff);
		}

		public void DrawText(string msg, float[] modelViewMatrix, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			program.UseProgram();

			// One texture, not sampler
			gl.ActiveTexture(glTextureUnit.TEXTURE0);
			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, texture);
			gl.BindSampler(0, 0);

			gl.UniformMatrix4fv(uniformIndexModelViewMatrix, 1, false, modelViewMatrix);

			int length; // dummy
			List<int> vboArray=BuildDrawBuffer(msg, 0, 0, out length, anchor);

			gl.BindVertexArray(vao);

			gl.BindBuffer(glBufferTarget.ARRAY_BUFFER, vbo);
			gl.BufferData(glBufferTarget.ARRAY_BUFFER, vboArray.Count*sizeof(int), vboArray.ToArray(), glBufferUsage.DYNAMIC_DRAW);
			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 2, glVertexAttribType.INT, false, sizeof(int)*3, 0);
			gl.EnableVertexAttribArray(1);
			gl.VertexAttribIPointer(1, 1, glVertexAttribType.INT, sizeof(int)*3, sizeof(int)*2);

			gl.DrawArrays(glDrawMode.TRIANGLES, 0, vboArray.Count/3); // each element(Point) has x, y and bitmask
		}

		public void DrawText(string msg, int posX, int posY, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			program.UseProgram();

			// One texture, not sampler
			gl.ActiveTexture(glTextureUnit.TEXTURE0);
			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, texture);
			gl.BindSampler(0, 0);

			gl.UniformMatrix4fv(uniformIndexModelViewMatrix, 1, false, Identity);

			int length; // dummy
			List<int> vboArray=BuildDrawBuffer(msg, posX, posY, out length, anchor);

			gl.BindVertexArray(vao);

			gl.BindBuffer(glBufferTarget.ARRAY_BUFFER, vbo);
			gl.BufferData(glBufferTarget.ARRAY_BUFFER, vboArray.Count*sizeof(int), vboArray.ToArray(), glBufferUsage.DYNAMIC_DRAW);
			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 2, glVertexAttribType.INT, false, sizeof(int)*3, 0);
			gl.EnableVertexAttribArray(1);
			gl.VertexAttribIPointer(1, 1, glVertexAttribType.INT, sizeof(int)*3, sizeof(int)*2);

			gl.DrawArrays(glDrawMode.TRIANGLES, 0, vboArray.Count/3); // each element(Point) has x, y and bitmask
		}

		public void Delete()
		{
			// Uninit and delete shader program
			program.Delete();

			// Unbind current vertex array
			gl.BindVertexArray(0);

			// Delete vertex array names and vertex buffer names
			gl.DeleteVertexArrays(1, vao);
			gl.DeleteBuffers(1, vbo);

			gl.ActiveTexture(glTextureUnit.TEXTURE0);

			// Unbind all textures
			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, 0);

			// Unbind all samplers
			gl.BindSampler(0, 0);

			// Delete texture names and sampler names
			gl.DeleteTextures(1, texture);
		}

		List<int> BuildDrawBuffer(string text, int posX, int posY, out int length, AnchorPlacement anchor)
		{
			int horizontalPlacement=(int)anchor&0x03;
			int veticalPlacement=((int)anchor>>4)&0x03;

			switch(veticalPlacement)
			{
				case 1: posY-=FontHeight/2; break; // center
				case 2: posY-=FontHeight; break; // right
			}

			if(horizontalPlacement==1||horizontalPlacement==2)
			{
				int len=0;
				for(int i=0; i<text.Length; i++, len++)
				{
					int ch=text[i];
					if(ch==','||ch=='.'||ch==';'||ch==':'||ch=='\''||ch=='"'||ch=='°') continue;
					else if(text.Length>i+1)
					{
						int peek=text[i+1];
						if(peek==','||peek=='.'||peek==';'||peek==':'||peek=='\''||peek=='"'||peek=='°') i++;
					}
				}

				len*=FontWidth;

				switch(horizontalPlacement)
				{
					case 1: posX-=len/2; break; // center
					case 2: posX-=len; break; // top
				}
			}

			List<int> ret=new List<int>();

			length=0;
			for(int i=0; i<text.Length; i++)
			{
				int ch=text[i];
				int add=0;
				if(ch==','||ch=='.'||ch==';'||ch==':') { add=ch; ch=' '; }
				else if(ch=='\''||ch=='"'||ch=='°') { add=ch; ch='0'; }
				else if(text.Length>i+1)
				{
					int peek=text[i+1];
					if(peek==','||peek=='.'||peek==';'||peek==':'||peek=='\''||peek=='"'||peek=='°')
					{
						add=peek;
						i++;
					}
				}

				if(ch=='ß') ch='#';
				else if(ch=='#'||ch>127) ch=1;
				if(add=='°') add=2;

				int entry=lookUpTable[ch]|lookUpTable[add];

				int left=posX;
				int right=left+FontWidth;
				int bottom=posY;
				int top=posY+FontHeight;

				#region Triangle first half
				ret.Add(left);
				ret.Add(bottom);

				ret.Add(entry);

				ret.Add(right);
				ret.Add(bottom);

				ret.Add(entry);

				ret.Add(left);
				ret.Add(top);

				ret.Add(entry);
				#endregion

				#region Triangle second half
				ret.Add(left);
				ret.Add(top);

				ret.Add(entry);

				ret.Add(right);
				ret.Add(bottom);

				ret.Add(entry);

				ret.Add(right);
				ret.Add(top);

				ret.Add(entry);
				#endregion

				posX+=FontWidth;
				length+=FontWidth;
			}

			return ret;
		}
	}
}
