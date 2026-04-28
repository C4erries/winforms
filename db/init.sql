CREATE TABLE Product (
    id SERIAL PRIMARY KEY,
    name varchar(50),
    ed varchar(20)
);

CREATE TABLE Client (
    id SERIAL PRIMARY KEY,
    name varchar(50),
    address varchar(100),
    phone varchar(20)
);

CREATE TABLE Futura (
    id SERIAL PRIMARY KEY,
    idClient integer REFERENCES Client(id) ON DELETE CASCADE,
    data date,
    totalSum numeric(10,2) DEFAULT 0
);

CREATE TABLE FuturaInfo (
    id SERIAL PRIMARY KEY,
    idFutura integer REFERENCES Futura(id) ON DELETE CASCADE,
    idProduct integer REFERENCES Product(id) ON DELETE CASCADE,
    quantity numeric(10,2),
    price numeric(10,2)
);

CREATE OR REPLACE FUNCTION insert_futura_info() RETURNS TRIGGER AS
$ad_fi_triggert$
BEGIN
    UPDATE futura SET totalsum = totalsum + NEW.quantity * NEW.price
    WHERE futura.id = NEW.idFutura;
    RETURN NULL;
END
$ad_fi_triggert$ LANGUAGE plpgsql;

CREATE TRIGGER ins_futura_info 
AFTER INSERT ON FuturaInfo
FOR EACH ROW 
EXECUTE FUNCTION insert_futura_info();

CREATE OR REPLACE FUNCTION delete_futura_info() RETURNS TRIGGER AS
$del_fi_triggert$
BEGIN
    UPDATE futura SET totalsum = totalsum - OLD.quantity * OLD.price
    WHERE futura.id = OLD.idFutura;
    RETURN NULL;
END
$del_fi_triggert$ LANGUAGE plpgsql;

CREATE TRIGGER del_futura_info 
AFTER DELETE ON FuturaInfo
FOR EACH ROW 
EXECUTE FUNCTION delete_futura_info();

CREATE OR REPLACE FUNCTION update_futura_info() RETURNS TRIGGER AS
$upd_fi_triggert$
BEGIN
    UPDATE futura 
    SET totalsum = totalsum - OLD.quantity * OLD.price + NEW.quantity * NEW.price
    WHERE futura.id = NEW.idFutura;
    RETURN NULL;
END
$upd_fi_triggert$ LANGUAGE plpgsql;

CREATE TRIGGER upd_futura_info 
AFTER UPDATE ON FuturaInfo
FOR EACH ROW 
EXECUTE FUNCTION update_futura_info();
