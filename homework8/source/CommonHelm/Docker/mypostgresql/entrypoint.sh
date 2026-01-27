#!/bin/sh
if [ -z "$(ls -A /var/lib/postgresql/data)" ]
then
  cd 
  if test -d /var/lib/postgresql/data; then echo 'dir exists'; else mkdir /var/lib/postgresql/data; fi
  chmod 0700 /var/lib/postgresql/data 
  echo "${POSTGRES_PASSWORD}" >> initp
  initdb -D /var/lib/postgresql/data --pwfile initp
  rm -f initp
  echo " host all all 0.0.0.0/0 md5" >> /var/lib/postgresql/data/pg_hba.conf 
  echo "listen_addresses='*'" >> /var/lib/postgresql/data/postgresql.conf 
  sed -i "s|^#* *unix_socket_directories.*$|unix_socket_directories=\'/tmp\'|" /var/lib/postgresql/data/postgresql.conf   
else
   echo "initialized"
fi

postgres -D /var/lib/postgresql/data