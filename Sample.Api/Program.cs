using MassTransit;
using MassTransit.Definition;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sample.Components.Consuments;
using Sample.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
builder.Services.AddMassTransit(cfg =>
{
    cfg.AddBus(provider => Bus.Factory.CreateUsingRabbitMq());

    //  ok
    //cfg.AddRequestClient<ISubmitOrder>();
    // jawnie zdefiniowany konsumer (nie powinno się tego robić), działa tak jak wyżej: w przypadku gdy 
    // zostanie wysłana wiadomość bez uruchomionego konsumera, gdy ten wstanie to odbierze wiadomość
    cfg.AddRequestClient<ISubmitOrder>(new Uri($"queue:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));
    //  w przypadku gdy zostanie wysłana wiadomość bez uruchomionego konsumera, gdy ten wstanie to NIE odbierze wiadomości, wysyłanie
    // wiadomości zakończy się błędem
    cfg.AddRequestClient<ISubmitOrder>(new Uri($"exchange:{KebabCaseEndpointNameFormatter.Instance.Consumer<SubmitOrderConsumer>()}"));

});

builder.Services.AddMassTransitHostedService();

builder.Services.AddOpenApiDocument(cfg => cfg.PostProcess = d => d.Info.Title = "Sample API Site");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
