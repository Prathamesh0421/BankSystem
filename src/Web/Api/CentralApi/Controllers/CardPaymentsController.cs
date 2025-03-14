﻿namespace CentralApi.Controllers
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Infrastructure.Helpers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Models;
    using Services.Bank;
    using Services.Models.Banks;
    using Microsoft.Extensions.Logging;

    [ApiController]
    [Route("api/[controller]")]
    public class CardPaymentsController : ControllerBase
    {
        private readonly IBanksService banksService;
        private readonly CentralApiConfiguration configuration;

        private readonly ILogger<CardPaymentsController> _logger;

        public CardPaymentsController(IBanksService banksService, IOptions<CentralApiConfiguration> configuration, ILogger<CardPaymentsController> logger)
        {
            this.banksService = banksService;
            this.configuration = configuration.Value;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CardPaymentDto model)
        {
            try
            {
                var first3Digits = model.Number.Substring(0, 3);
                var bank = await this.banksService
                    .GetBankByBankIdentificationCardNumbersAsync<BankPaymentServiceModel>(first3Digits);
                if (bank?.CardPaymentUrl == null)
                {
                    return this.BadRequest();
                }

                var encryptedAndSignedData = TransactionHelper.SignAndEncryptData(model, this.configuration.Key, bank.ApiKey);
                var client = new HttpClient();
                var request = await client.PostAsJsonAsync(bank.CardPaymentUrl, encryptedAndSignedData);

                if (request.StatusCode != HttpStatusCode.OK)
                {
                    return this.BadRequest();
                }
                _logger.LogInformation("Service executed successfully!");
                return this.Ok();
            }
            catch
            {
                return this.BadRequest();
            }
        }
    }
}