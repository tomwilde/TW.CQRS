using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using EstateAgent.Domain.Commands;
using EstateAgent.Reports.Model;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using TW.Commons.Interfaces;
using TW.Commons.TestingUtilities;
using TW.Commons.TestingUtilities.AutoMoqMocking;

namespace EstateAgent.System.Tests
{
    [TestFixture]
    [Category("Unit")]
    public class AdminScenarios : AbstractScenarioTest<InMemorySystemUnderTest>
    {
        // GIVEN that there are featured properties WHEN a passer by browses THEN the properties should be on show

        protected override void SetupDependencies(IUnityContainer container)
        {
            TW.Commons.BootStrapper.Register(container);

            container.RegisterMock<ILogger>();
            container.RegisterMock<ILoggerFactory>().Setup(x => x.Create()).Returns(container.Resolve<ILogger>());
            
            TW.CQRS.Core.BootStrapper.Register(container);

            TW.CQRS.Core.Data.Memory.BootStrapper.RegisterForEventStorage(container);
            TW.CQRS.Core.Data.Memory.BootStrapper.RegisterForReporting(container);
           
            Domain.BootStrapper.Register(container);
        }

        [Test]
        public void AddANewProperty()
        {
            // Define SUT within an in-memory domain... 
            using (var sut = BuildSUT())
            {
                var mre = new ManualResetEvent(false);
                var observer = Observer.Create<string>(s => mre.Set());

                // Create Scenario
                var name = "One Meeeelion Doalllaarz....";
                var price = 1000000.0m;

                var scenario = new Scenario()
                    .Given("an admin", () 
                        => sut.SubscribeToUpdates().Subscribe(observer))
                    .When("they add a new property", () 
                        => sut.IssueDomainCommand(new AddNewPropertyDomainCommand()
                        {
                            Name = name,
                            Price = price
                        }))
                    .Then("it should increment the total and be viewable", () =>
                    {
                        if (mre.WaitOne(5000))
                        {
                            var properties = sut.ReportingRepository.GetAll<Property>();

                            Assert.IsTrue(properties.Count == 1);
                            Assert.IsTrue(properties.First().Name == name);
                            Assert.IsTrue(properties.First().Price == price);
                        }
                        else
                        {
                            Assert.Fail("Timeout!");
                        }
                    });

                // ASSERT
                scenario.Assert();
            }
        }
    }
}