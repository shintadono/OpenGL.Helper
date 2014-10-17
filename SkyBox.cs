using System;
using OpenGL.Core;

namespace OpenGLHelper
{
	public class SkyBox
	{
		#region SkyBox (Triangle strip)
		static readonly sbyte[] SkyboxVertices=
		{
			// 1. Quad (complete)
			-1, -1, +1, // 0A
			-1, -1, -1, // 1B
			-1, +1, +1, // 2A
			-1, +1, -1, // 3B

			// 2. Quad
			+1, +1, +1, // 4A
			+1, +1, -1, // 5B

			// 3. Quad
			+1, -1, +1, // 6A
			+1, -1, -1, // 7B
			// | Loop 1 to stop rendering
			+1, -1, -1, // 8A=7B
			+1, +1, -1, // 9B=5B
			// | Loop 2 to reposition new start point
			// | 4. Quad (complete)
			+1, +1, -1, // 10A=9B=5B
			-1, +1, -1, // 11B=3B
			+1, -1, -1, // 12A=8A=7B
			-1, -1, -1, // 13B=1B

			// 5. Quad
			+1, -1, +1, // 14A=6A
			-1, -1, +1, // 15B=0A

			// 6. Quad
			+1, +1, +1, // 16A=4A
			-1, +1, +1, // 17B=2A
		};
		#endregion

		#region Shader (inlined)
		const string vertexShaderSource=
			"#version 400\n"+
			"uniform mat4 projectionMatrix;"+
			"uniform mat4 modelViewMatrix;"+
			"in vec3 inPosition;"+
			"out vec3 texCoord;"+
			"void main()"+
			"{"+
			"	/*Rotate our Z-Up co-ordinate system to the (obscure) -Y-Up co-ordinate system of the cubemap textures. (Thanks Microsoft, for Direct3D, not.)*/"+
			"	texCoord=vec3(inPosition.x, -inPosition.z, inPosition.y);"+
			"	gl_Position=projectionMatrix*modelViewMatrix*vec4(inPosition, 1.0);"+
			"}";

		const string fragmentShaderSource=
			"#version 400\n"+
			"uniform samplerCube sampler;"+
			"in vec3 texCoord;"+
			"out vec4 outputColor;"+
			"void main()"+
			"{"+
			"	outputColor=texture(sampler, texCoord);"+
			"}";
		#endregion

		#region Variables (OpenGL object names, shader program and uniform locations)
		uint vao, vbo, texture;
		ShaderProgram program;
		int uniformIndexProjectionMatrix, uniformIndexModelViewMatrix, uniformIndexSampler;
		#endregion

		public SkyBox()
		{
			#region Init shader program and get uniform locations
			program=new ShaderProgram(vertexShaderSource, fragmentShaderSource);
			uniformIndexProjectionMatrix=program.GetUniformLocation("projectionMatrix");
			uniformIndexModelViewMatrix=program.GetUniformLocation( "modelViewMatrix");
			uniformIndexSampler=program.GetUniformLocation("sampler");
			#endregion

			#region Init program constants (uniform that should never change)
			program.UseProgram();

			// Tell shader which texture unit to use
			gl.Uniform1i(uniformIndexSampler, 0);
			#endregion

			#region Make vertex array for the box
			vao=gl.GenVertexArray();
			vbo=gl.GenBuffer();

			gl.BindVertexArray(vao);

			gl.BindBuffer(glBufferTarget.ARRAY_BUFFER, vbo);
			gl.BufferData(glBufferTarget.ARRAY_BUFFER, SkyboxVertices.Length*sizeof(sbyte), SkyboxVertices, glBufferUsage.STATIC_DRAW);
			gl.EnableVertexAttribArray(0); // position AND texture coordinate as vectors
			gl.VertexAttribPointer(0, 3, glVertexAttribType.BYTE, false, 0, 0);
			#endregion

			// Get a name for the texture
			texture=gl.GenTexture();
		}

		public delegate byte[] TextureLoader(string filename, out int width, out int height, out glPixelFormat format);

