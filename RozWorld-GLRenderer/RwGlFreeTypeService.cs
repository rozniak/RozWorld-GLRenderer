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

using Pencil.Gaming;
using SharpFont;
using System;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents an implementation for FreeType rendering on an RozWorld OpenGL window manager.
    /// </summary>
    public sealed class RwGlFreeTypeService : IDisposable
    {
        /// <summary>
        /// The shared OpenGL context pointer.
        /// </summary>
        private GlfwWindowPtr SharedGlContext { get; set; }


        /// <summary>
        /// Initializes a new instance of the RwGlFreeTypeService class.
        /// </summary>
        /// <param name="sharedGlContext">The shared OpenGL context pointer.</param>
        public RwGlFreeTypeService(GlfwWindowPtr sharedGlContext)
        {
            SharedGlContext = sharedGlContext;
        }


        /// <summary>
        /// Releases all resources used by this RwGlFreeTypeService.
        /// </summary>
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
