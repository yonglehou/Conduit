using System;
using System.Linq;
using System.Threading;
using Conduit.Tests.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Conduit.Tests
{
    [TestClass]
    public abstract class With_a_node_created_by_ctor_with_a_bus_and_one_actor : SpecificationContext
    {
        protected ConduitNode Node;
        protected TestActor Actor;

        public override void Given()
        {
            FakeServiceBus bus = new FakeServiceBus();
            Node = new ConduitNode(bus);
            Actor = new TestActor();
        }

        [TestCleanup]
        public override void Cleanup()
        {
            if (Node != null)
            {
                Node.Close();
            }
        }
    }

    [TestClass]
    public class When_a_node_is_opened_with_a_node_created_by_ctor_with_a_bus_and_one_actor
        : With_a_node_created_by_ctor_with_a_bus_and_one_actor
    {
        public override void When()
        {
            Node.Open();
        }

        [TestMethod]
        public void Then_one_busopened_event_should_publish()
        {
            Assert.AreEqual<int>(
                1, Actor.BusOpenedCount,
                "Expected only 1 BusOpened message");
        }

        [TestMethod]
        public void Then_one_announceserviceidentity_event_should_publish()
        {
            Assert.AreEqual<int>(
                1, Actor.AnnounceServiceIdentityCount,
                "Expected only 1 AnnounceServiceIdentityCount message");
        }

        // TODO: I'm not sure if this test makes sense still.
        [TestMethod]
        public void Then_conduitnode_name_should_match_name_from_serviceidentity_response()
        {
            Assert.AreEqual<string>(
                typeof(ConduitNode).Name, Actor.AnnounceServiceIdentity.Name,
                "ConduitNode Name should match");
        }

        [TestMethod]
        public void Then_conduitnode_typename_should_match_typename_from_serviceidentity_response()
        {
            Assert.AreEqual<string>(
                typeof(ConduitNode).FullName, Actor.AnnounceServiceIdentity.Type,
                "Conduit Type should match");
        }

        [TestMethod]
        public void Then_serviceidentity_response_should_contain_7_capabilities()
        {
            Assert.AreEqual<int>(
                7, Actor.AnnounceServiceIdentity.Capabilities.Count(),
                "Expected 7 capabilities");
        }

        [TestMethod]
        public void Then_serviceidentity_response_should_have_testmessage1_capability()
        {
            Assert.AreEqual<int>(
                1, Actor.AnnounceServiceIdentity.Capabilities.Where(x => x == typeof(TestMessage1).FullName).Count(),
                "Expected TestMessage1 capability");
        }

        [TestMethod]
        public void Then_serviceidentity_response_should_have_testmessage2_capability()
        {
            Assert.AreEqual<int>(
                1, Actor.AnnounceServiceIdentity.Capabilities.Where(x => x == typeof(TestMessage2).FullName).Count(),
                "Expected TestMessage2 capability");
        }

        ////[TestMethod]
        //public void Node_And_Actor_Without_ServiceBus_Registered_With_Capabilities_Using_Fluent_Builder_AutoOpen()
        //{
        //    ConduitNode.Init()
        //        .WithServiceBus(new FakeServiceBus())
        //        .WithDiscoveryFrequency(TimeSpan.FromMilliseconds(1))
        //        .Open();

        //    TestActor actor = new TestActor();

        //    Thread.Sleep(100);

        //    Assert.AreEqual<int>(1, actor.BusOpenedCount, "Expected 1 BusOpened messages");
        //    Assert.AreEqual<bool>(true, actor.AnnounceServiceIdentityCount > 1, "Expected more than 1 AnnounceServiceIdentityCount messages");
        //    Assert.AreEqual<string>(typeof(ConduitNode).Name, actor.AnnounceServiceIdentity.Name, "ConduitNode Name should match");
        //    Assert.AreEqual<string>(typeof(ConduitNode).FullName, actor.AnnounceServiceIdentity.Type, "Conduit Type should match");
        //    Assert.AreEqual<int>(7, actor.AnnounceServiceIdentity.Capabilities.Count(), "Expected 7 capabilities");
        //    Assert.AreEqual<int>(1, actor.AnnounceServiceIdentity.Capabilities.Where(
        //        x => x == typeof(TestMessage1).FullName).Count(), "Expected TestMessage1 capability");
        //    Assert.AreEqual<int>(1, actor.AnnounceServiceIdentity.Capabilities.Where(
        //        x => x == typeof(TestMessage2).FullName).Count(), "Expected TestMessage2 capability");

        //    ConduitNode.Current.Close();
        //}

        ////[TestMethod]
        //public void Node_And_Two_Actors_Without_ServiceBus_Registered_With_Capabilities_Using_Fluent_Builder_AutoOpen()
        //{
        //    ConduitNode.Init()
        //        .WithServiceBus(new FakeServiceBus())
        //        .WithDiscoveryFrequency(TimeSpan.FromMilliseconds(1));

        //    TestActor actor1 = new TestActor();

        //    ConduitNode.Current.Open();

        //    TestActor2 actor2 = new TestActor2();

        //    Thread.Sleep(30);

        //    Assert.AreEqual<int>(1, actor1.BusOpenedCount, "Expected 1 BusOpened messages");
        //    Assert.AreEqual<bool>(true, actor1.AnnounceServiceIdentityCount > 1, "Expected more than 1 AnnounceServiceIdentityCount messages");
        //    Assert.AreEqual<string>(typeof(ConduitNode).Name, actor1.AnnounceServiceIdentity.Name, "ConduitNode Name should match");
        //    Assert.AreEqual<string>(typeof(ConduitNode).FullName, actor1.AnnounceServiceIdentity.Type, "Conduit Type should match");
        //    Assert.AreEqual<int>(7, actor1.AnnounceServiceIdentity.Capabilities.Count(), "Expected 7 capabilities");
        //    Assert.AreEqual<int>(1, actor1.AnnounceServiceIdentity.Capabilities.Where(
        //        x => x == typeof(TestMessage1).FullName).Count(), "Expected TestMessage1 capability");
        //    Assert.AreEqual<int>(1, actor1.AnnounceServiceIdentity.Capabilities.Where(
        //        x => x == typeof(TestMessage2).FullName).Count(), "Expected TestMessage2 capability");

        //    Assert.AreEqual<int>(1, actor2.BusOpenedCount, "Expected 1 BusOpened messages");
        //    Assert.AreEqual<bool>(true, actor2.AnnounceServiceIdentityCount > 1, "Expected more than 1 AnnounceServiceIdentityCount messages");
        //    Assert.AreEqual<string>(typeof(ConduitNode).Name, actor2.AnnounceServiceIdentity.Name, "ConduitNode Name should match");
        //    Assert.AreEqual<string>(typeof(ConduitNode).FullName, actor2.AnnounceServiceIdentity.Type, "Conduit Type should match");
        //    Assert.AreEqual<int>(7, actor2.AnnounceServiceIdentity.Capabilities.Count(), "Expected 7 capabilities");
        //    Assert.AreEqual<int>(1, actor2.AnnounceServiceIdentity.Capabilities.Where(
        //        x => x == typeof(TestMessage1).FullName).Count(), "Expected TestMessage1 capability");
        //    Assert.AreEqual<int>(1, actor2.AnnounceServiceIdentity.Capabilities.Where(
        //        x => x == typeof(TestMessage2).FullName).Count(), "Expected TestMessage2 capability");

        //    ConduitNode.Current.Close();
        //}
    }
}
