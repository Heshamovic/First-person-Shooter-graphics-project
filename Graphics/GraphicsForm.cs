﻿using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Diagnostics;
using System;
using System.IO;
using Graphics._3D_Models;
using GlmNet;
using System.Collections.Generic;

namespace Graphics
{
    public partial class GraphicsForm : Form
    {
        Screen sc = new Start_Screen(), sc1 = new Renderer();
        Thread MainLoopThread;
        string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        float prevX, prevY;
        public GraphicsForm()
        {
            InitializeComponent();
            simpleOpenGlControl1.InitializeContexts();
            MoveCursor();
            initialize();
            MainLoopThread = new Thread(MainLoop);
            MainLoopThread.Start();
        }
        void initialize()
        {
            sc = new Start_Screen();
            sc.Initialize();
        }
        void MainLoop()
        {
            while (true)
            {
                sc.Draw();
                sc.Update();
                if (sc is Renderer)
                {
                    textBox5.Text = ((Renderer)sc).zombie[0].animSt.curr_frame + "";
                    if (((Renderer)sc).bullets_pos.Count > 0)
                        pos.Text = ((int)((Renderer)sc).bullets_pos[0].x).ToString() + " " + ((int)((Renderer)sc).bullets_pos[0].y).ToString() + " " + ((int)((Renderer)sc).bullets_pos[0].z).ToString();
                }
                if (sc is Loading_Screen)
                {
                    sc.Close();
                    Screen sc1 = new Renderer();
                    sc1.Initialize();
                    done.WaitOne();
                    sc = sc1;
                }
                simpleOpenGlControl1.Refresh();
            }
        }
        private void GraphicsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            sc.Close();
        }

        private void simpleOpenGlControl1_Paint(object sender, PaintEventArgs e)
        {
            sc.Draw();
            sc.Update();
        }

