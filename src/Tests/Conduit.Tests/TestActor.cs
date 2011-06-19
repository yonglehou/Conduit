using Conduit.Messages;
using Conduit.Tests.Messages;

namespace Conduit.Tests
{
    public class TestActor : Actor,
                IHandle<BusOpened>,
                IHandle<AnnounceServiceIdentity>,
                IHandle<TestMessage1>,
                IHandle<TestMessage2>
    {
        public int BusOpenedCount { get; private set; }
        public int AnnounceServiceIdentityCount { get; private set; }
        public AnnounceServiceIdentity AnnounceServiceIdentity { get; private set; }

        public TestActor()
        {
        }

        public TestActor(ConduitNode node)
            : base(node)
        {        
        }

        public void Handle(AnnounceServiceIdentity message)
        {
            this.AnnounceServiceIdentityCount++;
            this.AnnounceServiceIdentity = message;
        }

        public void Handle(BusOpened message)
        {
            this.BusOpenedCount++;
        }

        public void Handle(TestMessage1 message)
        {
        }

        public void Handle(TestMessage2 message)
        {
        }
    }
}
