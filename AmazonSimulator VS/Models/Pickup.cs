using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace Models
{
    public class Pickup : Model
    {
        object attached;
        float bobnumber = 0;
        bool bobrising = true;

        public Pickup(Vector2 pos, ASTARGrid grid) : base(grid.grid[(int)pos.X, (int)pos.Y].xpos, 0, grid.grid[(int)pos.X, (int)pos.Y].ypos, 0, 0, 0)
        {
            this.type = "pickup";
            grid.grid[(int)pos.X, (int)pos.Y].attached = this; //bascially pickups can't exist without being on the grid.
            attached = grid.grid[(int)pos.X, (int)pos.Y];
            Move(x, y + 0.8, z);
            //ylevel = y;
        }

        public Pickup(double x, double y, double z, double rotationX, double rotationY, double rotationZ) : base(x, y, z, rotationX, rotationY, rotationZ)
        {
            this.type = "pickup";
            //ylevel = y;
        }

        private void Bob()
        {
            Move(x, y - bobnumber, z);

            if(bobrising)
            {
                bobnumber += 0.01f;
                if(bobnumber > 0.5)
                {
                    bobrising = false;
                    bobnumber = 0.5f;
                }
            }
            else
            {
                bobnumber -= 0.01f;
                if(bobnumber < 0)
                {
                    bobrising = true;
                    bobnumber = 0;
                }
            }

            Move(x, y + bobnumber, z);
        }

        public override bool Update(int tick)
        {
            Rotate(rotationX, rotationY + 0.03, rotationZ);
            Bob();
            return base.Update(tick);
        }
    }
}
