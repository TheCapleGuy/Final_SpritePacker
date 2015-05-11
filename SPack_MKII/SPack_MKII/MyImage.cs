using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace SPack_MKII
{
    public class MyImage : Image
    {
        private int _X, _Y, _rowPos;
        private string _name;

        public string name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int X
        {
            get { return _X; }
            set { _X = value; }
        }
        public int Y
        {
            get { return _Y; }
            set { _Y = value; }
        }
        public int rowPos
        {
            get { return _rowPos; }
            set { _rowPos = value; }
        }
    }
}
