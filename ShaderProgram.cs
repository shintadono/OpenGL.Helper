using System;
using OpenGL.Core;

namespace OpenGL.Helper
{
	public class ShaderProgram
	{
		uint program, shaderVertex, shaderFragment;

		public ShaderProgram(string vertexShaderSource, string fragmentShaderSource)
		{
			// Load shaders and create shader program
			shaderVertex=gl.CreateShader(glShaderType.VERTEX_SHADER);
			shaderFragment=gl.CreateShader(glShaderType.FRAGMENT_SHADER);

			try
			{
				int iCompilationStatus;
				gl.ShaderSource(shaderVertex, vertexShaderSource);
				gl.CompileShader(shaderVertex);
				gl.GetShaderi(shaderVertex, glShaderParameter.COMPILE_STATUS, out iCompilationStatus);
				if(iCompilationStatus==0)
					throw new Exception("Shader compilation error. (Vertex Shader)");

				gl.ShaderSource(shaderFragment, fragmentShaderSource);
				gl.CompileShader(shaderFragment);
				gl.GetShaderi(shaderFragment, glShaderParameter.COMPILE_STATUS, out iCompilationStatus);
				if(iCompilationStatus==0)
					throw new Exception("Shader compilation error. (Fragment Shader)");
			}
			catch
			{
				gl.DeleteShader(shaderVertex);
				gl.DeleteShader(shaderFragment);
				throw;
			}

			program=gl.CreateProgram();
			gl.AttachShader(program, shaderVertex);
			gl.AttachShader(program, shaderFragment);

			try
			{
				gl.LinkProgram(program);

				int iLinkStatus;
				gl.GetProgrami(program, glProgramParameter.LINK_STATUS, out iLinkStatus);
				if(iLinkStatus==0)
					throw new Exception("Shader program linkage error. (Shader Program)");
			}
			catch
			{
				gl.DetachShader(program, shaderVertex);
				gl.DetachShader(program, shaderVertex);
				gl.DeleteShader(shaderVertex);
				gl.DeleteShader(shaderFragment);
				gl.DeleteProgram(program);
				throw;
			}
		}

		public void UseProgram()
		{
			gl.UseProgram(program);
		}

		public int GetUniformLocation(string name)
		{
			return gl.GetUniformLocation(program, name);
		}

		public void Delete()
		{
			gl.UseProgram(0);
			gl.DetachShader(program, shaderVertex);
			gl.DetachShader(program, shaderFragment);
			gl.DeleteShader(shaderVertex);
			gl.DeleteShader(shaderFragment);
			gl.DeleteProgram(program);
		}
	}
}
