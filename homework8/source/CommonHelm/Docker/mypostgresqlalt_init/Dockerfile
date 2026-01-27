FROM cr.yandex/crpo2q44fm7kalr36b4h/alt:p11 AS alt
ENV POSTGRES_PASSWORD='postgres'
ENV POSTGRES_CREATE_USER='user1'
ENV POSTGRES_CREATE_USER_PASSWORD='user_pwd'
ENV POSTGRES_DB='db'
ENV POSTGRES_SERVER='postgresql'
RUN apt-get update && apt-get install postgresql17 gettext  -y && mkdir /run/pgsql && chown postgres:postgres /run/pgsql 
COPY init.sql /run/pgsql
COPY db.sql /run/pgsql
RUN chown -R "postgres:postgres" /run/pgsql/
USER postgres
ENTRYPOINT envsubst < /run/pgsql/init.sql > /run/pgsql/init-to-run.sql && \
  envsubst < /run/pgsql/db.sql > /run/pgsql/db-to-run.sql && \
  psql postgresql://postgres:$POSTGRES_PASSWORD@$POSTGRES_SERVER/postgres -f /run/pgsql/init-to-run.sql && psql postgresql://postgres:$POSTGRES_PASSWORD@$POSTGRES_SERVER/postgres -f /run/pgsql/db-to-run.sql && rm -f /run/pgsql/*.sql