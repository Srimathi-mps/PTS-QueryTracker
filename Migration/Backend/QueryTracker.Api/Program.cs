using QueryTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// ── 1 GB file upload size limit ─────────────────────────────────────────────────
// 1 GB = 1073741824 bytes
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 1073741824L;     // 1 GB
    o.ValueLengthLimit         = int.MaxValue;
    o.MultipartHeadersLengthLimit = int.MaxValue;
});
builder.WebHost.ConfigureKestrel(k =>
{
    k.Limits.MaxRequestBodySize = 1073741824L;    // 1 GB on Kestrel
});

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register application services
builder.Services.AddScoped<IRaiseProblemsService, RaiseProblemsService>();
builder.Services.AddScoped<ISummaryReportService, SummaryReportService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// CORS – allow the React UI from both dev (Vite) and production origins
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUI", policy =>
        policy.WithOrigins(
                  // Dev: Vite dev server (common ports)
                  "http://localhost:5173",
                  "http://localhost:5174",
                  "http://localhost:5175",
                  "http://localhost:5176",
                  // Prod: IIS-hosted UI at http://10.42.4.127/CTSUI
                  "http://10.42.4.127",
                  "https://itrakcloudbooks.mpslimited.com",
                  "http://itrakcloudbooks.mpslimited.com",
				   "http://192.168.60.224/"
              )
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();   // ← shows full stack trace on 500
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "QueryTracker API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("AllowUI");
app.UseAuthorization();
app.MapControllers();

app.Run();
