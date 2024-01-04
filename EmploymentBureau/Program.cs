using EmploymentBureau.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddScoped<EmployerRepository>(provider =>
    new EmployerRepository(configuration.GetConnectionString("EmploymentBureau")!));
builder.Services.AddScoped<ContractRepository>(provider =>
    new ContractRepository(configuration.GetConnectionString("EmploymentBureau")!));
builder.Services.AddScoped<ApplicantRepository>(provider =>
    new ApplicantRepository(configuration.GetConnectionString("EmploymentBureau")!));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();