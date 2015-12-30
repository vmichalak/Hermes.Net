# Hermes.Net
Fast & Minimalist Web Server Framework for Microsoft Universal Platform

```cs
class HelloWorldMiddleware : IMiddleware
{
	public async Task Run(HttpContext context)
	{
		context.Response.Send("Hello World");
	}
}
```

```cs
HttpServer server = new HttpServer();

server.AddGetRoute("/", new HelloWorldMiddleware());

server.Listen(port);
```
## Installation

To install Hermes.Net, run the following command in the Package Manager Console.

```
Install-Package HermesNet -Version 0.3.0-beta1 -Pre
```

In your project you need to activate capabilities :

	* Internet (Client & Server)
	* Private Networks (Client & Server)

## Features

	* Run in a Universal App (Windows 10 Desktop, Mobile, IoT)
	* Robust Routing

## People

The current lead maintainer is [Valentin Michalak] (https://github.com/vmichalak)

## Licence

[MIT](LICENCE)
