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
    public class loadgame<T>
    {
        public static Type type;

        public loadgame()
        {
            type = typeof(T);
        }

        public T LoadData(string filename)
        {
            T result;
            XmlSerializer xml = new XmlSerializer(type);
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            result = (T)xml.Deserialize(fs);
            fs.Close();
            return result;
        }
    }
}
