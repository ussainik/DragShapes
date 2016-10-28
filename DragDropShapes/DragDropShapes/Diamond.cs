using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DragDropShapes
{
    public class Diamond : Shape
    {
        public Diamond()
        {
            this.Stretch = Stretch.Fill;
        }

        protected override Geometry DefiningGeometry
        {
            get { return GetGeometry(); }
        }

        private Geometry GetGeometry()
        {
            return Geometry.Parse("M 100, 0 l 100, 100 l -100, 100 l -100, -100 Z");
        }
    }
}
