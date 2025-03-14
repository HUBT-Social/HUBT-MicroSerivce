FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
RUN apt-get update && apt-get install -y ca-certificates
RUN apt update && apt install -y iputils-ping procps curl
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

# Chỉ copy file .csproj trước để cache restore
COPY HUBT_Social_API/Chat_Data_API/Chat_Data_API.csproj HUBT_Social_API/Chat_Data_API/

# Restore dependencies
RUN dotnet restore HUBT_Social_API/Chat_Data_API/Chat_Data_API.csproj

# Copy toàn bộ source code
COPY . .

# Build project
WORKDIR /src/HUBT_Social_API/Chat_Data_API
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Chat_Data_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Run final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Chat_Data_API.dll"]
