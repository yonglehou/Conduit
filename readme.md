# Conduit
Conduit is a framework for developing distributed services. The goal of Conduit is to make building 
decoupled services a breeze by promoting an Event Driven Architecture (EDA) sitting on top of an 
Enterprise Service Bus (ESB).

The design principals behind Conduit are:

* Give you a foundation for writing services that can easily scale out for capacity growth and/or resiliency.
* Decoupling services should be simple.
* Writing event driven services should be simple.
* Moving functionality from one application/service to another should be simple.
* Usage of queues to make services resilient to load spikes and interruptions.
* Support service discovery.
* Provide a default easy to use service bus and queue technology but allow any service bus and queue 
implementation to sit underneath.
* An event based protocol layer on top of an Enterprise Service Bus (ESB).

Conduit allows you to write components which participate on a distributed network and provides the
ability to publish and subscribe messages in the form of commands/queries/events. Since Conduit is 
an Event Driven Architecture, components communicate through publishing and subscribing to messages.

Conduit abstracts the service bus implementation and currently has 1 implementation on top of MassTransit.
New service bus implementations are possible. Developing with Conduit you shouldn't need to talk to the
service bus directly.

Conduit makes publishing and subscribing to messages over the distributed network simple. When a message 
gets published, under the hood the message gets delivered to the local message bus and ConduitComponents 
who subscribe to the message within your Conduit will receive the message. The message is then sent out the 
service bus for distributed Conduits who subscribe to the message type. Conduit supports a flag for indicating 
that a message should be local only. This allows you to write internal only events that will not get published 
over the service bus.

#### Local message bus and the service bus

* The message bus is a bus internal to a Conduit (application or service).
* The service bus is the distributed network. The message bus is connected to the service bus and makes the 
service bus transparent.

This is how 2 services distributed with Conduit look connected by the service bus.

    ComponentA <-> |=========|     |=========|     |=============|     |=========|     |=========| <-> ComponentC
                   | Msg Bus | <-> | Conduit | <-> | Service Bus | <-> | Conduit | <-> | Msg Bus |
    ComponentB <-> |=========|     |=========|     |=============|     |=========|	   |=========| <-> ComponentD

Conduit has a message protocol on top of the service bus to help with common needs that come from
building a distributed service architecture. This message protocol supports service discovery and 
capability discovery. A ConduitComponent can publish a FindAvailableServices query which will
propogate the service bus. Every Conduit responds to this message by default allowing for your
ConduitComponent to detect what capabilities exist on the network. This is all automatic, by
building a ConduitComponent that subscribes to messages, the ConduitComponent already understands
what to send as a response to the query.

## How to get started
Building an array of services and components is simple using Conduit. The two key classes are a 
Conduit and a ConduitComponent.

A Conduit represents your application or service. You should only have one of these in your process.
A Conduit can contain many ConduitComponents.

ConduitComponents makes up the functionality of your application or service. You should have many of these
and they will likely communicate with each other. You should break your ConduitComponents based on
responsibility. One of the benefits to EDA is the ability to plug in and out functionality because
of the loose coupling. Another benefit is if you need to scale out later you can move your ConduitComponents
into new Conduits distributed throughout the network.

#### Namespaces
Conduit uses a similar system as XMPP uses with XML namespaces for identifying types and capabilities. These
namespaces get applied to messages, Conduits and ConduitComponents. Namespaces are defined as a string. 
It is recommended you use a Uri scheme but this is not forced.

#### Create a Message
    [ConduitMessageAttribute("http://company.com/Messages/Commands/ChangeCustomerAddress")]
    public class ChangeCustomerAddress : Message
    {
        public void ChangeCustomerAddress(Guid userId, string street, string city, string state, string country)
        {
            this.UserId = userId;
            this.Street = street;
            this.City = city;
            this.State = state;
            this.Country = country;
        }

        public Guid UserId { get; private set; }
        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
    }

#### Create a ConduitComponent
Implementing the IHandle interface is how your ConduitComponent receives messages from the local message bus
and the service bus.

Bus.Publish() is used for publishing messages to the local message bus. ConduitComponents within your Conduit who
subscribe to this message will receive it first and then the Conduit will forward the message out over the service bus
to distributed subscribers throughout the network.

    [ConduitComponent("http://company.com/Services/AccountService/CustomerProfileComponent")]
    class CustomerProfileComponent : ConduitComponent, IHandle<ChangeCustomerAddress>
    {
        public void Handle(ChangeCustomerAddress message)
        {
            // This message comes from either the local message bus or the service bus.
            // This is transparent to the ConduitComponent.

            // Example of updating the customer details.
            Customer customer = repository.GetById(message.UserId);
            customer.Street = message.Street;
            customer.City = message.City;
            customer.State = message.State;
            customer.Country = message.Country;

            // Publish an event about the customer address changing.
            // This event will traverse the Conduits local message loop and
            // to all the Conduits distributed on the service bus who subscribe to this message.
            Bus.Publish<CustomerAddressChanged(new CustomerAddressChanged(
                customer.Street, customer.City, customer.State, customer.Country);
        }
    }

#### Create a Conduit
    [Conduit("http://company.com/Services/AccountService")]
    class AccountConduit : Conduit,
        IHandle<BusOpened>,
        IHandle<AnnounceServiceIdentity>
    {
        public AccountConduit(IServiceBus bus)
            : base(bus)
        {
            // Add all your ConduitComponents here to be included in the Conduit.
            this.Components.Add(new CustomerProfileComponent());
            this.Components.Add(new OrderComponent());
        }

        public void Handle(BusOpened message)
        {
            // The service bus has opened, lets send a query out to discover what services
            // and capabilities exist on the distributed network.
            Bus.Publish<FindAvailableServices>();
        }

        public void Handle(AnnounceServiceIdentity message)
        {
            // Now that we are receiving service identities with capabilities
            // we could do something here if we wanted.
        }
    }

#### Open the Conduit
    // This is an example using Conduit.Bus.MassTransit
    MassTransitBus bus = new MassTransitBus("windsor.xml");
    AccountConduit conduit = new AccountConduit(bus);
    conduit.Open();

This is an example of how to create a simple distributed system using Conduit. More samples and 
functionality to come soon. The base protocol will evolve to include more functionality to 
support the needs required from building distributed services.