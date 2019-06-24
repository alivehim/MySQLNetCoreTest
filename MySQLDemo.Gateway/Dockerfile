FROM microsoft/dotnet:2.2-aspnetcore-runtime

WORKDIR /app

COPY . /app

EXPOSE 8700

ENTRYPOINT ["dotnet", "MySQLDemo.Gateway.dll"]