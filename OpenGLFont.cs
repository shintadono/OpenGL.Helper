using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using Win32;
using Font=System.Drawing.Font;

namespace OpenGL.Helper
{
	public class OpenGLFont
	{
		struct LookUpTableEntry
		{
			public ushort X, Y, Width;
			public short Offset, OffsetNext;
		}

		public int Width, Height;
		public int FontHeight;
		public byte[] bits;
		LookUpTableEntry[] lookUpTable;

		const int MOD_ADLER=65521;

		static int GetAdler32(byte[] data)
		{
			uint a=1, b=0;

			for(int i=0; i<data.Length; ++i)
			{
				a=(a+data[i])%MOD_ADLER;
				b=(b+a)%MOD_ADLER;
			}

			return (int)((b<<16)|a);
		}

		static bool ArraysEqual(byte[] a, byte[] b)
		{
			if(a.Length!=b.Length) return false;

			for(int i=0; i<a.Length; i++)
			{
				if(a[i]!=b[i]) return false;
			}
			return true;
		}

		public OpenGLFont(string fontFileName)
		{
			ReadFont(fontFileName);
		}

		public OpenGLFont(string fontName, float fontSize, FontStyle fontStyle, HashSet<ushort> chars)
		{
			Init(fontName, fontSize, fontStyle, 4096, null, chars);
		}

		public OpenGLFont(string fontName, float fontSize, FontStyle fontStyle, Tuple<ushort, ushort> range, HashSet<ushort> chars=null)
		{
			Init(fontName, fontSize, fontStyle, 4096, range, chars);
		}

		public OpenGLFont(string fontName, float fontSize, FontStyle fontStyle, int maxWidth, HashSet<ushort> chars)
		{
			Init(fontName, fontSize, fontStyle, maxWidth, null, chars);
		}

		public OpenGLFont(string fontName, float fontSize, FontStyle fontStyle, int maxWidth=4096, Tuple<ushort, ushort> range=null, HashSet<ushort> chars=null)
		{
			Init(fontName, fontSize, fontStyle, maxWidth, range, chars);
		}

