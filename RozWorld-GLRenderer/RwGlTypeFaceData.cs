﻿/**
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
    internal class RwGlTypeFaceData : IDisposable
    {
        /// <summary>
        /// The maximum number of font texture banks that can be active at once.
        /// </summary>
        public const byte MAX_BANKS = 4;


        /// <summary>
        /// Gets the value that indicates whether this RwGlTypeFaceData instance is in the process of disposing.
        /// </summary>
        public bool Disposing { get; private set; }


        /// <summary>
        /// The mapping of characters to the texture banks in which their rendered form is stored.
        /// </summary>
        private Dictionary<char, byte> CharacterToBankIds { get; set; }

        /// <summary>
        /// The value that indicates whether this RwGlTypeData instance has been disposed.
        /// </summary>
        private bool Disposed;

        /// <summary>
        /// The internal texture banks for storing rendered glyphs.
        /// </summary>
        private List<RwGlTypeFaceBank> TextureBanks { get; set; }

        /// <summary>
        /// The typeface that this object is based on.
        /// </summary>
        private Face Face { get; set; }


        /// <summary>
        /// Initializes a new instance of the RwGlTypeFaceData 
        /// </summary>
        /// <param name="face">The SharpFont.Face to use for rendering glyphs.</param>
        public RwGlTypeFaceData(Face face)
        {
            CharacterToBankIds = new Dictionary<char, byte>();
            Face = face;
            TextureBanks = new List<RwGlTypeFaceBank>();
        }


        /// <summary>
        /// Releases all resources used by this RwGlTypeFaceData instance.
        /// </summary>
        public void Dispose()
        {
            if (Disposing || Disposed)
                throw new ObjectDisposedException(Face.FamilyName);

            Disposing = true;

            foreach (RwGlTypeFaceBank bank in TextureBanks)
            {
                bank.Dispose();
            }

            Disposing = false;
            Disposed = true;
        }

        public RwGlTypeFaceBufferData GenerateVboForString(string text)
        {
            if (Disposing || Disposed)
                throw new ObjectDisposedException(Face.FamilyName);

            return null;
        }


        public RwGlTypeFaceBufferData GetVboForString(string text)
        {
            var characterRects = new List<Rectanglei>();
            var uvRects = new List<Rectanglei>();

            int xOffset = 0;

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // Obtain character UV
                //
                if (!InternalCharacterMap.ContainsKey(c))
                    LoadGlyphToTextureCache(c);

                Rectanglei uvRect = InternalCharacterMap[c];

                if (uvRect.Width > 0 && uvRect.Height > 0)
                {
                    characterRects.Add(new Rectanglei(xOffset, 0, uvRect.Width, uvRect.Height));
                    uvRects.Add(uvRect);
                }

                xOffset += 32; // Temporary until character metrics are implemented
            }

            // Expand Rectanglei objects into floats for VBOs and return them
            //
            return new RwGlTypeFaceBufferData(
                ExpandRectangleiList(characterRects),
                ExpandRectangleiList(uvRects)
                );
        }


        private float[] ExpandRectangleiList(IList<Rectanglei> source)
        {
            var expandedList = new List<float>();

            foreach (Rectanglei rectangle in source)
            {
                expandedList.AddRange(
                    new float[]
                    {
                        rectangle.X, rectangle.Y,
                        rectangle.X + rectangle.Width, rectangle.Y,
                        rectangle.X, rectangle.Y + rectangle.Height,

                        rectangle.X + rectangle.Width, rectangle.Y,
                        rectangle.X, rectangle.Y + rectangle.Height,
                        rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height
                    }
                    );
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

            // If the bitmap is zero size, skip the texture cache and add a dummy UV map to the dictionary
            //
            if (glyphBitmap.Width == 0 || glyphBitmap.Rows == 0)
            {
                InternalCharacterMap.Add(c, new Rectanglei(0, 0, 0, 0));
                return;
            }

            // Bind the glyph cache texture in GL
            //
            GL.BindTexture(TextureTarget.Texture2D, GlTextureId);

            // Store x-offset for the next glyph to be loaded into
            //
            int charXOffset = InternalTextureWidth;

            // Resize the glyph cache texture's height if needed
            //
            if (InternalTextureHeight < glyphBitmap.Rows)
                InternalTextureHeight = glyphBitmap.Rows;

            InternalTextureWidth += glyphBitmap.Width;

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, InternalTextureWidth, InternalTextureHeight,
                0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);

            // Load the glyph bitmap into the texture
            //
            byte[] gBuffer = new byte[glyphBitmap.Width * glyphBitmap.Rows * 3];

            for (int y = 0; y < glyphBitmap.Rows; y++)
            {
                for (int x = 0; x < glyphBitmap.Width; x++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        gBuffer[(x + y * glyphBitmap.Width) * 3 + i] = glyphBitmap.BufferData[x + glyphBitmap.Width * y];
                    }
                }
            }
            
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, charXOffset, 0, glyphBitmap.Width, glyphBitmap.Rows,
                PixelFormat.Rgb, PixelType.UnsignedByte, gBuffer);

            // Confirm the texture coordinates in the dictionary
            //
            InternalCharacterMap.Add(c, new Rectanglei(charXOffset, 0, glyphBitmap.Width, glyphBitmap.Rows));
        }

        private uint NearestPowerOfTwo(int number)
        {
            uint comp = 1;

            while (comp < number) comp <<= 1;

            return comp;
        }
    }
}
