# SQL for Files - DuckDb

> How to query CSVs and JSONs using SQL with DuckDB

![Query CSV as if it's a database](thumb.png)

CSVs and JSONs are perhaps the most common formats of an export nowadays. Still, to run even a simple query on them, we typically import them to some other format first, such as a model in code, a database, or simply a spreadsheet. However, we don't really need to. DuckDb already lets us query them as if they were a database.

> DuckDb is much more than the SQL for files engine. This article, however, focuses specifically on this part of the tool, and I believe it's actually a good place to start familiarizing yourself with the technology.

## Getting Started: Simple CSV Query

`orders-2025.csv`

```csv
category,amount,customer
electronics,200,1
clothing,50,2
home,90,3
electronics,300,4
clothing,20,3
```

> How cool is the fact that I can "embed" our "database" in the article, huh?

```sh
brew install duckdb
```

`2025-category-amount.sql`

```sql
SELECT SUM(amount), category
FROM 'orders-2025.csv'
GROUP BY category;
```

```sh
duckdb < 2025-category-amount.sql
```

```markdown
┌─────────────┬─────────────┐
│ sum(amount) │  category   │
│   int128    │   varchar   │
├─────────────┼─────────────┤
│          90 │ home        │
│         500 │ electronics │
│          70 │ clothing    │
└─────────────┴─────────────┘
```

> For the next scripts I will just show you the SQL itself without naming it or providing a command to run it
> You can use CLI in an interactive mode this way:
> 1. Type duckdb, hitting enter - welcome into interactive mode
> 2. Type your query
> 3. Type semicolon (;) in the end - I always forgot to do that
> 4. Hit Enter
> Ctrl + D when you are finished

## Getting Cooler: File Patterns and JSON

`orders-2026.csv`

```csv
category,amount,customer
clothing,900,3
clothing,50,3
clothing,300,3
```

```sql
SELECT SUM(amount), category
FROM 'orders-*.csv'
GROUP BY category
```

```text
┌─────────────┬─────────────┐
│ sum(amount) │  category   │
│   int128    │   varchar   │
├─────────────┼─────────────┤
│        1320 │ clothing    │
│         500 │ electronics │
│          90 │ home        │
└─────────────┴─────────────┘
```

`customers.json`

```json
[
    {
        "id" : 1,
        "country" : "Germany"
    },
    {
        "id" : 2,
        "country" : "Austria"
    },
    {
        "id" : 3,
        "country" : "Netherlands"
    },
    {
        "id" : 4,
        "country" : "Austria"
    }
]
```

```sql
SELECT COUNT(*), country
FROM 'customers.json'
GROUP BY country
ORDER BY COUNT(*) DESC
```

```text
┌──────────────┬─────────────┐
│ count_star() │   country   │
│    int64     │   varchar   │
├──────────────┼─────────────┤
│            2 │ Austria     │
│            1 │ Germany     │
│            1 │ Netherlands │
└──────────────┴─────────────┘
```

## Mind Blown Away: Cross-Format JOINs and Remote Files

```sql
SELECT SUM(o.amount) AS total_amount, c.country
FROM 'orders-*.csv' o
JOIN 'customers.json' c ON c.id = o.customer 
GROUP BY c.country
ORDER BY SUM(o.amount) DESC
```

```text
┌──────────────┬─────────────┐
│ total_amount │   country   │
│    int128    │   varchar   │
├──────────────┼─────────────┤
│         1360 │ Netherlands │
│          350 │ Austria     │
│          200 │ Germany     │
└──────────────┴─────────────┘
```

`taxes.csv` and I put in on github so that it is publicly available via [the link: https://raw.githubusercontent.com/astorDev/persic/refs/heads/main/duckdb/play/taxes.csv](https://raw.githubusercontent.com/astorDev/persic/refs/heads/main/duckdb/play/taxes.csv):

```csv
country,rate
Austria,0.20
Germany,0.19
Netherlands,0.21
```

```sql
SELECT SUM(o.amount * (1 - t.rate)) AS total_amount_post, c.country
FROM 'orders-*.csv' o
JOIN 'customers.json' c ON c.id = o.customer
JOIN 'https://raw.githubusercontent.com/astorDev/persic/refs/heads/main/duckdb/play/taxes.csv' t ON t.country = c.country
GROUP BY c.country
ORDER BY SUM(o.amount * (1 - t.rate)) DESC
```

```csv
┌───────────────────┬─────────────┐
│ total_amount_post │   country   │
│      double       │   varchar   │
├───────────────────┼─────────────┤
│            1074.4 │ Netherlands │
│             280.0 │ Austria     │
│             162.0 │ Germany     │
└───────────────────┴─────────────┘
```

Most importantly, it is not limited to public HTTP endpoints. It's totally possible to connect to files in private accounts on file storages like S3 or Azure services! But I think the article is big enough and show impressive enough capabilities to leave demoing such things out of scope.

## TLDR;

The last query in this article demonstrates the impressive capabilities of DuckDb for querying files. In this one query, we were able to:

- Query multiple CSVs as a single file using a file pattern.
- JOIN files across formats
- Use a remote file in the same query.

You can file all the scripts along with the example files in the [persic](https://github.com/astorDev/persic/duckdb/files/src) repository. The project is all persistence technologies and provides various DB-related tools. Check it out on [GitHub](https://github.com/astorDev/persic) and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉
