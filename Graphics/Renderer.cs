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
    class Renderer
    {
        public static List<Obstacle> Obstacles = new List<Obstacle>();
        Shader sh;
        uint groundtextBufferID2;
        uint groundtextBufferID1;//grass
        uint groundtextBufferID3;//wall
        uint groundtextBufferID4;//sky
        uint ShootID;
        int viewID, projID, transID, EyePositionID, tmp = 0, c = 0, timer = 5;
        vec3 playerPos;
        public Camera cam;
        public float Speed = 1;
        public bool draw = false;
        Texture dn, upp, lf, rt, bk, ft, shoot;
        int AmbientLightID, DataID;
        public md2LOL blade, blade1, blade2, fofa;
        public List<vec3> positions = new List<vec3>();
        public List<md2LOL> zombie = new List<md2LOL>();
        public List<Model3D> bullets = new List<Model3D>();
        
        Model3D building, house, building2, m, car, scar, Lara, tree, tree1;
        mat4 ProjectionMatrix, ViewMatrix, down, up, left, right, front, back;
        public string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        public void createNewZombie(int x, int y, int z, int s)
        {
            positions.Add(new vec3(x, y, z));
            md2LOL tmp;
            tmp = new md2LOL(projectPath + "\\ModelFiles\\animated\\md2LOL\\zombie.md2");
            tmp.StartAnimation(animType_LOL.STAND);
            tmp.rotationMatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            tmp.scaleMatrix = glm.scale(new mat4(1), new vec3(s, s, s));
            tmp.TranslationMatrix = glm.translate(new mat4(1), new vec3(x, y, z));
            zombie.Add(tmp);
        }

        public void createBullet()
        {
            Model3D bullet = new Model3D();
            bullet.LoadFile(projectPath + "\\ModelFiles\\static\\bullet", "bullet.obj", 1);
            bullet.scalematrix = glm.scale(new mat4(1), new vec3(300, 300, 300));
            bullet.transmatrix = glm.translate(new mat4(1), new vec3(4000, -400, 4000));
        }

        public void create_square(mat4 arr, Texture tex)
        {
            tex.Bind();
            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, arr.to_array());
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

        }
        public void round(md2LOL zombie,float angle)
        {
            zombie.rotationMatrix = MathHelper.MultiplyMatrices(new List<mat4>()
            {
                glm.rotate(90.0f * 180.0f / 3.1412f, new vec3(1, 0, 0)),glm.rotate(+angle,new vec3(0,1,0))
             });

        }
        public void create_shoot()
        {
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, ShootID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)0);

            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(3 * sizeof(float)));

            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(6 * sizeof(float)));

            Gl.glEnableVertexAttribArray(3);
            Gl.glVertexAttribPointer(3, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 11 * sizeof(float), (IntPtr)(8 * sizeof(float)));

            shoot.Bind();
            vec3 shootpos = cam.GetCameraTarget();
            shootpos.y -= 1.5f;
            shootpos += cam.GetLookDirection() * 8;

            Gl.glUniformMatrix4fv(transID, 1, Gl.GL_FALSE, MathHelper.MultiplyMatrices(new List<mat4>() { glm.scale(new mat4(1),new vec3(2+(float)c/10,2 + (float)c / 10, 2 + (float)c / 10)),
                glm.rotate(cam.mAngleX, new vec3(0, 1, 0)),glm.rotate((float)c/10, new vec3(0, 0, 1)),glm.translate(new mat4(1),shootpos),
            }).to_array());
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            if (draw)
            {
                cam.mAngleY -= 0.01f;
                Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
                c--;
                if (c < 0)
                {
                    cam.mAngleY = 0;
                    c = timer;
                    draw = false;
                }
            }
        }


        public void InitializeObstacles()
        {

            building = new Model3D();
            building.LoadFile(projectPath + "\\ModelFiles\\static\\building", "Building 02.obj", 1);
            building.scalematrix = glm.scale(new mat4(1), new vec3(300, 300, 300));
            building.transmatrix = glm.translate(new mat4(1), new vec3(1, 1, 1));
            Obstacles.Add(new Obstacle(building, new vec3(1, 1, 1), 2000));

            building2 = new Model3D();
            building2.LoadFile(projectPath + "\\ModelFiles\\M4", "guard post.3ds", 10);
            building2.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            building2.scalematrix = glm.scale(new mat4(1), new vec3(400, 400, 800));
            building2.transmatrix = glm.translate(new mat4(1), new vec3(10000, 1, 500));
            Obstacles.Add(new Obstacle(building2, new vec3(10000, 1, 500), 2000));

            house = new Model3D();
            house.LoadFile(projectPath + "\\ModelFiles\\static\\House", "house.obj", 1);
            house.scalematrix = glm.scale(new mat4(1), new vec3(300, 300, 300));
            house.transmatrix = glm.translate(new mat4(1), new vec3(4000, -400, 4000));
            Obstacles.Add(new Obstacle(house, new vec3(4000, -400, 4000), 2000));

            tree = new Model3D();
            tree.LoadFile(projectPath + "\\ModelFiles\\static\\tree", "tree.obj", 4);
            tree.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            tree.transmatrix = glm.translate(new mat4(1), new vec3(1, -400, 2000));
            Obstacles.Add(new Obstacle(tree, new vec3(1, -400, 2000), 800));

            tree1 = new Model3D();
            tree1.LoadFile(projectPath + "\\ModelFiles\\static\\tree\\Tree", "Tree.fbx", 22);
            tree1.scalematrix = glm.scale(new mat4(1), new vec3(200, 200, 200));
            tree1.transmatrix = glm.translate(new mat4(1), new vec3(1500, -400, 4000));
            Obstacles.Add(new Obstacle(tree1, new vec3(1500, -400, 4000), 800));

            Lara = new Model3D();
            Lara.LoadFile(projectPath + "\\ModelFiles\\Heshambyhbd\\Box", "box.obj", 27);
            Lara.scalematrix = glm.scale(new mat4(1), new vec3(50, 50, 50));
            Lara.transmatrix = glm.translate(new mat4(1), new vec3(1000, -200, 1231));
            Obstacles.Add(new Obstacle(Lara, new vec3(1000, -200, 1231), 1000));

            car = new Model3D();
            car.LoadFile(projectPath + "\\ModelFiles\\static\\car", "dpv.obj", 3);
            car.scalematrix = glm.scale(new mat4(1), new vec3(1, 1, 1));
            car.transmatrix = glm.translate(new mat4(1), new vec3(-500, 1, -100));
            car.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            Obstacles.Add(new Obstacle(car, new vec3(-500, 1, -100), 1000));

            scar = new Model3D();
            scar.LoadFile(projectPath + "\\ModelFiles\\scar", "Scar-X.obj.obj", 10);
            scar.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            scar.transmatrix = glm.translate(new mat4(1), new vec3(500, 1, 500));

        }

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
            shoot = new Texture(projectPath + "\\Textures\\gunshot.png", 5 , true);
            sh.UseShader();

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
            float[] shootvertices = {
                -1,1,0,
                1,0,0,
                0,0,
                0,1,0,
                1,-1,0,
                0,0,1,
                1,1,
                0,1,0,
                -1,-1,0,
                0,0,1,
                0,1,
                0,1,0,
                1,1,0,
                0,0,1,
                1,0,
                0,1,0,
                -1,1,0,
                0,1,0,
                0,0,
                0,1,0,
                1,-1,0,
                1,0,0,
                1,1,
                0,1,0
            };
            
            groundtextBufferID2 = GPU.GenerateBuffer(ground);
            groundtextBufferID1 = GPU.GenerateBuffer(groundTex);
            groundtextBufferID3 = GPU.GenerateBuffer(wallTex);
            groundtextBufferID4 = GPU.GenerateBuffer(skyTex);
            ShootID = GPU.GenerateBuffer(shootvertices);

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

            InitializeObstacles();
    
            createNewZombie(1, 1, 1, 10);
            createNewZombie(4000, -400, 4000, 10);
            createNewZombie(1000, -400, 1031, 10);
            createNewZombie(10000, 1, 500, 20);
            
            Gl.glClearColor(0, 0, 0, 1);
            
            cam = new Camera();
            cam.Reset(555, 34, 55, 11000, 50, 11000, 0, 1, 0);


            DataID = Gl.glGetUniformLocation(sh.ID, "data");
            vec2 data = new vec2(40000, 5000);
            Gl.glUniform2fv(DataID, 1, data.to_array());
            int LightPositionID = Gl.glGetUniformLocation(sh.ID, "LightPosition_worldspace");
            vec3 lightPosition = new vec3(0.0f, 1000, 0.0f);
            Gl.glUniform3fv(LightPositionID, 1, lightPosition.to_array());
            AmbientLightID = Gl.glGetUniformLocation(sh.ID, "ambientLight");
            vec3 ambientLight = new vec3(0.5f, 0.58f, 0.58f);
            Gl.glUniform3fv(AmbientLightID, 1, ambientLight.to_array());
            EyePositionID = Gl.glGetUniformLocation(sh.ID, "EyePosition_worldspace");

            m = new Model3D();
            m.LoadFile(projectPath + "\\ModelFiles\\hands with gun", "gun.obj", 2);
            playerPos = cam.GetCameraTarget();
            playerPos.y += 7;
            m.scalematrix = glm.scale(new mat4(1), new vec3(0.2f, 0.2f, 0.2f));
            m.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            m.transmatrix = glm.translate(new mat4(1), playerPos);
            
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            transID = Gl.glGetUniformLocation(sh.ID, "model");
            projID = Gl.glGetUniformLocation(sh.ID, "projection");
            viewID = Gl.glGetUniformLocation(sh.ID, "view");


            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glDepthFunc(Gl.GL_LESS);
        }

        public void Draw()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            sh.UseShader();

            playerPos = cam.GetCameraTarget();
            playerPos.y -= 2.5f;
            m.rotmatrix = glm.rotate(3.1412f + cam.mAngleX, new vec3(0, 1, 0));
            m.transmatrix = glm.translate(new mat4(1), playerPos);
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();

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

            create_square(up, upp);
          
            GPU.BindBuffer(groundtextBufferID3);
            Gl.glEnableVertexAttribArray(2);
            Gl.glVertexAttribPointer(2, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 0, IntPtr.Zero);

            create_square(left, lf);
            create_square(right, rt);
            create_square(front, ft);
            create_square(back, bk);
            


            Gl.glDepthFunc(Gl.GL_LEQUAL);
            m.Draw(transID);
            building.Draw(transID);
            building2.Draw(transID);
            house.Draw(transID);
            Gl.glDepthFunc(Gl.GL_LESS);
            tree.Draw(transID);
            tree1.Draw(transID);
            foreach (md2LOL z in zombie)
                z.Draw(transID);

            create_shoot();
            Gl.glDisable(Gl.GL_BLEND);
            scar.Draw(transID);
        }
        public void Update(float deltaTime)
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            for (int i = 0; i < zombie.Count; i++)
            {
                vec2 dir = new vec2();
                dir.x = cam.mCenter.x - positions[i].x;
                dir.y = cam.mCenter.z - positions[i].z;
                float dis = (float)(Math.Sqrt(dir.x * dir.x + dir.y * dir.y));
                dir.x /= dis;
                dir.y /= dis;
                if (dis <= 200)
                {
                    if (zombie[i].animSt.type != animType_LOL.ATTACK1)
                        zombie[i].StartAnimation(animType_LOL.ATTACK1);
                }
                else if (dis < 1000)
                {
                    vec3 t = new vec3(dir.x + positions[i].x ,positions[i].y ,dir.y + positions[i].z);
                    round(zombie[i], cam.mAngleX);
                    positions[i] = t ;
                    float x = positions[i].x,  y = positions[i].y , z = positions[i].z;
                    if(zombie[i].animSt.type != animType_LOL.RUN)
                     zombie[i].StartAnimation(animType_LOL.RUN);
                    zombie[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(x,y,z));
                    
                }
                else
                    if (zombie[i].animSt.type != animType_LOL.STAND)
                        zombie[i].StartAnimation(animType_LOL.STAND);

                zombie[i].UpdateAnimation();
            }

            
        }




        public void CleanUp()
        {
            sh.DestroyShader();
        }
    }
}

