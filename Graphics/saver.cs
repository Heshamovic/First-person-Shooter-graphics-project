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
       public List<float> zombiebars; // the health for each zombie 
       public List<vec3> positions; // the postions for each zombie 
        public saver( List<float> zombar , List<vec3> zompos)
        {
            zombiebars = zombar;
            positions = zompos;
            savegame();
        }
        public void savegame()
        {
            StreamWriter writer = null;
            try
            {
                XmlSerializer xml = new XmlSerializer(positions.GetType());
                writer = new StreamWriter("modelsPos.xml");
                xml.Serialize(writer, positions);

                XmlSerializer xml2 = new XmlSerializer(zombiebars.GetType());
                writer = new StreamWriter("modelsBar.xml");
                xml2.Serialize(writer, zombiebars);
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
