# 1. AŞAMA: İnşa (Build)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["TodoApp.UI.csproj", "./"]
RUN dotnet restore "TodoApp.UI.csproj"

COPY . .
RUN dotnet publish "TodoApp.UI.csproj" -c Release -o /app/publish

# 2. AŞAMA: Sunum (Serve)
# Blazor WASM statik dosyalardan ibaret olduğu için Nginx ile sunuyoruz
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Derlenen statik wwwroot dosyalarını Nginx'in içine kopyalıyoruz
COPY --from=build /app/publish/wwwroot .

# Blazor uygulamalarında sayfa yenilendiğinde 404 hatası vermemesi için Nginx ayarı (SPA Routing)
RUN sed -i 's/try_files $uri $uri\/ =404;/try_files $uri $uri\/ \/index.html;/g' /etc/nginx/conf.d/default.conf

EXPOSE 80
