# Digital Nomad Dave ASP.NET Core
[![Build status](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave/_apis/build/status/Digital%20Nomad%20Dave-ASP.NET%20Core%20(.NET%20Framework)-CI)](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave/_build/latest?definitionId=1)

A custom built Content Management System using Bootstrap 4, Angular, C# MVC Core, xUnit, SpecFlow, Selenium, Azure Devops - Previously VSTS, JwtTokens, OpenID Connect, Identity Server 4.

## URLs
* [Website](http://www.digitalnomaddave.com)
* [Swagger UI](http://www.digitalnomaddave.com/swagger)
* [GitHub](https://github.com/davidikin45/DigitalNomadDaveAspNetCore)
* [Azure DevOps](https://davidikin.visualstudio.com/Digital%20Nomad%20Dave)

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
## Folder Structure

Wherever possible I have opted for Business Component (Functional) organization over Categorical organization. 
It's very easy to stick with the standard Model, View and Controllers structure but organizing into business components makes it alot easier to maintain and gives ability to easily copy/paste an entire piece of functionality.

###  MVC
This can be achieved in ASP.NET Core by creating a custom view locator implementing IViewLocationExpander.

+ MVCImplementation
    + Aggregate Root

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/MVC.png?raw=true "MVC Folder Structure")

###  Bounded Context Assembly
This can be achieved at an assembly level by creating an Assembly per Bounded Context

+ Bounded Context
    + Aggregate Root
        + Entity

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/BC.png?raw=true "Bounded Context Folder Structure")

## Domain Driven Design, Generic Admin MVC UI and Api
When I started looking into building a generic admin UI and Api that could handle any data model it became clear that the following rules would need to be adhered to when creating the data model. Alot of the rules are the basis of Domain Driven Design.
1. Never have Many-To-Many relationships. Create a join Entity so the relationship becomes 1-To-Many and Many-To-1.
2. Never have 1-To-1 Composition/Owned ENTITY Relationships. For Composition relationships use Value types instead if required. In EF6 extend from BaseValueObject. In EF Core decorate value type with [OwnedAttribute]. Not sure if this attribute can be applied on base class. ValueTypes are included by default.
3. Use collections only for Composition/Owned relationships(1-To-Many, child cannot exist independent of the parent). Don't use collections for Aggregation/Associated relationshiships(child can exist independently of the parent, reference relationship). Never use a Collection for Navigation purposes.
4. All Aggregation/Associated Properties should have an associated Id property.
5. Use Complex properties for Aggregation/Associated relationships only (Many-To-1, child can exist independently of the parent, reference relationship). These become Navigation properties.

Once I adhered to the above rules I was able to expose the following actions in a generic way for both admin MVC UI and Api.

| Action				          | Api Http Method | Api Route								|UI Http Method      | UI Route									   | Security Scope								 |
| -------				          | --------		| --------								| ------			 | --------                                    | --------                                    |
| GetList/Search				  | GET				| api/*items*							| GET				 | admin/*items*                               | api.read									 |
| Details with Owned Collections  | GET				| api/*items*/*1*						| GET				 | admin/*items*/details/*1*                   | api.read									 |
| Full Graph Details		      | GET				| api/*items*/full-graph/*1*			| GET				 |							                   | api.read                                    |
| GetCreate				          | GET				| api/*items*/new						| GET				 | admin/*items*/new						   | api.create									 |
| Create				          | POST			| api/*items*							| POST				 | admin/*items*/new						   | api.create									 |
| BulkCreate				      | POST			| api/*items*/bulk						| 					 | 											   | api.create									 |
| GetUpdate				          | GET				| api/*items*/edit/*1*					| GET				 | admin/*items*/edit/*1*					   | api.update									 |
| Update				          |	PUT				| api/*items*/*1*						| POST				 | admin/*items*/edit/*1*	                   | api.update									 |
| Update Partial				  |	PATCH			| api/*items*/*1*						|					 |											   | api.update									 |
| GetBulkUpdate				      | GET				| api/*items*/bulk/edit/*1,2,3*			| 				     | 											   | api.update									 |
| BulkUpdate				      |	PUT				| api/*items*/bulk						| 					 |											   | api.update								     |
| BulkPatchUpdate				  |	PATCH			| api/*items*/bulk						| 					 |											   | api.update									 |
| GetDelete				          | GET				| api/*items*/delete/*1*				| GET				 | admin/*items*/delete/*1*					   | api.delete									 |
| Delete				          | DELETE			| api/*items*/*1*						| POST				 | admin/*items*/delete/*1*					   | api.delete									 |
| GetBulkDelete				      | GET				| api/*items*/bulk/delete/*1,2,3*		| 				     | 											   | api.delete									 |
| BulkDelete				      | DELETE			| api/*items*/bulk						| 				     | 											   | api.delete									 |
| GetCollectionList		          | GET				| api/*items*/*1*/*collection*			| GET				 | admin/*items*/details/*1*/*collection*      | api.read									 |
| GetCollectionListItem		      | GET				| api/*items*/*1*/*collection*/*10*		| GET				 | admin/*items*/details/*1*/*collection*/*10* | api.read									 |
| GetCreateCollectionListItem     | GET				| api/*items*/new/*collection*			| GET				 | admin/*items*/new/*collection*			   | api.write									 |
| TriggerAction					  | POST			| api/*items*/*1*/trigger-action		| GET				 | admin/*items*/*1*/trigger-action			   | api.update									 |
| BulkTriggerAction				  | POST			| api/*items*/bulk/trigger-action		| GET				 | admin/*items*/*1*/trigger-action			   | api.update									 |
| GetOptions					  | OPTIONS			| api/*items*							|					 |											   | api.read									 |
| Notifications					  | WebSocket		| api/*items*/notifications				| 					 |											   | api.notifications							 |

## Domain Events
* [Domain events: design and implementation](https://docs.microsoft.com/en-us/dotnet/standard/microservices-architecture/microservice-ddd-cqrs-patterns/domain-events-design-implementation)

From my personal experience alot of things (emails, correspondence) in business applications need to get triggered when an entity is inserted/updated/deleted or a property is changed.\
This functionality is often built into service methods or achieved using Domain Events.\
I read alot of .NET Core articles related to deferred domain events which require the programmer to add domain events to an Aggregate Root collection property which are then dispatched either before or after DbContext SaveChanges() is called.\
Although the Aggregate Root collection is needed for complex business logic triggers, I wanted a more generic approach for simple triggers and the ability to fire Before AND after SaveChanges(). I develped an approach where events are fired each time an entity is inserted/updated/deleted & property change.\
Once then events are fired the IDomainEventHandler interface allows the programmer to write PreCommit and PostCommit code.\
The PreCommit actions are atomic and can be used for chaining transactions. Once an exception is thrown nothing is commited. \
The PostCommit events are independent and by default are handed off to [Hangfire](https://www.hangfire.io/) for processing out of process. This would be useful for sending emails and correspondence.\
The key to the PreCommit functionality is using a Unit of Work pattern which is which is aware of the [Ambient DbContext](http://mehdi.me/ambient-dbcontext-in-ef6/), this ensures a DbContext is only ever created for the first call. Any subsequent inner calls use the same DbContext.\
Because determining if a property has changed relies on fetching the original values from the DB for each entity instance, Interface IFirePropertyUpdatedEvents needs to be applied to the entity to opt-in to property update events.\
For performance reaons an event is only ever fired if there is at least one Handler for that event type.\
The methods IDomainEvent methods HandlePreCommitCondition and HandlePostCommitCondition can be used for scenarios where you only want to handle PreCommit or PostCommit.\
Below is the intended design pattern and an example of two IDomainEventHandlers. Note: This is still a work in progress.

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/Domain%20Events%20Diagram.png?raw=true "Domain Events Diagram")

```C#
 public class Tag
{
    public string Name
    { get; set; }
}

public class TagInsertedEventHandler : IDomainEventHandler<EntityInsertedEvent<Tag>>
{
	public bool HandlePreCommitCondition(EntityInsertedEvent<Tag> domainEvent)
	{
		return true;
	}

    public async Task<Result> HandlePreCommitAsync(EntityInsertedEvent<Tag> domainEvent)
    {
        var before = domainEvent.Entity;

        return Result.Ok();
    }

	public bool HandlePostCommitCondition(EntityInsertedEvent<Tag> domainEvent)
	{
		return true;
	}

    public async Task<Result> HandlePostCommitAsync(EntityInsertedEvent<Tag> domainEvent)
    {
        var after = domainEvent.Entity;

            //Send Email

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

    public bool HandlePreCommitCondition(EntityPropertyUpdatedEvent<Category> domainEvent)
    {
        return true;
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

	public bool HandlePostCommitCondition(EntityPropertyUpdatedEvent<Category> domainEvent)
    {
        return true;
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
}
```

## Jwt Tokens, OpenID Connect and Identity Server 4

Todo

## Whats Next

Make generic UI compatible with any complex Dto datastructure. Convention first, configured by model data attributes where necessary.

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/GenericList.png?raw=true "Generic List")

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/GenericCreateUpdate.png?raw=true "Generic Create Update")

Make generic RESTful API compatible with any complex Dto datastructure. Convention first, configured by model data attributes where necessary.

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/GenericAPI.png?raw=true "Generic API")

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

## Continuous Integration (CI)

Using [Azure DevOps - Previously VSTS](https://azure.microsoft.com/en-us/services/devops/)

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/CI.png?raw=true "Continuous Integration")

## Continuous Deployment (CD)

Using [Visual Studio Team Services](https://www.visualstudio.com/team-services/)

![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/CD.png?raw=true "Continuous Deployment")
![alt text](https://github.com/davidikin45/DigitalNomadDaveAspNetCore/blob/master/docs/CD2.png?raw=true "Continuous Deployment 2")

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
* [Stackify Prefix](https://stackify.com/prefix/) - Runtime performance profiler for .NET Core
* [Visual Studio Team Services](https://www.visualstudio.com/team-services/) - Continuous Integration (CI) and Continuous Deployment (CD)
* [Identity Server 4](http://docs.identityserver.io/en/release/) - .NET Core Identity Provider

## Authors

* **Dave Ikin** - [davidikin45](https://github.com/davidikin45)

## License

This project is licensed under the MIT License

## Acknowledgments

* [DevOps Assessment](http://devopsassessment.azurewebsites.net/)
* [Pride Parrot](http://www.prideparrot.com)
* [Mehdi El Gueddari](http://mehdi.me/ambient-dbcontext-in-ef6/)
* [Favicon.io](https://favicon.io/)
* [Favic-o-matic](http://www.favicomatic.com/)
* [Highway.Data EF6 InMemory DbContext](https://github.com/HighwayFramework/Highway.Data)
* [Privacy Policies](https://privacypolicies.com)
* [Jwt Tokens](https://jwt.io/)
* [OpenID Connect](http://openid.net/connect/)
* [Productivity Power Pack](https://marketplace.visualstudio.com/items?itemName=VisualStudioProductTeam.ProductivityPowerPack2017)

## Pluralsight Courses for Builing a Web App with MVC and Angular
* [Building a Web App with ASP.NET Core, MVC, Entity Framework Core, Bootstrap, and Angular](https://app.pluralsight.com/library/courses/aspnetcore-mvc-efcore-bootstrap-angular-web/table-of-contents)
* [Build Your Own Application Framework with ASP.NET MVC 5](https://app.pluralsight.com/library/courses/build-application-framework-aspdotnet-mvc-5/table-of-contents)
* [Building Strongly-typed AngularJS Apps with ASP.NET MVC 5](https://app.pluralsight.com/library/courses/building-strongly-typed-angularjs-apps-aspdotnet-mvc5/table-of-contents)
* [Improving .NET Core MVC Apps Using Extension Points](https://app.pluralsight.com/library/courses/dotnet-core-mvc-apps-extensions/table-of-contents)

## Pluralsight Courses for Builing a Single Page Application (SPA) with Angular, API and OpenID Connect
* [Building Business Applications with Angular and ASP.NET Core](https://app.pluralsight.com/library/courses/angular-aspdotnet-core-business-applications/table-of-contents)

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

## Pluralsight Courses for securing APIs with Bearer Tokens
* [Building a Web App with ASP.NET Core, MVC, Entity Framework Core, Bootstrap, and Angular](https://app.pluralsight.com/library/courses/aspnetcore-mvc-efcore-bootstrap-angular-web/table-of-contents)

## Pluralsight Courses for Concurrecny and Identity
* [Enterprise Patterns: Concurrency in Business Applications](https://app.pluralsight.com/library/courses/enterprise-patterns-concurrency-business-applications/table-of-contents)
* [ASP.NET Core Identity Deep Dive](https://app.pluralsight.com/library/courses/aspdotnet-core-identity-deep-dive/table-of-contents)

## Pluralsight Courses for Identity Server (OAuth2 and OpenID Connect)
* [Securing ASP.NET Core with OAuth2 and OpenID Connect](https://app.pluralsight.com/library/courses/asp-dotnet-core-oauth2-openid-connect-securing/exercise-files)
* [Securing ASP.NET Core 2 with OAuth2 and OpenID Connect](https://app.pluralsight.com/library/courses/securing-aspdotnet-core2-oauth2-openid-connect/table-of-contents)
* [ASP.NET Core 2 Authentication Playbook](https://app.pluralsight.com/library/courses/aspnet-core-identity-management-playbook/table-of-contents)

## Pluralsight Courses for Unit Testing, Integration Testing and UI Testing
* [ASP.NET Core MVC Testing Fundamentals](https://app.pluralsight.com/library/courses/aspdotnet-core-mvc-testing-fundamentals/table-of-contents)
* [Mocking in .NET Core Unit Tests with Moq: Getting Started](https://app.pluralsight.com/library/courses/moq-dot-net-core-unit-tests/table-of-contents)
* [Automated Business Readable Web Tests with Selenium and SpecFlow](https://app.pluralsight.com/library/courses/selenium-specflow-automated-business-readable-web-tests/table-of-contents)
* [Business Readable Automated Tests with SpecFlow 2.0](https://app.pluralsight.com/library/courses/specflow-2-0-business-readable-automated-tests/exercise-files)
* [Integration Testing of Entity Framework Applications](https://app.pluralsight.com/library/courses/entity-framework-applications-integration-testing/table-of-contents)

## Pluralsight Courses for Continuous Integration and Deployment
* [Developing with .NET on Microsoft Azure - Getting Started](https://app.pluralsight.com/library/courses/developing-dotnet-microsoft-azure-getting-started/table-of-contents)
* [Getting Started with Visual Studio Team Services (2018)](https://app.pluralsight.com/library/courses/getting-started-visual-studio-team-services-2018/table-of-contents)

## Pluralsight Courses for Chatbot
* [Creating Voice and Chatbots That Work Everywhere](https://app.pluralsight.com/library/courses/creating-voice-chatbots-work-everywhere/table-of-contents)