		public void LoadTextures(string sourceDirectory, TextureLoader textureLoader)
		{
			glPixelFormat textureFormat;
			int textureWidth, textureHeight;

			gl.PixelStorei(glPixelStoreParameter.UNPACK_ALIGNMENT, 1);

			gl.BindTexture(glTextureTarget.TEXTURE_CUBE_MAP, texture);

			// Set up the textures in the (obscure) -Y-Up co-ordinate system of the cubemap textures. (Thanks Microsoft, for Direct3D, not.)
			byte[] textureBits=textureLoader(sourceDirectory+"\\south.png", out textureWidth, out textureHeight, out textureFormat); // back
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_CUBE_MAP_NEGATIVE_Z, 0, glInternalFormat.RGB8, textureWidth, textureHeight, 0, textureFormat, glPixelDataType.UNSIGNED_BYTE, textureBits);
			textureBits=textureLoader(sourceDirectory+"\\north.png", out textureWidth, out textureHeight, out textureFormat); // front
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_CUBE_MAP_POSITIVE_Z, 0, glInternalFormat.RGB8, textureWidth, textureHeight, 0, textureFormat, glPixelDataType.UNSIGNED_BYTE, textureBits);
			textureBits=textureLoader(sourceDirectory+"\\zenith.png", out textureWidth, out textureHeight, out textureFormat); // top; connected with bottom edge to North's top edge
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_CUBE_MAP_NEGATIVE_Y, 0, glInternalFormat.RGB8, textureWidth, textureHeight, 0, textureFormat, glPixelDataType.UNSIGNED_BYTE, textureBits);
			textureBits=textureLoader(sourceDirectory+"\\nadir.png", out textureWidth, out textureHeight, out textureFormat); // bottom; connected with top edge to North's bottom edge
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_CUBE_MAP_POSITIVE_Y, 0, glInternalFormat.RGB8, textureWidth, textureHeight, 0, textureFormat, glPixelDataType.UNSIGNED_BYTE, textureBits);
			textureBits=textureLoader(sourceDirectory+"\\west.png", out textureWidth, out textureHeight, out textureFormat); // left
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_CUBE_MAP_NEGATIVE_X, 0, glInternalFormat.RGB8, textureWidth, textureHeight, 0, textureFormat, glPixelDataType.UNSIGNED_BYTE, textureBits);
			textureBits=textureLoader(sourceDirectory+"\\east.png", out textureWidth, out textureHeight, out textureFormat); // right
			gl.TexImage2D(glTexture2DProxyTarget.TEXTURE_CUBE_MAP_POSITIVE_X, 0, glInternalFormat.RGB8, textureWidth, textureHeight, 0, textureFormat, glPixelDataType.UNSIGNED_BYTE, textureBits);

			gl.TexParameteri(glTextureTarget.TEXTURE_CUBE_MAP, glTextureParameter.TEXTURE_MAG_FILTER, glFilter.LINEAR);
			gl.TexParameteri(glTextureTarget.TEXTURE_CUBE_MAP, glTextureParameter.TEXTURE_MIN_FILTER, glFilter.LINEAR);

			// We use the new 'Seamless Cube Map Filtering'.
			gl.Enable(glCapability.TEXTURE_CUBE_MAP_SEAMLESS);

			// Not need for CLAMP_TO_EDGE anymore with 'Seamless Cube Map Filtering'. For safety reasons and in case someone disables 'Seamless Cube Map Filtering' again let's do it nevertheless.
			gl.TexParameteri(glTextureTarget.TEXTURE_CUBE_MAP, glTextureParameter.TEXTURE_WRAP_R, glTextureWrapMode.CLAMP_TO_EDGE);
			gl.TexParameteri(glTextureTarget.TEXTURE_CUBE_MAP, glTextureParameter.TEXTURE_WRAP_S, glTextureWrapMode.CLAMP_TO_EDGE);
			gl.TexParameteri(glTextureTarget.TEXTURE_CUBE_MAP, glTextureParameter.TEXTURE_WRAP_T, glTextureWrapMode.CLAMP_TO_EDGE);
		}

		public void SetProjectionMatrix(float[] projectionMatrix)
		{
			program.UseProgram();
			gl.UniformMatrix4fv(uniformIndexProjectionMatrix, 1, false, projectionMatrix);
		}

		public void Draw(float[] modelViewMatrix)
		{
			gl.DepthMask(false);

			program.UseProgram();

			// One texture, not sampler
			gl.ActiveTexture(glTextureUnit.TEXTURE0);
			gl.BindTexture(glTextureTarget.TEXTURE_CUBE_MAP, texture);
			gl.BindSampler(0, 0);

			// Set vertex array object for the next draws
			gl.BindVertexArray(vao);

			// Set camera
			gl.UniformMatrix4fv(uniformIndexModelViewMatrix, 1, false, modelViewMatrix);

			gl.DrawArrays(glDrawMode.TRIANGLE_STRIP, 0, 18);

			gl.DepthMask(true);
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
			gl.BindTexture(glTextureTarget.TEXTURE_CUBE_MAP, 0);

			// Unbind all samplers
			gl.BindSampler(0, 0);

			// Delete texture names and sampler names
			gl.DeleteTextures(1, texture);
		}
	}
}
