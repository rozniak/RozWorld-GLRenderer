/**
 * Oddmatics.RozWorld.FrontEnd.OpenGl.RwGlTypeFaceBufferData -- RozWorld FreeType String OpenGL VBO Data
 *
 * This source-code is part of the OpenGL renderer for the RozWorld project by rozza of Oddmatics:
 * <<http://www.oddmatics.uk>>
 * <<http://roz.world>>
 * <<http://github.com/rozniak/RozWorld-GLRenderer>>
 *
 * Sharing, editing and general licence term information can be found inside of the "LICENCE.MD" file that should be located in the root of this project's directory structure.
 */

namespace Oddmatics.RozWorld.FrontEnd.OpenGl
{
    /// <summary>
    /// Contains vertex buffer data for the drawing and UV coordinates.
    /// </summary>
    internal struct RwGlTypeFaceBufferData
    {
        public float[] DrawVboData { get; private set; }

        public float[] UvVboData { get; private set; }


        public RwGlTypeFaceBufferData(float[] drawVboData, float[] uvVboData)
        {
            DrawVboData = drawVboData;
            UvVboData = uvVboData;
        }
    }
}
