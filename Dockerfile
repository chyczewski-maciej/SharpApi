FROM microsoft/dotnet:2.1-sdk as build
RUN dotnet tool install --global Paket
ENV PATH="/root/.dotnet/tools:${PATH}"
COPY . /app
WORKDIR /app/SharpApi.RestApi
RUN paket install
RUN dotnet publish -c release

FROM microsoft/dotnet:2.1-runtime as run
COPY --from=build /app/SharpApi.RestApi/bin/release/netcoreapp2.1/publish /app
COPY --from=build /app/api.yaml /app
WORKDIR /app
EXPOSE 5000
ENV ASPNETCORE_URLS="http://*:5000"
ENV LANG=C
CMD dotnet ./SharpApi.RestApi.dll ../api.yaml