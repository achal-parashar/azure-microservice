﻿using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using WisdomPetMedicine.Hospital.Api.Infrastructure;

namespace WisdomPetMedicine.Hospital.Api.IntegrationEvents
{
    public class PetTransferredToHospitalIntegrationEventHandler : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<PetTransferredToHospitalIntegrationEventHandler> _logger;
        private readonly ServiceBusClient _client;
        private readonly ServiceBusProcessor _processor;
        public PetTransferredToHospitalIntegrationEventHandler(IConfiguration configuration,
                                                               IServiceScopeFactory serviceScopeFactory,
                                                               ILogger<PetTransferredToHospitalIntegrationEventHandler> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;

            _client = new ServiceBusClient(configuration["ServiceBus:ConnectionString"]);
            _processor = _client.CreateProcessor(configuration["ServiceBus:TopicName"], configuration["ServiceBus:SubscriptionName"]);
            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopProcessingAsync(cancellationToken);
        }

        private async Task Processor_ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            var theEvent = JsonConvert.DeserializeObject<PetTransferredToHospitalIntegration>(body);
            await args.CompleteMessageAsync(args.Message);

            using var scope = _serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<HospitalDbContext>();

            var existingPatient = await dbContext.PatientsMetadata.FindAsync(theEvent.Id);
            if (existingPatient == null)
            {
                dbContext.PatientsMetadata.Add(theEvent);
                await dbContext.SaveChangesAsync();
            }
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception.ToString());
            return Task.CompletedTask;
        }
    }
}
