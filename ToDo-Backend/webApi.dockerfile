FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


ENV JwtConfig__Secret="Lejv8BdbSIP9hNRrJmLAPXtnbnX3QUJf46G3ZnhE"
ENV DB_CONNECTION_STRING="Server=192.168.1.84,1433;Database=ToDoDb;User Id=sa;Password=p1234;MultipleActiveResultSets=true;TrustServerCertificate=True"

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToDo-Backend-FrameworksDrivers-API/ToDo-Backend-FrameworksDrivers-API.csproj", "ToDo-Backend-FrameworksDrivers-API/"]
COPY ["ToDo-Backend-CA-AplicationLayer/ToDo-Backend-CA-AplicationLayer.csproj", "ToDo-Backend-CA-AplicationLayer/"]
COPY ["ToDo-Backend-CA-EnterpriseLayer/ToDo-Backend-CA-EnterpriseLayer.csproj", "ToDo-Backend-CA-EnterpriseLayer/"]
COPY ["ToDo-Backend-CA-IntefaceAdapters-Presenters/ToDo-Backend-CA-IntefaceAdapters-Presenters.csproj", "ToDo-Backend-CA-IntefaceAdapters-Presenters/"]
COPY ["ToDo-Backend-InterfaceAdapters-Models/ToDo-Backend-InterfaceAdapters-Models.csproj", "ToDo-Backend-InterfaceAdapters-Models/"]
COPY ["ToDo-Backend-CA-InterfaceAdapters-Data/ToDo-Backend-CA-InterfaceAdapters-Data.csproj", "ToDo-Backend-CA-InterfaceAdapters-Data/"]
COPY ["ToDo-Backend-InterfaceAdapters-Mappers/ToDo-Backend-InterfaceAdapters-Mappers.csproj", "ToDo-Backend-InterfaceAdapters-Mappers/"]
COPY ["ToDo-Backend-InterfaceAdapters-Repository/ToDo-Backend-InterfaceAdapters-Repository.csproj", "ToDo-Backend-InterfaceAdapters-Repository/"]
RUN dotnet restore "ToDo-Backend-FrameworksDrivers-API/ToDo-Backend-FrameworksDrivers-API.csproj"
COPY . .
WORKDIR "/src/ToDo-Backend-FrameworksDrivers-API"
RUN dotnet build "ToDo-Backend-FrameworksDrivers-API.csproj" -c $BUILD_CONFIGURATION -o /app/build 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ToDo-Backend-FrameworksDrivers-API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo-Backend-FrameworksDrivers-API.dll"]
