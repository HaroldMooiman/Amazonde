using System;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using System.Numerics;

namespace Models {
    public class World : IObservable<Command>, IUpdatable
    {
        private List<Robot> robots = new List<Robot>(); //DONT forget to change this to robots when you write the child class
        private List<Model> worldObjects = new List<Model>();
        private List<IObserver<Command>> observers = new List<IObserver<Command>>();
        private ASTARGrid griddy;
        Taskmanager taskm;
        
        public World() {
            griddy = new ASTARGrid(); //setup grid
            taskm = new Taskmanager(robots, griddy); //setup taskmanager
            //griddy.VisualiseNodes(this); //visualise nodes
            Robot r = CreateRobot(0,0,0);
            r.Move(0, 0, 0);

            
            
            Pickup pickup = new Pickup(new Vector2(27, 29), griddy);
            worldObjects.Add(pickup);

            PickupTask(r, pickup);
        }

        private void PickupTask(Robot r, Pickup p)
        {
            Task t = new PathTask(r, p, griddy);
            taskm.AddTask(t);
            t = new FetchTask(r, p, griddy);
            taskm.AddTask(t);
            t = new PathTask(r, new Vector2(0, 0), griddy);
            taskm.AddTask(t);
            t = new DropTask(r, new Vector2(0, 0), griddy);
            taskm.AddTask(t);
            t = new PathTask(r, new Vector2(15, 15), griddy);
            taskm.AddTask(t);
        }

        public void AddObject(Model m)
        {
            this.worldObjects.Add(m);
            
        }

        private Robot CreateRobot(double x, double y, double z) {
            Robot r = new Robot(x,y,z,0,0,0);
            worldObjects.Add(r);
            this.robots.Add(r);
            return r;
        }

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            if (!observers.Contains(observer)) {
                observers.Add(observer);

                SendCreationCommandsToObserver(observer);
            }
            return new Unsubscriber<Command>(observers, observer);
        }

        private void SendCommandToObservers(Command c) {
            for(int i = 0; i < this.observers.Count; i++) {
                this.observers[i].OnNext(c);
            }
        }

        private void SendCreationCommandsToObserver(IObserver<Command> obs) {
            foreach(Model m3d in worldObjects) {
                obs.OnNext(new UpdateModel3DCommand(m3d));
            }
        }

        public bool Update(int tick)
        {
            taskm.Update();
            for(int i = 0; i < worldObjects.Count; i++) {
                Model u = worldObjects[i];

                if(u is IUpdatable) {
                    bool needsCommand = ((IUpdatable)u).Update(tick);

                    if(needsCommand) {
                        SendCommandToObservers(new UpdateModel3DCommand(u));
                    }
                }
            }

            return true;
        }
    }

    internal class Unsubscriber<Command> : IDisposable
    {
        private List<IObserver<Command>> _observers;
        private IObserver<Command> _observer;

        internal Unsubscriber(List<IObserver<Command>> observers, IObserver<Command> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose() 
        {
            if (_observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}