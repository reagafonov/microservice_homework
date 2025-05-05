#!/bin/sh
 until nc -z db 5432 &> /dev/null          
        do
          echo "waiting for db container..."
          sleep 2
        done