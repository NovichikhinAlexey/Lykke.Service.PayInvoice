cd ../Lykke.Pay.Service.Invoces.Client
iwr http://pay-invoice-service.lykke-pay.svc.cluster.local/swagger/v1/swagger.json -o Service.Invoice.json
autorest --input-file=Service.Invoice.json --csharp --namespace=Lykke.Pay.Service.Invoces.Client --output-folder=./