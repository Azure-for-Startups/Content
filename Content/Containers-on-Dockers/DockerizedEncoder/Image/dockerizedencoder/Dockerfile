FROM microsoft/azure-cli

RUN apt-get update && apt-get install -y \
    uuid \
    wget \
 && rm -rf /var/lib/apt/lists/*

COPY ./docker-entrypoint.sh /
RUN chmod +x /docker-entrypoint.sh

ENTRYPOINT ["/docker-entrypoint.sh"]
