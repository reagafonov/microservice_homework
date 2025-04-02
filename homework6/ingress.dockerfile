FROM  cr.yandex/yc-marketplace/yandex-cloud/ingress-nginx/chart/ingress-nginx1715955444137116324872537268606122775404000495249@sha256:87c88e1c38a6c8d4483c8f70b69e2cca49853bb3ec3124b9b1be648edf139af3
RUN mkdir /etc/nginx/oidc && touch /etc/nginx/oidc/oidc.conf
CMD ["/nginx-ingress-controller"]
