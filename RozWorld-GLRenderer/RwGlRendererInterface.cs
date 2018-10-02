/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlRendererInterface -- RozWorld OpenGL Renderer Controller Interface
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

using Oddmatics.RozWorld.API.Client.Window;
using SharpFont;
using System;
using System.Collections.Generic;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents a controller for issuing commands to the OpenGL renderer.
    /// </summary>
    internal sealed class RwGlRendererInterface : IRendererInterface
    {
        /// <summary>
        /// The loaded font resources.
        /// </summary>
        private Dictionary<uint, Face> FontResources;

        /// <summary>
        /// The active instructions for the renderer.
        /// </summary>
        private Dictionary<uint, AbstractRendererInstruction> RenderInstructions;


        #region API Implementations

        /// <summary>
        /// Adds an instruction to the render instruction queue.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        /// <returns>The ID of the instruction as it exists in the queue.</returns>
        public uint AddInstruction(AbstractRendererInstruction instruction)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Acquires an ID for a loaded font.
        /// </summary>
        /// <param name="font">The definitions for the font to load.</param>
        /// <returns>The ID of the font resource that was loaded.</returns>
        public uint GetFont(FontInfo font)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Removes an instruction from the render instruction queue.
        /// </summary>
        /// <param name="id">The ID of the instruction.</param>
        public void RemoveInstruction(uint id)
        {
            throw new System.NotImplementedException();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Gets a loaded font face by the internal resource ID.
        /// </summary>
        /// <param name="id">The resource ID of the font face.</param>
        /// <returns>The SharpFont.Face instance that was referred to by the resource ID.</returns>
        public Face GetFaceByResourceId(uint id)
        {
            if (!FontResources.ContainsKey(id))
            {
                throw new KeyNotFoundException(
                    String.Format(
                        "RwGlRendererInterface.GetFaceByResourceId: The ID {0} did not match any loaded font resource.",
                        id
                        )
                    );
            }

            return FontResources[id];
        }

        /// <summary>
        /// Gets the render instructions.
        /// </summary>
        /// <returns>The render instructions as an IList&ltAbstractRendererInstruction&gt; collection.</returns>
        public IList<AbstractRendererInstruction> GetInstructions()
        {
            return new List<AbstractRendererInstruction>(RenderInstructions.Values).AsReadOnly();
        }

        #endregion
    }
}
