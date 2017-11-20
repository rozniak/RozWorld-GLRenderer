/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlMethods -- RozWorld OpenGL Methods
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

using Pencil.Gaming.Graphics;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents the internal OpenGL methods used by the renderer.
    /// </summary>
    internal static class RwGlMethods
    {
        /// <summary>
        /// Loads a texture into memory.
        /// </summary>
        /// <param name="textureSource">The System.Drawing.Bitmap of the texture.</param>
        /// <returns>The ID of the loaded texture.</returns>
        public static uint LoadTexture(Bitmap textureSource)
        {
            textureSource.RotateFlip(RotateFlipType.RotateNoneFlipY); // Flip-Y as Bitmaps read from bottom to top

            // Lock bitmap data into memory
            BitmapData data = textureSource.LockBits(new Rectangle(0, 0, textureSource.Width, textureSource.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Now load it into OpenGL
            int textureId = GL.GenTexture();

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
            GL.BindTexture(TextureTarget.Texture2D, textureId);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, textureSource.Width, textureSource.Height,
                0, Pencil.Gaming.Graphics.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, new int[] { (int)TextureMagFilter.Linear });
            GL.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, new int[] { (int)TextureMinFilter.Linear });

            // Unlock bits again and dispose
            textureSource.UnlockBits(data);
            textureSource.Dispose();

            return 0;
        }

        /// <summary>
        /// Loads and compiles shaders into a shader program.
        /// </summary>
        /// <param name="vertexSource">The GLSL-source for the vertex shader.</param>
        /// <param name="fragmentSource">The GLSL-source for the fragment shader.</param>
        /// <returns>The ID of the compiled shader program.</returns>
        public static uint LoadShaders(string vertexSource, string fragmentSource)
        {
            string infoLog = String.Empty; // In case there are any errors

            // Create shaders
            uint vertexShaderId = GL.CreateShader(ShaderType.VertexShader);
            uint fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader);

            // Compile vertex shader
            GL.ShaderSource(vertexShaderId, vertexSource);
            GL.CompileShader(vertexShaderId);

            // Check vertex shader
            GL.GetShaderInfoLog((int)vertexShaderId, out infoLog);

            if (infoLog.Length > 0)
                throw new ArgumentException("GLMethods.LoadShaders: Failure compiling vertex shader, message: " + infoLog);


            // Compile fragment shader
            GL.ShaderSource(fragmentShaderId, fragmentSource);
            GL.CompileShader(fragmentShaderId);

            // Check fragment shader
            GL.GetShaderInfoLog((int)fragmentShaderId, out infoLog);

            if (infoLog.Length > 0)
                throw new ArgumentException("GLMethods.LoadShaders: Failure compiling fragment shader, message: " + infoLog);


            // Link the program
            uint programId = GL.CreateProgram();

            GL.AttachShader(programId, vertexShaderId);
            GL.AttachShader(programId, fragmentShaderId);
            GL.LinkProgram(programId);

            // Check the program
            GL.GetProgramInfoLog((int)programId, out infoLog);

            if (infoLog.Length > 0)
                throw new ArgumentException("GLMethods.LoadShaders: Failure linking program, message: " + infoLog);


            GL.DetachShader(programId, vertexShaderId);
            GL.DetachShader(programId, fragmentShaderId);
            GL.DeleteShader(vertexShaderId);
            GL.DeleteShader(fragmentShaderId);


            return programId;
        }
    }
}
