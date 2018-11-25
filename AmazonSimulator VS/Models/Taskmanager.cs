using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace Models
{
    public class Taskmanager
    {
        ASTARGrid grid;
        List<Robot> robots;
        List<Task> tasklist = new List<Task>();

        public Taskmanager(List<Robot> robots, ASTARGrid grid)
        {
            this.robots = robots;
        }

        public void AddTask(Task t)
        {
            tasklist.Add(t);
        }

        public void Update()
        {
            for (int i = 0, l = tasklist.Count(); i < l; i++)
            {
                if (tasklist[i].completed)
                {
                    tasklist.RemoveAt(i);
                    i--;
                    l = tasklist.Count();
                }
            }

            foreach (Robot r in robots)
            {
                for(int i = 0, l = tasklist.Count(); i<l; i++)
                {
                    if(tasklist[i].robot == r)
                    {
                        tasklist[i].DoTask();
                        break;
                    }
                }
            }

        }
    }

    public abstract class Task
    {
        public bool completed = false;
        public Robot robot;

        public abstract void DoTask();

    }

    public class PathTask : Task
    {
        Path path;

        public PathTask(Robot r, Vector2 destination, ASTARGrid grid)
        {
            robot = r;
            path = new Path(grid, new Vector2((int)r.x, (int)r.z), destination);
        }

        public PathTask(Robot r, Pickup p, ASTARGrid grid)
        {
            robot = r;
            path = new Path(grid, new Vector2((int)r.x, (int)r.z), new Vector2((int)p.x, (int)p.z));
        }

        public override void DoTask()
        {
            if(completed)
            {
                return;
            }

            if (Math.Abs(robot.x - path.pathlist[0].xpos) < 0.05 && Math.Abs(robot.z - path.pathlist[0].ypos) < 0.05)
            {
                path.pathlist.RemoveAt(0);
                if (path.pathlist.Count() == 0)
                {
                    completed = true;
                    return;
                }
            }

            double movex = robot.x - path.pathlist[0].xpos;
            double movez = robot.z - path.pathlist[0].ypos;

            if (movex > 0)
            {
                movex = -0.1;
            }
            else if (movex < 0)
            {
                movex = 0.1;
            }
            if (movez > 0)
            {
                movez = -0.1;
            }
            else if (movez < 0)
            {
                movez = 0.1;
            }

            robot.Move(Math.Round(robot.x + movex, 1), robot.y, Math.Round(robot.z + movez, 1));
        }
    }

    public class DropTask : Task
    {
        Vector2 destination;
        ASTARGrid grid;

        public DropTask(Robot r, Vector2 destination, ASTARGrid grid)
        {
            robot = r;
            this.destination = destination;
            this.grid = grid;
        }

        public override void DoTask()
        {
            Pickup p = robot.attached;
            robot.attached = null;

            p.Move(grid.grid[(int)destination.X, (int)destination.Y].xpos, 0.3, grid.grid[(int)destination.X, (int)destination.Y].ypos);
            grid.grid[(int)destination.X, (int)destination.Y].attached = p;
            completed = true;
        }
    }

    public class FetchTask : Task
    {
        Pickup pickup;
        ASTARGrid grid;

        public FetchTask(Robot r, Pickup p, ASTARGrid grid)
        {
            robot = r;
            pickup = p;
            this.grid = grid;
        }

        public override void DoTask()
        {
            robot.attached = pickup;
            grid.grid[(int)pickup.x, (int)pickup.z].attached = null;
            completed = true;
        }
    }
}
