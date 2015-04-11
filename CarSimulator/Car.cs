using FarseerPhysics.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulator
{
    class Car
    {
        public Body Body;

        public Car(Body body)
        {
            this.Body = body;
        }
    }
}
