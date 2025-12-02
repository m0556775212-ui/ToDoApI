# --- Build React frontend ---
FROM node:20 AS build-frontend
WORKDIR /app
COPY ToDoListReact-master/package*.json ./ToDoListReact/
COPY ToDoListReact-master/.env ./ToDoListReact/
COPY ToDoListReact-master/ ./ToDoListReact/
RUN cd ToDoListReact && npm install
RUN cd ToDoListReact && npm run build

# --- Build .NET backend ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src
COPY TodoApi/TodoApi.csproj ./TodoApi/
RUN dotnet restore TodoApi/TodoApi.csproj
COPY TodoApi/ ./TodoApi/
# העתק React build ל-wwwroot של ה-backend
COPY --from=build-frontend /app/ToDoListReact/build ./TodoApi/wwwroot
RUN dotnet publish TodoApi/TodoApi.csproj -c Release -o /app/publish

# --- Runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-backend /app/publish .
# שימוש ב-ENV עבור PORT
ENV ASPNETCORE_URLS=http://+:${PORT}
EXPOSE ${PORT}
ENTRYPOINT ["dotnet", "TodoApi.dll"]
