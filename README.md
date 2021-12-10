# Highly Maintainable Web Services

The Highly Maintainable Web Services project is a reference architecture application for .NET that demonstrates how to build highly maintainable web services.

This project contains no documentation, just code. Please visit the following article for the reasoning behind this project:

* [Writing Highly Maintainable WCF services](https://blogs.cuttingedge.it/steven/posts/2012/writing-highly-maintainable-wcf-services/)

For more background about the used design, please read the following articles:

* [Meanwhile… on the command side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-command-side-of-my-architecture/)
* [Meanwhile… on the query side of my architecture](https://blogs.cuttingedge.it/steven/posts/2011/meanwhile-on-the-query-side-of-my-architecture/)


This project contains the following Web Service projects that all expose the same set of commands and queries:

* [WCF](https://github.com/dotnetjunkie/solidservices/tree/master/src/WcfService)
* [ASP.NET 'Classic' 4.8 Web API](https://github.com/dotnetjunkie/solidservices/tree/master/src/WebApiService) (includes a Swagger API)
* [ASP.NET Core 3.1 Web API](https://github.com/dotnetjunkie/solidservices/tree/master/src/WebCore3Service)
* [ASP.NET Core 6 Web API](https://github.com/dotnetjunkie/solidservices/tree/master/src/WebCore6Service) (includes a Swagger API)