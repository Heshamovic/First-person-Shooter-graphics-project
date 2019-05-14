using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using Graphics._3D_Models;
using System.Windows.Forms;

namespace Graphics
{
    class Start_Screen : Screen
    {
        string projectPath;
        mat4 healthbar;
        Shader shader2D;
        Texture bg;
        uint bgID;
        int mloc;
        public Camera cam;
        public override void Close()
        {
            shader2D.DestroyShader();
        }
        public override void Draw()
        {
            shader2D.UseShader();
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, bgID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), IntPtr.Zero);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            bg.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
        }
        public override void Initialize()
        {
            projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
           // bg = new Texture(projectPath + "\\Resources\battlefield_1_they_shall_not_pass_dlc - wide.jpg", 10, false);
            bg = new Texture(projectPath + "\\Textures\\HP.bmp", 9, true);
            Gl.glClearColor(0, 1, 0, 1);
            float[] backGroundVertices = {
                0, 0, 0,
                0, 0,
                1, 0, 0,
                1, 0,
                1, 1, 0,
                1, 1,
                1, 1, 0,
                1, 1,
                0, 1, 0,
                0, 1,
                0, 0, 0,
                0, 0
            };
            bgID = GPU.GenerateBuffer(backGroundVertices);

            cam = new Camera();
            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");
        }
        public override void Load()
        { }
        public override void Update()
        {
        }
    }
}
