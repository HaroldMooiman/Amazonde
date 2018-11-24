using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;

namespace Models
{
    public class ASTARGrid
    {
        int sizex = 30;
        int sizey = 30;
        public Gridnodes[,] grid;

        public ASTARGrid()
        {
            //set up the grid in array form
            grid = new Gridnodes[sizex, sizey];
            for (int i = 0; i < sizex; i++)
            {
                for (int j = 0; j < sizey; j++)
                {
                    //and fill the grid with the apporpriate number for its arraylocation
                    grid[i, j] = new Gridnodes(i, j);
                }
            }

        }

        /// <summary>
        /// call if you want to see nodes in the world
        /// </summary>
        public void VisualiseNodes(World w)
        {
            foreach (Gridnodes gridnode in this.grid)
            {
                w.AddObject(new VisGridNode(gridnode.xpos, gridnode.ypos));
            }
        }
    }

    /// <summary>
    /// A Gridnodes is a single node on the ASTAR grid. Yes I know it's a multiple word for a singular object but counterpoint: screw you it's my code.
    /// </summary>
    public class Gridnodes
    {
        public Gridnodes prev;
        public double G, H;
        public double F;
        public int xnumber { get; }
        public int ynumber { get; }
        public float xpos, ypos;
        float offset = 0.5f;

        object attached = null;

        public Gridnodes(int x, int y)
        {
            this.xnumber = x;
            this.ynumber = y;
            this.xpos = x + offset;
            this.ypos = y + offset;
        }

        public void GHFs(Gridnodes prev, Gridnodes end)
        {
            this.prev = prev;
            G = CalculateDistance(this, prev) + prev.G;
            H = CalculateDistance(this, end);
            F = G + H;
        }

        private double CalculateDistance(Gridnodes one, Gridnodes two)
        {
            double result = Math.Sqrt(Math.Pow(one.xpos - two.xpos, 2) + Math.Pow(one.ypos - two.ypos, 2));
            return result;
        }
    }

    /// <summary>
    /// model class purely for visualising nodes in the world
    /// </summary>
    public class VisGridNode : Model
    {
        public VisGridNode(float x, float z) : base(x, -0.1, z, 0, 0, 0)
        {
            this.type = "node";
        }
    }

    public class VisPath : Model
    {
        public VisPath(float x, float z) : base(x, 2, z, 0, 0, 0)
        {
            this.type = "path";
        }
    }

    public class Path
    {
        private List<Gridnodes> closedlist = new List<Gridnodes>();
        private List<Gridnodes> openlist = new List<Gridnodes>();
        public List<Gridnodes> pathlist = new List<Gridnodes>();
        ASTARGrid grid;
        Gridnodes start, destination;


        public Path(ASTARGrid grid, Vector2 start, Vector2 destination)
        {
            this.grid = grid;
            this.start = grid.grid[(int)start.X, (int)start.Y];
            this.destination = grid.grid[(int)destination.X, (int)destination.Y];
            SetPath();
        }

        public void VisualisePath(World w)
        {
            foreach (Gridnodes gridnode in this.pathlist)
            {
                w.AddObject(new VisPath(gridnode.xpos, gridnode.ypos));
            }
        }

        private double CalculateDistance(Gridnodes one, Gridnodes two)
        {
            double result = Math.Sqrt(Math.Pow(one.xpos - two.xpos, 2) + Math.Pow(one.ypos - two.ypos, 2));
            return result;
        }

        public void SetPath()
        {
            //start witht he first node
            Gridnodes pivot = start;
            closedlist.Add(start);
            pivot.F = CalculateDistance(pivot, destination); //first node's G is 0 so we can directly define F

            //look around all 9 adjacent nodes
            for(int i = -1; i < 2; i++)
            {
                for(int j = -1; j < 2; j++)
                {
                    if(pivot.xnumber + i > -1 && pivot.ynumber + j > -1 && pivot.xnumber + i < 30 && pivot.ynumber + j < 30) //prevents going out of bounds in the array
                    {
                        if (i != 0 || j != 0)
                        {
                            Gridnodes newnode = grid.grid[pivot.xnumber + i, pivot.ynumber + j];
                            newnode.GHFs(pivot, destination);
                            openlist.Add(newnode);
                        }
                    }
                }
            }

            //and start searching
            while (pivot.xnumber != destination.xnumber || pivot.ynumber != destination.ynumber)
            {
                //get the best candidate
                Gridnodes best = openlist[0];
                foreach (Gridnodes test in openlist)
                {
                    if (test.F < best.F)
                    {
                        best = test;
                    }
                }

                //store previous pivot in closed
                TakeOutOpenList(pivot);
                closedlist.Add(pivot);
                pivot = best;

                //surrounding candidates get opened
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        if (pivot.xnumber + i > -1 && pivot.ynumber + j > -1 && pivot.xnumber + i < 30 && pivot.ynumber + j < 30) //prevents going out of bounds in the array
                        {
                            if(!IsClosed(grid.grid[pivot.xnumber + i, pivot.ynumber + j])) //not already closed
                            {
                                if(!IsOpen(grid.grid[pivot.xnumber + i, pivot.ynumber + j])) //if it's open and we keep getting the best candidate, naturally anything on the open list already has a shorter path towards it
                                {
                                    if (i != 0 || j != 0)
                                    {
                                        Gridnodes newnode = grid.grid[pivot.xnumber + i, pivot.ynumber + j];
                                        newnode.GHFs(pivot, destination);
                                        openlist.Add(newnode);
                                    }
                                }
                            }
                            
                        }
                    }
                }
            }

            //set the path
            Gridnodes whileloop = pivot;
            while(whileloop.prev != null)
            {
                pathlist.Insert(0, whileloop);
                whileloop = whileloop.prev;
            }
            pathlist.Insert(0, start);
        }

        private bool IsClosed(Gridnodes node)
        {
            foreach (Gridnodes listnode in closedlist)
            {
                if (node.xnumber == listnode.xnumber && node.ynumber == listnode.ynumber)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsOpen(Gridnodes node)
        {
            foreach (Gridnodes listnode in openlist)
            {
                if (node.xnumber == listnode.xnumber && node.ynumber == listnode.ynumber)
                {
                    return true;
                }
            }
            return false;
        }

        private void TakeOutOpenList(Gridnodes node)
        {
            for (int i = 0, q = openlist.Count; i < q; i++)
            {
                if (node.xnumber == openlist[i].xnumber && node.ynumber == openlist[i].ynumber)
                {
                    openlist.RemoveAt(i);
                    return;
                }
            }
        }
    }
}

