SELECT SUM(o.amount * (1 - t.rate)) AS total_amount_post, c.country
FROM 'orders-*.csv' o
JOIN 'customers.json' c ON c.id = o.customer
JOIN 'https://raw.githubusercontent.com/astorDev/persic/refs/heads/main/duckdb/play/taxes.csv' t ON t.country = c.country
GROUP BY c.country
ORDER BY SUM(o.amount * (1 - t.rate)) DESC