		void Init(string fontName, float fontSize, FontStyle fontStyle, int maxWidth, Tuple<ushort, ushort> range, HashSet<ushort> chars)
		{
			int numberOfEntries=ushort.MaxValue+1;
			ushort rangeMin=ushort.MinValue, rangeMax=ushort.MaxValue;
			if(range!=null)
			{
				rangeMin=Math.Min(range.Item1, range.Item2);
				rangeMax=Math.Max(range.Item1, range.Item2);
			}

			bool isChars=chars!=null&&chars.Count>0;

			Mat2 mat2=Mat2.Identity;

			byte[][] bmp=new byte[numberOfEntries][];
			Dictionary<int, List<uint>> dubCheck=new Dictionary<int, List<uint>>();
			ushort[] dubs=new ushort[numberOfEntries]; // dub lookup
			GlyphMetrics[] gms=new GlyphMetrics[numberOfEntries];
			GlyphMetrics gmSpace;

			int minY=0, maxY=0, sumX=0, lines=1;

			#region Gather font bitmaps and metrics, check for dubs
			using(var font=new Font(fontName, fontSize, fontStyle))
			{
				using(var bitmap=new Bitmap(16, 16, PixelFormat.Format24bppRgb))
				{
					using(var graphics=Graphics.FromImage(bitmap))
					{
						IntPtr hdc=graphics.GetHdc();

						try
						{
							IntPtr hfont=font.ToHfont();

							try
							{
								IntPtr oldFont=Win32.GDI.SelectObject(hdc, hfont);

								try
								{
									uint bufsize=Win32.Font.GetGlyphOutline(hdc, ' ', Win32.GGO.GRAY8_BITMAP, out gmSpace, 0, null, ref mat2);
									if(bufsize==uint.MaxValue) throw new Exception("Error initializing font."); // no space no font!

									for(uint ch=0; ch<=ushort.MaxValue; ch++)
									{
										dubs[ch]=0; // error signal
										bmp[ch]=null; // no bitmap signal

										// Range check
										if(ch<rangeMin||ch>rangeMax)
										{
											// Chars check
											if(!isChars) continue;
											if(!chars.Contains((ushort)ch)) continue;
										}

										bufsize=Win32.Font.GetGlyphOutline(hdc, ch, Win32.GGO.GRAY8_BITMAP, out gms[ch], 0, null, ref mat2);
										if(bufsize==uint.MaxValue) continue;

										dubs[ch]=(ushort)ch; // regular case: gms[ch] is filled

										// is bitmap is available?
										if(bufsize==0) continue; // no

										GlyphMetrics gm;
										bmp[ch]=new byte[bufsize];
										bufsize=Win32.Font.GetGlyphOutline(hdc, ch, Win32.GGO.GRAY8_BITMAP, out gm, bufsize, bmp[ch], ref mat2);
										if(bufsize==uint.MaxValue||bufsize==0)
										{
											bmp[ch]=null; // no bitmap signal
											continue;
										}

										int hash=GetAdler32(bmp[ch]);
										if(dubCheck.ContainsKey(hash))
										{
											List<uint> hits=dubCheck[hash];
											uint found=uint.MaxValue;
											foreach(uint hit in hits)
											{
												if(!ArraysEqual(bmp[ch], bmp[hit])) continue;
												found=hit;
												break;
											}

											if(found==uint.MaxValue) dubCheck[hash].Add(ch);
											else
											{
												bmp[ch]=bmp[found]; // replace bitmap (save memory)
												dubs[ch]=(ushort)found; // replace lookup
												continue;
											}
										}
										else
										{
											dubCheck.Add(hash, new List<uint>());
											dubCheck[hash].Add(ch);
										}

										gm=gms[ch]; // use first gathered GlyphMetrics

										// line to long?
										if(sumX+(int)gm.gmBlackBoxX+1>maxWidth)
										{
											sumX=0;
											lines++;
										}

										sumX+=(int)gm.gmBlackBoxX+1;

										// determine line height
										if(maxY<gm.gmptGlyphOrigin.y) maxY=gm.gmptGlyphOrigin.y;

										int mY=(int)gm.gmBlackBoxY-gm.gmptGlyphOrigin.y;
										if(minY<mY) minY=mY;
									}

									dubCheck=null;
								}
								finally
								{
									Win32.GDI.SelectObject(hdc, oldFont);
								}
							}
							finally
							{
								Win32.GDI.DeleteObject(hfont);
							}
						}
						finally
						{
							graphics.ReleaseHdc();
						}
					} // Graphics
				} // Bitmap
			} // Font
			#endregion

			#region Render image and build lookup table
			FontHeight=maxY+minY;
			Width=sumX;
			if(lines>1) Width=maxWidth;
			Height=FontHeight*lines;

			bits=new byte[Width*Height];
			lookUpTable=new LookUpTableEntry[numberOfEntries];

			LookUpTableEntry spaceEntry;
			spaceEntry.X=spaceEntry.Y=0; // not in image
			spaceEntry.Width=0; // not in image
			spaceEntry.Offset=0; // not in image
			spaceEntry.OffsetNext=gmSpace.gmCellIncX; // just an offset

			int posX=0, posY=0;

			for(uint ch=0; ch<=ushort.MaxValue; ch++)
			{
				if(dubs[ch]==0)
				{
					lookUpTable[ch]=spaceEntry;
					continue;
				}

				if(dubs[ch]==ch)
				{
					if(bmp[ch]!=null)
					{
						byte[] buffer=bmp[ch];
						int bufsize=buffer.Length;
						GlyphMetrics gm=gms[ch];

						if(posX+gm.gmBlackBoxX+1>maxWidth)
						{
							posY+=FontHeight;
							posX=0;
						}

						int lw=(int)(bufsize/gm.gmBlackBoxY);

						int c0=(posY+(int)maxY-gm.gmptGlyphOrigin.y)*Width+(int)posX;
						for(int y=0; y<gm.gmBlackBoxY; y++)
						{
							int c=c0+y*Width;
							for(int x=0, i=y*lw; x<gm.gmBlackBoxX; x++, c++, i++)
							{
								if(buffer[i]==64) bits[c]=255;
								else bits[c]=(byte)(buffer[i]<<2);
							}
						}

						lookUpTable[ch].X=(ushort)posX;
						lookUpTable[ch].Y=(ushort)posY;
						lookUpTable[ch].Width=(ushort)gm.gmBlackBoxX;
						lookUpTable[ch].Offset=(short)gm.gmptGlyphOrigin.x;
						lookUpTable[ch].OffsetNext=gm.gmCellIncX;

						posX+=(int)gm.gmBlackBoxX+1;
					}
					else
					{
						lookUpTable[ch]=spaceEntry;
						continue;
					}
				}
				else lookUpTable[ch]=lookUpTable[dubs[ch]]; // ref
			}
			#endregion
		}

