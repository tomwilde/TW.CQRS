using System;
using System.Collections.Generic;
using TW.Commons.Interfaces;

namespace TW.Commons.TestingUtilities.Acceptance.Scenarios
{
    public class Scenario<T>
    {
        private readonly T _sut;

        private readonly LinkedList<Step> steps = new LinkedList<Step>();
        private readonly ILogger _logger;

        public Scenario(ILoggerFactory loggerFactory, T sut)
        {
            _logger = loggerFactory.Create();

            _sut = sut;
        }

        public Scenario<T> Given(string txt, Func<dynamic> action)
        {
            steps.AddLast(new GivenStep() { Msg = txt, StepCall =  action });
            return this;
        }

        public Scenario<T> When(string txt, Func<T, dynamic, dynamic> action)
        {
            steps.AddLast(new WhenStep() { Msg = txt, StepCall =  action });
            return this;
        }

        public Scenario<T> Then(string txt, Action<T, dynamic> action)
        {
            steps.AddLast(new ThenStep() { Msg = txt, StepCall =  action });
            return this;
        }
        
        public void Run()
        {
            dynamic parameter = null;
            dynamic result = null;

            var node = steps.First;

            while (node != null)
            {
                var msg = node.Value.Msg;
                var step = node.Value.StepCall;

                _logger.InfoFormat("{0}: {1} ", node.Value.Type, msg);

                if (node.Value.Type == StepName.Given)
                {
                    parameter = (step as Func<dynamic>).Invoke();
                }
                else if (node.Value.Type == StepName.When)
                {
                    result = (step as Func<T, dynamic, dynamic>).Invoke(_sut, parameter);
                }
                else if (node.Value.Type == StepName.Then)
                {
                    (step as Action<T, dynamic>).Invoke(_sut, result);
                }

                node = node.Next;
            }

            _logger.Info("[Complete]");
        }
    }
}