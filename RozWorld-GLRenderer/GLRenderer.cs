using Oddmatics.RozWorld.API.Client;
using Pencil.Gaming;

namespace Oddmatics.RozWorld.FrontEnd.OpenGL
{
    public sealed class GLRenderer : Renderer
    {
        public override bool Initialised { get; protected set; }

        public override byte WindowCount { get; private set; }


        public override void Initialise()
        {
            Glfw.SetErrorCallback(OnError);

            Glfw.WindowHint(WindowHint.ContextVersionMajor, 3);
            Glfw.WindowHint(WindowHint.ContextVersionMinor, 2);
            Glfw.WindowHint(WindowHint.OpenGLForwardCompat, 1);
            Glfw.WindowHint(WindowHint.OpenGLProfile, (int)OpenGLProfile.Core);
            // TODO: Finish this
        }

        public override void SetWindowSize(byte window, short width, short height)
        {
            throw new System.NotImplementedException();
        }

        public override void SetWindows(byte count)
        {
            throw new System.NotImplementedException();
        }


        private void OnError(GlfwError code, string desc)
        {
            // Handle errors here
            Initialised = false;
        }
    }
}
