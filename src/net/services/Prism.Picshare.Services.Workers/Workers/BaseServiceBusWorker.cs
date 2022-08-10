// -----------------------------------------------------------------------
//  <copyright file = "BaseServiceBusWorker.cs" company = "Prism">
//  Copyright (c) Prism.All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System.Text;
using System.Text.Json;
using MediatR;
using Prism.Picshare.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Prism.Picshare.Services.Workers.Workers;

public abstract class BaseServiceBusWorker<T> : BackgroundService
{
    private readonly ILogger _logger;

    private IModel? _channel;
    private IConnection? _connection;
    private string? _consumerTag;

    protected BaseServiceBusWorker(ILogger logger)
    {
        _logger = logger;
    }

    public abstract string Queue { get; }

    protected virtual ushort PrefetchCount => 50;

    public override void Dispose()
    {
        if (_channel != null)
        {
            if (!string.IsNullOrWhiteSpace(_consumerTag))
            {
                _channel.BasicCancel(_consumerTag);
            }

            _channel.Dispose();
        }

        if (_connection != null)
        {
            _connection.Dispose();
        }

        GC.SuppressFinalize(this);
        base.Dispose();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Start listening on queue {queue}", Queue);

        var factory = new ConnectionFactory
        {
            Uri = new Uri(EnvironmentConfiguration.GetMandatoryConfiguration("RABBITMQ_CONNECTION_STRING"))
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.QueueDeclare("workers/" + Queue, true, false, false);
        _channel.QueueBind("workers/" + Queue, Queue, Topics.Subscription);

        _channel.BasicQos(0, PrefetchCount, false);

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (_, args) =>
        {
            using var scope = SharedInstances.ServiceProvider.CreateScope();

            try
            {
                _logger.LogInformation("Processing message {id} on queue {queue}", args.DeliveryTag, Queue);

                var body = args.Body.ToArray();
                var json = Encoding.Default.GetString(body);
                var payload = JsonSerializer.Deserialize<T>(json);

                if (payload != null)
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    await ProcessMessageAsync(mediator, payload);
                }

                _channel.BasicAck(args.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Cannot process message {id} on queue {queue}", args.DeliveryTag, Queue);
            }
        };

        _consumerTag = _channel.BasicConsume("workers/" + Queue, false, consumer);

        return Task.CompletedTask;
    }

    internal abstract Task ProcessMessageAsync(IMediator mediator, T payload);
}