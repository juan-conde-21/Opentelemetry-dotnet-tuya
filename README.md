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

4. Agregar el label a nivel de los deployments

    instana-autotrace: "true"

        apiVersion: apps/v1
        kind: Deployment
        metadata:
          labels:
            instana-autotrace: "true"
            app: rolldice
          name: rolldice
        spec:
          replicas: 2
          selector:
            matchLabels:
              app: rolldice
          template:
            metadata:
              labels:
                app: rolldice
            spec:
              containers:
              - image: juanconde24/rolldice:2.7
                name: rolldice

5. Para revertir los cambios se debe retirar la etiqueta "instana-autotrace" y eliminar el deployment.

# Despliegue de Agente Instana en Kubernetes

    helm install instana-agent \
       --repo https://agents.instana.io/helm \
       --namespace instana-agent \
       --create-namespace \
       --set agent.key={apikey}\
       --set agent.downloadKey={apikey} \
       --set agent.endpointHost=ingress-coral-saas.instana.io \
       --set agent.endpointPort=443 \
       --set cluster.name='{definir}' \
       --set zone.name='{definir}' \
       --set opentelemetry.enabled=true \
       --set opentelemetry.grpc.enabled=true \
       --set opentelemetry.http.enabled=true \
       instana-agent

