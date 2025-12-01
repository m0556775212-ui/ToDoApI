##See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER app
#WORKDIR /app
#EXPOSE 8080
#EXPOSE 8081
#
#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#ARG BUILD_CONFIGURATION=Release
#WORKDIR /src
#COPY ["TodoApi.csproj", "."]
#RUN dotnet restore "./TodoApi.csproj"
#COPY . .
#WORKDIR "/src/."
#RUN dotnet build "./TodoApi.csproj" -c $BUILD_CONFIGURATION -o /app/build
#
#FROM build AS publish
#ARG BUILD_CONFIGURATION=Release
#RUN dotnet publish "./TodoApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "TodoApi.dll"]
# --- Build React ---
# --- Build React frontend ---
# --- Build React frontend ---
# --- Build React frontend ---
# --- Build React frontend ---
FROM node:20 AS build-frontend
WORKDIR /app

# העתק package.json ו-.env של React
COPY ../ToDoListReact-master/package*.json ./
COPY ../ToDoListReact-master/.env ./
RUN npm install

# העתק את כל קבצי React
COPY ../ToDoListReact-master/ ./
RUN npm run build

# --- Build .NET backend ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

# העתק .csproj ותקן restore
COPY ["TodoApi.csproj", "./"]
RUN dotnet restore

# העתק את שאר הקבצים של backend
COPY . .

# העתק React build לתוך wwwroot
COPY --from=build-frontend /app/build ./wwwroot

# Publish backend
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# העתק מהשלב הקודם
COPY --from=build-backend /app/publish .

# מאזין לפורט דינמי של Render
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}
EXPOSE ${PORT:-8080}

# הפעלת backend
ENTRYPOINT ["dotnet", "TodoApi.dll"]
