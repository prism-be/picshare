// -----------------------------------------------------------------------
//  <copyright file = "BaseServiceBusWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using Azure.Messaging.ServiceBus;

namespace Prism.Picshare.Services.Workers.Workers;

public abstract class BaseServiceBusWorker<T> : BackgroundService, IAsyncDisposable
{
    private readonly ILogger _logger;

    private ServiceBusClient? _client;
    private ServiceBusProcessor? _processor;

    protected BaseServiceBusWorker(ILogger logger)
    {
        _logger = logger;
    }
    
    public abstract string Queue { get; }
    
    public async ValueTask DisposeAsync()
    {
        if (_processor != null)
        {
            await _processor.DisposeAsync().ConfigureAwait(false);
        }

        if (_client != null)
        {
            await _client.DisposeAsync().ConfigureAwait(false);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var serviceBusProcessorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        };

        _client = new ServiceBusClient(EnvironmentConfiguration.GetMandatoryConfiguration("SERVICE_BUS_CONNECTION_STRING"));
        _processor = _client.CreateProcessor(Queue, serviceBusProcessorOptions);
        _processor.ProcessMessageAsync += ProcessMessagesAsync;
        _processor.ProcessErrorAsync += ProcessErrorAsync;

        _logger.LogInformation("Start listening to queue : {queue}", Queue);
        await _processor.StartProcessingAsync(stoppingToken).ConfigureAwait(false);
    }
    
    private Task ProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Message handler encountered an exception");
        _logger.LogDebug($"- ErrorSource: {arg.ErrorSource}");
        _logger.LogDebug($"- Entity Path: {arg.EntityPath}");
        _logger.LogDebug($"- FullyQualifiedNamespace: {arg.FullyQualifiedNamespace}");

        return Task.CompletedTask;
    }

    private async Task ProcessMessagesAsync(ProcessMessageEventArgs args)
    {
        try
        {
            _logger.LogInformation("Processing message {squenceNumber} on queue {queue}", args.Message.SequenceNumber, Queue);
            var payload = args.Message.Body.ToObjectFromJson<T>();

            await args.CompleteMessageAsync(args.Message).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Erreur when processing message on queue {queue}", Queue);
            throw;
        }
    }
    
    public async Task ProcessMessage<T>(T payload)
}