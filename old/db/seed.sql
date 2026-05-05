INSERT INTO Product (id, name, ed) VALUES
    (1, 'Молоко', 'л'),
    (2, 'Хлеб', 'шт')
ON CONFLICT (id) DO NOTHING;

INSERT INTO Client (id, name, address, phone) VALUES
    (1, 'Иванов Иван', 'ул. Ленина, 1', '89990000001'),
    (2, 'Петров Петр', 'ул. Мира, 5', '89990000002')
ON CONFLICT (id) DO NOTHING;

INSERT INTO Futura (id, idClient, data, totalSum) VALUES
    (1, 1, CURRENT_DATE, 0)
ON CONFLICT (id) DO NOTHING;

INSERT INTO FuturaInfo (id, idFutura, idProduct, quantity, price) VALUES
    (1, 1, 1, 2, 80),
    (2, 1, 2, 1, 50)
ON CONFLICT (id) DO NOTHING;

SELECT setval('product_id_seq', (SELECT COALESCE(MAX(id), 1) FROM product));
SELECT setval('client_id_seq', (SELECT COALESCE(MAX(id), 1) FROM client));
SELECT setval('futura_id_seq', (SELECT COALESCE(MAX(id), 1) FROM futura));
SELECT setval('futurainfo_id_seq', (SELECT COALESCE(MAX(id), 1) FROM futurainfo));
