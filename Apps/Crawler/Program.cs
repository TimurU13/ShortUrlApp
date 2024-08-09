using Crawler.Interfaces;
using Crawler;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders(); 
builder.Logging.AddConsole(); 
builder.Logging.AddDebug(); 

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IManager, Manager>();
builder.Services.AddSingleton<IWorkerPool, WorkerPool>();
builder.Services.AddSingleton<IArchivator, Archivator>();
builder.Services.AddSingleton<ICustomQueue, CustomQueue>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();