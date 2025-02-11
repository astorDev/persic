ensureInitialized();

function ensureInitialized() {
    try {
        const status = rs.status();
        print("Replica set status: ", status);
        return;
    }
    catch (e) {
        print("Replica set not initialized");
    }

    print("Initializing replica set");
    rs.initiate({
        _id: "rs0",
        members: [
            { _id: 0, host: "localhost:27017" }
        ]
    });
}