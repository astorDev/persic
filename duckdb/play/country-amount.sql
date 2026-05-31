SELECT SUM(o.amount) AS total_amount, c.country
FROM 'orders-*.csv' o
JOIN 'customers.json' c ON c.id = o.customer 
GROUP BY c.country
ORDER BY SUM(o.amount) DESC