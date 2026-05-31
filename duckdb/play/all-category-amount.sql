SELECT SUM(amount), category
FROM 'orders-*.csv'
GROUP BY category