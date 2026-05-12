CREATE TABLE product_group (
    id SERIAL PRIMARY KEY,
    name varchar(80) NOT NULL UNIQUE
);

CREATE TABLE product (
    id SERIAL PRIMARY KEY,
    group_id integer NOT NULL REFERENCES product_group(id) ON DELETE RESTRICT,
    name varchar(120) NOT NULL,
    unit varchar(20) NOT NULL DEFAULT 'шт',
    UNIQUE(group_id, name)
);

CREATE TABLE supplier (
    id SERIAL PRIMARY KEY,
    name varchar(120) NOT NULL UNIQUE,
    address varchar(160),
    phone varchar(30)
);

CREATE TABLE invoice (
    id SERIAL PRIMARY KEY,
    supplier_id integer NOT NULL REFERENCES supplier(id) ON DELETE RESTRICT,
    number varchar(40) NOT NULL,
    invoice_date date NOT NULL,
    invoice_type varchar(10) NOT NULL CHECK (invoice_type IN ('income', 'expense')),
    note varchar(200),
    total_sum numeric(12,2) NOT NULL DEFAULT 0,
    current_debt numeric(12,2) NOT NULL DEFAULT 0,
    UNIQUE(number, invoice_type)
);

CREATE TABLE invoice_item (
    id SERIAL PRIMARY KEY,
    invoice_id integer NOT NULL REFERENCES invoice(id) ON DELETE CASCADE,
    product_id integer NOT NULL REFERENCES product(id) ON DELETE RESTRICT,
    quantity numeric(12,3) NOT NULL CHECK (quantity > 0),
    price numeric(12,2) NOT NULL CHECK (price >= 0)
);

CREATE TABLE payment (
    id SERIAL PRIMARY KEY,
    invoice_id integer NOT NULL REFERENCES invoice(id) ON DELETE CASCADE,
    payment_date date NOT NULL,
    amount numeric(12,2) NOT NULL CHECK (amount > 0),
    note varchar(200)
);

CREATE OR REPLACE FUNCTION refresh_invoice_debt(p_invoice_id integer) RETURNS void AS
$$
BEGIN
    UPDATE invoice i
    SET current_debt = CASE
        WHEN i.invoice_type = 'income' THEN GREATEST(
            i.total_sum - COALESCE((
                SELECT SUM(p.amount)
                FROM payment p
                WHERE p.invoice_id = p_invoice_id
            ), 0),
            0
        )
        ELSE 0
    END
    WHERE i.id = p_invoice_id;
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION refresh_invoice_total(p_invoice_id integer) RETURNS void AS
$$
BEGIN
    UPDATE invoice
    SET total_sum = COALESCE((
        SELECT SUM(quantity * price)
        FROM invoice_item
        WHERE invoice_id = p_invoice_id
    ), 0)
    WHERE id = p_invoice_id;

    PERFORM refresh_invoice_debt(p_invoice_id);
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION invoice_item_total_trigger() RETURNS TRIGGER AS
$$
BEGIN
    IF TG_OP = 'DELETE' THEN
        PERFORM refresh_invoice_total(OLD.invoice_id);
        RETURN OLD;
    END IF;

    PERFORM refresh_invoice_total(NEW.invoice_id);
    IF TG_OP = 'UPDATE' AND OLD.invoice_id <> NEW.invoice_id THEN
        PERFORM refresh_invoice_total(OLD.invoice_id);
    END IF;
    RETURN NEW;
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION payment_debt_trigger() RETURNS TRIGGER AS
$$
BEGIN
    IF TG_OP = 'DELETE' THEN
        PERFORM refresh_invoice_debt(OLD.invoice_id);
        RETURN OLD;
    END IF;

    PERFORM refresh_invoice_debt(NEW.invoice_id);
    IF TG_OP = 'UPDATE' AND OLD.invoice_id <> NEW.invoice_id THEN
        PERFORM refresh_invoice_debt(OLD.invoice_id);
    END IF;
    RETURN NEW;
END
$$ LANGUAGE plpgsql;

CREATE OR REPLACE FUNCTION invoice_debt_trigger() RETURNS TRIGGER AS
$$
BEGIN
    PERFORM refresh_invoice_debt(NEW.id);
    RETURN NEW;
END
$$ LANGUAGE plpgsql;

CREATE TRIGGER invoice_item_total_ins
AFTER INSERT ON invoice_item
FOR EACH ROW EXECUTE FUNCTION invoice_item_total_trigger();

CREATE TRIGGER invoice_item_total_upd
AFTER UPDATE ON invoice_item
FOR EACH ROW EXECUTE FUNCTION invoice_item_total_trigger();

CREATE TRIGGER invoice_item_total_del
AFTER DELETE ON invoice_item
FOR EACH ROW EXECUTE FUNCTION invoice_item_total_trigger();

CREATE TRIGGER payment_debt_ins
AFTER INSERT ON payment
FOR EACH ROW EXECUTE FUNCTION payment_debt_trigger();

CREATE TRIGGER payment_debt_upd
AFTER UPDATE ON payment
FOR EACH ROW EXECUTE FUNCTION payment_debt_trigger();

CREATE TRIGGER payment_debt_del
AFTER DELETE ON payment
FOR EACH ROW EXECUTE FUNCTION payment_debt_trigger();

CREATE TRIGGER invoice_debt_upd
AFTER UPDATE OF invoice_type ON invoice
FOR EACH ROW EXECUTE FUNCTION invoice_debt_trigger();
