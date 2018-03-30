# Lykke.Service.PayInvoice

LykkePay invoice subsystem

Client: [Nuget](https://www.nuget.org/packages/Lykke.Service.PayInvoice.Client/)

# Client usage

Register client services in container:

```cs
ContainerBuilder builder;
...
var settings = new PayInvoiceServiceClientSettings("http://<service>:[port]/");
builder.RegisterInstance(new PayInvoiceClient(settings))
    .As<IPayInvoiceClient>()
    .SingleInstance();
```

Now you can use:

* IPayInvoiceClient - HTTP client for service API