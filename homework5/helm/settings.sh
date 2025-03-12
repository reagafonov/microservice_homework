#!/bin/bash
helm upgrade --install settings ./vparking-settings --namespace keycloak --create-namespace --wait
