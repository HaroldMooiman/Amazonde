using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace Models
{
    public class PseudoPickup : Model
    {
        Vector2 destination;
        public bool arrived = false;
        double xdes, ydes, xincr, yincr;

        public PseudoPickup(Vector2 destination): base(destination.X + 0.5, 99.9, destination.Y + 0.5, 0, 0, 0)
        {
            this.type = "pickup";
            this.destination = destination;
        }

        private void CheckArrival()
        {
            if(y < -0.5)
            {
                arrived = true;
            }
        }

        public override bool Update(int tick)
        {
            Move(x, y - 1, z);
            CheckArrival();
            return base.Update(tick);
        }
    }
}
