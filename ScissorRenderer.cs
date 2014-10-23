﻿using System.Text;
using OpenGL.Core;

namespace OpenGL.Helper
{
	public static class ScissorRenderer
	{
		#region Font 256 chars 6x8 pixel
		static readonly byte[] font256x6x8= {
				0, 0, 0, 0, 0, 0, 0, 62, 69, 81, 69, 62, 0, 62, 107, 111, 107, 62, 0, 28, 62, 124, 62, 28,
				0, 24, 60, 126, 60, 24, 0, 48, 54, 127, 54, 48, 0, 24, 92, 126, 92, 24, 0, 0, 24, 24, 0, 0,
				126, 126, 102, 102, 126, 126, 0, 24, 36, 36, 24, 0, 126, 102, 90, 90, 102, 126, 0, 48, 72, 74, 54, 14,
				0, 6, 41, 121, 41, 6, 0, 48, 112, 63, 3, 6, 0, 96, 126, 10, 53, 63, 0, 42, 28, 54, 28, 42,
				0, 0, 127, 62, 28, 8, 0, 8, 28, 62, 127, 0, 0, 20, 54, 127, 54, 20, 0, 0, 95, 0, 95, 0,
				0, 6, 73, 127, 1, 127, 0, 34, 77, 85, 89, 34, 0, 96, 96, 96, 96, 0, 0, 148, 182, 255, 182, 148,
				0, 4, 6, 127, 6, 4, 0, 16, 48, 127, 48, 16, 0, 8, 8, 62, 28, 8, 0, 8, 28, 62, 8, 8,
				0, 56, 32, 32, 32, 32, 0, 8, 62, 8, 62, 8, 0, 48, 60, 63, 60, 48, 0, 3, 15, 63, 15, 3,
				0, 0, 0, 0, 0, 0, 0, 0, 6, 95, 6, 0, 0, 7, 3, 0, 7, 3, 0, 36, 126, 36, 126, 36,
				0, 36, 43, 106, 18, 0, 0, 99, 19, 8, 100, 99, 0, 54, 73, 86, 32, 80, 0, 0, 7, 3, 0, 0,
				0, 0, 62, 65, 0, 0, 0, 0, 65, 62, 0, 0, 0, 8, 62, 28, 62, 8, 0, 8, 8, 62, 8, 8,
				0, 0, 224, 96, 0, 0, 0, 8, 8, 8, 8, 8, 0, 0, 96, 96, 0, 0, 0, 32, 16, 8, 4, 2,
				0, 62, 81, 73, 69, 62, 0, 0, 66, 127, 64, 0, 0, 98, 81, 73, 73, 70, 0, 34, 73, 73, 73, 54,
				0, 24, 20, 18, 127, 16, 0, 47, 73, 73, 73, 49, 0, 60, 74, 73, 73, 48, 0, 1, 113, 9, 5, 3,
				0, 54, 73, 73, 73, 54, 0, 6, 73, 73, 41, 30, 0, 0, 108, 108, 0, 0, 0, 0, 236, 108, 0, 0,
				0, 8, 20, 34, 65, 0, 0, 36, 36, 36, 36, 36, 0, 0, 65, 34, 20, 8, 0, 2, 1, 89, 9, 6,
				0, 62, 65, 93, 85, 30, 0, 126, 17, 17, 17, 126, 0, 127, 73, 73, 73, 54, 0, 62, 65, 65, 65, 34,
				0, 127, 65, 65, 65, 62, 0, 127, 73, 73, 73, 65, 0, 127, 9, 9, 9, 1, 0, 62, 65, 73, 73, 122,
				0, 127, 8, 8, 8, 127, 0, 0, 65, 127, 65, 0, 0, 48, 64, 64, 64, 63, 0, 127, 8, 20, 34, 65,
				0, 127, 64, 64, 64, 64, 0, 127, 2, 4, 2, 127, 0, 127, 2, 4, 8, 127, 0, 62, 65, 65, 65, 62,
				0, 127, 9, 9, 9, 6, 0, 62, 65, 81, 33, 94, 0, 127, 9, 9, 25, 102, 0, 38, 73, 73, 73, 50,
				0, 1, 1, 127, 1, 1, 0, 63, 64, 64, 64, 63, 0, 31, 32, 64, 32, 31, 0, 63, 64, 60, 64, 63,
				0, 99, 20, 8, 20, 99, 0, 7, 8, 112, 8, 7, 0, 113, 73, 69, 67, 0, 0, 0, 127, 65, 65, 0,
				0, 2, 4, 8, 16, 32, 0, 0, 65, 65, 127, 0, 0, 4, 2, 1, 2, 4, 128, 128, 128, 128, 128, 128,
				0, 0, 3, 7, 0, 0, 0, 32, 84, 84, 84, 120, 0, 127, 68, 68, 68, 56, 0, 56, 68, 68, 68, 40,
				0, 56, 68, 68, 68, 127, 0, 56, 84, 84, 84, 8, 0, 8, 126, 9, 9, 0, 0, 24, 164, 164, 164, 124,
				0, 127, 4, 4, 120, 0, 0, 0, 0, 125, 64, 0, 0, 64, 128, 132, 125, 0, 0, 127, 16, 40, 68, 0,
				0, 0, 0, 127, 64, 0, 0, 124, 4, 24, 4, 120, 0, 124, 4, 4, 120, 0, 0, 56, 68, 68, 68, 56,
				0, 252, 68, 68, 68, 56, 0, 56, 68, 68, 68, 252, 0, 68, 120, 68, 4, 8, 0, 8, 84, 84, 84, 32,
				0, 4, 62, 68, 36, 0, 0, 60, 64, 32, 124, 0, 0, 28, 32, 64, 32, 28, 0, 60, 96, 48, 96, 60,
				0, 108, 16, 16, 108, 0, 0, 156, 160, 96, 60, 0, 0, 100, 84, 84, 76, 0, 0, 8, 62, 65, 65, 0,
				0, 0, 0, 119, 0, 0, 0, 0, 65, 65, 62, 8, 0, 2, 1, 2, 1, 0, 0, 60, 38, 35, 38, 60,
				20, 62, 85, 85, 65, 34, 0, 56, 68, 68, 56, 68, 0, 254, 1, 73, 54, 0, 0, 4, 24, 224, 16, 12,
				0, 56, 84, 68, 40, 0, 0, 25, 37, 163, 65, 0, 0, 124, 8, 4, 4, 248, 0, 62, 73, 73, 73, 62,
				0, 60, 64, 64, 32, 0, 0, 124, 16, 40, 68, 0, 0, 65, 51, 12, 48, 64, 0, 12, 48, 64, 32, 28,
				0, 54, 73, 73, 128, 0, 0, 68, 60, 4, 60, 68, 0, 240, 72, 72, 48, 0, 0, 48, 72, 72, 56, 8,
				0, 0, 8, 120, 72, 0, 0, 56, 64, 64, 56, 0, 0, 56, 68, 240, 72, 48, 0, 68, 44, 16, 104, 68,
				0, 28, 32, 252, 32, 28, 0, 48, 72, 32, 72, 48, 0, 127, 1, 1, 1, 0, 0, 112, 76, 67, 76, 112,
				0, 62, 65, 73, 65, 62, 0, 96, 24, 6, 24, 96, 0, 65, 73, 73, 73, 65, 0, 127, 1, 1, 1, 127,
				0, 99, 85, 73, 65, 65, 0, 28, 34, 127, 34, 28, 0, 31, 32, 127, 32, 31, 0, 78, 113, 1, 113, 78,
				0, 0, 0, 0, 0, 0, 0, 0, 48, 125, 48, 0, 0, 24, 36, 102, 36, 0, 0, 72, 62, 73, 73, 98,
				0, 93, 34, 34, 34, 93, 0, 41, 42, 124, 42, 41, 0, 0, 0, 119, 0, 0, 0, 34, 77, 85, 89, 34,
				0, 0, 8, 0, 8, 0, 62, 65, 93, 85, 65, 62, 0, 8, 85, 85, 85, 94, 0, 8, 20, 0, 8, 20,
				4, 4, 4, 4, 4, 28, 0, 0, 8, 8, 8, 0, 62, 65, 93, 75, 85, 62, 0, 0, 2, 2, 2, 0,
				0, 6, 9, 9, 6, 0, 0, 0, 36, 46, 36, 0, 0, 9, 13, 10, 0, 0, 0, 9, 15, 5, 0, 0,
				0, 0, 7, 3, 0, 0, 0, 252, 32, 32, 28, 0, 0, 6, 73, 127, 1, 127, 0, 0, 8, 0, 0, 0,
				0, 0, 8, 24, 24, 0, 0, 2, 15, 0, 0, 0, 0, 78, 81, 81, 78, 0, 0, 20, 8, 0, 20, 8,
				0, 23, 8, 52, 42, 120, 0, 23, 8, 76, 106, 80, 5, 23, 10, 52, 42, 120, 0, 48, 72, 77, 64, 32,
				0, 112, 41, 37, 40, 112, 0, 112, 40, 37, 41, 112, 0, 112, 41, 37, 41, 112, 0, 112, 42, 37, 42, 113,
				0, 112, 41, 36, 41, 112, 0, 120, 47, 37, 47, 120, 0, 126, 9, 127, 73, 73, 0, 30, 161, 225, 33, 18,
				0, 124, 85, 85, 84, 68, 0, 124, 84, 84, 85, 69, 0, 124, 85, 85, 85, 68, 0, 124, 85, 84, 85, 68,
				0, 0, 69, 125, 68, 0, 0, 0, 68, 125, 69, 0, 0, 0, 69, 125, 69, 0, 0, 0, 69, 124, 69, 0,
				0, 8, 127, 73, 65, 62, 0, 122, 17, 34, 121, 0, 0, 61, 67, 66, 60, 0, 0, 60, 66, 67, 61, 0,
				0, 60, 67, 67, 61, 0, 0, 58, 69, 70, 57, 0, 0, 61, 66, 66, 61, 0, 0, 34, 20, 8, 20, 34,
				0, 126, 97, 93, 67, 63, 0, 61, 65, 64, 60, 0, 0, 60, 64, 65, 61, 0, 0, 60, 65, 65, 61, 0,
				0, 60, 65, 64, 61, 0, 0, 4, 8, 113, 9, 4, 0, 255, 165, 36, 24, 0, 0, 254, 74, 74, 52, 0,
				0, 32, 85, 85, 84, 120, 0, 32, 84, 85, 85, 120, 0, 32, 85, 85, 85, 120, 0, 32, 86, 85, 86, 121,
				0, 32, 85, 84, 85, 120, 0, 32, 87, 85, 87, 120, 0, 52, 84, 124, 84, 88, 0, 28, 162, 226, 34, 20,
				0, 56, 85, 85, 84, 8, 0, 56, 84, 84, 85, 9, 0, 56, 85, 85, 85, 8, 0, 56, 85, 84, 85, 8,
				0, 0, 1, 124, 64, 0, 0, 0, 0, 125, 65, 0, 0, 0, 1, 125, 65, 0, 0, 0, 1, 124, 65, 0,
				0, 34, 85, 89, 48, 0, 0, 122, 9, 10, 113, 0, 0, 57, 69, 68, 56, 0, 0, 56, 68, 69, 57, 0,
				0, 56, 69, 69, 57, 0, 0, 50, 73, 74, 49, 0, 0, 56, 69, 68, 57, 0, 0, 8, 8, 42, 8, 8,
				128, 112, 104, 88, 56, 4, 0, 61, 65, 32, 124, 0, 0, 60, 64, 33, 125, 0, 0, 60, 65, 33, 125, 0,
				0, 61, 64, 32, 125, 0, 0, 156, 160, 97, 61, 0, 0, 254, 170, 40, 16, 0, 0, 156, 161, 96, 61, 0,
			};
		#endregion

