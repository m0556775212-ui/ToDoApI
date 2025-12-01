# --- Build React frontend ---
FROM node:20 AS build-frontend
WORKDIR /app

# העתק package.json וה-.env של React
COPY ../ToDoListReact-master/package*.json ./ToDoListReact/
COPY ../ToDoListReact-master/.env ./ToDoListReact/

RUN cd ToDoListReact && npm install

# העתק את כל קבצי React
COPY ../ToDoListReact-master/ ./ToDoListReact/
RUN cd ToDoListReact && npm run build

# --- Build .NET backend ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src

# העתק את הקובץ csproj ואז הרץ restore
COPY TodoApi.csproj ./
RUN dotnet restore

# העתק את כל קבצי backend
COPY . ./

# העתק React build ל-wwwroot
COPY --from=build-frontend /app/ToDoListReact/build ./wwwroot

# פרסום backend
RUN dotnet publish -c Release -o /app/publish

# --- Runtime stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build-backend /app/publish .

# מאזין לפורט דינמי שמסופק ע"י Render
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}

ENTRYPOINT ["dotnet", "TodoApi.dll"]
