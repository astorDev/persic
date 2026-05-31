# SQL for Files - DuckDb

> How to query CSVs and JSONs using SQL with DuckDB

![Query CSV as if it's a database](thumb.png)

CSVs and JSONs are perhaps the most common format for an export nowadays. Still to run even a simple query on them we typically import them to some other format first, being that a model in code, database or simply a spreadsheet. However, we don't really need to. DuckDb already lets us query them as if they were a database.

> DuckDb is much more then the SQL for files engine. This article however focuses specifically on this part of the tool and I believe it's actually a good place to start familiarizing yourself with the technology.

## TLDR;

The last query in this article demonstrates the impressive capabilities of DuckDb for querying files. In this one query we were able to:

- Query multiple CSVs as a single file using file pattern.
- JOIN files accross formats
- Use a remote file in the same query.

You can file all the scripts along with the example files in the [persic](https://github.com/astorDev/persic/duckdb/files/src) repository. The project is all persistence technologies and provides various db-related tools. Check it out on [GitHub](https://github.com/astorDev/persic) and don't hesitate to give it a star! ⭐

Claps for this article are also highly appreciated! 😉