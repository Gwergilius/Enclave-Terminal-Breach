using Enclave.Echelon.Core.Services;
using Enclave.Ghost.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Game state: scoped so each browser session gets its own instance.
builder.Services.AddScoped<GhostAppState>();

// Solver: stateless, safe as singleton.
builder.Services.AddSingleton<ISolverConfiguration, GhostSolverConfig>();
builder.Services.AddSingleton<IRandom, GameRandom>();
builder.Services.AddSingleton<IPasswordSolver, HouseGambitPasswordSolver>();
builder.Services.AddSingleton<IPasswordSolver, BestBucketPasswordSolver>();
builder.Services.AddSingleton<IPasswordSolver, TieBreakerPasswordSolver>();
builder.Services.AddSingleton<ISolverFactory, SolverFactory>();

var app = builder.Build();

app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<Enclave.Ghost.App>()
   .AddInteractiveServerRenderMode();

await app.RunAsync();