		// Look-up tables for repositioned greek letters
		static readonly byte[] greekBigLetter= { 0x41, 0x42, 0x96, 0x97, 0x45, 0x5A, 0x48, 0x98, 0x49, 0x4B, 0x99, 0x4D, 0x4E, 0x9A, 0x4F, 0x9B, 0x50, 0xC7, 0x9C, 0x54, 0x59, 0x9D, 0x58, 0x9E, 0x9F };
		static readonly byte[] greekSmallLetter= { 0x81, 0x82, 0x83, 0x90, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0xB5, 0x8B, 0x8C, 0x6F, 0x8D, 0x8E, 0xE7, 0x8F, 0x90, 0x91, 0x92, 0x93, 0x94, 0x95 };

		public static void DrawText(string text, int offsetX, int offsetY, int scaleX, int scaleY, byte tabLength=0)
		{
			Encoding enc=Encoding.GetEncoding(28605); // Latin-1
			
			byte[] buf=new byte[8];

			int x=0;
			for(int c=0; c<text.Length; c++)
			{
				byte b=0;
				char ch=text[c];

				// Handle euro-Symbol and greek letters special
				if(ch=='€') b=0x80;
				else if(ch>=0x0391&&ch<=0x03A9) b=greekBigLetter[ch-0x0391];
				else if(ch>=0x03B1&&ch<=0x03C9) b=greekSmallLetter[ch-0x03B1];
				else if(tabLength>0&&ch=='\t')
				{
					x+=tabLength*6;
					continue;
				}
				else
				{
					if(enc.GetBytes(text, c, 1, buf, 0)!=1) continue;
					b=buf[0];
				}

				for(int i=0; i<6; i++, x++)
				{
					byte letterColumn=font256x6x8[b*6+i];
					if(letterColumn==0) continue;

					for(int y=0; y<8; y++)
					{
						if((letterColumn&1)!=0)
						{
							gl.Scissor(x*scaleX+offsetX, offsetY-(y+1)*scaleY+1, scaleX, scaleY);
							gl.Clear(glBufferMask.COLOR_BUFFER_BIT);
						}

						letterColumn>>=1;
						if(letterColumn==0) break;
					}
				}
			}
		}

