ensureInitialized();

function ensureInitialized() {
    if (rs.status().ok === 1) {
        print("Replica set already initialized");
        return;
    }

    print("Initializing replica set");
    rs.initiate({
        _id: "rs0",
        members: [
            { _id: 0, host: "localhost:27017" }
        ]
    });
}