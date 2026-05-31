SELECT COUNT(*), country
FROM 'customers.json'
GROUP BY country
ORDER BY COUNT(*) DESC