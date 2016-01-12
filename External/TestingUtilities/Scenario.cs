using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TW.Commons.TestingUtilities
{
    public class Scenario
    {
        private string scenario;

        private readonly LinkedList<Tuple<string, Action>> actions = new LinkedList<Tuple<string, Action>>();

        public Scenario()
        {
            this.scenario = new StackFrame(1).GetMethod().Name;
        }

        public Scenario(string txt)
        {
            this.scenario = txt;
        }

        public Scenario Given(string txt, Action action)
        {
            actions.Clear();
            actions.AddFirst(new Tuple<string, Action>(string.Format("Given {0}", txt), action));
            return this;
        }

        public Scenario When(string txt, Action action)
        {
            actions.AddLast(new Tuple<string, Action>(string.Format("When {0}", txt), action));
            return this;
        }

        public Scenario Then(string txt, Action action)
        {
            actions.AddLast(new Tuple<string, Action>(string.Format("Then {0}", txt), action));
            return this;
        }

        public Scenario And(string txt, Action action)
        {
            actions.AddLast(new Tuple<string, Action>(string.Format("  and {0}", txt), action));
            return this;
        }

        public void Assert()
        {
            var node = actions.First;

            while (node != null)
            {
                var name = node.Value.Item1;
                var action = node.Value.Item2;

                Console.WriteLine(name);
                action.Invoke();

                node = node.Next;
            } 
        }
    }

}