		public static void DrawImage(byte[] image, int width, int height, glPixelFormat pixelFormat, int offsetX, int offsetY, int scaleX, int scaleY)
		{
			int i=0;

			for(int y=0; y<height; y++)
			{
				for(int x=0; x<width; x++)
				{
					float red=0, green=0, blue=0, alpha=1;

					switch(pixelFormat)
					{
						default: break;
						case glPixelFormat.RGB: red=image[i++]/255.0f; green=image[i++]/255.0f; blue=image[i++]/255.0f; break;
						case glPixelFormat.RGBA: red=image[i++]/255.0f; green=image[i++]/255.0f; blue=image[i++]/255.0f; alpha=image[i++]/255.0f; break;
						case glPixelFormat.BGR: blue=image[i++]/255.0f; green=image[i++]/255.0f; red=image[i++]/255.0f; break;
						case glPixelFormat.BGRA: blue=image[i++]/255.0f; green=image[i++]/255.0f; red=image[i++]/255.0f; alpha=image[i++]/255.0f; break;
						case glPixelFormat.BLUE:
						case glPixelFormat.GREEN:
						case glPixelFormat.RED: red=green=blue=image[i++]/255.0f; break;
						case glPixelFormat.RG: red=green=blue=image[i++]/255.0f; alpha=image[i++]/255.0f; break;
					}

					gl.ClearColor(red, green, blue, alpha);
					gl.Scissor(x*scaleX+offsetX, offsetY-(y+1)*scaleY+1, scaleX, scaleY);
					gl.Clear(glBufferMask.COLOR_BUFFER_BIT);
				}
			}
		}
	}
}
