/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlTypeFaceBank -- RozWorld FreeType Type Face Texture Bank
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
    /// Represents a texture bank for holding typeface glyphs.
    /// </summary>
    internal class RwGlTypeFaceBank : IDisposable
    {
        /// <summary>
        /// The designated height of a font texture bank.
        /// </summary>
        public const int TEXTURE_BANK_HEIGHT = 2048;

        /// <summary>
        /// The designated width of a font texture bank.
        /// </summary>
        public const int TEXTURE_BANK_WIDTH = 2048;


        /// <summary>
        /// The ID of this texture in OpenGL.
        /// </summary>
        public int GlTextureId { get; private set; }


        /// <summary>
        /// The mapping of characters to rectangle regions on the texture.
        /// </summary>
        private Dictionary<char, Rectanglei> CharacterMap { get; set; }


        /// <summary>
        /// Initializes a new instance of the RwGlTypeFaceBank class.
        /// </summary>
        public RwGlTypeFaceBank(Face face)
        {
            CharacterMap = new Dictionary<char, Rectanglei>();

            GlTextureId = GL.GenTexture();

            // Set up the texture to use for this bank
            //
            GL.BindTexture(TextureTarget.Texture2D, GlTextureId);

            GL.TexImage2D(
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgb,
                TEXTURE_BANK_WIDTH,
                TEXTURE_BANK_HEIGHT,
                0,
                PixelFormat.Rgb,
                PixelType.UnsignedByte,
                IntPtr.Zero
                );

            GL.TexParameterI(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                new int[] { (int)TextureMinFilter.Linear }
                );
            GL.TexParameterI(
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                new int[] { (int)TextureMagFilter.Linear }
                );
        }


        /// <summary>
        /// Releases all resources used by this RwGlTypeFaceBank instance.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteTexture(GlTextureId);
        }
    }
}
