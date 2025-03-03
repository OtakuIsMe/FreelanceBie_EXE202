start:
	dotnet watch --launch-profile dev

build:
	dotnet build

test:
	dotnet test

rabbitmq:
	docker run -d --hostname rabbitmq --name rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management

redislocal:
	docker-compose up -d

docker:
	docker build -t flb_server -f Dockerfile .

release:
	docker build --build-arg APP_VERSION=$(version) -t flb_server -f Dockerfile .
	docker tag flb_server freelancebieregistry.azurecr.io/flb_server:$(version)
	docker push freelancebieregistry.azurecr.io/flb_server:$(version)

new-migration:
	dotnet ef migrations add $(name) --verbose 

migration:
	dotnet ef database update

push-registries:
	docker tag flb_server freelancebieregistry.azurecr.io/flb_server:$(version)
	docker push freelancebieregistry.azurecr.io/flb_server:$(version)

.PHONY: start build docker new-migration migration test release