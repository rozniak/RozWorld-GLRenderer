/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlTypeFaceData -- RozWorld FreeType Type Face Data
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

using Pencil.Gaming.Graphics;
using Pencil.Gaming.MathUtils;
using SharpFont;
using System;
using System.Collections.Generic;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents typeface metadata used by the RozWorld OpenGL FreeType service.
    /// </summary>
    internal class RwGlTypeFaceData
    {
        public int GlTextureId { get; private set; }

        public Vector2i TextureCacheDimensions { get { return new Vector2i(InternalTextureWidth, InternalTextureHeight); } }


        private Face Face { get; set; }

        private Dictionary<char, Rectanglei> InternalCharacterMap { get; set; }

        private int InternalTextureHeight { get; set; }

        private int InternalTextureWidth { get; set; }


        public RwGlTypeFaceData(Face face)
        {
            InternalCharacterMap = new Dictionary<char, Rectanglei>();
            InternalTextureHeight = 0;
            InternalTextureWidth = 0;

            Face = face;
            GlTextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, GlTextureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 0, 0,
                0, PixelFormat.Rgba, PixelType.Byte, IntPtr.Zero);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Linear });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMagFilter.Linear });
        }


        public RwGlTypeFaceBufferData GetVboForString(string text)
        {
            var characterRects = new List<Rectanglei>();
            var uvRects = new List<Rectanglei>();

            foreach (char c in text)
            {
                // Obtain character UV
                //
                if (!InternalCharacterMap.ContainsKey(c))
                    LoadGlyphToTextureCache(c);

                uvRects.Add(InternalCharacterMap[c]);

                // Obtain character vectors
                //

                // TODO: Code this
            }

            // Expand Rectanglei objects into floats for VBOs
            //


            // Temp
            return new RwGlTypeFaceBufferData();
        }


        private float[] ExpandRectangleiList(IList<Rectanglei> source)
        {
            var expandedList = new List<float>();

            foreach (Rectanglei rectangle in source)
            {
                // TODO: Code this
            }

            return expandedList.ToArray();
        }

        private void LoadGlyphToTextureCache(char c)
        {
            if (InternalCharacterMap.ContainsKey(c))
                throw new InvalidOperationException("The glyph is already present in the texture cache.");
            
            // Load and render the glyph into memory
            //
            uint glyphIndex = Face.GetCharIndex(c);

            Face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
            Face.Glyph.RenderGlyph(RenderMode.Normal);

            FTBitmap glyphBitmap = Face.Glyph.Bitmap;

            // Bind the glyph cache texture in GL
            //
            GL.BindTexture(TextureTarget.Texture2D, GlTextureId);

            // Resize the glyph cache texture's height if needed
            //
            if (InternalTextureHeight < glyphBitmap.Rows)
                InternalTextureHeight = glyphBitmap.Rows;

            InternalTextureWidth += glyphBitmap.Width;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, InternalTextureWidth, InternalTextureHeight, 0, PixelFormat.Rgba, PixelType.Byte, IntPtr.Zero);

            // For now output to console
            //
            Console.WriteLine(glyphBitmap.PixelMode);
        }

        private uint NearestPowerOfTwo(int number)
        {
            uint comp = 1;

            while (comp < number) comp <<= 1;

            return comp;
        }
    }
}
