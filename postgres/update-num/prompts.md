## Prompt 1

I'm investigating two ways to achieve "update number" in PostgreSQL. The update number should act as an update cursor (increasing number) for updates of a specific database entity. 

I'm considering two options:
1. `update_number` column inside of the source `records` table with the value comming from a dedicated sequence.
2. dedicated `updates` table with `BIGSERIAL` `id` and reference to the `records` table.

Both number should be set inside of a trigger on the source `records` table. 

My assumption is that the first approach is not strictly-chronological i.e. in a certain high-load scenarios the `update_number` order will not much the exact order when changes are committed. The second approach on the other side WILL BE stictly-choronological i.e. there will not be any order mismatch even on a very high-loaded systems.

I have two questions:

1. Do you think my assumption is correct?
2. How can I reliably test proper chronological ordering?