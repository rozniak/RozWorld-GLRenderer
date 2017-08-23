/**
 * Oddmatics.RozWorld.FrontEnd.OpenGL.GLRenderer -- RozWorld OpenGL Renderer
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */
 
using Oddmatics.RozWorld.API.Client.Graphics;
using Oddmatics.RozWorld.API.Generic;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Timers;

namespace Oddmatics.RozWorld.FrontEnd.OpenGL
{
    /// <summary>
    /// Represents the OpenGL based renderer that will be loaded by the RozWorld client.
    /// </summary>
    public sealed class GLRenderer : Renderer
    {
        /// <summary>
        /// Gets the value that indicates whether this renderer has been initialised.
        /// </summary>
        public override bool Initialised { get; protected set; }

        /// <summary>
        /// Gets the amount of windows active in this renderer.
        /// </summary>
        public override byte WindowCount
        {
            get { return (byte)Windows.Count; }
        }


        /// <summary>
        /// The GLFW pointer to the parent window of this renderer.
        /// </summary>
        public GlfwWindowPtr ParentGlfwPointer { get; private set; }

        /// <summary>
        /// The collection of windows that are currently active.
        /// </summary>
        private List<GLWindow> Windows;


        #region OpenGL Resource Pointers

        private uint ProgramId;

        // Testing purposes
        private int TilemapBuffer;

        private float[] TilemapVertexData;

        private int TranslationMatrixId;

        private int UniformTimeId;
        private float UniformTime;

        private int VertexUVBuffer;

        #endregion


        /// <summary>
        /// Occurs when the user closes this renderer's last window.
        /// </summary>
        public override event EventHandler Closed;



        /// <summary>
        /// Gets the render context of a window.
        /// </summary>
        /// <param name="window">The index of the window.</param>
        /// <returns>The IRendererContext used by the window if it was found, null otherwise.</returns>
        public override IRendererContext GetContext(byte window)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initialises this renderer.
        /// </summary>
        /// <returns>True if the renderer was successfully initialised.</returns>
        public override bool Initialise()
        {
            if (Initialised)
                throw new InvalidOperationException("GLRenderer.Initialise: The renderer is already initialised.");

            Windows = new List<GLWindow>();

            Glfw.Init();

            Glfw.SetErrorCallback(OnError);

            Glfw.WindowHint(WindowHint.ContextVersionMajor, 3);
            Glfw.WindowHint(WindowHint.ContextVersionMinor, 2);
            Glfw.WindowHint(WindowHint.OpenGLForwardCompat, 1);
            Glfw.WindowHint(WindowHint.OpenGLProfile, (int)OpenGLProfile.Core);
            
            Initialised = true;

            return true; // For now - this should report back if there were any problems
        }

        /// <summary>
        /// Loads a font from the specified filepath and point size, and maps it to the given identifier.
        /// </summary>
        /// <param name="filepath">The filepath of the font.</param>
        /// <param name="pointSize">The point size to load.</param>
        /// <param name="identifier">The font identifier.</param>
        /// <returns>Success if the font was loaded and mapped to the identifier.</returns>
        public override RwResult LoadFont(string filepath, int pointSize, string identifier)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Renders the next frame to all contexts.
        /// </summary>
        public override void RenderFrame()
        {
            if (Glfw.WindowShouldClose(ParentGlfwPointer))
                Stop();

            UniformTime += (float)Glfw.GetTime();
            Glfw.SetTime(0);

            foreach (GLWindow window in Windows)
            {
                Glfw.MakeContextCurrent(window.GlfwPointer);

                GL.Clear(ClearBufferMask.ColorBufferBit);

                GL.UseProgram(ProgramId);
                GL.Uniform1(UniformTimeId, UniformTime);

                // Do drawing here
                GL.EnableVertexAttribArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, TilemapBuffer);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.EnableVertexAttribArray(1);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexUVBuffer);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);


                GL.DrawArrays(BeginMode.Triangles, 0, TilemapVertexData.Length);
                GL.DisableVertexAttribArray(0);

