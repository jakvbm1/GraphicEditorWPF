using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GraphicEditorWPF
{
    public class Layer
    {
        public Canvas LayerCanvas { get; set; }
        public bool IsVisible { get; set; } = true;
        public string Name { get; set; }

        public Layer(string name)
        {
            LayerCanvas = new Canvas();
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }

}
