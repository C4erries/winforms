INSERT INTO product_group (name) VALUES
('Бумага'),
('Письменные принадлежности'),
('Папки и файлы'),
('Настольные принадлежности')
ON CONFLICT DO NOTHING;

INSERT INTO product (group_id, name, unit)
SELECT id, 'Бумага А4', 'пачка' FROM product_group WHERE name = 'Бумага'
ON CONFLICT DO NOTHING;

INSERT INTO product (group_id, name, unit)
SELECT id, 'Ручка шариковая', 'шт' FROM product_group WHERE name = 'Письменные принадлежности'
ON CONFLICT DO NOTHING;

INSERT INTO product (group_id, name, unit)
SELECT id, 'Папка-регистратор', 'шт' FROM product_group WHERE name = 'Папки и файлы'
ON CONFLICT DO NOTHING;

INSERT INTO product (group_id, name, unit)
SELECT id, 'Файл-вкладыш А4', 'упак.' FROM product_group WHERE name = 'Папки и файлы'
ON CONFLICT DO NOTHING;

INSERT INTO product (group_id, name, unit)
SELECT id, 'Степлер №24', 'шт' FROM product_group WHERE name = 'Настольные принадлежности'
ON CONFLICT DO NOTHING;

INSERT INTO product (group_id, name, unit)
SELECT id, 'Скобы №24/6', 'кор.' FROM product_group WHERE name = 'Настольные принадлежности'
ON CONFLICT DO NOTHING;

INSERT INTO supplier (name, address, phone) VALUES
('ООО КанцПоставка', 'Москва, ул. Складская, 1', '+7 495 111-22-33'),
('ИП Смирнов', 'Москва, ул. Бумажная, 7', '+7 916 222-33-44'),
('ООО ОфисМаркет', 'Москва, ул. Офисная, 15', '+7 495 555-66-77')
ON CONFLICT DO NOTHING;

INSERT INTO invoice (supplier_id, number, invoice_date, invoice_type, note)
SELECT s.id, 'ПР-001', DATE '2026-05-02', 'income', 'Первая поставка бумаги и ручек'
FROM supplier s
WHERE s.name = 'ООО КанцПоставка'
ON CONFLICT DO NOTHING;

INSERT INTO invoice (supplier_id, number, invoice_date, invoice_type, note)
SELECT s.id, 'ПР-002', DATE '2026-05-06', 'income', 'Поставка папок и настольных принадлежностей'
FROM supplier s
WHERE s.name = 'ООО ОфисМаркет'
ON CONFLICT DO NOTHING;

INSERT INTO invoice (supplier_id, number, invoice_date, invoice_type, note)
SELECT s.id, 'РС-001', DATE '2026-05-09', 'expense', 'Реализация товаров покупателям'
FROM supplier s
WHERE s.name = 'ИП Смирнов'
ON CONFLICT DO NOTHING;

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 40, 310.00
FROM invoice i
JOIN product p ON p.name = 'Бумага А4'
WHERE i.number = 'ПР-001' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 120, 18.50
FROM invoice i
JOIN product p ON p.name = 'Ручка шариковая'
WHERE i.number = 'ПР-001' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 25, 145.00
FROM invoice i
JOIN product p ON p.name = 'Папка-регистратор'
WHERE i.number = 'ПР-002' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 15, 265.00
FROM invoice i
JOIN product p ON p.name = 'Степлер №24'
WHERE i.number = 'ПР-002' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 10, 450.00
FROM invoice i
JOIN product p ON p.name = 'Файл-вкладыш А4'
WHERE i.number = 'ПР-002' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 8, 390.00
FROM invoice i
JOIN product p ON p.name = 'Бумага А4'
WHERE i.number = 'РС-001' AND i.invoice_type = 'expense'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO invoice_item (invoice_id, product_id, quantity, price)
SELECT i.id, p.id, 20, 35.00
FROM invoice i
JOIN product p ON p.name = 'Ручка шариковая'
WHERE i.number = 'РС-001' AND i.invoice_type = 'expense'
  AND NOT EXISTS (
      SELECT 1 FROM invoice_item ii WHERE ii.invoice_id = i.id AND ii.product_id = p.id
  );

INSERT INTO payment (invoice_id, payment_date, amount, note)
SELECT i.id, DATE '2026-05-04', 8000.00, 'Первый платеж по накладной ПР-001'
FROM invoice i
WHERE i.number = 'ПР-001' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM payment p WHERE p.invoice_id = i.id AND p.payment_date = DATE '2026-05-04'
  );

INSERT INTO payment (invoice_id, payment_date, amount, note)
SELECT i.id, DATE '2026-05-10', 4000.00, 'Второй платеж по накладной ПР-001'
FROM invoice i
WHERE i.number = 'ПР-001' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM payment p WHERE p.invoice_id = i.id AND p.payment_date = DATE '2026-05-10'
  );

INSERT INTO payment (invoice_id, payment_date, amount, note)
SELECT i.id, DATE '2026-05-12', 5000.00, 'Частичная оплата накладной ПР-002'
FROM invoice i
WHERE i.number = 'ПР-002' AND i.invoice_type = 'income'
  AND NOT EXISTS (
      SELECT 1 FROM payment p WHERE p.invoice_id = i.id AND p.payment_date = DATE '2026-05-12'
  );
