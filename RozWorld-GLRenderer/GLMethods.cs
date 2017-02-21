/**
 * Oddmatics.RozWorld.FrontEnd.OpenGL.GLMethods -- RozWorld OpenGL Methods
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

namespace Oddmatics.RozWorld.FrontEnd.OpenGL
{
    /// <summary>
    /// Represents the internal OpenGL methods used by the renderer.
    /// </summary>
    internal static class GLMethods
    {
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
