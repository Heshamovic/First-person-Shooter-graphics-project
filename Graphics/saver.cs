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
using System.IO;
using System.Xml.Serialization; 


namespace Graphics
{
    class saver
    {
       public static List<md2LOL> zombies; // information about zombies  
       public List<mat4> zombiebars; // the health for each zombie 
       public List<vec3> positions; // the postions for each zombie 
        saver(List<md2LOL>zom , List<mat4> zombar , List<vec3> zompos)
        {
            zombies = new List<md2LOL>();
            zombiebars = new List<mat4>();
            positions = new List<vec3>();
            zombies = zom;
            zombiebars = zombar;
            positions = zompos; 
        }
        public static void savegame(List<md2LOL> temp)
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer xml = new XmlSerializer(temp.GetType());
              //  writer = new StreamWriter();
                xml.Serialize(writer, temp);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
                writer = null;
            }
        }
    }

}
