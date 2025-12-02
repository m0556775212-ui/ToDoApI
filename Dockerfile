# -----------------------
# Build React frontend
# -----------------------
FROM node:20 AS build-frontend
WORKDIR /app

# העתק package.json ו?package-lock.json של React
COPY ToDoListReact-master/package*.json ./ToDoListReact/
# העתק .env הראשי ל?frontend
COPY .env ./ToDoListReact/
# העתק כל הקליינט
COPY ToDoListReact-master/ ./ToDoListReact/

# התקנת חבילות ובניית React
RUN cd ToDoListReact && npm install
RUN cd ToDoListReact && npm run build

# -----------------------
# Build .NET backend
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

# העתק csproj והפעל restore
COPY TodoApi/TodoApi.csproj ./TodoApi/
RUN dotnet restore TodoApi/TodoApi.csproj

# העתק את כל ה-backend
COPY TodoApi/ ./TodoApi/

# העתק את React build ל-wwwroot של backend
COPY --from=build-frontend /app/ToDoListReact/build ./TodoApi/wwwroot

# publish backend
RUN dotnet publish TodoApi/TodoApi.csproj -c Release -o /app/publish

# -----------------------
# Runtime
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-backend /app/publish .

# הגדרת PORT באמצעות ENV (Render ייתן את PORT שלו)
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "TodoApi.dll"]
