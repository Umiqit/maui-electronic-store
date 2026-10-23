-- =============================================================
--  База данных «Электронный магазин предприятия»
--  Лабораторная работа №3. PostgreSQL
--  Выполнение: в pgAdmin (Query Tool) или psql целиком,
--  либо по блокам.
-- =============================================================

-- -------------------------------------------------------------
-- 1. Создание базы данных
-- -------------------------------------------------------------
DROP DATABASE IF EXISTS shopdb;
CREATE DATABASE shopdb;

\connect shopdb

-- -------------------------------------------------------------
-- 2. Таблицы
-- -------------------------------------------------------------

-- Категории товаров
CREATE TABLE categories (
    id   SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

-- Товары
CREATE TABLE products (
    id             SERIAL PRIMARY KEY,
    name           VARCHAR(150) NOT NULL,
    description    TEXT,
    price          NUMERIC(12, 2) NOT NULL CHECK (price >= 0),
    stock_quantity INT NOT NULL DEFAULT 0 CHECK (stock_quantity >= 0),
    category_id    INT NOT NULL REFERENCES categories (id) ON DELETE RESTRICT
);

-- Покупатели
CREATE TABLE customers (
    id    SERIAL PRIMARY KEY,
    name  VARCHAR(100) NOT NULL,
    email VARCHAR(120) NOT NULL UNIQUE,
    phone VARCHAR(20)
);

-- Заказы
CREATE TABLE orders (
    id          SERIAL PRIMARY KEY,
    customer_id INT NOT NULL REFERENCES customers (id) ON DELETE RESTRICT,
    order_date  TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    status      VARCHAR(20) NOT NULL DEFAULT 'new'
                CHECK (status IN ('new', 'paid', 'shipped', 'cancelled'))
);

-- Состав заказов
CREATE TABLE order_items (
    id         SERIAL PRIMARY KEY,
    order_id   INT NOT NULL REFERENCES orders (id) ON DELETE CASCADE,
    product_id INT NOT NULL REFERENCES products (id) ON DELETE RESTRICT,
    quantity   INT NOT NULL CHECK (quantity > 0),
    price      NUMERIC(12, 2) NOT NULL CHECK (price >= 0)
);

-- -------------------------------------------------------------
-- 3. Индексы
-- -------------------------------------------------------------
CREATE INDEX idx_products_category   ON products (category_id);
CREATE INDEX idx_products_name       ON products (name);
CREATE INDEX idx_orders_customer     ON orders (customer_id);
CREATE INDEX idx_order_items_order   ON order_items (order_id);
CREATE INDEX idx_order_items_product ON order_items (product_id);

-- -------------------------------------------------------------
-- 4. Тестовые (начальные) данные
-- -------------------------------------------------------------
INSERT INTO categories (name) VALUES
    ('Смартфоны'),
    ('Ноутбуки'),
    ('Планшеты'),
    ('Аксессуары');

INSERT INTO products (name, description, price, stock_quantity, category_id) VALUES
    ('Смартфон Galaxy A55',   'Смартфон, 8/128 ГБ, Dual SIM',        39990.00, 15, 1),
    ('Смартфон iPhone 16',    'Смартфон, 128 ГБ',                    79990.00,  8, 1),
    ('Ноутбук ThinkPad E14',  'Ноутбук, Core i5, 16/512 ГБ',         89990.00,  6, 2),
    ('Ноутбук MacBook Air M3','Ноутбук, 16/512 ГБ',                 139990.00,  4, 2),
    ('Планшет iPad 10',       'Планшет, 64 ГБ, Wi-Fi',               54990.00, 10, 3),
    ('Планшет Xiaomi Pad 6',  'Планшет, 8/256 ГБ',                   32990.00, 12, 3),
    ('Наушники AirPods Pro 2','Беспроводные наушники с шумоподавлением', 24990.00, 20, 4),
    ('Мышь Logitech MX Master 3S', 'Беспроводная мышь',              12990.00, 25, 4);

INSERT INTO customers (name, email, phone) VALUES
    ('Иванов Иван Иванович',     'ivanov@example.com',   '+7 900 111-22-33'),
    ('Петрова Анна Сергеевна',   'petrova@example.com',  '+7 900 222-33-44'),
    ('Сидоров Пётр Петрович',    'sidorov@example.com',  '+7 900 333-44-55');

INSERT INTO orders (customer_id, status) VALUES
    (1, 'paid'),
    (2, 'new'),
    (3, 'shipped');

INSERT INTO order_items (order_id, product_id, quantity, price) VALUES
    (1, 1, 1, 39990.00),
    (1, 7, 1, 24990.00),
    (2, 6, 2, 32990.00),
    (3, 3, 1, 89990.00);

-- -------------------------------------------------------------
-- 5. Представление для отчёта по продажам (для меню «Отчёт»)
-- -------------------------------------------------------------
CREATE VIEW sales_report AS
SELECT
    o.id          AS order_id,
    c.name        AS customer_name,
    p.name        AS product_name,
    oi.quantity,
    oi.price,
    oi.quantity * oi.price AS total,
    o.order_date,
    o.status
FROM orders o
JOIN customers   c  ON c.id = o.customer_id
JOIN order_items oi ON oi.order_id = o.id
JOIN products    p  ON p.id = oi.product_id;

-- -------------------------------------------------------------
-- 6. Проверочные запросы
-- -------------------------------------------------------------
-- SELECT * FROM products;
-- SELECT p.name, p.price, c.name AS category
--   FROM products p JOIN categories c ON c.id = p.category_id;
-- SELECT * FROM sales_report;
