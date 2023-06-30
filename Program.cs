using Microsoft.AspNetCore.Hosting.Server;
using System.Diagnostics;
using System.Net;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using testBotAsp;


var builder = WebApplication.CreateBuilder();
List<MessageUpdate> mu = new List<MessageUpdate>();
List<string> users = new List<string>();
while(true)
{
    var botClient = new TelegramBotClient("6104982128:AAFlG61y44DFOegDeIbslhSOSyEAK8WuU9U");
    using var cts = new CancellationTokenSource();
    var receiverOptions = new ReceiverOptions
    {
        AllowedUpdates = Array.Empty<UpdateType>() // receive all update types

    };
    botClient.StartReceiving(HandleUpdateAsync,
                                HandlePollingErrorAsync,
                                receiverOptions,
                                cts.Token);

    Task HandlePollingErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        cts.Cancel();
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }


    async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
    {
        Console.WriteLine($"{update?.Message?.Chat.Username} | {update?.Message?.Text} | {update?.Message?.Contact?.PhoneNumber}");
        if (update?.Type == UpdateType.Message && update.Message != null)
        {
            for (int a = 0; a < users.Count; a++)
            {
                if (users[a] == update?.Message?.Chat.Username)
                {
                    mu[a].Mes(client, update, token);
                    return;
                }
            }
            mu.Add(new MessageUpdate());
            users.Add(update?.Message?.Chat.Username);
            mu[mu.Count() - 1].Mes(client, update, token);
        }
    }
}



// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();