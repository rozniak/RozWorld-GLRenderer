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

using Oddmatics.RozWorld.API.Client;
using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Oddmatics.RozWorld.FrontEnd.OpenGL
{
    /// <summary>
    /// Represents the OpenGL based renderer that will be loaded by the RozWorld client.
    /// </summary>
    public sealed class GLRenderer : Renderer
    {
        public override bool Initialised { get; protected set; }
        public override byte WindowCount
        {
            get { return (byte)Windows.Count; }
        }


        public GlfwWindowPtr ParentGlfwPointer { get; private set; }
        private List<GLWindow> Windows;


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

        public override void SetWindowSize(byte window, short width, short height)
        {
            // TODO: Implement this
            throw new System.NotImplementedException();
        }

        public override void SetWindows(byte count)
        {
            // TODO: Implement this
            throw new System.NotImplementedException();
        }

        public override void Start()
        {
            var firstWindow = new GLWindow(this, 0, GlfwWindowPtr.Null);
            ParentGlfwPointer = firstWindow.GlfwPointer;
            Windows.Add(firstWindow);

            Glfw.MakeContextCurrent(ParentGlfwPointer);

            // Create VAO initially
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);

            // Create test triangle
            float[] vertexBufferData = new float[] {
                -1.0f, -1.0f, 0.0f,
                1.0f, -1.0f, 0.0f,
                0.0f, 1.0f, 0.0f
            };

            int size = 36;

            int vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(36), vertexBufferData, BufferUsageHint.StaticDraw);
            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
            
            while (!Glfw.WindowShouldClose(ParentGlfwPointer)) // TODO: In future - wait for termination signal from engine
            {
                foreach (GLWindow window in Windows)
                {
                    Glfw.MakeContextCurrent(window.GlfwPointer);

                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    // Do drawing here
                    GL.EnableVertexAttribArray(0);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
                    GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                    GL.DrawArrays(BeginMode.Triangles, 0, 3);
                    GL.DisableVertexAttribArray(0);

                    Glfw.SwapBuffers(window.GlfwPointer);
                }

                Glfw.PollEvents();
            }

            Stop();
        }

        public override void Stop()
        {
            Glfw.Terminate();
            // TODO: Handle terminate signal
        }


        private void OnError(GlfwError code, string desc)
        {
            // Handle errors here
            Initialised = false;

            Glfw.Terminate();
        }
    }
}
