using System;
using System.Threading;
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
    class Renderer : Screen
    {
        public static List<Obstacle> Obstacles = new List<Obstacle>();
        public Shader sh;
        uint groundtextBufferID2;
        uint groundtextBufferID1;//grass
        uint groundtextBufferID3;//wall
        uint groundtextBufferID4;//sky
        uint ShootID;
        int viewID, projID, transID, EyePositionID, tmp = 0, c = 0, timer = 5;
        vec3 playerPos;
        public Camera cam;
        public float Speed = 1;
        public bool draw = false , jump = false, close = false;
        Texture dn, upp, lf, rt, bk, ft, shoot;
        int AmbientLightID, DataID , cc = 10;
        //public List<Zomby> zombies;
        public List<vec3> positions = new List<vec3>();
        public List<md2LOL> zombie = new List<md2LOL>();
        public List<Model3D> bullets = new List<Model3D>();
        public List<mat4> zombiebars = new List<mat4>();
        public List<vec3> bullets_pos = new List<vec3>();
        public List<float> hps = new List<float>();
        public List<bool> hit = new List<bool>();
        public List<vec2> direct = new List<vec2>();
        Model3D building, house, building2, m, car, scar, Lara, tree, tree1;
        mat4 ProjectionMatrix, ViewMatrix, down, up, left, right, front, back;
        Texture hp , ehp;
        Texture bhp;
        uint hpID;
        mat4 healthbar;
        mat4 backhealthbar;
        public Shader shader2D;
        int mloc;
        public float scalef;
        System.Media.SoundPlayer p1;
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
            mat4 bar = MathHelper.MultiplyMatrices(new List<mat4>() {
                 glm.scale(new mat4(1), new vec3(100.48f, 100.1f, 500)), glm.translate(new mat4(1), new vec3(x, y+1000, z)),
                    });
            zombiebars.Add(bar);
            zombie.Add(tmp);
            hps.Add(1);
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
        public void create_bars(mat4 arr, Texture tex , float hp)
        {
           arr = MathHelper.MultiplyMatrices(new List<mat4>() {
                  glm.scale(new mat4(1), new vec3(0.48f, 0.1f, 1*hp)),arr

                    });
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
            building.LoadFile(projectPath + "\\ModelFiles\\obt", "City_House_2_BI.fbx", 1);
            building.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            building.scalematrix = glm.scale(new mat4(1), new vec3(400, 400, 800));
            building.transmatrix = glm.translate(new mat4(1), new vec3(1, -400, 1));
            Obstacles.Add(new Obstacle(building, new vec3(1, -400, 1), 1000));

            building2 = new Model3D();
            building2.LoadFile(projectPath + "\\ModelFiles\\obt", "City_House_2_BI.fbx", 10);
            building2.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            building2.scalematrix = glm.scale(new mat4(1), new vec3(400, 400, 800));
            building2.transmatrix = glm.translate(new mat4(1), new vec3(10000, -400, 500));
            Obstacles.Add(new Obstacle(building2, new vec3(10000, -400, 500), 1000));

            house = new Model3D();
            house.LoadFile(projectPath + "\\ModelFiles\\obt", "City_House_2_BI.fbx", 10);
            house.rotmatrix = glm.rotate((float)((-90.0f / 180) * Math.PI), new vec3(1, 0, 0));
            house.scalematrix = glm.scale(new mat4(1), new vec3(400, 400, 800));
            house.transmatrix = glm.translate(new mat4(1), new vec3(4000, -400, 4000));
            Obstacles.Add(new Obstacle(house, new vec3(4000, -400, 4000), 1000));

            tree = new Model3D();
            tree.LoadFile(projectPath + "\\ModelFiles\\static\\tree", "tree.obj", 4);
            tree.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            tree.transmatrix = glm.translate(new mat4(1), new vec3(1, -400, 2000));
            Obstacles.Add(new Obstacle(tree, new vec3(1, -400, 2000), 500));

            tree1 = new Model3D();
            tree1.LoadFile(projectPath + "\\ModelFiles\\static\\tree\\Tree", "Tree.fbx", 22);
            tree1.scalematrix = glm.scale(new mat4(1), new vec3(200, 200, 200));
            tree1.transmatrix = glm.translate(new mat4(1), new vec3(1500, -400, 4000));
            Obstacles.Add(new Obstacle(tree1, new vec3(1500, -400, 4000), 500));

            Lara = new Model3D();
            Lara.LoadFile(projectPath + "\\ModelFiles\\Heshambyhbd\\Box", "box.obj", 27);
            Lara.scalematrix = glm.scale(new mat4(1), new vec3(50, 50, 50));
            Lara.transmatrix = glm.translate(new mat4(1), new vec3(1000, -200, 1231));
            Obstacles.Add(new Obstacle(Lara, new vec3(1000, -200, 1231), 400));

            car = new Model3D();
            car.LoadFile(projectPath + "\\ModelFiles\\static\\car", "dpv.obj", 3);
            car.scalematrix = glm.scale(new mat4(1), new vec3(1, 1, 1));
            car.transmatrix = glm.translate(new mat4(1), new vec3(-500, 1, -100));
            car.rotmatrix = glm.rotate(3.1412f, new vec3(0, 1, 0));
            Obstacles.Add(new Obstacle(car, new vec3(-500, 1, -100), 600));

            scar = new Model3D();
            scar.LoadFile(projectPath + "\\ModelFiles\\scar", "Scar-X.obj.obj", 10);
            scar.scalematrix = glm.scale(new mat4(1), new vec3(100, 100, 100));
            scar.transmatrix = glm.translate(new mat4(1), new vec3(500, 1, 100));

        }
        double calc_distance(vec3 first, vec3 second)
        {
            return Math.Sqrt(Math.Pow((first.x - second.x), 2) + Math.Pow((first.z - second.z), 2));
        }
        public override void Initialize()
        {
            string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
            shader2D = new Shader(projectPath + "\\Shaders\\2Dvertex.vertexshader", projectPath + "\\Shaders\\2Dfrag.fragmentshader");
            sh = new Shader(projectPath + "\\Shaders\\SimpleVertexShader.vertexshader", projectPath + "\\Shaders\\SimpleFragmentShader.fragmentshader");
            dn = new Texture(projectPath + "\\Textures\\sandcastle_dn.png", 2, true);
            lf = new Texture(projectPath + "\\Textures\\sandcastle_lf.png", 2, true);
            rt = new Texture(projectPath + "\\Textures\\sandcastle_rt.png", 2, true);
            upp = new Texture(projectPath + "\\Textures\\sandcastle_up.png", 2, true);
            ft = new Texture(projectPath + "\\Textures\\sandcastle_ft.png", 2, true);
            bk = new Texture(projectPath + "\\Textures\\sandcastle_bk.png", 2, true);
            shoot = new Texture(projectPath + "\\Textures\\gunshot.png", 5 , true);
            hp = new Texture(projectPath + "\\Textures\\HP.bmp", 9,true);
            ehp = new Texture(projectPath + "\\Textures\\ehp.jpg", 9, true);
            bhp = new Texture(projectPath + "\\Textures\\BackHP.bmp", 10,true);
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
            backhealthbar = MathHelper.MultiplyMatrices(new List<mat4>(){
                glm.scale(new mat4(1), new vec3(0.5f,0.1f, 1)), glm.translate(new mat4(1),new vec3(-0.5f,0.9f,0)) });
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                glm.scale(new mat4(1), new vec3(0.48f, 0.1f, 1)), glm.translate(new mat4(1), new vec3(-0.5f, 0.9f, 0)) });

            mloc = Gl.glGetUniformLocation(shader2D.ID, "model");
            scalef = 1;

            hpID = GPU.GenerateBuffer(squarevertices);
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
           
            createNewZombie(-8864, -400, 5322, 10);
            createNewZombie(14000, -400, 4000, 10);
            createNewZombie(15000, -400, 1031, 10);
            createNewZombie(8540, -400, 10363, 10);
            
            Gl.glClearColor(0, 0, 0, 1);
            
            cam = new Camera();
            cam.Reset(555, 34, 55, 5000, -400, 4000, 0, 1, 0);


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
            GraphicsForm.done.Set();
        }

        public override void Draw()
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

            
            
            for (int i = 0; i < zombiebars.Count; i++)
            {
                create_bars(zombiebars[i], ehp , hps[i]);
            }

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

            

            Gl.glDisable(Gl.GL_DEPTH_TEST);
            shader2D.UseShader();
            Gl.glBindBuffer(Gl.GL_ARRAY_BUFFER, hpID);
            Gl.glEnableVertexAttribArray(0);
            Gl.glVertexAttribPointer(0, 3, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)0);
            Gl.glEnableVertexAttribArray(1);
            Gl.glVertexAttribPointer(1, 2, Gl.GL_FLOAT, Gl.GL_FALSE, 5 * sizeof(float), (IntPtr)(3 * sizeof(float)));
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, backhealthbar.to_array());
            bhp.Bind();
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);

            
            if (scalef < 0)
                scalef = 0;
            healthbar = MathHelper.MultiplyMatrices(new List<mat4>() {
                 glm.scale(new mat4(1), new vec3(0.48f * scalef, 0.1f, 1)), glm.translate(new mat4(1), new vec3(-0.5f-((1-scalef)*0.48f), 0.9f, 0)) });
            Gl.glUniformMatrix4fv(mloc, 1, Gl.GL_FALSE, healthbar.to_array());
            hp.Bind();
            if (scalef == 0)
            {
                close = true;
            }
            Gl.glDrawArrays(Gl.GL_TRIANGLES, 0, 6);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
        }
        void create_jump()
        {
            if(cc >= 25)
             cam.mCenter.y += 300f;
            else if (cc >= 0)
            {
                cam.mCenter.y -= 300f;
            }
            else
            {
                jump = false;
                cc = 50;
            }
            cc--;
            if (cam.mCenter.y > 800)
                cam.mCenter.y = 800;
            if (cam.mCenter.y < 5)
                cam.mCenter.y = 5;
        }
        public override void Update()
        {
            cam.UpdateViewMatrix();
            ProjectionMatrix = cam.GetProjectionMatrix();
            ViewMatrix = cam.GetViewMatrix();
            if (jump)
            {
                create_jump();
            }
            for (int i = 0; i < Obstacles.Count; i++)
            {
                vec3 obspos = Obstacles[i].position;
                for (int j = 0; j < bullets_pos.Count; j++)
                {
                    vec3 pos = bullets_pos[j];
                    float dis = (float)calc_distance(pos,obspos);
                    if (dis < Obstacles[i].radius && hit[j] == false)
                    {
                        hit[j] = true;
                    }
                }
                
            }
            for (int i = 0; i < zombie.Count; i++)
            {
                float dis;
                if (hps[i] <= 0.2f)
                {
                    hps[i] = 0;
                    if (zombie[i].animSt.type != animType_LOL.DEATH)
                        zombie[i].StartAnimation(animType_LOL.DEATH);
                    zombie[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(10000000, 1, 1));
                    positions[i] = new vec3(10000000, 1, 1);
                    continue;
                }
                for (int j = 0; j < bullets_pos.Count; j++)
                {
                    vec3 t1 = positions[i];
                    vec3 t2 = bullets_pos[j];
                    vec3 t3 = new vec3(t1.x - t2.x, t1.y - t2.y, t1.z - t2.z);
                    dis = (float)(Math.Sqrt(t3.x * t3.x + t3.z * t3.z));
                    if (dis <= 200 && hit[j] != true)
                    {
                        hps[i] -= 0.2f;
                        if (hps[i] <= 0.2f)
                        {
                            hps[i] = 0;
                            if (zombie[i].animSt.type != animType_LOL.DEATH)
                                zombie[i].StartAnimation(animType_LOL.DEATH);
                            zombie[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(10000000, 1, 1));
                            positions[i] = new vec3(10000000, 1, 1);
                        }
                        hit[j] = true;
                    }
                }
                vec2 dir = new vec2();
                dir.x = cam.mCenter.x - positions[i].x;
                dir.y = cam.mCenter.z - positions[i].z;
                dis = (float)(Math.Sqrt(dir.x * dir.x + dir.y * dir.y));
                dir.x /= dis;
                dir.y /= dis;
                if (dis <= 500)
                {
                    if (zombie[i].animSt.type != animType_LOL.ATTACK1)
                        zombie[i].StartAnimation(animType_LOL.ATTACK1);
                    scalef -= 0.0005f;
                }
                else if (dis < 2500)
                {
                    vec3 t = new vec3(dir.x*3 + positions[i].x, positions[i].y, dir.y*3 + positions[i].z);
                    round(zombie[i], cam.mAngleX);
                    bool can = true;
                    for (int j = 0; j < Obstacles.Count; j++)
                    {
                        float diss = (float)calc_distance(t, Obstacles[j].position);
                        if (diss < 1000)
                        {
                            can = false;
                            break;
                        }
                    }
                    if (can)
                    {
                        positions[i] = t;
                    }
                    float x = positions[i].x, y = positions[i].y, z = positions[i].z;
                    if (zombie[i].animSt.type != animType_LOL.RUN)
                        zombie[i].StartAnimation(animType_LOL.RUN);
                    zombie[i].TranslationMatrix = glm.translate(new mat4(1), new vec3(x, y, z));
                    zombiebars[i] = MathHelper.MultiplyMatrices(new List<mat4>() {
                         glm.scale(new mat4(1), new vec3(100.48f, 100.1f, 500)), glm.translate(new mat4(1), new vec3(x, y+1000, z)),
                    //    glm.rotate(-90 / 180.0f * 3.1412f , new vec3(1,0,0))
                         });
                }
                else
                {
                    if (zombie[i].animSt.type != animType_LOL.STAND)
                        zombie[i].StartAnimation(animType_LOL.STAND);
                }
                zombie[i].UpdateAnimation();
            }

            for (int i = 0; i < bullets_pos.Count; i++)
            {
                float[] temp = { bullets_pos[i].x + direct[i].x*50f, bullets_pos[i].y, bullets_pos[i].z + direct[i].y*50f };
                bullets_pos[i] = new vec3(temp[0], temp[1], temp[2]);
            }
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

