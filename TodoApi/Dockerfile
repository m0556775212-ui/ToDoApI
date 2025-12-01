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
