using SFML.Graphics;
using SFML.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            var rw = new RenderWindow(new VideoMode(1024, 768), "Car Simulator", Styles.Close, new ContextSettings { AntialiasingLevel = 8 });
            rw.Closed += (s, a) => rw.Close();

            while (rw.IsOpen)
            {
                rw.Clear();

                rw.DispatchEvents();

                rw.Display();
            }
        }
    }
}
