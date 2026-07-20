# 1. AŞAMA: İnşa (Build)
# SDK imajını kullanarak kodları derliyoruz
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sadece proje dosyasını kopyalayıp bağımlılıkları indiriyoruz (Docker Cache avantajı)
COPY ["TodoApp.API.csproj", "./"]
RUN dotnet restore "TodoApp.API.csproj"

# Tüm kodları kopyalayıp uygulamayı yayınlıyoruz (Publish)
COPY . .
RUN dotnet publish "TodoApp.API.csproj" -c Release -o /app/publish

# 2. AŞAMA: Çalıştırma (Run)
# Sadece çalışma zamanı (Runtime) imajını alıyoruz, bu sayede imaj boyutu çok küçük oluyor
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Build aşamasında oluşan yayın dosyalarını bu imaja kopyalıyoruz
COPY --from=build /app/publish .

# .NET 8'de varsayılan port 8080'dir
EXPOSE 8080

ENTRYPOINT ["dotnet", "TodoApp.API.dll"]
