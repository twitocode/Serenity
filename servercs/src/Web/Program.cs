using Serilog;
using Microsoft.OpenApi.Models;
using Serenity.Application;
using Serenity.Infrastructure;
using Serenity.Infrastructure.Persistence;
using Serenity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => {
	configuration.WriteTo.Console().MinimumLevel.Information();
});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebServices(builder.Configuration);

builder.Services.AddAuthorization();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(option => {
	option.SwaggerDoc("v1", new OpenApiInfo { Title = "Serenity Backend", Version = "v1" });
	option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
		In = ParameterLocation.Header,
		Description = "Please enter a valid token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "Bearer"
	});
	option.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type=ReferenceType.SecurityScheme,
					Id="Bearer"
				}
			},
			Array.Empty<string>()
		}
	});
});

var app = builder.Build();

app.UseCookiePolicy(new CookiePolicyOptions() {
	MinimumSameSitePolicy = SameSiteMode.Lax
});

using (var scope = app.Services.CreateScope()) {
	var services = scope.ServiceProvider;

	var context = services.GetRequiredService<DataContext>();
	context.Database.EnsureCreated();
}


// app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment()) {
	app.UseSwagger();
	app.UseSwaggerUI(options => {
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
		options.RoutePrefix = string.Empty;
	});
}
app.Run();
