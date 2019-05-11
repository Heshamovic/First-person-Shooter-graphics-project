using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using System.IO;
using Graphics._3D_Models;
using GlmNet;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Renderer renderer = new Renderer();
        Thread MainLoopThread;
        string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        float deltaTime, prevX, prevY;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
            MoveCursor();
            initialize();
            deltaTime = 0.005f;
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();
        }
        void initialize()
        {
            renderer.Initialize();   
        }
        void MainLoop()
        {
            while (true)
            {
                renderer.Draw();
                renderer.Update(deltaTime);
                simpleOpenGlControl1.Refresh();
                textBox5.Text = renderer.zombie[0].animSt.curr_frame + "";
                if(renderer.bullets_pos.Count > 0)
                pos.Text = ((int)renderer.bullets_pos[0].x).ToString() + " " + ((int)renderer.bullets_pos[0].y).ToString() + " " + ((int)renderer.bullets_pos[0].z).ToString();
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            renderer.CleanUp();
            MainLoopThread.Abort();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            renderer.Draw();
            renderer.Update(deltaTime);
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            float speed = float.Parse(textBox1.Text);
            if (e.KeyChar == 'a')
                renderer.cam.Strafe(-speed);
            if (e.KeyChar == 'd')
                renderer.cam.Strafe(speed);
            if (e.KeyChar == 's')
                renderer.cam.Walk(-speed);
            if (e.KeyChar == 'w')
                renderer.cam.Walk(speed);
            if (e.KeyChar == 'z')
                renderer.cam.Fly(-speed);
            if (e.KeyChar == 'c')
                renderer.cam.Fly(speed);
            if (e.KeyChar == ' ')
            {
                renderer.jump = true;
            }
            if (e.KeyChar == 'k')
            {
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(Environment.CurrentDirectory + "\\shot.wav");
                player.Play();
            }
            label6.Text = "X: " + renderer.cam.GetCameraPosition().x;
            label7.Text = "Y: " + renderer.cam.GetCameraPosition().y;
            label8.Text = "Z: " + renderer.cam.GetCameraPosition().z;
        }

        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            float speed = 0.05f;
            float delta = e.X - prevX;
            if (delta > 2)
                renderer.cam.Yaw(-speed);
            else if (delta < -2)
                renderer.cam.Yaw(speed);


            delta = e.Y - prevY;
            if (delta > 2)
                renderer.cam.Pitch(-speed);
            else if (delta < -2)
                renderer.cam.Pitch(speed);

            MoveCursor();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            renderer.zombie[0].StartAnimation(_3D_Models.animType_LOL.STAND);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            renderer.zombie[0].StartAnimation(_3D_Models.animType_LOL.ATTACK1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            renderer.zombie[0].StartAnimation(_3D_Models.animType_LOL.ATTACK2);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            renderer.zombie[0].StartAnimation(_3D_Models.animType_LOL.RUN);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            renderer.zombie[0].StartAnimation(_3D_Models.animType_LOL.SPELL1);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            renderer.zombie[0].StartAnimation(_3D_Models.animType_LOL.SPELL2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            start form = new start();
            this.Hide();
            form.Show();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float res = 0;
            if (float.TryParse(textBox1.Text,out res))
            {
                renderer.zombie[0].AnimationSpeed = res;
            }
        }


        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            Cursor.Position = new Point(simpleOpenGlControl1.Size.Width/2+p.X, simpleOpenGlControl1.Size.Height/2+p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X+simpleOpenGlControl1.Size.Width/2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }

        private void simpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            renderer.draw = true;
            renderer.cam.mAngleY += 0.1f;
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(projectPath + "\\Sounds\\shot.wav");
            player.Play();
            vec3 v = new vec3();
            v.x = renderer.cam.mPosition.x;
            v.z = renderer.cam.mPosition.z;
            v.y = renderer.cam.mPosition.y;
            vec2 dir = new vec2();
            dir.x = renderer.cam.mCenter.x - v.x;
            dir.y = renderer.cam.mCenter.z - v.z;
            float dis = (float)(Math.Sqrt(dir.x * dir.x + dir.y * dir.y));
            dir.x /= dis;
            dir.y /= dis;
            renderer.bullets_pos.Add(v);
            renderer.hit.Add(false);
            renderer.direct.Add(dir);
        }
    }
}
