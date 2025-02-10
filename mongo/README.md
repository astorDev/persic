```sh
export VERSION=<VERSION>
```

```sh
docker build --build-arg VERSION=$VERSION -t vosarat/mongo-rs0:$VERSION .
```

```sh
docker push vosarat/mongo-rs0 --all-tags
```