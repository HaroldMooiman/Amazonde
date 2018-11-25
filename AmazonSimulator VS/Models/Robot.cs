using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Robot : Model
    {
        public bool idle = true;
        public Pickup attached = null;

        public Robot(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "robot";
        }

        public override void Move(double x, double y, double z)
        {
            if(this.x - x != 0)
            {
                if (this.z - z != 0)
                {
                    if(this.z -z < 0 && this.x - x > 0)
                    {
                        Rotate(rotationX, 1.5 * Math.PI / 2, rotationZ);
                    }
                    else
                    {
                        Rotate(rotationX, 0.5 * Math.PI / 2, rotationZ);
                    }
                }
                else
                {
                    Rotate(rotationX, Math.PI / 2, rotationZ);
                }
            }
            else
            {
                Rotate(rotationX, 0, rotationZ);
            }
            base.Move(x, y, z);

            if(attached != null)
            {
                attached.Move(x, y + 0.7, z);
            }
        }

    }
}
