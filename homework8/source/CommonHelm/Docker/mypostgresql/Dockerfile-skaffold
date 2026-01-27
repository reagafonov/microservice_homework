FROM  cr.yandex/crpo2q44fm7kalr36b4h/alpine:3.23.2 AS alpine
ENV POSTGRES_PASSWORD='postgres'
RUN apk add postgresql17 --no-cache && mkdir /run/postgresql && chown postgres:postgres /run/postgresql 
COPY entrypoint.sh /run/postgresql
RUN chown "postgres:postgres" /run/postgresql/entrypoint.sh
USER postgres
RUN chmod +x /run/postgresql/entrypoint.sh
EXPOSE 5432
ENTRYPOINT  /run/postgresql/entrypoint.sh 