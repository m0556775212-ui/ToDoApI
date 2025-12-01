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
FROM node:20 as build-frontend
WORKDIR /app

# העתק package.json ו-.env
COPY react-todos/package*.json ./
COPY react-todos/.env ./
RUN npm install
COPY react-todos/ ./
RUN npm run build

# --- Build .NET backend ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

# שם הקובץ צריך להיות בדיוק כמו הפרויקט שלך
COPY ["ToDoApi.csproj", "./"]
RUN dotnet restore
COPY . .

# העתק React build ל-wwwroot
COPY --from=build-frontend /app/build ./wwwroot

RUN dotnet publish -c Release -o /app/publish

# --- Final stage: run ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-backend /app/publish .

# מאזין לפורט דינמי
ENV ASPNETCORE_URLS=http://+:${PORT:-5000}
EXPOSE ${PORT:-5000}

ENTRYPOINT ["dotnet", "ToDoApi.dll"]
