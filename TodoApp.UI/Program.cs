using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System;
using System.Net.Http;
using TodoApp.UI; // Kendi UI projenizin namespace'i

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- 3. DİNAMİK API ADRESİ EKLENEN KISIM ---
builder.Services.AddScoped(sp => 
{
    // UI hangi adresten/IP'den açıldıysa onu algılar
    var baseAddress = builder.HostEnvironment.BaseAddress; 
    var uri = new Uri(baseAddress);
    
    // İstekleri otomatik olarak aynı IP'nin 5050 portuna (API'ye) yönlendirir
    var apiAddress = $"{uri.Scheme}://{uri.Host}:5050/"; 
    
    return new HttpClient { BaseAddress = new Uri(apiAddress) };
});
// -------------------------------------------

await builder.Build().RunAsync();
