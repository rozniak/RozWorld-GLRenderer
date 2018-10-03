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
using System.IO;

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Represents a controller for issuing commands to the OpenGL renderer.
    /// </summary>
    internal sealed class RwGlRendererInterface : IRendererInterface
    {
        /// <summary>
        /// The default DPI setting for displaying fonts at.
        /// </summary>
        private const uint DEFAULT_FONT_DPI = 96;


        /// <summary>
        /// The loaded font resources.
        /// </summary>
        private Dictionary<uint, Face> FontResources;

        /// <summary>
        /// The FreeType library instance.
        /// </summary>
        private Library FreeTypeLibrary;

        /// <summary>
        /// The local Random instance used for generating IDs.
        /// </summary>
        private Random LocalRandom;

        /// <summary>
        /// The active instructions for the renderer.
        /// </summary>
        private Dictionary<uint, AbstractRendererInstruction> RenderInstructions;


        /// <summary>
        /// Initializes a new instance of the RwGlRendererInterface class.
        /// </summary>
        public RwGlRendererInterface()
        {
            FontResources = new Dictionary<uint, Face>();
            FreeTypeLibrary = new Library();
            LocalRandom = new Random();
            RenderInstructions = new Dictionary<uint, AbstractRendererInstruction>();
        }


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
            // Check if we already loaded the font face
            //
            foreach (var fontResourceListing in FontResources)
            {
                uint id = fontResourceListing.Key;
                Face face = fontResourceListing.Value;

                if (
                    face.FamilyName == font.FamilyName &&
                    face.Size.Metrics.Height == font.Size
                    )
                {
                    return id;
                }
            }

            // We haven't (or my checking sucks, which is equally likely), so let's try
            // to load the face now
            //
            var newFace = new Face(FreeTypeLibrary, FindFont(font.FamilyName));
            newFace.SetCharSize(0, font.Size, 0, DEFAULT_FONT_DPI);

            // Success - now add to resource cache
            //
            bool validId = false;
            uint newId = 0;

            while (!validId)
            {
                newId = (uint)LocalRandom.Next(1, Int32.MaxValue);

                if (!FontResources.ContainsKey(newId))
                    validId = true;
            }

            FontResources.Add(newId, newFace);

            return newId;
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

        #region Private Methods

        /// <summary>
        /// Locates a font's source file from its specified family name.
        /// </summary>
        /// <param name="familyName">The font's family name.</param>
        /// <returns>The path to the font's source file.</returns>
        private string FindFont(string familyName)
        {
            //
            // FIXME: This function is incredibly lazy and will only work on Windows!!
            //

            string targetFile = @"C:\Windows\Fonts\" + familyName + ".ttf";

            if (!File.Exists(targetFile))
            {
                throw new FileNotFoundException(
                    String.Format(
                        "RwGlRendererInterface.FindFont: Failed to locate source file for {0}.",
                        familyName
                        )
                    );
            }

            return targetFile;
        }

        #endregion
    }
}
