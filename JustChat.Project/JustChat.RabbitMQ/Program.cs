using BLL.Interfaces;
using BLL.Services;
using JustChat.BLL.Interfaces;
using JustChat.BLL.Services;
using JustChat.RbbitMQ.Consumer;

var host = Host
    .CreateDefaultBuilder(args)
    .Build();

await host.RunAsync();