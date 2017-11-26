/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlFreeTypeService -- RozWorld FreeType OpenGL Service
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

using Pencil.Gaming;
using Pencil.Gaming.Graphics;
using SharpFont;
using System;
using System.Collections.Generic;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents an implementation for FreeType rendering on an RozWorld OpenGL window manager.
    /// </summary>
    internal sealed class RwGlFreeTypeService : IDisposable
    {
        /// <summary>
        /// Gets the FreeType library instance.
        /// </summary>
        public Library FreeTypeLibrary { get; private set; }


        /// <summary>
        /// The storage for fonts and their respective OpenGL texture pointers.
        /// </summary>
        private Dictionary<Face, RwGlTypeFaceData> RenderedFontReference { get; set; }

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
            FreeTypeLibrary = new Library();
            RenderedFontReference = new Dictionary<Face, RwGlTypeFaceData>();
            SharedGlContext = sharedGlContext;
        }


        public RwGlTypeFaceData GetFontCache(Face face)
        {
            if (!RenderedFontReference.ContainsKey(face))
                throw new KeyNotFoundException("The given face has not been rendered yet.");

            return RenderedFontReference[face];
        }

        /// <summary>
        /// Calculates and retrieves VBO data for the given string as the specified font face.
        /// </summary>
        /// <param name="text">The string to provide VBO data for.</param>
        /// <param name="face">The FreeType font face to use.</param>
        public RwGlTypeFaceBufferData GetStringVboData(string text, Face face)
        {
            RwGlTypeFaceData typeFace = null;

            if (RenderedFontReference.ContainsKey(face))
                typeFace = RenderedFontReference[face];
            else
            {
                typeFace = new RwGlTypeFaceData(face);
                RenderedFontReference.Add(face, typeFace);
            }

            return typeFace.GetVboForString(text);
        }


        /// <summary>
        /// Releases all resources used by this RwGlFreeTypeService.
        /// </summary>
        public void Dispose()
        {
            foreach (Face face in RenderedFontReference.Keys)
            {
                face.Dispose();
            }
        }
    }
}
