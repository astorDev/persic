test:
	dotnet test --filter FullyQualifiedName~Persic.EF.Postgres.Playground.$(NAME) --logger "console;verbosity=detailed"

prep:
	docker compose up -d

down:
	docker compose down
	docker volume prune -f

reset:
	make down
	make prep