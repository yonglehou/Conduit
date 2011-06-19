# Conduit
Conduit is an [Actor Model](http://en.wikipedia.org/wiki/Actor_model) based framework for developing distributed services. 
The goal of Conduit is to make building scalable and fault tolerant services a breeze by promoting an 
Event Driven Architecture (EDA) sitting on top of an Enterprise Service Bus (ESB).

Conduit is inspired by the work done in the [Akka project](http://akka.io/) which is also an Actor model 
implementation for Java and Scala.

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

Conduit allows you to write actors which have publish and subscribe capabilities for commands/queries/events. Actors can be 
located in-process or distributed out-of-process and still operate the same. This gives you the flexibility to decouple 
behaviors and responsibilities as scalability becomes more important.

Conduit applications and services are inherently resilient.

## How to get started
The two key classes are ConduitNode and Actor.

ConduitNode represents your application or service. Typically you should only have one of these in your process.
A node can contain many actors.

Actors makes up the functionality of your application or service. You should have many of these
and they will likely communicate with each other. You should break up your actors based on
responsibility. One of the benefits to EDA is the ability to plug in and out functionality because
of the loose coupling. Another benefit is if you need to scale out later you can move your actors
into new nodes distributed throughout the network.

Think of an actor like a class that instead of communicating by method calls, communicates by messaging and 
should encapsulate some functionality and behavior.

#### Create a Message
    public class ChangeCustomerAddress : Message
    {
        public void ChangeCustomerAddress()
        {
        }

        public void ChangeCustomerAddress(Guid userId, string street, string city, string state, string country)
        {
            this.UserId = userId;
            this.Street = street;
            this.City = city;
            this.State = state;
            this.Country = country;
        }

        public Guid UserId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
    }

#### Create an Actor
Implementing the IHandle interface is how your actors receive messages. Whether a messages is 
published or subscribed via other actors in-process or distributed anywhere on the network is transparent.

Publish() is used for publishing messages. Actors within your node who
subscribe to this message will receive it first and then the node will forward the message out over the 
service bus to distributed subscribers throughout the network.

    public class CustomerProfileActor : Actor, 
        IHandle<BusOpened>,
        IHandle<AnnounceServiceIdentity>,
        IHandle<ChangeCustomerAddress>
    {
        public void Handle(BusOpened message)
        {
            // The service bus has opened, lets send a query out to discover what services
            // and capabilities exist on the distributed network.
            Publish<FindAvailableServices>();
        }

        public void Handle(AnnounceServiceIdentity message)
        {
            // Now that we are receiving service identities with capabilities
            // we could do something here if we wanted.
        }

        public void Handle(ChangeCustomerAddress message)
        {
            // This command message came from another Actor that could be in-process or distributed on the network.
            // The Actor does not care where the message came from.

            // Example of updating the customer details.
            Customer customer = repository.GetById(message.CustomerId);
            customer.Street = message.Street;
            customer.City = message.City;
            customer.State = message.State;
            customer.Country = message.Country;
            customer.Save();

            // Publish an event about the customer address changing.
            // This event will publish to all subscribers in-process and out-of-process distributed on the network.            
            Publish<CustomerAddressChanged(new CustomerAddressChanged(
                customer.Street, customer.City, customer.State, customer.Country);
        }
    }

#### Open the ConduitNode
    // This is an example using the fluent builder and the Conduit.Bus.MassTransit service bus.
    ConduitNode node = ConduitNode.Create()
                           .WithServiceBus(new MassTransitBus("windsor.xml"););

    CustomerProfileActor actor = new CustomerProfileActor();
    node.Open();

## Coming soon

* Basic instructions for getting a MassTransit server running.
* More samples.
* Correlation support in messages so that relating requests with responses is built in to ease the pain of developing
on top of an event based system.
* More functionality.