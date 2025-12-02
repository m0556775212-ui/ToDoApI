# --- Build React frontend ---
FROM node:20 AS build-frontend
WORKDIR /app

# העתקת קבצי React לתיקייה זמנית
COPY ../ToDoListReact-master/package*.json ./ToDoListReact/
COPY ../ToDoListReact-master/.env ./ToDoListReact/
COPY ../ToDoListReact-master/ ./ToDoListReact/
RUN cd ToDoListReact && npm install
RUN cd ToDoListReact && npm run build

# --- Build .NET backend ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

# העתקת קובץ csproj והפעלת restore
COPY TodoApi.csproj ./
RUN dotnet restore

# העתקת כל הקבצים של backend
COPY . ./

# העתקת React build ל-wwwroot
COPY --from=build-frontend /app/ToDoListReact/build ./wwwroot

# בניית ה-backend
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# העתקת הקבצים המפורסמים לשלב הריצה
COPY --from=build-backend /app/publish .

# הגדרת PORT של Render
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "TodoApi.dll"]
