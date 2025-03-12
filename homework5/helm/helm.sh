#!/bin/bash
helm upgrade --install  monitoring ./kube-prometheus-stack -n monitoring --create-namespace --wait
helm upgrade --install nginx ./ingress-nginx --set controller.metrics.enabled=true --set controller.metrics.serviceMonitor.enabled=true --namespace nginx-ingress --create-namespace -f ./nginx-metrix-new.yaml --wait
helm upgrade --install db ./postgresql-16.3.5.tgz -n keycloak --create-namespace -f postgres.yaml --wait
helm upgrade --install settings ./vparking-settings --namespace keycloak --create-namespace --wait
