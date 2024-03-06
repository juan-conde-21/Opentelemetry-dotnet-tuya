# Opentelemetry-dotnet-tuya


# Despliegue de Autotrace

1. Descargar helm local e instalar local

    helm pull instana-autotrace-webhook --repo https://agents.instana.io/helm instana-autotrace-webhook --untar

2. Personalizar archivo values.yaml con los limites de cpu y memoria

    instana-autotrace-webhook/values

3. Instalar desde el repositorio local

    helm install --create-namespace --namespace instana-autotrace-webhook instana-autotrace-webhook \
    --repo ./instana-autotrace-webhook instana-autotrace-webhook \
    --set webhook.imagePullCredentials.password={apikey}\
    --set autotrace.opt_in=true


