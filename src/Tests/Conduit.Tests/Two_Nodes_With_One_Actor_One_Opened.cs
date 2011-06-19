using System;
using System.Linq;
using System.Threading;
using Conduit.Tests.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Conduit.Tests
{
    [TestClass]
    public abstract class With_two_nodes_created_by_constructor_with_one_actor_and_one_opened : SpecificationContext
    {
        protected ConduitNode Node1;
        protected ConduitNode Node2;
        protected TestActor Actor1;
        protected TestActor Actor2;
        
        public override void Given()
        {
            Node1 = new ConduitNode();
            Node2 = new ConduitNode();
            Actor1 = new TestActor(Node1);
            Actor2 = new TestActor(Node2);
        }

        [TestCleanup]
        public override void Cleanup()
        {
            if (Node1 != null)
            {
                Node1.Close();
            }
            if (Node2 != null)
            {
                Node2.Close();
            }
        }
    }

    [TestClass]
    public class When_two_node_are_opened_with_two_nodes_created_by_constructor_with_one_actor_and_one_opened
        : With_two_nodes_created_by_constructor_with_one_actor_and_one_opened
    {
        public override void When()
        {
            this.Node1.Open();
        }

        [TestMethod]
        public void Then_one_busopened_event_should_publish()
        {
            Assert.AreEqual<int>(
                1, Actor1.BusOpenedCount, 
                "Expected only 1 BusOpened message for Actor1");

            Assert.AreEqual<int>(
                0, Actor2.BusOpenedCount,
                "Expected no BusOpened message for Actor2");
        }

        [TestMethod]
        public void Then_one_announceserviceidentity_event_should_publish()
        {
            Assert.AreEqual<int>(
                1, Actor1.AnnounceServiceIdentityCount,
                "Expected only 1 AnnounceServiceIdentityCount message for Actor1");

            Assert.AreEqual<int>(
                0, Actor2.AnnounceServiceIdentityCount,
                "Expected no AnnounceServiceIdentityCount message for Actor2");
        }

        [TestMethod]
        public void Then_conduitnode_two_serviceidentity_response_should_be_null()
        {
            Assert.AreEqual<bool>(
                true, Actor2.AnnounceServiceIdentity == null,
                "Actor2 should not have an AnnounceServiceIdentity response");
        }

        // TODO: I'm not sure if this test makes sense still.
        [TestMethod]
        public void Then_conduitnode_name_should_match_name_from_serviceidentity_response()
        {
            Assert.AreEqual<string>(
                typeof(ConduitNode).Name, Actor1.AnnounceServiceIdentity.Name,
                "ConduitNode Name should match for Actor1");
        }

        [TestMethod]
        public void Then_conduitnode_typename_should_match_typename_from_serviceidentity_response()
        {
            Assert.AreEqual<string>(
                typeof(ConduitNode).FullName, Actor1.AnnounceServiceIdentity.Type,
                "Conduit Type should match for Actor1");
        }

        [TestMethod]
        public void Then_serviceidentity_response_should_contain_7_capabilities()
        {
            Assert.AreEqual<int>(
                7, Actor1.AnnounceServiceIdentity.Capabilities.Count(),
                "Expected 7 capabilities for Actor1");
        }

        [TestMethod]
        public void Then_serviceidentity_response_should_have_testmessage1_capability()
        {
            Assert.AreEqual<int>(
                1, Actor1.AnnounceServiceIdentity.Capabilities.Where(x => x == typeof(TestMessage1).FullName).Count(),
                "Expected TestMessage1 capability for Actor1");
        }

        [TestMethod]
        public void Then_serviceidentity_response_should_have_testmessage2_capability()
        {
            Assert.AreEqual<int>(
                1, Actor1.AnnounceServiceIdentity.Capabilities.Where(x => x == typeof(TestMessage2).FullName).Count(),
                "Expected TestMessage2 capability for Actor1");
        }
    }
}