                Glfw.SwapBuffers(window.GlfwPointer);
            }

            Glfw.PollEvents();
        }

        /// <summary>
        /// Sets the amount of windows in this renderer.
        /// </summary>
        /// <param name="count">The amount of windows.</param>
        public override void SetWindows(byte count)
        {
            // TODO: Implement this
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Sets the size of a window.
        /// </summary>
        /// <param name="window">The index of the window.</param>
        /// <param name="width">The new width.</param>
        /// <param name="height">The new height</param>
        public override void SetWindowSize(byte window, short width, short height)
        {
            // TODO: Implement this
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Starts this renderer.
        /// </summary>
        public override void Start()
        {
            var firstWindow = new GLWindow(this, 0, GlfwWindowPtr.Null);
            ParentGlfwPointer = firstWindow.GlfwPointer;
            Windows.Add(firstWindow);

            Glfw.MakeContextCurrent(ParentGlfwPointer);

            // Create VAO initially
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);


            // Create test tilemap
            const int tileWidth = 64;
            const int tileHeight = 64;
            const int tilemapWidth = 32;
            const int tilemapHeight = 32;
            TilemapVertexData = new float[tilemapWidth * tilemapHeight * 6 * 2];

            for (int y = 0; y < tilemapHeight; y++)
            {
                for (int x = 0; x < tilemapWidth; x++)
                {
                    float xCoordLeft = ((tileWidth * x) / (float)firstWindow.Size.Width) * 2 - 1f;
                    float xCoordRight = ((tileWidth * (x + 1)) / (float)firstWindow.Size.Width) * 2 - 1f;
                    float yCoordBottom = ((tileHeight * y) / (float)firstWindow.Size.Height) * 2 - 1f;
                    float yCoordTop = ((tileHeight * (y + 1)) / (float)firstWindow.Size.Height) * 2 - 1f;

                    int baseIndex = (y * tilemapWidth * 12) + (x * 12);

                    float[] quad = new float[] {
                        xCoordLeft, yCoordBottom,
                        xCoordLeft, yCoordTop,
                        xCoordRight, yCoordTop,

                        xCoordRight, yCoordTop,
                        xCoordLeft, yCoordBottom,
                        xCoordRight, yCoordBottom
                    };

                    // Copy data between arrays
                    Array.Copy(quad, 0, TilemapVertexData, baseIndex, 12);
                }
            }

            TilemapBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, TilemapBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * TilemapVertexData.Length), TilemapVertexData, BufferUsageHint.StaticDraw);


            // Create test UVs
            float[] uvQuadData = new float[] {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                1.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f
            };
            float[] uvVertexData = new float[TilemapVertexData.Length];

            for (int quad = 0; quad < uvVertexData.Length; quad += 12)
            {
                Array.Copy(uvQuadData, 0, uvVertexData, quad, 12);
            }
            
            

            VertexUVBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexUVBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * uvVertexData.Length), uvVertexData, BufferUsageHint.StaticDraw);

            uint textureId = GLMethods.LoadTexture((Bitmap)Bitmap.FromFile(Environment.CurrentDirectory + @"\gl\sample.bmp"));

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            

            ProgramId = GLMethods.LoadShaders(
                File.ReadAllText(Environment.CurrentDirectory + @"\gl\vertex.glsl"),
                File.ReadAllText(Environment.CurrentDirectory + @"\gl\fragment.glsl")
                );

            // Get fTime
            UniformTimeId = GL.GetUniformLocation(ProgramId, "fTime");
            UniformTime = 0f;

            // Get TranslationMatrix
            TranslationMatrixId = GL.GetUniformLocation(ProgramId, "TranslationMatrix");
            GL.UniformMatrix4(TranslationMatrixId, 1, false, new float[] {
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 0, 0,
                0, 0, 0, 1
            });
        }

        /// <summary>
        /// Stops this renderer.
        /// </summary>
        public override void Stop()
        {
            Glfw.Terminate();

            Closed?.Invoke(this, EventArgs.Empty);
        }

        
        /// <summary>
        /// [Event] GLFW/GL error occurred.
        /// </summary>
        private void OnError(GlfwError code, string desc)
        {
            // Handle errors here
            Stop();
        }
    }
}
