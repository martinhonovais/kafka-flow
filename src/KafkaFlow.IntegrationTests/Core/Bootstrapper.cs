namespace KafkaFlow.IntegrationTests.Core
{
    using System;
    using System.IO;
    using System.Threading;
    using Confluent.Kafka;
    using global::Microsoft.Extensions.Configuration;
    using global::Microsoft.Extensions.DependencyInjection;
    using global::Microsoft.Extensions.Hosting;
    using KafkaFlow.Compressor;
    using KafkaFlow.Compressor.Gzip;
    using KafkaFlow.IntegrationTests.Core.Handlers;
    using KafkaFlow.IntegrationTests.Core.Messages;
    using KafkaFlow.IntegrationTests.Core.Middlewares;
    using KafkaFlow.IntegrationTests.Core.Middlewares.Producers;
    using KafkaFlow.Serializer;
    using KafkaFlow.Serializer.Json;
    using KafkaFlow.Serializer.ProtoBuf;
    using KafkaFlow.TypedHandler;
    using AutoOffsetReset = KafkaFlow.AutoOffsetReset;

    public static class Bootstrapper
    {
        private const string ProtobufTopicName = "test-protobuf";
        private const string JsonTopicName = "test-json";
        private const string GzipTopicName = "test-gzip";
        private const string JsonGzipTopicName = "test-json-gzip";
        private const string ProtobufGzipTopicName = "test-protobuf-gzip";
        private const string ProtobufGzipTopicName2 = "test-protobuf-gzip-2";
        public const string PauseResumeTopicName = "test-pause-resume";
        
        private const string ProtobufGroupId = "consumer-protobuf";
        private const string JsonGroupId = "consumer-json";
        private const string GzipGroupId = "consumer-gzip";
        private const string JsonGzipGroupId = "consumer-json-gzip";
        private const string ProtobufGzipGroupId = "consumer-protobuf-gzip";
        private const string PauseResumeGroupId = "consumer-pause-resume";

        public const int MaxPollIntervalMs = 7000;

        private static readonly Lazy<IServiceProvider> lazyProvider = new Lazy<IServiceProvider>(SetupProvider);

        public static IServiceProvider GetServiceProvider() => lazyProvider.Value;

        private static IServiceProvider SetupProvider()
        {
            var builder = Host
                .CreateDefaultBuilder()
                .ConfigureAppConfiguration(
                    (builderContext, config) =>
                    {
                        config
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile(
                                "conf/appsettings.json",
                                false,
                                true)
                            .AddEnvironmentVariables();
                    })
                .ConfigureServices(SetupServices)
                .UseDefaultServiceProvider(
                    (context, options) =>
                    {
                        options.ValidateScopes = true;
                        options.ValidateOnBuild = true;
                    });

            var host = builder.Build();
            var bus = host.Services.CreateKafkaBus();
            bus.StartAsync().GetAwaiter().GetResult();
            //Wait partition assignment
            Thread.Sleep(10000);

            return host.Services;
        }

        private static void SetupServices(HostBuilderContext context, IServiceCollection services)
        {
            var brokers = context.Configuration.GetValue<string>("Kafka:Brokers");

            services.AddKafka(
                kafka => kafka
                    .UseLogHandler<TraceLogHandler>()
                    .AddCluster(
                        cluster => cluster
                            .WithBrokers(brokers.Split(';'))
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(ProtobufTopicName)
                                    .WithGroupId(ProtobufGroupId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSingleTypeSerializer<TestMessage1, ProtobufMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(PauseResumeTopicName)
                                    .WithGroupId(PauseResumeGroupId)
                                    .WithBufferSize(3)
                                    .WithWorkersCount(3)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .WithConsumerConfig(new ConsumerConfig
                                    {
                                        MaxPollIntervalMs = MaxPollIntervalMs, 
                                        SessionTimeoutMs = MaxPollIntervalMs
                                    })
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSingleTypeSerializer<TestMessage1, ProtobufMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<PauseResumeHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topic(JsonTopicName)
                                    .WithGroupId(JsonGzipGroupId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>()
                                                        .AddHandler<MessageHandler2>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(GzipTopicName)
                                    .WithGroupId(GzipGroupId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .Add<GzipMiddleware>()
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(JsonGzipTopicName)
                                    .WithGroupId(JsonGzipGroupId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor(r => new GzipMessageCompressor())
                                            .AddSerializer(r => new JsonMessageSerializer())
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddConsumer(
                                consumer => consumer
                                    .Topics(ProtobufGzipTopicName, ProtobufGzipTopicName2)
                                    .WithGroupId(ProtobufGzipGroupId)
                                    .WithBufferSize(100)
                                    .WithWorkersCount(10)
                                    .WithAutoOffsetReset(AutoOffsetReset.Latest)
                                    .WithAutoCommitIntervalMs(1)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                            .AddSerializer<ProtobufMessageSerializer>()
                                            .AddTypedHandlers(
                                                handlers =>
                                                    handlers
                                                        .WithHandlerLifetime(InstanceLifetime.Singleton)
                                                        .AddHandler<MessageHandler>())
                                    )
                            )
                            .AddProducer<JsonProducer>(
                                producer => producer
                                    .DefaultTopic(JsonTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                    )
                            )
                            .AddProducer<JsonGzipProducer>(
                                producer => producer
                                    .DefaultTopic(JsonGzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<JsonMessageSerializer>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<ProtobufProducer>(
                                producer => producer
                                    .DefaultTopic(ProtobufTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSingleTypeSerializer<TestMessage1, ProtobufMessageSerializer>()
                                    )
                            )
                            .AddProducer<ProtobufGzipProducer>(
                                producer => producer
                                    .DefaultTopic(ProtobufGzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer<ProtobufMessageSerializer>()
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                            .AddProducer<ProtobufGzipProducer2>(
                                producer => producer
                                    .DefaultTopic(ProtobufGzipTopicName2)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddSerializer(r => new ProtobufMessageSerializer())
                                            .AddCompressor(r => new GzipMessageCompressor())
                                    )
                            )
                            .AddProducer<GzipProducer>(
                                producer => producer
                                    .DefaultTopic(GzipTopicName)
                                    .AddMiddlewares(
                                        middlewares => middlewares
                                            .AddCompressor<GzipMessageCompressor>()
                                    )
                            )
                    )
            );

            services.AddSingleton<JsonProducer>();
            services.AddSingleton<JsonGzipProducer>();
            services.AddSingleton<ProtobufProducer>();
            services.AddSingleton<ProtobufGzipProducer2>();
            services.AddSingleton<ProtobufGzipProducer>();
            services.AddSingleton<GzipProducer>();
        }
    }
}
