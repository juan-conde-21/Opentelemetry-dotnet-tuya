# Fase de compilación utilizando el SDK de .NET 6 Alpine
# FROM mcr.microsoft.com/dotnet/sdk:6.0
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build-env
WORKDIR /app

# Copia los archivos csproj y restaura las dependencias
COPY *.csproj ./
RUN dotnet restore

# Copia los demás archivos del proyecto y construye la aplicación
COPY . ./
RUN dotnet publish -c Release -o out

#Setup de Otel instrumentacion automatica

#Ejecucion de app utilizando script de instrumentacion automatica
RUN apk update && apk add unzip && apk add curl && apk add bash
RUN mkdir /otel
RUN curl -L -o /tmp/opentelemetry-dotnet-instrumentation-linux-musl.zip https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/latest/download/opentelemetry-dotnet-instrumentation-linux-musl.zip


RUN unzip /tmp/opentelemetry-dotnet-instrumentation-linux-musl.zip -d /otel

#RUN curl -L -o /otel/otel-dotnet-auto-install.sh https://github.com/open-telemetry/opentelemetry-dotnet-instrumentation/releases/latest/download/otel-dotnet-auto-install.sh
RUN chmod +x /otel/instrument.sh
ENV OTEL_DOTNET_AUTO_HOME=/otel
RUN /bin/bash /otel/instrument.sh

# Fase de ejecución utilizando la imagen runtime de .NET 6 Alpine
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-env $OTEL_DOTNET_AUTO_HOME $OTEL_DOTNET_AUTO_HOME

ENV OTEL_TRACES_EXPORTER=otlp \
    OTEL_METRICS_EXPORTER=otlp \
    OTEL_LOGS_EXPORTER=otlp \
    OTEL_EXPORTER_OTLP_PROTOCOL=grpc \
    OTEL_DOTNET_AUTO_TRACES_CONSOLE_EXPORTER_ENABLED=true \
    OTEL_DOTNET_AUTO_METRICS_CONSOLE_EXPORTER_ENABLED=true \
    OTEL_DOTNET_AUTO_LOGS_CONSOLE_EXPORTER_ENABLED=true \
    OTEL_DOTNET_AUTO_HOME=/app/otel

#RUN chmod u+x $OTEL_DOTNET_AUTO_HOME/instrument.sh
#RUN sh $OTEL_DOTNET_AUTO_HOME/instrument.sh

ENV DOTNET_ADDITIONAL_DEPS=$OTEL_DOTNET_AUTO_HOME/AdditionalDeps:$OTEL_DOTNET_AUTO_HOME/AdditionalDeps \
    DOTNET_STARTUP_HOOKS=$OTEL_DOTNET_AUTO_HOME/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll:$OTEL_DOTNET_AUTO_HOME/net/OpenTelemetry.AutoInstrumentation.StartupHook.dll \
    DOTNET_SHARED_STORE=$OTEL_DOTNET_AUTO_HOME/store:$OTEL_DOTNET_AUTO_HOME/store \
    CORECLR_ENABLE_PROFILING=1 \
    CORECLR_PROFILER={918728DD-259F-4A6A-AC2B-B85E1B658318} \
    CORECLR_PROFILER_PATH=$OTEL_DOTNET_AUTO_HOME/linux-musl-x64/OpenTelemetry.AutoInstrumentation.Native.so

ENTRYPOINT ["dotnet", "TodoApi.dll"]

