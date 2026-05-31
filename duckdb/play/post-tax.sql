SELECT SUM(o.amount * (1 - t.rate)) AS total_amount_post, c.country
FROM 'orders-*.csv' o
JOIN 'customers.json' c ON c.id = o.customer
JOIN 'taxes.csv' t ON t.country = c.country -- TODO: Switch to http when available via github
GROUP BY c.country
ORDER BY SUM(o.amount * (1 - t.rate)) DESC