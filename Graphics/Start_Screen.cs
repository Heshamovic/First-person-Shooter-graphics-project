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
        Shader sh;
        public Camera cam;
        public bool close = false;
        mat4 ProjectionMatrix, ViewMatrix;
        Texture hp, startBtn;
        uint hpID, startBtnID;
        mat4 healthbar, startBtnMat4;
        Shader shader2D;
        int mloc;
        public string projectPath;
        
        
        public override void Initialize()
        {
            projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            hp = new Texture(projectPath + "\\Resources\\battlefield_1_they_shall_not_pass_dlc-wide.jpg", 9, false);
            startBtn = new Texture(projectPath + "\\Resources\\startButton.jpg", 10, false);
            sh.UseShader();
            
            float[] squarevertices = {
                -1,1,0,
                0,0,
                1,-1,0,
                1,1,
                -1,-1,0,
                0,1,
                1,1,0,
                1,0,
                -1,1,0,
                0,0,
                1,-1,0,
                1,1
            };

            float[] startVertices = {
                0.55f, 0.75f, 0,
                0, 0,
                0.8f, 0.5f, 0,
                1, 1,
                0.55f, 0.5f,0,
                0, 1,
                0.8f, 0.75f, 0,
                1, 0,
                0.55f, 0.75f, 0,
                0, 0,
                0.8f, 0.5f, 0,
                1, 1
            };
            startBtnMat4 = glm.scale(new mat4(1), new vec3(1, 1, 1));
            healthbar = glm.scale(new mat4(1), new vec3(1, 1, 1));
            // shader2D.UseShader();
            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");

            startBtnID = GPU.GenerateBuffer(startVertices);
            hpID = GPU.GenerateBuffer(squarevertices);
            

            Gl.glClearColor(0, 0, 0, 1);

            cam = new Camera();
            cam.Reset(555, 34, 55, 5000, -400, 4000, 0, 1, 0);
            

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();


            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public override void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

         
            Gl.glDisable(Gl.GL_BLEND);



            Gl.glDisable(Gl.GL_DEPTH_TEST);
            shader2D.UseShader();
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            
            healthbar = glm.scale(new mat4(1), new vec3(1, 1, 1));
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
            hp.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);


            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, startBtnID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            startBtnMat4 = glm.scale(new mat4(1), new vec3(1, 1, 1));
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, startBtnMat4.to_array());
            startBtn.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }
        public override void Update()
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            
        }

        public override void Close()
        {
            sh.DestroyShader();
            shader2D.DestroyShader();
        }
        public override void Load()
        { }
    }
}

