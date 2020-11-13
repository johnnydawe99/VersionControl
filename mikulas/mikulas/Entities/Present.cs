using mikulas.Abstractions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mikulas.Entities
{
    public class Present : Toy
    {
        public SolidBrush BoxColor { get; set; }
        public SolidBrush RibbonColor { get; set; }
        public Present(Color ribbon, Color box)
        {
            BoxColor = new SolidBrush(box);
            RibbonColor = new SolidBrush(ribbon);
        }
        protected override void DrawImage(Graphics g)
        {
            g.FillRectangle(BoxColor, 0, 0, Width, Height);
            g.FillRectangle(RibbonColor, 2 * (Width / 5), 0, Width / 5, Height);
            g.FillRectangle(RibbonColor, 0, 2 * (Height / 5), Width, Height / 5);
        }
    }
}
