/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlWindow -- RozWorld OpenGL Window
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

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents a single GLFW-based OpenGL window.
    /// </summary>
    public sealed class RwGlWindow
    {
        /// <summary>
        /// Gets the GLFW pointer of this window.
        /// </summary>
        public GlfwWindowPtr GlfwPointer { get; private set; }

        /// <summary>
        /// Gets ID of this window.
        /// </summary>
        public byte Id { get; private set; }

        /// <summary>
        /// Gets the size of this window.
        /// </summary>
        public RwSize Size { get; set; }


        /// <summary>
        /// The parent renderer instance.
        /// </summary>
        private readonly RwGlWindowManager Parent;


        /// <summary>
        /// Initializes a new instance of the GLWindow class.
        /// </summary>
        /// <param name="parent">The parent renderer instance.</param>
        /// <param name="windowId">The ID of this window.</param>
        /// <param name="sharedContext">The GLFW pointer to use for sharing GL contexts.</param>
        public RwGlWindow(RwGlWindowManager parent, byte windowId, GlfwWindowPtr sharedContext)
        {
            Size = parent.ParentClient.DisplayResolutions[windowId];

            GlfwPointer = Glfw.CreateWindow(Size.Width, Size.Height,
                parent.ParentClient.ClientWindowTitle, GlfwMonitorPtr.Null, sharedContext);

            Parent = parent;
            Id = windowId;
        }
    }
}
