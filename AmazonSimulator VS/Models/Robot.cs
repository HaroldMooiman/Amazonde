using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Robot : Model
    {
        public Pickup attached = null;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "robot";
        }

        public override void Move(double x, double y, double z)
        {
            base.Move(x, y, z);

            if(attached != null)
            {
                attached.Move(x, y + 0.7, z);
            }
        }

    }
}
