# syntax=docker/dockerfile:1

####################################
# Cratis Chronicle MCP
# Build runtime image
####################################
FROM mcr.microsoft.com/dotnet/runtime:9.0.0-bookworm-slim
ARG CONFIGURATION=Release
ARG VERSION

WORKDIR /app

RUN echo Configuration = ${CONFIGURATION}
RUN echo Version = ${VERSION}

COPY Docker/copy-server-files.sh ./copy-server-files.sh
RUN chmod +x ./copy-server-files.sh

COPY ./Source/out ./out

RUN echo $PWD
RUN ./copy-server-files.sh

ENTRYPOINT ["./Cratis.Chronicle.Mcp"]
