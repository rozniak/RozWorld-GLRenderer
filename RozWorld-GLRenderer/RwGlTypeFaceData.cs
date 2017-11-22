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
        public Face Face { get; private set; }

        public int GlTextureId { get; private set; }


        private Dictionary<char, Rectanglei> InternalCharacterMap { get; set; }

        private int InternalTextureWidth { get; set; }


        public RwGlTypeFaceData(Face face)
        {
            InternalCharacterMap = new Dictionary<char, Rectanglei>();
            InternalTextureWidth = 0;

            Face = face;
            GlTextureId = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, GlTextureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb8, 0, face.Height,
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
            // TODO: Code this
        }

        private uint NearestPowerOfTwo(int number)
        {
            uint comp = 1;

            while (comp < number) comp <<= 1;

            return comp;
        }
    }
}
