using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Taskmanager
    {
        List<Robot> robots;
        List<Path> tasks;

        public Taskmanager(List<Robot> robots, List<Path> tasks)
        {
            this.robots = robots;
            this.tasks = tasks;
        }

        public void Update()
        {
            if(tasks.Count() == 0)
            {
                return;
            }

            if(Math.Abs(robots[0].x - tasks[0].pathlist[0].xpos) < 0.05 && Math.Abs(robots[0].z - tasks[0].pathlist[0].ypos) < 0.05)
            {
                tasks[0].pathlist.RemoveAt(0);
                if(tasks[0].pathlist.Count() == 0)
                {
                    tasks.RemoveAt(0);
                }
            }

            double movex = robots[0].x - tasks[0].pathlist[0].xpos;
            double movez = robots[0].z - tasks[0].pathlist[0].ypos;

            if(movex > 0)
            {
                movex = -0.1;
            }
            else if(movex < 0)
            {
                movex = 0.1;
            }
            if(movez > 0)
            {
                movez = -0.1;
            }
            else if(movez < 0)
            {
                movez = 0.1;
            }

            robots[0].Move(Math.Round(robots[0].x + movex, 1), robots[0].y, Math.Round(robots[0].z + movez, 1));
        }
    }

    public abstract class Task
    {

    }

    public class PathTask : Task
    {

    }

    public class DropTask : Task
    {

    }

    public class FetchTask : Task
    {

    }
}
