﻿using System;
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
    class Loading_Screen : Screen
    {
        public bool close = false;
        Texture BG, loadingBar;
        uint BGID, loadingBarID;
        mat4 BGMat4, loadingBarMat4;
        public Shader shader2D;
        int mloc;
        public float loadingBarProgress;
        public string projectPath;


        public override void Initialize()
        {
            loadingBarProgress = 0;
            projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
            BG = new Texture(projectPath + "\\Resources\\639069385_preview_loading_screen.jpg", 9, false);
            loadingBar = new Texture(projectPath + "\\Resources\\loadingBar.jpg", 10, false);

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
            float[] loadingVertices = {
                -0.5f, -0.75f, 0,
                0, 0,
                0.5f, -0.5f, 0,
                1, 1,
                -0.5f, -0.5f,0,
                0, 1,
                0.5f, -0.75f, 0,
                1, 0,
                -0.5f, -0.75f, 0,
                0, 0,
                0.5f, -0.5f, 0,
                1, 1
            };
            loadingBarMat4 = glm.scale(new mat4(1), new vec3(1, 1, 1));
            BGMat4 = glm.scale(new mat4(1), new vec3(1, 1, 1));

            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");
            loadingBarID = GPU.GenerateBuffer(loadingVertices);
            BGID = GPU.GenerateBuffer(squarevertices);
            Gl.glClearColor(0, 0, 0, 1);
   
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public override void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            Gl.glDisable(Gl.GL_BLEND);


            Gl.glDisable(Gl.GL_DEPTH_TEST);
            shader2D.UseShader();
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, BGID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);


            BGMat4 = glm.scale(new mat4(1), new vec3(1, 1, 1));
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, BGMat4.to_array());
            BG.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);


            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, loadingBarID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            loadingBarMat4 = glm.scale(new mat4(1), new vec3(1, 1, 1));
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, loadingBarMat4.to_array());
            loadingBar.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }
        public override void Update()
        {

        }

        public override void Close()
        {
            shader2D.DestroyShader();
        }
        public override void Load()
        { }
    }
}

