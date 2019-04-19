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
namespace Graphics
{
    class Renderer
    {
        Shader sh;
        uint groundtextBufferID2;
        uint groundtextBufferID1;//grass
        uint groundtextBufferID3;//wall
        uint groundtextBufferID4;//sky
        Texture dn , upp , lf , rt , bk , ft ;
        int modelID;
        int viewID;
        int projID;
        int transID;

        int EyePositionID;
        int AmbientLightID;
        int DataID;

        mat4 ProjectionMatrix;
        mat4 ViewMatrix;
        mat4 down , up , left , right , front , back;
        public float Speed = 1;

        Model3D building , house, building2;
        Model3D car, scar, Lara;
        Model3D tree , tree1;
        public md2LOL zombie;
        public md2LOL blade, blade1, blade2, fofa;


        public Camera cam;

        public void Initialize()
        {

            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            
            dn = new Texture(projectPath + "\\Textures\\sandcastle_dn.png", 2, true);
            lf = new Texture(projectPath + "\\Textures\\sandcastle_lf.png", 2, true);
            rt = new Texture(projectPath + "\\Textures\\sandcastle_rt.png", 2, true);
            upp = new Texture(projectPath + "\\Textures\\sandcastle_up.png", 2, true);
            ft = new Texture(projectPath + "\\Textures\\sandcastle_ft.png", 2, true);
            bk = new Texture(projectPath + "\\Textures\\sandcastle_bk.png", 2, true);

          /*  float[] ground = {
                -1,0,1,    1,0,0,
                1,0,1,     1,0,0,
                -1,0,-1,   1,0,0,
                1,0,1,     1,0,0,
                -1,0,-1,   1,0,0,
                1,0,-1,    1,0,0
            };*/
            float groundX = 1, groundY = 0, groundZ = 1;

            float[] ground = {
                -groundX, -groundY, groundZ,    1,0,0,
                 groundX, -groundY, -groundZ,   1,0,0,
                -groundX, -groundY, -groundZ,    1,0,0,
                                               
                 groundX, -groundY, groundZ, 1,0,0,
                -groundX, -groundY, groundZ, 1,0,0,
                 groundX, -groundY, -groundZ, 1,0,0
            };
            int gsize = 1;
            float[] groundTex = {
                0,gsize,
                gsize,0,
                0,0,
                gsize,gsize,
                0,gsize,
                gsize,0,
            };
            int wallsize = 1;
            float[] wallTex = {
                0,wallsize,
                wallsize,0,
                0,0,
                wallsize,wallsize,
                0,wallsize,
                wallsize,0,
            };
            float skysize = 1;
            float[] skyTex = {
                0,skysize,
                skysize,0,
                0,0,
                skysize,skysize,
                0,skysize,
                skysize,0,
            };
            groundtextBufferID2 = GPU.GenerateBuffer(ground);
            groundtextBufferID1 = GPU.GenerateBuffer(groundTex);
            groundtextBufferID3 = GPU.GenerateBuffer(wallTex);
            groundtextBufferID4 = GPU.GenerateBuffer(skyTex);

            // Add a buidling model, file name is "Building 02.obj"
            // Steps: 
            // 1- make a new instance of Model3D class, and name the variable as building
            // 2- load the building object "Building 02.obj"
            // 3- Add scaling and translation accordingly (for the building most probably you will not need a rotation)
            int sz = 25000;
            down = MathHelper.MultiplyMatrices(new List<mat4>()
            {    
                glm.scale(new mat4(1), new vec3(sz, sz, sz)),
                glm.translate(new mat4(1),new vec3(0,-400,0))
                
            });
            up = MathHelper.MultiplyMatrices(new List<mat4>()
            {    
                 glm.scale(new mat4(1), new vec3(sz, sz, sz)),
                glm.translate(new mat4(1),new vec3(0,sz + sz - 450 ,0))
                
            });
            left = MathHelper.MultiplyMatrices(new List<mat4>()
            {    
                glm.scale(new mat4(1), new vec3(sz, sz, sz)),
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(0, 0, 1)) ,
                glm.translate(new mat4(1),new vec3(-sz,sz - 400,0)),
            });
            right = MathHelper.MultiplyMatrices(new List<mat4>()
            {    
                glm.scale(new mat4(1), new vec3(sz, sz, sz)),
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(0, 0, 1)) ,
                glm.translate(new mat4(1),new vec3(sz,sz - 400,0))
                
            });
            front = MathHelper.MultiplyMatrices(new List<mat4>()
            {    
                glm.scale(new mat4(1), new vec3(sz, sz, sz)),
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(1, 0, 0)) ,
                glm.translate(new mat4(1),new vec3(0,sz - 400,-sz))
                
            });
            back = MathHelper.MultiplyMatrices(new List<mat4>()
            {    
                glm.scale(new mat4(1), new vec3(sz, sz, sz)),
                glm.rotate(90.0f / 180.0f * 3.1412f, new vec3(1, 0, 0)) ,
                glm.translate(new mat4(1),new vec3(0,sz - 400,sz))
                
            });

            building = new Model3D();
            building.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 1);
            building.scalematrix = glm.scale(new mat4(1), new vec3(300, 300, 300));
            building.transmatrix = glm.translate(new mat4(1), new vec3(1, 1, 1));

            building2 = new Model3D();
            building2.LoadFile(projectPath + "\\ModelFiles\\M4", "guard post.3ds", 10);
            building2.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            building2.scalematrix = glm.scale(new mat4(1), new vec3(400, 400, 800));
            building2.transmatrix = glm.translate(new mat4(1), new vec3(10000, 1, 500));

            house = new Model3D();
            house.LoadFile(projectPath + "\\ModelFiles\\static\\House", "house.obj", 1);
            house.scalematrix = glm.scale(new mat4(1), new vec3(300, 300, 300));
            house.transmatrix = glm.translate(new mat4(1), new vec3(4000, -400, 4000));
            ////////////////////
            //// Similarly Add a tree inside the building, file name is "Tree.obj"


            tree = new Model3D();
            tree.LoadFile(projectPath + "\\ModelFiles\\static\\tree", "tree.obj", 4);
            tree.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            tree.transmatrix = glm.translate(new mat4(1), new vec3(1, -400, 4000));


            tree1 = new Model3D();
            tree1.LoadFile(projectPath + "\\ModelFiles\\static\\tree\\Tree", "Tree.fbx", 22);
            tree1.scalematrix = glm.scale(new mat4(1), new vec3(200, 200, 200));
            tree1.transmatrix = glm.translate(new mat4(1), new vec3(1500, -400, 4000));

            Lara = new Model3D();
            Lara.LoadFile(projectPath + "\\ModelFiles\\Heshambyhbd\\Box", "box.obj", 27);
            Lara.scalematrix = glm.scale(new mat4(1), new vec3(50, 50, 50));
            Lara.transmatrix = glm.translate(new mat4(1), new vec3(1000, -200, 1231));
            ////////////////////
            //// Similarly Add a car outside the building, file name is "dvp.obj"

            car = new Model3D();
            car.LoadFile(projectPath + "\\ModelFiles\\static\\car", "dpv.obj", 3);
            car.scalematrix = glm.scale(new mat4(1), new vec3(1, 1, 1));
            car.transmatrix = glm.translate(new mat4(1), new vec3(-500, 1, -100));
            car.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));

            scar = new Model3D();
            scar.LoadFile(projectPath + "\\ModelFiles\\scar", "Scar-X.obj.obj", 10);
            scar.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            scar.transmatrix = glm.translate(new mat4(1), new vec3(500, 1, 500));
            
            ////////////////////
            ////////////////////
            //// Add an animated model, file name is "zombie.md2" with animation: "Stand"
            /// Steps:
            // 1- make a new instance of MD2LOL or MD2 class, while instanciating the new model, it loads the model
            // 2- start animating the object using StartAnimation method which is inside the md2/md2LOL 
            // 3- Add rotation, scaling and translation accordingly 

            zombie = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            zombie.StartAnimation(animType_LOL.STAND);
            zombie.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            zombie.scaleMatrix = glm.scale(new mat4(1), new vec3(10, 10, 10));
            zombie.TranslationMatrix = glm.translate(new mat4(1), new vec3(1, 1, 1));

            ////////////////////
            /// Similarly, Add an animated model, file name is "Blade.md2" with animation: "Run"

            blade = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            blade.StartAnimation(animType_LOL.STAND);
            blade.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            blade.scaleMatrix = glm.scale(new mat4(1), new vec3(5, 5, 5));
            blade.TranslationMatrix = glm.translate(new mat4(1), new vec3(1, 1, 1));

            blade1 = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            blade1.StartAnimation(animType_LOL.ATTACK1);
            blade1.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            blade1.scaleMatrix = glm.scale(new mat4(1), new vec3(10, 10, 10));
            blade1.TranslationMatrix = glm.translate(new mat4(1), new vec3(4000, -400, 4000));

            blade2 = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            blade2.StartAnimation(animType_LOL.RUN);
            blade2.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            blade2.scaleMatrix = glm.scale(new mat4(1), new vec3(5, 5, 5));
            blade2.TranslationMatrix = glm.translate(new mat4(1), new vec3(1000, -400, 1031));

            fofa = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            fofa.StartAnimation(animType_LOL.STAND);
            fofa.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            fofa.scaleMatrix = glm.scale(new mat4(1), new vec3(20, 20, 20));
            fofa.TranslationMatrix = glm.translate(new mat4(1), new vec3(10000, 1, 500));
            //////////////////////////////////////////////////////////////////////////////

            Gl.glClearColor(0, 0, 0, 1);
            
             



            
            cam = new Camera();
            cam.Reset(0, 34, 55, 0, 0, 0, 0, 1, 0);

            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            transID = Gl.glGetUniformLocation(sh.ID, "model");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");

            sh.UseShader();


            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public void Draw()
        {

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT|Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();
           
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, down.to_array());
            Gl.glUniformMatrix4fv(projID, 1, Gl.GL_FALSE, ProjectionMatrix.to_array());
            Gl.glUniformMatrix4fv(viewID, 1, Gl.GL_FALSE, ViewMatrix.to_array());

            Gl.glUniform3fv(EyePositionID, 1, cam.GetCameraPosition().to_array());

           

            GPU.BindBuffer(groundtextBufferID2);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), IntPtr.Zero);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 6 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            GPU.BindBuffer(groundtextBufferID1);
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);

           
            dn.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);


            GPU.BindBuffer(groundtextBufferID4);
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);

                         
            upp.Bind();
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, up.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            GPU.BindBuffer(groundtextBufferID3);
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);

            lf.Bind();
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, left.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            rt.Bind();
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, right.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            ft.Bind();
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, front.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            bk.Bind();
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, back.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            
            // For each of your model, 
            // 1- call the draw method
            // 2- Pass to it the ID of the model in the vertex shader so that it can apply the correct transformations on its vertices
          //  zombie.Draw(modelID);
            Gl.glDepthFunc(Gl.GL_LEQUAL);

            building.Draw(transID);
            building2.Draw(transID);
            house.Draw(transID);
            Gl.glDepthFunc(Gl.GL_LESS);
            //zombie.Draw(transID);
            //Lara.Draw(transID);
            tree.Draw(transID);
            tree1.Draw(transID);
            fofa.Draw(transID);
            blade1.Draw(transID);
            blade2.Draw(transID);
            blade.Draw(transID);
            car.Draw(transID);
            //scar.transmatrix = glm.translate(new mat4(1), cam.GetCameraPosition());
            scar.Draw(transID);
        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

            // For the animated models, call the UpdateAnimation of the model so that it can interpolate the vertices to the correct position

            zombie.UpdateAnimation();
            blade.UpdateAnimation();
            blade1.UpdateAnimation();
            blade2.UpdateAnimation();
        }
        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}
