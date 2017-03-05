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
using System.IO;
using System.Drawing;

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


            // Create test tilemap
            const int tileWidth = 64;
            const int tileHeight = 64;
            const int tilemapWidth = 32;
            const int tilemapHeight = 32;
            float[] tilemapVertexData = new float[tilemapWidth * tilemapHeight * 6 * 2];

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
                    Array.Copy(quad, 0, tilemapVertexData, baseIndex, 12);
                }
            }

            int tilemapBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, tilemapBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * tilemapVertexData.Length), tilemapVertexData, BufferUsageHint.StaticDraw);


            // Create test UVs
            float[] uvQuadData = new float[] {
                0.0f, 0.0f,
                0.0f, 1.0f,
                1.0f, 1.0f,

                1.0f, 1.0f,
                0.0f, 0.0f,
                1.0f, 0.0f
            };
            float[] uvVertexData = new float[tilemapVertexData.Length];

            for (int quad = 0; quad < uvVertexData.Length; quad += 12)
            {
                Array.Copy(uvQuadData, 0, uvVertexData, quad, 12);
            }
            
            

            int vertexUVBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexUVBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * uvVertexData.Length), uvVertexData, BufferUsageHint.StaticDraw);

            uint textureId = GLMethods.LoadTexture((Bitmap)Bitmap.FromFile(Environment.CurrentDirectory + @"\gl\sample.bmp"));

            GL.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);

            

            uint programId = GLMethods.LoadShaders(File.ReadAllText(Environment.CurrentDirectory + @"\gl\vertex.glsl"),
                File.ReadAllText(Environment.CurrentDirectory + @"\gl\fragment.glsl"));

            int uniformTimeId = GL.GetUniformLocation(programId, "fTime");
            float uniformTime = 0;
            
            while (!Glfw.WindowShouldClose(ParentGlfwPointer)) // TODO: In future - wait for termination signal from engine
            {
                uniformTime += (float)Glfw.GetTime();
                Glfw.SetTime(0);

                foreach (GLWindow window in Windows)
                {
                    Glfw.MakeContextCurrent(window.GlfwPointer);

                    GL.Clear(ClearBufferMask.ColorBufferBit);

                    GL.UseProgram(programId);
                    GL.Uniform1(uniformTimeId, uniformTime);

                    // Do drawing here
                    GL.EnableVertexAttribArray(0);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, tilemapBuffer);
                    GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);

                    GL.EnableVertexAttribArray(1);
                    GL.BindBuffer(BufferTarget.ArrayBuffer, vertexUVBuffer);
                    GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);


                    GL.DrawArrays(BeginMode.Triangles, 0, tilemapVertexData.Length);
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
