# Build React frontend
FROM node:20 AS build-frontend
WORKDIR /app

# העתק package.json וה־.env של React
COPY ../ToDoListReact-master/package*.json ./
COPY ../ToDoListReact-master/.env ./
COPY ../ToDoListReact-master/ ./

RUN npm install
RUN npm run build

# Build .NET backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

COPY TodoApi.csproj ./
RUN dotnet restore TodoApi.csproj

COPY . ./

# העתק React build ל-wwwroot של backend
COPY --from=build-frontend /app/build ./wwwroot

RUN dotnet publish TodoApi.csproj -c Release -o /app/publish

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-backend /app/publish .
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}
ENTRYPOINT ["dotnet", "TodoApi.dll"]
