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
    public enum PickupType{
        Weapon, Gold, Item
    }
    class Pickup
    {
        public string name;
        public PickupType type;
        public Model3D model;
        public vec3 pos;
        public int amount;

        public Pickup(string Name, PickupType Type, Model3D Model, vec3 Pos, int Amount = 0)
        {
            name   = Name;
            type   = Type;
            model  = Model;
            pos    = Pos;
            amount = Amount;
        }
    }
}
