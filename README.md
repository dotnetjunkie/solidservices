# Highly Maintainable Web Services

The Highly Maintainable Web Services project is a reference architecture application for .NET that demonstrates how to build highly maintainable web services.

This project contains no documentation, just code. Please visit the following article for the reasoning behind this project:

* [Writing Highly Maintainable WCF services](https://blogs.cuttingedge.it/steven/posts/2012/writing-highly-maintainable-wcf-services/)

For more background about the used design, please read the following articles:

* [Meanwhile… on the command side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/)
* [Meanwhile… on the query side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-query-side-of-my-architecture/)


This project contains the following Web Service projects that all expose the same set of commands and queries:

* [WCF](https://github.com/dotnetjunkie/solidservices/tree/master/src/WcfService). This project exposes command and query messages through a WCF SOAP service, while all messages are specified explicitly through a WSDL. This exposes an explicit contract to the client, although serialization and deserialization of messages is quite limited in WCF, which likely causes problems when sending and receiving messages, unless the messages are explicitly designed with the WCF-serialization constraints in mind. The [Client](https://github.com/dotnetjunkie/solidservices/tree/master/src/Client) project sends queries and commands through the WCF Service. Due to the setup, it gives full integration into the WCF pipeline, which includes security, logging, encryption, and authorization.
* [ASP.NET 'Classic' 4.8 Web API](https://github.com/dotnetjunkie/solidservices/tree/master/src/WebApiService) (includes a Swagger API). This project exposes commands and queries as REST API through the System.Web.Http stack (the 'legacy' ASP.NET Web API) of .NET 4.8. REST makes the contract less explicit, but allows more flexibility over a SOAP service. It uses JSON.NET as serialization mechanism, which allows much flexibility in defining command and query messages. Incoming requests are mapped to HttpMessageHandlers, which dispatch messages to underlying command and query handlers. In doing so, it circumvents a lot of the Web API infrastructure, which means logging and security might need to be dealt with separately. This project uses an external NuGet library to allow exposing its API through an OpenAPI/Swagger interface.
* [ASP.NET Core 3.1 Web API](https://github.com/dotnetjunkie/solidservices/tree/master/src/WebCore3Service). This project exposes commands and queries as REST API through ASP.NET Core 3.1's Web API. REST makes the contract less explicit but allows more flexibility over a SOAP service. Just as the previous 'classic' Web API project, it uses JSON.NET as serialization mechanism, which allows much flexibility in defining command and query messages. Incoming requests are mapped to specific Middleware, which dispatches messages to underlying command and query handlers. In doing so, it circumvents a lot of the ASP.NET Core Web API infrastructure, which means logging and security might need to be dealt with separately. This project has _no_ support for exposing its API through OpenAPI/Swagger.
* [ASP.NET Core 6 Web API](https://github.com/dotnetjunkie/solidservices/tree/master/src/WebCore6Service) (includes a Swagger API). This project exposes commands and queries as REST API through ASP.NET Core 6 Minimal API. The project uses .NET 6's System.Text.Json as serialization mechanism, which is the built-in mechanism. It is less flexibility compared to JSON.NET, but gives superb performance. This project makes full use of the new Minimal API functionality and maps each query and command to a specific URL. This allows full integration into the ASP.NET Core pipeline, including logging, security, and OpenAPI/Swagger. There is some extra code added to expose command and query XML documentation summaries through as part of the operation's documentation. 