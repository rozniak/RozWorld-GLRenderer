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
using Pencil.Gaming.MathUtils;
using SharpFont;
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
        /// The FreeType service instance.
        /// </summary>
        private RwGlFreeTypeService FreeTypeService { get; set; }

        /// <summary>
        /// The collection of windows that are currently active.
        /// </summary>
        private List<RwGlWindow> Windows;


        #region OpenGL Resource Pointers

        // // // //
        // TEST DATA
        //

        private Face TestFontFace;
        private RwGlTypeFaceBufferData TestVbos;

        private int FontDrawVboId;
        private int FontUvVboId;

        private int UniformFontCacheDimensions;
        private int UniformWindowResolution;

        //
        // END TEST DATA
        // // // //

        private uint ProgramId;
        
        private int TranslationMatrixId;

        private int UniformTimeId;
        private float UniformTime;

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
                GL.Uniform2(UniformFontCacheDimensions, FreeTypeService.GetFontCache(TestFontFace).TextureCacheDimensions);
                GL.Uniform2(UniformWindowResolution, new Vector2(Windows[0].Size.Width, Windows[0].Size.Height));

                GL.BindTexture(TextureTarget.Texture2D, FreeTypeService.GetFontCache(TestFontFace).GlTextureId);
                
                // Do drawing here
                GL.EnableVertexAttribArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, FontDrawVboId);
                GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

                GL.EnableVertexAttribArray(1);
                GL.BindBuffer(BufferTarget.ArrayBuffer, FontUvVboId);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                

                GL.DrawArrays(BeginMode.Triangles, 0, TestVbos.DrawVboData.Length);
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

            // Set pixel storage parameters
            //
            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            // Create font service instance
            //
            FreeTypeService = new RwGlFreeTypeService(ParentGlfwPointer);

            // Load font face
            //
            TestFontFace = new Face(FreeTypeService.FreeTypeLibrary, @"C:\Windows\Fonts\Arial.ttf");
            TestFontFace.SetCharSize(0, 36, 0, 96);

            // // // // //
            // TEST DATA - TESTING FONT
            // 

            TestVbos = FreeTypeService.GetStringVboData("3", TestFontFace);

            FontDrawVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, FontDrawVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * TestVbos.DrawVboData.Length), TestVbos.DrawVboData, BufferUsageHint.StaticDraw);

            FontUvVboId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, FontUvVboId);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * TestVbos.UvVboData.Length), TestVbos.UvVboData, BufferUsageHint.StaticDraw);

            //
            // END TEST DATA
            // // // // //


            // Set up remaining OpenGL stuff
            //
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            
            ProgramId = RwGlMethods.LoadShaders(
                File.ReadAllText(Environment.CurrentDirectory + @"\renderers\OpenGL\GlShaders\Default-VertexShader.glsl"),
                File.ReadAllText(Environment.CurrentDirectory + @"\renderers\OpenGL\GlShaders\Default-FragmentShader.glsl")
                );

            //
            // TEMP: Get FontCacheDimensions
            //
            UniformFontCacheDimensions = GL.GetUniformLocation(ProgramId, "FontCacheDimensions");

            // Get WindowResolution
            UniformWindowResolution = GL.GetUniformLocation(ProgramId, "WindowResolution");

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

            GL.Viewport(0, 0, firstWindow.Size.Width, firstWindow.Size.Height);

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
            FreeTypeService.Dispose();

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
