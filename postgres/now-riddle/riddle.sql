DO $$ 
DECLARE 
    first_timestamp timestamp;
    second_timestamp timestamp;
    result interval;
BEGIN
    SELECT now() INTO first_timestamp;
    PERFORM pg_sleep(2);
    SELECT now() INTO second_timestamp;
    result := first_timestamp - second_timestamp;
    RAISE NOTICE '%', result;
END $$;
