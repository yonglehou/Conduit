using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Conduit;
using Conduit.Messages;
using Conduit.Messages.Queries;

namespace Conduit.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DiscoveryTests
    {
        public DiscoveryTests()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Node_And_Component_Registered_With_Capabilities()
        {
            TestConduit host = new TestConduit();
            host.Open();

            Assert.AreEqual<int>(1, host.BusOpenedCount, "Expected 1 BusOpened messages");
            Assert.AreEqual<int>(1, host.AnnounceServiceIdentityCount, "Expected only 1 AnnounceServiceIdentityCount messages");
            Assert.AreEqual<string>(typeof(TestConduit).Name, host.AnnounceServiceIdentity.Name, "ConduitNode Name should match");
            Assert.AreEqual<string>(typeof(TestConduit).FullName, host.AnnounceServiceIdentity.Type, "Conduit Type should match");
            Assert.AreEqual<int>(7, host.AnnounceServiceIdentity.Capabilities.Count(), "Expected 7 capabilities");
            Assert.AreEqual<int>(1, host.AnnounceServiceIdentity.Capabilities.Where(
                x => x == typeof(TestMessage1).FullName).Count(), "Expected TestMessage1 capability");
            Assert.AreEqual<int>(1, host.AnnounceServiceIdentity.Capabilities.Where(
                x => x == typeof(TestMessage2).FullName).Count(), "Expected TestMessage2 capability");
        }

        class TestConduit : ConduitNode,
            IHandle<BusOpened>,
            IHandle<AnnounceServiceIdentity>
        {
            public TestConduit()
                : base(new FakeServiceBus())
            {
                this.Components.Add(new TestComponent());
            }

            public int BusOpenedCount { get; private set; }
            public int AnnounceServiceIdentityCount { get; private set; }
            public AnnounceServiceIdentity AnnounceServiceIdentity { get; private set; }

            public void Handle(AnnounceServiceIdentity message)
            {
                this.AnnounceServiceIdentityCount++;
                this.AnnounceServiceIdentity = message;
            }

            public void Handle(BusOpened message)
            {
                this.BusOpenedCount++;                
            }
        }

        class TestComponent : ConduitComponent,
            IHandle<TestMessage1>,
            IHandle<TestMessage2>
        {
            public void Handle(TestMessage1 message)
            {
            }

            public void Handle(TestMessage2 message)
            {
            }
        }

        class FakeServiceBus : IServiceBus
        {
            public event MessageReceivedHandler MessageReceived;

            public void Open()
            {
            }

            public void Publish<T>(T message) where T : Message
            {
                // Mimic the behavior of a real service bus where the msgs bounce back.
                // TODO: We could remove this dependency if the message bus looped the message.
                if (MessageReceived != null)
                {
                    MessageReceived(message);
                }
            }

            public void Subscribe<T>() where T : Message
            {
            }

            public void Unsubscribe<T>() where T : Message
            {
            }

            public void Dispose()
            {
            }
        }
    }

    public class TestMessage1 : Message
    {
    }

    public class TestMessage2 : Message
    {
    }
}
