# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DemoMvc/DemoMvc.csproj", "DemoMvc/"]
COPY ["DemoMvc.Core/DemoMvc.Core.csproj", "DemoMvc.Core/"]
COPY ["DemoMvc.Infrastructure/DemoMvc.Infrastructure.csproj", "DemoMvc.Infrastructure/"]
RUN dotnet restore "DemoMvc/DemoMvc.csproj"
COPY . .
WORKDIR "/src/DemoMvc"
RUN dotnet build "DemoMvc.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "DemoMvc.csproj" -c Release -o /app/publish

# Stage 3: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoMvc.dll"]
