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
using System;
using System.Collections.Generic;

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

            // TODO: Is this all for now?
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
