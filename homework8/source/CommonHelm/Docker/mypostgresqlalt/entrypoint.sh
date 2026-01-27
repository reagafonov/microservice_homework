#!/bin/sh
if [ -z "$(ls -A /var/lib/pgsql/data)" ]
then
  cd 
  if test -d /var/lib/pgsql/data; then echo 'dir exists'; else mkdir /var/lib/pgsql/data; fi
  chmod 0700 /var/lib/pgsql/data 
  echo "${POSTGRES_PASSWORD}" >> initp
  initdb -D /var/lib/pgsql/data --pwfile initp
  
  rm -f initp
  echo " host all all 0.0.0.0/0 md5" >> /var/lib/pgsql/data/pg_hba.conf 
  echo "listen_addresses='*'" >> /var/lib/pgsql/data/postgresql.conf 
  sed -i "s|^#* *unix_socket_directories.*$|unix_socket_directories=\'/tmp\'|" /var/lib/pgsql/data/postgresql.conf   
  sed -i "s|^#* *log_destination.*$|log_destination=\'stderr\'|" /var/lib/pgsql/data/postgresql.conf   
  sed -i "s|^#* *logging_collector.*$|logging_collector=\'off\'|" /var/lib/pgsql/data/postgresql.conf   
else
   echo "initialized"
fi

rm -f /run/pgsql/entrypoint.sh
postgres -D /var/lib/pgsql/data -c log_statement=all 

