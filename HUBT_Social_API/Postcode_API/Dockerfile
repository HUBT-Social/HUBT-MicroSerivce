# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN apt-get update && apt-get install -y ca-certificates
RUN apt update && apt install -y iputils-ping procps curl

USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
# Chỉ copy các file .csproj để cache restore
COPY HUBT_Social_API/Postcode_API/Postcode_API.csproj HUBT_Social_API/Postcode_API/

# Restore tất cả dependencies
RUN dotnet restore HUBT_Social_API/Postcode_API/Postcode_API.csproj

# Copy toàn bộ mã nguồn sau khi restore thành công
COPY . .

# Tiếp tục build
WORKDIR /src/HUBT_Social_API/Postcode_API
RUN dotnet build -c Release -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Postcode_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Postcode_API.dll"]