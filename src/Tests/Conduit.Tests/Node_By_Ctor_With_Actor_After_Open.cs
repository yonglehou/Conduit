﻿using System;
using System.Linq;
using System.Threading;
using Conduit.Tests.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Conduit.Tests
{
    [TestClass]
    public abstract class With_a_node_created_by_constructor_with_one_actor_created_after_open : SpecificationContext
    {
        protected ConduitNode Node;
        protected TestActor Actor;

        public override void Given()
        {
            Node = new ConduitNode(TimeSpan.FromMilliseconds(1));
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
    public class When_a_node_is_opened_with_a_node_created_by_constructor_with_one_actor_created_after_open
        : With_a_node_created_by_constructor_with_one_actor_created_after_open
    {
        public override void When()
        {
            Node.Open();
            Actor = new TestActor();
            Thread.Sleep(30);
        }

        [TestMethod]
        public void Then_one_busopened_event_should_publish()
        {
            Assert.AreEqual<int>(
                1, Actor.BusOpenedCount,
                "Expected only 1 BusOpened message");
        }

        [TestMethod]
        public void Then_more_than_one_announceserviceidentity_event_should_publish()
        {
            Assert.AreEqual<bool>(
                true, Actor.AnnounceServiceIdentityCount > 1,
                "Expected more than 1 AnnounceServiceIdentityCount messages");
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
    }
}
