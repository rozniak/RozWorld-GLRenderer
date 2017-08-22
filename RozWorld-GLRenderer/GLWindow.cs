/**
 * Oddmatics.RozWorld.FrontEnd.OpenGL.GLWindow -- RozWorld OpenGL Window
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

using Oddmatics.RozWorld.API.Generic;
using Pencil.Gaming;

namespace Oddmatics.RozWorld.FrontEnd.OpenGL
{
    /// <summary>
    /// Represents a single GLFW-based OpenGL window.
    /// </summary>
    public class GLWindow
    {
        public GlfwWindowPtr GlfwPointer { get; private set; }
        public byte Id { get; private set; }
        public RwSize Size { get; set; }

        private readonly GLRenderer Parent;


        public GLWindow(GLRenderer parent, byte windowId, GlfwWindowPtr sharedContext)
        {
            Size = RwCore.Client.DisplayResolutions[windowId];

            GlfwPointer = Glfw.CreateWindow(Size.Width, Size.Height,
                RwCore.Client.ClientWindowTitle, GlfwMonitorPtr.Null, sharedContext);

            Parent = parent;
            Id = windowId;
        }
    }
}
