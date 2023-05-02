using Vinbat_be;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Vinbat_be.Contexts;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Vinbat_be.Telegram;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Authentication.Certificate;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = Encoding.UTF8;

var botClient = new TelegramBotClient("6021365111:AAFGpLocQpOiQHKCWGrIiNME79dUOkZqAVk");
Thread thread = new(async () => await StartBot());
thread.Start();

var builder = WebApplication.CreateBuilder(args);

// Add connection to builder from secret.json
builder.Configuration.AddJsonFile("secret.json", optional: true, reloadOnChange: true);

// Add connection to db from secret.json
builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Debug")));
builder.Services.AddDbContext<CasesContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Debug")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("NonAuth", policy =>
    {
        policy.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AuthReq", policy =>
    {
        policy.WithOrigins("http://176.32.9.163:3000", "http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
                .AllowCredentials();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/// Add JWT authentication
var key = "zxcvbnm123321mnbvcxz";
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddSingleton<JwtAuthentificationManager>(new JwtAuthentificationManager(key));
builder.Services.AddAuthentication(CertificateAuthenticationDefaults.AuthenticationScheme)
    .AddCertificate();
///Add JWT authentication/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NonAuth");

app.UseCors("AuthReq");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

async Task StartBot()
{
    using CancellationTokenSource cts = new();

    // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
    ReceiverOptions receiverOptions = new()
    {
        AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
    };
    botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

    var me = await botClient.GetMeAsync();
}
async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    if (update.Message is not { } message)
        return;
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;
    CasesContext dbContext = new CasesContext(new DbContextOptionsBuilder<CasesContext>()
        .UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Vinabat;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False")
        .Options);
    DataBaseTG dataBaseTG = new DataBaseTG(dbContext);

    if (messageText == "/GetAll")
    {
        var cases = await dataBaseTG.GetAllCases();
        string requestMessage = "";
        foreach (var item in cases)
        {
            if(item.Status)
                requestMessage += $"Id: {item.Id}\nUsername: {item.Username}\nConnection type: {item.Connection}\nMessage: {item.Message}\nStatus: Done\n\n\n";
            else
                requestMessage += $"Id: {item.Id}\nUsername: {item.Username}\nConnection type: {item.Connection}\nMessage: {item.Message}\nStatus: Not done\n\n\n";
        }
        await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: requestMessage,
            cancellationToken: cancellationToken);
    }
    else if (messageText.Contains("/CloseOpen:"))
    {
        if(messageText.Substring(11).Length >= Convert.ToString(Int32.MaxValue).Length)
        {
            await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "Id is too long",
                  cancellationToken: cancellationToken);
            return;
        }
        var id = Convert.ToInt32(messageText.Substring(11));
        var @case = await dataBaseTG.CloseOpenCase(id);
        if (@case == null)
        {
            await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: $"Case {id} not found",
                  cancellationToken: cancellationToken);
        }
        else
        {
            var requesMessage = "";
            if (@case.Status)
                requesMessage = $"Id: {@case.Id}\nUsername: {@case.Username}\nConnection type: {@case.Connection}\nMessage: {@case.Message}\nStatus: Done";
            else
                requesMessage = $"Id: {@case.Id}\nUsername: {@case.Username}\nConnection type: {@case.Connection}\nnMessage: {@case.Message}\nStatus: Not done";
            await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: requesMessage,
                  cancellationToken: cancellationToken);
        }
    }
    else if (messageText.Contains("/GetCase:"))
    {
        if (messageText.Substring(9).Length >= Convert.ToString(Int32.MaxValue).Length)
        {
            await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: "Id is too long",
                  cancellationToken: cancellationToken);
            return;
        }
        var id = Convert.ToInt32(messageText.Substring(9));
        var @case = await dataBaseTG.GetCase(id);
        if (@case == null)
        {
            await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: $"Case {id} not found",
                  cancellationToken: cancellationToken);
        }
        else
        {
            var requesMessage = "";
            if (@case.Status)
                requesMessage = $"Id: {@case.Id}\nUsername: {@case.Username}\nConnection type: {@case.Connection}\nMessage: {@case.Message}\nStatus: Done";
            else
                requesMessage = $"Id: {@case.Id}\nUsername: {@case.Username}\nConnection type: {@case.Connection}\nMessage: {@case.Message}\nStatus: Not done";
            await botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: requesMessage,
                  cancellationToken: cancellationToken);
        }
    }
    else
    {
        await botClient.SendTextMessageAsync(
                       chatId: chatId,
                       text: "Введіть ",
                       cancellationToken: cancellationToken);
    }
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}