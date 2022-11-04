using GenCore.Data.Repositories.Implementation;
using GenCore.Data.Repositories.Interface;
using GenCryptography.Service.Utilities.Interface;
using GenNotification.Service.Utilities.Interface;
using GenTokenization.Service.Utilities.Interface;
using GenValidation.Service.Utilities.Interface;

string connectionString = "Data Source=localhost;Initial Catalog=CwRetail;Persist Security Info=true;User ID=TestLogin; Password = ABC123";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IValidator>();
builder.Services.AddTransient<IKeyGenerator>();
builder.Services.AddTransient<IEncryptor>();
builder.Services.AddTransient<ITokenizer>();
builder.Services.AddTransient<IEmailDespatcher>();
builder.Services.AddTransient<ISmsDespatcher>();
builder.Services.AddTransient<IUserRepository>(s => new UserRepository(connectionString));
builder.Services.AddTransient<IRolesRepository>(s => new RolesRepository(connectionString));
builder.Services.AddTransient<IUserVerificationRepository>(s => new UserVerificationRepository(connectionString));
builder.Services.AddTransient<IUserRolesRepository>(s => new UserRolesRepository(connectionString));
builder.Services.AddTransient<IUserEncryptionRepository>(s => new UserEncryptionRepository(connectionString));
builder.Services.AddTransient<IUserTokensRepository>(s => new UserTokensRepository(connectionString));

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
