SELECT SUM(amount), category
FROM 'orders-2025.csv'
GROUP BY category