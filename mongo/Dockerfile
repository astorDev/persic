ARG VERSION=latest
FROM mongo:${VERSION}

COPY init.js /docker-entrypoint-initdb.d/init.js

CMD ["mongod", "--replSet", "rs0", "--bind_ip_all"]