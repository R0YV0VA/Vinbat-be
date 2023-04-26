#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Vinbat-be/Vinbat-be.csproj", "Vinbat-be/"]
RUN dotnet restore "Vinbat-be/Vinbat-be.csproj"
COPY . .
WORKDIR "/src/Vinbat-be"
RUN dotnet build "Vinbat-be.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Vinbat-be.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Vinbat-be.dll"]