		void ReadFont(string filename)
		{
			using(BinaryReader br=new BinaryReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read)))
			{
				byte[] buf=br.ReadBytes(16);
				byte[] sig=Encoding.ASCII.GetBytes("BitmapFontFile01");
				if(!ArraysEqual(buf, sig)) throw new FileLoadException("File format not supported.", filename);

				// available character range (for future use)
				ushort min=br.ReadUInt16();
				ushort max=br.ReadUInt16();
				if(min!=ushort.MinValue||max!=ushort.MaxValue) throw new FileLoadException("File format not supported.", filename);

				// max. height of the letters/symbols
				FontHeight=br.ReadUInt16();

				// bitmap size
				Width=br.ReadInt32();
				Height=br.ReadInt32();
				if(Width<=0||Height<=0) throw new FileLoadException("File format not supported.", filename);

				lookUpTable=new LookUpTableEntry[max-min+1];

				// lookup table
				for(int i=min; i<=max; i++)
				{
					lookUpTable[i].X=br.ReadUInt16();
					lookUpTable[i].Y=br.ReadUInt16();
					lookUpTable[i].Width=br.ReadUInt16();
					lookUpTable[i].Offset=br.ReadInt16();
					lookUpTable[i].OffsetNext=br.ReadInt16();
				}

				// bitmap
				bits=br.ReadBytes(Width*Height);
			}
		}

		public void SaveFont(string filename)
		{
			if(bits==null) throw new NotSupportedException("A font can only be saved after initialization and before disposal or memory freeing operations.");

			using(BinaryWriter bw=new BinaryWriter(File.Create(filename)))
			{
				byte[] sig=Encoding.ASCII.GetBytes("BitmapFontFile01");
				bw.Write(sig, 0, 16);

				// available character range (for future use)
				bw.Write(ushort.MinValue);
				bw.Write(ushort.MaxValue);

				// max. height of the letters/symbols
				bw.Write((ushort)FontHeight);

				// bitmap size
				bw.Write((int)Width);
				bw.Write((int)Height);

				// lookup table
				for(int i=0; i<=ushort.MaxValue; i++)
				{
					bw.Write(lookUpTable[i].X);
					bw.Write(lookUpTable[i].Y);
					bw.Write(lookUpTable[i].Width);
					bw.Write(lookUpTable[i].Offset);
					bw.Write(lookUpTable[i].OffsetNext);
				}

				// bitmap
				bw.Write(bits, 0, bits.Length);
			}
		}

		public List<int> BuildDrawBuffer(string text, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			int length; // dummy
			return BuildDrawBuffer(text, 0, 0, out length, anchor);
		}

		public List<int> BuildDrawBuffer(string text, out int length, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			return BuildDrawBuffer(text, 0, 0, out length, anchor);
		}

		public List<int> BuildDrawBuffer(string text, int posX, int posY, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			int length; // dummy
			return BuildDrawBuffer(text, posX, posY, out length, anchor);
		}

		public List<int> BuildDrawBuffer(string text, int posX, int posY, out int length, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
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
				foreach(char ch in text) len+=lookUpTable[ch].OffsetNext;

				switch(horizontalPlacement)
				{
					case 1: posX-=len/2; break; // center
					case 2: posX-=len; break; // top
				}
			}

			List<int> ret=new List<int>();

			length=0;
			foreach(char ch in text)
			{
				var entry=lookUpTable[ch];

				if(entry.Width==0) // whitespaces
				{
					posX+=entry.OffsetNext;
					length+=entry.OffsetNext;
					continue;
				}

				int left=posX+entry.Offset;
				int right=left+entry.Width;
				int bottom=posY;
				int top=posY+FontHeight;

				int tcLeft=entry.X;
				int tcRight=tcLeft+entry.Width;
				int tcBottom=entry.Y+FontHeight;
				int tcTop=entry.Y;

				#region Triangle first half
				ret.Add(left);
				ret.Add(bottom);

				ret.Add(tcLeft);
				ret.Add(tcBottom);

				ret.Add(right);
				ret.Add(bottom);

				ret.Add(tcRight);
				ret.Add(tcBottom);

				ret.Add(left);
				ret.Add(top);

				ret.Add(tcLeft);
				ret.Add(tcTop);
				#endregion

				#region Triangle second half
				ret.Add(left);
				ret.Add(top);

				ret.Add(tcLeft);
				ret.Add(tcTop);

				ret.Add(right);
				ret.Add(bottom);

				ret.Add(tcRight);
				ret.Add(tcBottom);

				ret.Add(right);
				ret.Add(top);

				ret.Add(tcRight);
				ret.Add(tcTop);
				#endregion

				posX+=entry.OffsetNext;
				length+=entry.OffsetNext;
			}

			return ret;
		}

		public List<int> BuildDrawBufferBottomUp(string text, int posX, int posY, out int length, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			int horizontalPlacement=(int)anchor&0x03;
			int veticalPlacement=((int)anchor>>4)&0x03;

			switch(horizontalPlacement)
			{
				case 1: posX-=FontHeight/2; break; // center
				case 2: posX-=FontHeight; break; // right
			}

			if(veticalPlacement==1||veticalPlacement==2)
			{
				int len=0;
				foreach(char ch in text) len+=lookUpTable[ch].OffsetNext;

				switch(veticalPlacement)
				{
					case 1: posY-=len/2; break; // center
					case 2: posY-=len; break; // top
				}
			}

			List<int> ret=new List<int>();

			length=0;
			foreach(char ch in text)
			{
				var entry=lookUpTable[ch];

				if(entry.Width==0) // whitespaces
				{
					posY+=entry.OffsetNext;
					length+=entry.OffsetNext;
					continue;
				}

				int left=posY+entry.Offset;
				int right=left+entry.Width;
				int top=posX;
				int bottom=top+FontHeight;

				int tcLeft=entry.X;
				int tcRight=tcLeft+entry.Width;
				int tcTop=entry.Y;
				int tcBottom=tcTop+FontHeight;

				#region Triangle first half
				ret.Add(bottom);
				ret.Add(left);

				ret.Add(tcLeft);
				ret.Add(tcBottom);

				ret.Add(bottom);
				ret.Add(right);

				ret.Add(tcRight);
				ret.Add(tcBottom);

				ret.Add(top);
				ret.Add(left);

				ret.Add(tcLeft);
				ret.Add(tcTop);
				#endregion

				#region Triangle second half
				ret.Add(top);
				ret.Add(left);

				ret.Add(tcLeft);
				ret.Add(tcTop);

				ret.Add(bottom);
				ret.Add(right);

				ret.Add(tcRight);
				ret.Add(tcBottom);

				ret.Add(top);
				ret.Add(right);

				ret.Add(tcRight);
				ret.Add(tcTop);
				#endregion

				posY+=entry.OffsetNext;
				length+=entry.OffsetNext;
			}

			return ret;
		}

		public List<int> BuildDrawBufferTopDown(string text, int posX, int posY, out int length, AnchorPlacement anchor=AnchorPlacement.BottomLeft)
		{
			int horizontalPlacement=(int)anchor&0x03;
			int veticalPlacement=((int)anchor>>4)&0x03;

			switch(horizontalPlacement)
			{
				case 1: posX-=FontHeight/2; break; // center
				case 2: posX-=FontHeight; break; // right
			}

			if(veticalPlacement==1||veticalPlacement==0)
			{
				int len=0;
				foreach(char ch in text) len+=lookUpTable[ch].OffsetNext;

				switch(veticalPlacement)
				{
					case 0: posY+=len; break; // bottom
					case 1: posY+=len/2; break; // center
				}
			}

			List<int> ret=new List<int>();

			length=0;
			foreach(char ch in text)
			{
				var entry=lookUpTable[ch];

				if(entry.Width==0) // whitespaces
				{
					posY-=entry.OffsetNext;
					length+=entry.OffsetNext;
					continue;
				}

				int left=posY-entry.Offset;
				int right=left-entry.Width;
				int bottom=posX;
				int top=bottom+FontHeight;

				int tcLeft=entry.X;
				int tcRight=tcLeft+entry.Width;
				int tcTop=entry.Y;
				int tcBottom=tcTop+FontHeight;

				#region Triangle first half
				ret.Add(bottom);
				ret.Add(left);

				ret.Add(tcLeft);
				ret.Add(tcBottom);

				ret.Add(bottom);
				ret.Add(right);

				ret.Add(tcRight);
				ret.Add(tcBottom);

				ret.Add(top);
				ret.Add(left);

				ret.Add(tcLeft);
				ret.Add(tcTop);
				#endregion

				#region Triangle second half
				ret.Add(top);
				ret.Add(left);

				ret.Add(tcLeft);
				ret.Add(tcTop);

				ret.Add(bottom);
				ret.Add(right);

				ret.Add(tcRight);
				ret.Add(tcBottom);

				ret.Add(top);
				ret.Add(right);

				ret.Add(tcRight);
				ret.Add(tcTop);
				#endregion

				posY-=entry.OffsetNext;
				length+=entry.OffsetNext;
			}

			return ret;
		}
	}
}
