using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    public abstract class Screen
    {
        public abstract void Close();
        public abstract void Draw();
        public abstract void Initialize();
        public abstract void Load();
        public abstract void Update();
    }
}
