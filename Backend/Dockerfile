FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ExamPlatform.API/ExamPlatform.API.csproj", "ExamPlatform.API/"]
RUN dotnet restore "ExamPlatform.API/ExamPlatform.API.csproj"
COPY . .
WORKDIR "/src/ExamPlatform.API"
RUN dotnet build "ExamPlatform.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExamPlatform.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExamPlatform.API.dll"]
