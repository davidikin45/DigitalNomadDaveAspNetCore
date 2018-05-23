# Digital Nomad Dave ASP.NET Core

A custom built Content Management System using Bootstrap 4, Angular, C# MVC Core, xUnit, SpecFlow, Selenium.

## URLs
* [Website](http://www.digitalnomaddave.com)
* [Swagger UI](http://www.digitalnomaddave.com/swagger)

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.

### Prerequisites

```
.NET Framework 4.7.2
```

### Installing

```
Build DND.Solution
```

```
Execute BatchFiles\Launch\LaunchDNDWebDebug.bat to launch Kestral Web Host using dotnet run.
```
```
Login to /admin with username: admin password: password
```
## Domain Events
* [Domain events: design and implementation](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

From my personal experience alot of things (emails, correspondence) in business applications need to get triggered when an entity is inserted/updated/deleted or a property is changed.\
This functionality is often built into service methods or achieved using Domain Events.\
I read alot of .NET Core articles related to deferred domain events which require the programmer to add domain events to an Aggregate Root collection property which are then dispatched either before or after DbContext SaveChanges() is called.\
Although this is useful for complex triggers, I wanted a more generic approach for simple triggers. I develped an approach where events are fired each time an entity is inserted/updated/deleted & property change.\
Once then events are fired the IDomainEventHandler interface allows the programmer to write PreCommit and PostCommit code.\
The PreCommit actions are atomic and can be used for chaining transactions. Once an exception is thrown nothing is commited.\
The PostCommit events are independent and by default are handed off to [Hangfire](https://www.hangfire.io/) for processing out of process. This would be useful for sending emails and correspondence.\
Because determining if a property has changed relies on fetching the original values from the DB for each entity instance, Interface IFirePropertyUpdatedEvents needs to be applied to the entity to opt-in to property update events.\
Below is an example of setup + two examples of IDomainEventHandlers.

```C#
 public ApplicationDbContext(string nameOrConnectionString, IDomainEvents domainEvents = null)
: base(nameOrConnectionString)
{
    _dbContextDomainEvents = new DbContextDomainEventsEF6(this, domainEvents);
}

public new int SaveChanges()
{
    int objectCount = 0;

    _dbContextDomainEvents.FirePreCommitEventsAsync().Wait();

    objectCount = base.SaveChanges();

    _dbContextDomainEvents.FirePostCommitEventsAsync().Wait();

    return objectCount;
}
```

```C#
 public class Tag
{
    public string Name
    { get; set; }
}

public class TagInsertedEventHandler : IDomainEventHandler<EntityInsertedEvent<Tag>>
{
    public async Task<Result> HandlePostCommitAsync(EntityInsertedEvent<Tag> domainEvent)
    {
        var after = domainEvent.Entity;

            //Send Email

        return Result.Ok();
    }

    public async Task<Result> HandlePreCommitAsync(EntityInsertedEvent<Tag> domainEvent)
    {
        var before = domainEvent.Entity;

        return Result.Ok();
    }
}
```

```C#
 public class Category : IFirePropertyUpdatedEvents
{
    public string Name
    { get; set; }
}

public class CategoryPropertyUpdatedEventHandler : IDomainEventHandler<EntityPropertyUpdatedEvent<Category>>
{
    private ITagDomainService _tagService;
    public CategoryPropertyUpdatedEventHandler(ITagDomainService tagService)
    {
        _tagService = tagService;
    }

    public async Task<Result> HandlePostCommitAsync(EntityPropertyUpdatedEvent<Category> domainEvent)
    {
        var after = domainEvent.Entity;
        if (domainEvent.PropertyName == "Name")
        {
            //Send Email
        }

        return Result.Ok();
    }

    public async Task<Result> HandlePreCommitAsync(EntityPropertyUpdatedEvent<Category> domainEvent)
    {
        var before = domainEvent.Entity;
        if(domainEvent.PropertyName == "Name")
        {
            //Trigger creating/updating another db record
        }

        return Result.Ok();
    }
}
```

## Running the tests

All web host processes and database creation/teardown have been automated using xUnit/NUnit test fixtures.

### DND.UnitTests (In Memory DbContext)

No database required. No Domain Events fired.

```
Build DND.Solution
```
```
Execute BatchFiles\Test\UnitTests.bat
```

### DND.IntegrationTestsXUnit (TestServer)

Automatically creates an Integration database on Local\MSSQLLOCALDB, seeds and runs an in process TestServer. On completion database is deleted. Domain Events fired.

```
Build DND.Solution
```
```
Execute BatchFiles\Test\IntegrationTestsXUnit.bat
```
### DND.IntegrationTestsNUnit (Mocking)

Automatically creates a Integration database on Local\MSSQLLOCALDB and seeds. On completion database is deleted. No Domain Events fired.

```
Build DND.Solution
```
```
Execute BatchFiles\Test\IntegrationTestsNUnit.bat
```
### DND.UITests (SpecFlow & Selenium)

Automatically creates a Integration database on Local\MSSQLLOCALDB, seeds and launches a Kestral Web Host using dotnet run. On completion database is deleted. Domain Events fired.

```
Set SeleniumUrl in test\DND.UITests\app.config
```
```
Build DND.Solution
```
```
Execute BatchFiles\Test\UITests.bat
```

## Deployment

```
Publish DND.Web
```

## Built With

* [MVC Core](https://www.asp.net/mvc) - Microsoft .NET framework
* [Bootstrap 4](https://v4-alpha.getbootstrap.com/) - responsive HTML, CSS, JS framework
* [Angular](https://angular.io/) - Google JS framework
* [Automapper](http://automapper.org/) - Convention based object-object mapper
* [Entity Framework](https://msdn.microsoft.com/en-us/library/aa937723(v=vs.113).aspx) - Microsoft ORM framework
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) - Microsoft ORM framework
* [MongoDB](https://www.mongodb.com) - NoSQL database
* [Serilog](https://serilog.net/) - Logging
* [Autofac](http://www.autofac.org/) - Dependency injector for .NET
* [Hangfire](https://rometools.github.io/rome/) - Background job processing
* [Swagger](https://swagger.io/) - API testing
* [Http Cache Headers](https://github.com/KevinDockx/HttpCacheHeaders) - ETags
* [WebSurge](http://websurge.west-wind.com/) - Load Testing
* [xUnit](https://xunit.github.io/) - Open Source .NET Testing framework
* [NUnit](http://nunit.org/) - .NET Testing framework
* [Moq](https://github.com/Moq) - .NET mocking framework
* [SpecFlow](http://specflow.org/) - Behaviour Driven Development (BDD) testing framework
* [Selenium](https://www.seleniumhq.org/) - Browser Automation
* [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x) - .NET Core Command Line Tools
* [.NET Core TestServer](https://docs.microsoft.com/en-us/aspnet/core/testing/integration-testing?view=aspnetcore-2.0) - InProcess Integration Tests

## Authors

* **Dave Ikin** - [davidikin45](https://github.com/davidikin45)

## License

This project is licensed under the MIT License

## Acknowledgments

* [Pride Parrot](http://www.prideparrot.com)
* [Mehdi El Gueddari](http://mehdi.me/ambient-dbcontext-in-ef6/)
* [Favicon.io](https://favicon.io/)
* [Favic-o-matic](http://www.favicomatic.com/)
* [Highway.Data EF6 InMemory DbContext](https://github.com/HighwayFramework/Highway.Data)

## Pluralsight Courses for Builing a Web App with MVC and Angular
* [Building a Web App with ASP.NET Core, MVC, Entity Framework Core, Bootstrap, and Angular](https://app.pluralsight.com/library/courses/aspnetcore-mvc-efcore-bootstrap-angular-web/table-of-contents)
* [Build Your Own Application Framework with ASP.NET MVC 5](https://app.pluralsight.com/library/courses/build-application-framework-aspdotnet-mvc-5/table-of-contents)
* [Building Strongly-typed AngularJS Apps with ASP.NET MVC 5](https://app.pluralsight.com/library/courses/building-strongly-typed-angularjs-apps-aspdotnet-mvc5/table-of-contents)

## Pluralsight Courses for Domain Driven Design and Rich Model
* [Clean Architecture: Patterns, Practices, and Principles](https://app.pluralsight.com/library/courses/clean-architecture-patterns-practices-principles/table-of-contents)
* [Modern Software Architecture: Domain Models, CQRS, and Event Sourcing](https://app.pluralsight.com/library/courses/modern-software-architecture-domain-models-cqrs-event-sourcing/table-of-contents)
* [Refactoring from Anemic Domain Model Towards a Rich One](https://app.pluralsight.com/library/courses/refactoring-anemic-domain-model/table-of-contents)
* [Applying Functional Principles in C#](https://app.pluralsight.com/library/courses/csharp-applying-functional-principles/table-of-contents)
* [Domain-Driven Design in Practice](https://app.pluralsight.com/library/courses/domain-driven-design-in-practice/table-of-contents)
* [Domain-Driven Design Fundamentals](https://app.pluralsight.com/library/courses/domain-driven-design-fundamentals/table-of-contents)

## Pluralsight Courses for Entity Framework
* [Entity Framework Core 2: Getting Started](https://app.pluralsight.com/library/courses/entity-framework-core-2-getting-started/table-of-contents)
* [Entity Framework in the Enterprise](https://app.pluralsight.com/library/courses/entity-framework-enterprise-update/table-of-contents)

## Pluralsight Courses for APIs
* [Building a RESTful API with ASP.NET Core](https://app.pluralsight.com/library/courses/asp-dot-net-core-restful-api-building/table-of-contents)
* [Building Business Applications with Angular and ASP.NET Core](https://www.pluralsight.com/courses/angular-aspdotnet-core-business-applications)

## Pluralsight Courses for Concurrecny and Identity
* [Enterprise Patterns: Concurrency in Business Applications](https://app.pluralsight.com/library/courses/enterprise-patterns-concurrency-business-applications/table-of-contents)
* [ASP.NET Core Identity Deep Dive](https://app.pluralsight.com/library/courses/aspdotnet-core-identity-deep-dive/table-of-contents)

## Pluralsight Courses for Unit Testing, Integration Testing and UI Testing
* [ASP.NET Core MVC Testing Fundamentals](https://app.pluralsight.com/library/courses/aspdotnet-core-mvc-testing-fundamentals/table-of-contents)
* [Mocking in .NET Core Unit Tests with Moq: Getting Started](https://app.pluralsight.com/library/courses/moq-dot-net-core-unit-tests/table-of-contents)
* [Automated Business Readable Web Tests with Selenium and SpecFlow](https://app.pluralsight.com/library/courses/selenium-specflow-automated-business-readable-web-tests/table-of-contents)
* [Business Readable Automated Tests with SpecFlow 2.0](https://app.pluralsight.com/library/courses/specflow-2-0-business-readable-automated-tests/exercise-files)
* [Integration Testing of Entity Framework Applications](https://app.pluralsight.com/library/courses/entity-framework-applications-integration-testing/table-of-contents)

## Pluralsight Courses for Chatbot
* [Creating Voice and Chatbots That Work Everywhere](https://app.pluralsight.com/library/courses/creating-voice-chatbots-work-everywhere/table-of-contents)