# Hermes.Net
Fast & Minimalist Web Server Framework for Microsoft Universal Platform

```cs
class HelloWorldMiddleware : IMiddleware
{
	public Task Run(HttpContext context)
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
Soon on nuget.

## Features

	* Run in a Universal App (Windows 10 Desktop, Mobile, IoT)
	* Robust Routing

## People

The current lead maintaine is [Valentin Michalak] (https://github.com/vmichalak)

## Licence

[MIT](LICENCE)
