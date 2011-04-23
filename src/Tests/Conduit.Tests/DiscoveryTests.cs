using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Conduit;
using Conduit.Bus.MassTransit;
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
        private const string TestConduitName = "TestConduit";
        private const string TestConduitUri = "http://Test/TestConduit";
        private const string TestComponentName = "TestComponent";
        private const string TestComponentUri = "http://Test/TestConduit/TestComponent";

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
        public void Conduit_And_Component_Registered_With_Capabilities()
        {
            TestConduit host = new TestConduit();
            host.Open();

            Assert.AreEqual<int>(1, host.BusOpenedCount, "Expected 1 BusOpened messages");
            Assert.AreEqual<int>(1, host.AnnounceServiceIdentityCount, "Expected only 1 AnnounceServiceIdentityCount messages");
            Assert.AreEqual<string>(TestConduitName, host.AnnounceServiceIdentity.Name, "Conduit Name should match");
            Assert.AreEqual<int>(7, host.AnnounceServiceIdentity.Capabilities.Count(), "Expected 7 capabilities");
            Assert.AreEqual<int>(1, host.AnnounceServiceIdentity.Capabilities.Where(
                x => x == TestMessage1.Uri).Count(), "Expected TestMessage1 capability");
            Assert.AreEqual<int>(1, host.AnnounceServiceIdentity.Capabilities.Where(
                x => x == TestMessage2.Uri).Count(), "Expected TestMessage2 capability");
        }

        [Conduit(TestConduitUri)]
        class TestConduit : Conduit,
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
                Bus.Publish<FindAvailableServices>();
            }
        }

        [ConduitComponent(TestComponentUri)]
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
            public void Open()
            {
            }

            public void Publish<T>(T message) where T : class
            {
            }

            public void Subscribe<T>() where T : class
            {
            }

            public void Dispose()
            {
            }
        }

        [ConduitMessageAttribute(Uri)]
        public class TestMessage1 : Message
        {
            public const string Uri = "http://test/messages/TestMessage1";
        }

        [ConduitMessageAttribute(Uri)]
        public class TestMessage2 : Message
        {
            public const string Uri = "http://test/messages/TestMessage2";
        }
    }
}
