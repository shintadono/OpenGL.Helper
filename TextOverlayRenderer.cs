using System;
using System.Collections.Generic;
using System.Drawing;
using OpenGL.Core;

namespace OpenGL.Helper
{
	public class TextOverlayRenderer
	{
		static readonly float[] Identity= { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };

		#region Shader (inlined)
		const string vertexShaderSource=
			"#version 330\n"+
			"uniform mat4 projectionMatrix;"+
			"uniform mat4 modelViewMatrix;"+
			"layout (location=0) in vec2 inPosition;"+
			"layout (location=1) in vec2 inTexCoord;"+
			"out vec2 texCoord;"+
			"void main()"+
			"{"+
			"	gl_Position=projectionMatrix*modelViewMatrix*vec4(inPosition, 0.0, 1.0);"+
			"	texCoord=inTexCoord;"+
			"}";

		const string fragmentShaderSource=
			"#version 330\n"+
			"uniform sampler2DRect sampler;"+
			"uniform vec4 color;"+
			"in vec2 texCoord;"+
			"out vec4 outputColor;"+
			"void main()"+
			"{"+
			"	outputColor=vec4(color.rgb, color.a*texture(sampler, texCoord).r);"+
			"}";
		#endregion

		uint vao, vbo, texture;
		ShaderProgram program;
		int uniformIndexProjectionMatrix, uniformIndexModelViewMatrix, uniformIndexSampler, uniformIndexColor;

		OpenGLFont oglFont;

		public TextOverlayRenderer(OpenGLFont font=null, bool releaseFontBits=true)
		{
			#region Init shader program and get uniform locations
			program=new ShaderProgram(vertexShaderSource, fragmentShaderSource);
			uniformIndexProjectionMatrix=program.GetUniformLocation("projectionMatrix");
			uniformIndexModelViewMatrix=program.GetUniformLocation("modelViewMatrix");
			uniformIndexSampler=program.GetUniformLocation("sampler");
			uniformIndexColor=program.GetUniformLocation("color");
			#endregion

			#region Init program constants (uniform that should never change)
			program.UseProgram();

			// Tell shader which texture unit to use
			gl.Uniform1i(uniformIndexSampler, 0);
			#endregion

			// Get a name for the array, buffer and texture
			vao=gl.GenVertexArray();
			vbo=gl.GenBuffer();
			texture=gl.GenTexture();

			if(font!=null) ChangeFont(font, releaseFontBits);
			else ChangeFont(new OpenGLFont("Segoe UI", 9, FontStyle.Regular, new Tuple<ushort, ushort>(32, 126)));
		}

		public void ChangeFont(OpenGLFont oglFont, bool releaseFontBits=true)
		{
			this.oglFont=oglFont;

			gl.PixelStorei(glPixelStoreParameter.UNPACK_ALIGNMENT, 1);

			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, texture);
			
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_RECTANGLE, 0, glInternalFormat.RED, oglFont.Width, oglFont.Height, 0, glPixelFormat.RED, glPixelDataType.UNSIGNED_BYTE, oglFont.bits);

			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_WRAP_S, glTextureWrapMode.CLAMP_TO_EDGE); // we need this line for rectangle textures to work
			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_WRAP_T, glTextureWrapMode.CLAMP_TO_EDGE); // we need this line for rectangle textures to work
			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_MAG_FILTER, glFilter.NEAREST);
			gl.TexParameteri(glTextureTarget.TEXTURE_RECTANGLE, glTextureParameter.TEXTURE_MIN_FILTER, glFilter.NEAREST);

			if(releaseFontBits) oglFont.bits=null; // free memory
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

		public void DrawText(string msg, float[] modelViewMatrix, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			program.UseProgram();

			// One texture, not sampler
			gl.ActiveTexture(glTextureUnit.TEXTURE0);
			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, texture);
			gl.BindSampler(0, 0);

			gl.UniformMatrix4fv(uniformIndexModelViewMatrix, 1, false, modelViewMatrix);

			List<int> vboArray=oglFont.BuildDrawBuffer(msg, anchor);

			gl.BindVertexArray(vao);

			gl.BindBuffer(glBufferTarget.ARRAY_BUFFER, vbo);
			gl.BufferData(glBufferTarget.ARRAY_BUFFER, vboArray.Count*sizeof(int), vboArray.ToArray(), glBufferUsage.DYNAMIC_DRAW);
			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 2, glVertexAttribType.INT, false, sizeof(int)*4, 0);
			gl.EnableVertexAttribArray(1);
			gl.VertexAttribPointer(1, 2, glVertexAttribType.INT, false, sizeof(int)*4, sizeof(int)*2);

			gl.DrawArrays(glDrawMode.TRIANGLES, 0, vboArray.Count/4); // each element(Point) has x, y, u and v
		}

		public void DrawText(string msg, int posX, int posY, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			program.UseProgram();

			// One texture, not sampler
			gl.ActiveTexture(glTextureUnit.TEXTURE0);
			gl.BindTexture(glTextureTarget.TEXTURE_RECTANGLE, texture);
			gl.BindSampler(0, 0);

			gl.UniformMatrix4fv(uniformIndexModelViewMatrix, 1, false, Identity);

			List<int> vboArray=oglFont.BuildDrawBuffer(msg, posX, posY, anchor);

			gl.BindVertexArray(vao);

			gl.BindBuffer(glBufferTarget.ARRAY_BUFFER, vbo);
			gl.BufferData(glBufferTarget.ARRAY_BUFFER, vboArray.Count*sizeof(int), vboArray.ToArray(), glBufferUsage.DYNAMIC_DRAW);
			gl.EnableVertexAttribArray(0);
			gl.VertexAttribPointer(0, 2, glVertexAttribType.INT, false, sizeof(int)*4, 0);
			gl.EnableVertexAttribArray(1);
			gl.VertexAttribPointer(1, 2, glVertexAttribType.INT, false, sizeof(int)*4, sizeof(int)*2);

			gl.DrawArrays(glDrawMode.TRIANGLES, 0, vboArray.Count/4); // each element(Point) has x, y, u and v
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
	}
}
