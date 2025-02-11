## Replica Set Image

```sh
docker compose up persisted
```

```sh
mongosh "mongodb://localhost:27017"
```

```sh
db.runCommand({ ping: 1 })
```

```sh
mongosh "mongodb://localhost:27017/?replicaSet=rs0"
```

```sh
rs.status()
```

```sh
docker compose up imaged
```

```sh
mongosh "mongodb://localhost:27018/?replicaSet=rs0"
```

## Replica Set Image Did Not Work

⚠️ `init.js` inside `vosarat/mongo-rs0:latest` only executes on a fresh database. On an existing database you may need to configure replicaSet on your own:

1. Connect to the mongo instance **without using replicaSet**

```sh
mongosh "mongodb://localhost:27017"
```

2. Execute the `init.js`

> Assumes you have loaded `init.js` to the folder you are running mongosh from

```sh
load("init.js")
```

## Publishing Image 

```sh
export VERSION=<VERSION>
```

```sh
docker build --build-arg VERSION=$VERSION -t vosarat/mongo-rs0:$VERSION .
```

```sh
docker push vosarat/mongo-rs0 --all-tags
```