        private void simpleOpenGlControl1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (sc is Renderer)
            {
                if (e.KeyChar == 'm')
                {
                    trigger = !trigger;
                    return;
                }
                float speed = float.Parse(textBox1.Text);
                if (e.KeyChar == 'a')
                    ((Renderer)sc).cam.Strafe(-speed);
                if (e.KeyChar == 'd')
                    ((Renderer)sc).cam.Strafe(speed);
                if (e.KeyChar == 's')
                    ((Renderer)sc).cam.Walk(-speed);
                if (e.KeyChar == 'w')
                    ((Renderer)sc).cam.Walk(speed);
                if (e.KeyChar == 'z')
                    ((Renderer)sc).cam.Fly(-speed);
                if (e.KeyChar == 'c')
                    ((Renderer)sc).cam.Fly(speed);
                if (e.KeyChar == ' ')
                {
                    ((Renderer)sc).jump = true;
                }
                if (e.KeyChar == 'k')
                {
                    System.Media.SoundPlayer player = new System.Media.SoundPlayer(Environment.CurrentDirectory + "\\shot.wav");
                    player.Play();
                }
                label6.Text = "X: " + ((Renderer)sc).cam.GetCameraPosition().x;
                label7.Text = "Y: " + ((Renderer)sc).cam.GetCameraPosition().y;
                label8.Text = "Z: " + ((Renderer)sc).cam.GetCameraPosition().z;
            }
            
        }
        bool trigger = true;
        private void simpleOpenGlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!trigger)
                return;
            float speed = 0.05f;
            float delta = e.X - prevX;
            if (sc is Renderer)
            {
                if (delta > 2)
                    ((Renderer)sc).cam.Yaw(-speed);
                    
                else if (delta < -2)
                    ((Renderer)sc).cam.Yaw(speed);


                delta = e.Y - prevY;
                if (delta > 2)
                    ((Renderer)sc).cam.Pitch(-speed);
                else if (delta < -2)
                    ((Renderer)sc).cam.Pitch(speed);
            }
            MoveCursor();
        }
        
        private void button8_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float res = 0;
            if (float.TryParse(textBox1.Text,out res))
            {
                //((Renderer)sc).zombie[0].AnimationSpeed = res;
            }
        }


        private void MoveCursor()
        {
            this.Cursor = new Cursor(Cursor.Current.Handle);
            Point p = PointToScreen(simpleOpenGlControl1.Location);
            if (sc is Renderer)
                Cursor.Position = new Point(simpleOpenGlControl1.Size.Width / 2 + p.X, simpleOpenGlControl1.Size.Height / 2 + p.Y);
            Cursor.Clip = new Rectangle(this.Location, this.Size);
            prevX = simpleOpenGlControl1.Location.X + simpleOpenGlControl1.Size.Width / 2;
            prevY = simpleOpenGlControl1.Location.Y + simpleOpenGlControl1.Size.Height / 2;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (sc is Loading_Screen)
            {
                MessageBox.Show("Wait till loading finish");
                return;
            }
            if (!(sc is Renderer))
            {
                sc.Close();
            }
            trigger = !trigger;
            sc = new Renderer();
            sc.Initialize();
            loadgame<List<vec3>> loadgam = new loadgame<List<vec3>>();
            ((Renderer)sc).positions = loadgam.LoadData("modelsPos.xml");
            loadgame<List<float>> loadgam2 = new loadgame<List<float>>();
            ((Renderer)sc).hps = loadgam2.LoadData("modelsBar.xml");
            ((Renderer)sc).scalef = ((Renderer)sc).hps[((Renderer)sc).hps.Count - 1];
            ((Renderer)sc).hps.RemoveAt(((Renderer)sc).hps.Count - 1);
            ((Renderer)sc).cam.mCenter = ((Renderer)sc).positions[((Renderer)sc).positions.Count - 1];
            ((Renderer)sc).positions.RemoveAt(((Renderer)sc).positions.Count - 1);
            ((Renderer)sc).Draw();
            ((Renderer)sc).Update();
        }

        private void GraphicsForm_Load(object sender, EventArgs e)
        {

        }

        private void simpleOpenGlControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (sc is Renderer)
            {
                ((Renderer)sc).draw = true;
                ((Renderer)sc).cam.mAngleY += 0.1f;
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(projectPath + "\\Sounds\\shot.wav");
                player.Play();
                vec3 v = new vec3();
                v.x = ((Renderer)sc).cam.mPosition.x;
                v.z = ((Renderer)sc).cam.mPosition.z;
                v.y = ((Renderer)sc).cam.mPosition.y;
                vec2 dir = new vec2();
                dir.x = ((Renderer)sc).cam.mCenter.x - v.x;
                dir.y = ((Renderer)sc).cam.mCenter.z - v.z;
                float dis = (float)(Math.Sqrt(dir.x * dir.x + dir.y * dir.y));
                dir.x /= dis;
                dir.y /= dis;
                ((Renderer)sc).bullets_pos.Add(v);
                ((Renderer)sc).hit.Add(false);
                ((Renderer)sc).direct.Add(dir);

            }
            else if (sc is Start_Screen)
            {
                Point p = PointToScreen(simpleOpenGlControl1.Location);
                float xpos = simpleOpenGlControl1.Size.Width + p.X, ypos = simpleOpenGlControl1.Size.Height / 2 + p.Y;
                if (Cursor.Position.X >= 0.83 * xpos && Cursor.Position.X <= 0.95 * xpos && Cursor.Position.Y >= 0.57 * ypos && Cursor.Position.Y <= 0.72 * ypos)
                {
                    sc.Close();
                    sc = new Loading_Screen();
                    sc.Initialize();
                    //Screen sc1 = new Renderer();
                    //sc1.Initialize();
                    //done.WaitOne();
                    //sc.Close();
                    //sc = sc1;
                }
            }
        }
        public static EventWaitHandle done = new EventWaitHandle(false, EventResetMode.AutoReset);
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (sc is Loading_Screen)
            {
                MessageBox.Show("Wait till loading finish");
                return;
            }
            ((Renderer)sc).hps.Add(((Renderer)sc).scalef);
            ((Renderer)sc).positions.Add(((Renderer)sc).cam.mCenter);
            saver s = new saver(((Renderer)sc).hps, ((Renderer)sc).positions);
            ((Renderer)sc).hps.RemoveAt(((Renderer)sc).hps.Count - 1);
            ((Renderer)sc).positions.RemoveAt(((Renderer)sc).positions.Count - 1);
            MessageBox.Show("Saved!");
        }
    }
}
