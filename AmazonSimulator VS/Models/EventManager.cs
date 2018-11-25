using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace Models
{
    public class EventManager
    {
        int ticks;
        Random rtd = new Random();
        public List<Pickup> idles = new List<Pickup>();
        List<PseudoPickup> meteors = new List<PseudoPickup>();
        World w;


        public EventManager(World w)
        {
            this.w = w;
        }

        private void Landed(PseudoPickup pp)
        {
            for (int i = 0, l = w.worldObjects.Count(); i < l; i++)
            {
                if(pp.guid == w.worldObjects[i].guid)
                {
                    w.worldObjects.RemoveAt(i);
                    break;
                }
            }

            if(pp.x > 29 || pp.z > 29)
            {
                pp.Move(29, pp.y, 29);
            }
            Pickup p = new Pickup(new Vector2((int)pp.x, (int)pp.z), w.griddy);
            w.worldObjects.Add(p);
            idles.Add(p);

        }

        private void Spawnmeteors()
        {
            if(idles.Count() == 0 && meteors.Count() < 5)
            {
                PseudoPickup pp = new PseudoPickup(new Vector2(rtd.Next(29), rtd.Next(29)));
                meteors.Add(pp);
                w.worldObjects.Add(pp);
            }
        }

        public void Update()
        {
            Spawnmeteors();

            for (int i = 0, l = meteors.Count(); i < l; i++)
            {
                if (meteors[i].arrived)
                {
                    Landed(meteors[i]);
                    meteors.RemoveAt(i);
                    i--;
                    l = meteors.Count();
                }
            }
        }
    }
}
