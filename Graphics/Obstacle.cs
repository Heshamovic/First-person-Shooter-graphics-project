using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tao.OpenGl;
using GlmNet;
using System.IO;
using Graphics._3D_Models;
using System.Windows.Forms;

namespace Graphics
{
    class Obstacle
    {
        public Model3D model;
        public vec3 position;
        public double radius;

        public Obstacle(Model3D m, vec3 p, double r)
        {
            model = m;
            position = p;
            radius = r;
        }
    }


}
