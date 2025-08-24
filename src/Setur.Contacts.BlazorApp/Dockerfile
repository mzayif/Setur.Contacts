FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Setur.Contacts.BlazorApp/Setur.Contacts.BlazorApp.csproj", "src/Setur.Contacts.BlazorApp/"]
COPY ["src/Setur.Contacts.Base/Setur.Contacts.Base.csproj", "src/Setur.Contacts.Base/"]
COPY ["src/Setur.Contacts.Domain/Setur.Contacts.Domain.csproj", "src/Setur.Contacts.Domain/"]
RUN dotnet restore "src/Setur.Contacts.BlazorApp/Setur.Contacts.BlazorApp.csproj"
COPY . .
WORKDIR "/src/src/Setur.Contacts.BlazorApp"
RUN dotnet build "Setur.Contacts.BlazorApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Setur.Contacts.BlazorApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Setur.Contacts.BlazorApp.dll"]
