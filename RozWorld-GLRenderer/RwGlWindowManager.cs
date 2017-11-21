/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlWindowManager -- RozWorld OpenGL Window Manager
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

using Oddmatics.RozWorld.API.Client;
using Oddmatics.RozWorld.API.Client.Window;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents the OpenGL based renderer that will be loaded by the RozWorld client.
    /// </summary>
    public sealed class RwGlWindowManager : IWindowManager
    {
        /// <summary>
        /// Gets the 'nice' name of this window manager.
        /// </summary>
        public string NiceName { get { return "RozWorld OpenGL 3.2 Window Manager"; } }

        /// <summary>
        /// Gets the reference to the parent RozWorld client instance.
        /// </summary>
        public IRwClient ParentClient { get; set; }

        /// <summary>
        /// The GLFW pointer to the parent window of this renderer.
        /// </summary>
        public GlfwWindowPtr ParentGlfwPointer { get; private set; }

        /// <summary>
        /// Gets the amount of windows active in this renderer.
        /// </summary>
        public byte WindowCount
        {
            get { return (byte)Windows.Count; }
            set { throw new NotImplementedException(); }
        }


        /// <summary>
        /// The current input state.
        /// </summary>
        private InputUpdate CurrentInputState { get; set; }

        /// <summary>
        /// The collection of windows that are currently active.
        /// </summary>
        private List<RwGlWindow> Windows;


        #region OpenGL Resource Pointers

        private int TilemapBuffer;

        private float[] TilemapVertexData;

        private uint ProgramId;

        // Testing purposes
        private int TranslationMatrixId;

        private int UniformTimeId;
        private float UniformTime;

        private int VertexUVBuffer;

        #endregion


        /// <summary>
        /// Occurs when the user closes this renderer's last window.
        /// </summary>
        public event EventHandler Closed;
        

        /// <summary>
        /// Initializes a new instance of the RwGlWindowManager class.
        /// </summary>
        public RwGlWindowManager()
        {
            CurrentInputState = new InputUpdate();
            Windows = new List<RwGlWindow>();
        }


        /// <summary>
        /// Retrieves the latest input state from this window manager.
        /// </summary>
        /// <returns>The latest input state.</returns>
        public InputUpdate GetInputEvents()
        {
            var thisUpdate = CurrentInputState;

            thisUpdate.FinalizeForReporting();
            
            CurrentInputState = new InputUpdate(thisUpdate.DownedInputs);

            return thisUpdate;
        }

        /// <summary>
        /// Renders the next game frame.
        /// </summary>
        public void RenderFrame()
        {
            if (Glfw.WindowShouldClose(ParentGlfwPointer))
                Stop();

            UniformTime += (float)Glfw.GetTime();
            Glfw.SetTime(0);

            foreach (RwGlWindow window in Windows)
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
        /// Starts this renderer.
        /// </summary>
        public bool Start(IRwClient clientReference)
        {
            // Add client reference first
            //
            ParentClient = clientReference;

            // Set up GLFW
            //
            Glfw.Init();

            Glfw.SetErrorCallback(OnError);

            Glfw.WindowHint(WindowHint.ContextVersionMajor, 3);
            Glfw.WindowHint(WindowHint.ContextVersionMinor, 2);
            Glfw.WindowHint(WindowHint.OpenGLForwardCompat, 1);
            Glfw.WindowHint(WindowHint.OpenGLProfile, (int)OpenGLProfile.Core);

            var firstWindow = new RwGlWindow(this, 0, GlfwWindowPtr.Null, OnChar, OnKey);

            ParentGlfwPointer = firstWindow.GlfwPointer;
            Windows.Add(firstWindow);

            Glfw.MakeContextCurrent(ParentGlfwPointer);

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

            // Create VAO initially
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

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

            uint textureId = RwGlMethods.LoadTexture((Bitmap)Bitmap.FromFile(Environment.CurrentDirectory + @"\gl\sample.bmp"));

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            
            ProgramId = RwGlMethods.LoadShaders(
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

            return true;
        }

        /// <summary>
        /// Stops this renderer.
        /// </summary>
        public void Stop()
        {
            // Destroy client reference
            //
            ParentClient = null;

            Glfw.Terminate();

            Closed?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// [Event] GLFW filtered keyboard event occurred.
        /// </summary>
        private void OnChar(GlfwWindowPtr wnd, char ch)
        {
            CurrentInputState.ReportConsoleInput(ch);
        }

        /// <summary>
        /// [Event] GLFW/GL error occurred.
        /// </summary>
        private void OnError(GlfwError code, string desc)
        {
            // Handle errors here
            Stop();
        }

        /// <summary>
        /// [Event] GLFW keyboard event occurred.
        /// </summary>
        private void OnKey(GlfwWindowPtr wnd, Key key, int scanCode, KeyAction action, KeyModifiers mods)
        {
            string inputString = "vk." + key.ToString();

            if (action == KeyAction.Press)
                CurrentInputState.ReportPress(inputString);
            else if (action == KeyAction.Release)
                CurrentInputState.ReportRelease(inputString);
        }
    }
}
