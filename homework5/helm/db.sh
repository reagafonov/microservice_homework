#!/bin/bash
helm upgrade --install db ./postgresql-16.3.5.tgz -n keycloak --create-namespace -f postgres.yaml --wait