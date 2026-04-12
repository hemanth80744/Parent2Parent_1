-- =============================================
-- Parent2Parent PostgreSQL Schema
-- =============================================

-- 1. Create Tables

CREATE TABLE IF NOT EXISTS users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(100) NOT NULL,
    school VARCHAR(150) NOT NULL,
    class VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS requests (
    id SERIAL PRIMARY KEY,
    senderid INT NOT NULL REFERENCES users(id),
    receiverid INT NOT NULL REFERENCES users(id),
    status VARCHAR(50) DEFAULT 'Pending',
    createdat TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS messages (
    id SERIAL PRIMARY KEY,
    senderid INT NOT NULL REFERENCES users(id),
    receiverid INT NOT NULL REFERENCES users(id),
    message TEXT NOT NULL,
    sentat TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Create Functions (Stored Procedures in PostgreSQL)

-- User Registration
CREATE OR REPLACE FUNCTION sp_register_user(
    p_name VARCHAR(100),
    p_username VARCHAR(50),
    p_password VARCHAR(100),
    p_school VARCHAR(150),
    p_class VARCHAR(50)
) RETURNS INT AS $$
BEGIN
    INSERT INTO users (name, username, password, school, class)
    VALUES (p_name, p_username, p_password, p_school, p_class);
    RETURN 1;
END;
$$ LANGUAGE plpgsql;

-- User Login
CREATE OR REPLACE FUNCTION sp_login_user(
    p_username VARCHAR(50),
    p_password VARCHAR(100)
) RETURNS TABLE(id INT, name VARCHAR) AS $$
BEGIN
    RETURN QUERY 
    SELECT u.id, u.name 
    FROM users u 
    WHERE u.username = p_username AND u.password = p_password;
END;
$$ LANGUAGE plpgsql;

-- Search Schools/Parents
CREATE OR REPLACE FUNCTION sp_search_school(
    p_schoolname VARCHAR(150)
) RETURNS TABLE(id INT, name VARCHAR, childclass VARCHAR) AS $$
BEGIN
    RETURN QUERY 
    SELECT u.id, u.name, u.class as childclass
    FROM users u 
    WHERE u.school ILIKE '%' || p_schoolname || '%';
END;
$$ LANGUAGE plpgsql;

-- Send Connection Request
CREATE OR REPLACE FUNCTION sp_send_request(
    p_senderid INT,
    p_receiverid INT
) RETURNS INT AS $$
BEGIN
    IF NOT EXISTS (SELECT 1 FROM requests WHERE senderid = p_senderid AND receiverid = p_receiverid AND status = 'Pending') THEN
        INSERT INTO requests (senderid, receiverid, status)
        VALUES (p_senderid, p_receiverid, 'Pending');
        RETURN 1;
    END IF;
    RETURN 0;
END;
$$ LANGUAGE plpgsql;

-- View Requests
CREATE OR REPLACE FUNCTION sp_view_requests(
    p_userid INT DEFAULT NULL,
    p_senderid INT DEFAULT NULL
) RETURNS TABLE(requestid INT, senderid INT, receiverid INT, sendername VARCHAR, receivername VARCHAR, status VARCHAR, createdat TIMESTAMP) AS $$
BEGIN
    IF p_userid IS NOT NULL THEN
        RETURN QUERY 
        SELECT r.id, r.senderid, r.receiverid, u.name as sendername, ''::VARCHAR as receivername, r.status, r.createdat
        FROM requests r
        JOIN users u ON r.senderid = u.id
        WHERE r.receiverid = p_userid;
    ELSIF p_senderid IS NOT NULL THEN
        RETURN QUERY 
        SELECT r.id, r.senderid, r.receiverid, ''::VARCHAR as sendername, u.name as receivername, r.status, r.createdat
        FROM requests r
        JOIN users u ON r.receiverid = u.id
        WHERE r.senderid = p_senderid;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Accept Request
CREATE OR REPLACE FUNCTION sp_accept_request(
    p_requestid INT
) RETURNS INT AS $$
BEGIN
    UPDATE requests SET status = 'Accepted' WHERE id = p_requestid;
    RETURN 1;
END;
$$ LANGUAGE plpgsql;

-- Reject Request
CREATE OR REPLACE FUNCTION sp_reject_request(
    p_requestid INT
) RETURNS INT AS $$
BEGIN
    UPDATE requests SET status = 'Rejected' WHERE id = p_requestid;
    RETURN 1;
END;
$$ LANGUAGE plpgsql;

-- Send Message
CREATE OR REPLACE FUNCTION sp_send_message(
    p_senderid INT,
    p_receiverid INT,
    p_message TEXT
) RETURNS INT AS $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM requests 
        WHERE status = 'Accepted' 
        AND ((senderid = p_senderid AND receiverid = p_receiverid) 
             OR (senderid = p_receiverid AND receiverid = p_senderid))
    ) THEN
        INSERT INTO messages (senderid, receiverid, message)
        VALUES (p_senderid, p_receiverid, p_message);
        RETURN 1;
    ELSE
        RETURN 0;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Get Chat Messages
CREATE OR REPLACE FUNCTION sp_get_messages(
    p_user1 INT,
    p_user2 INT
) RETURNS TABLE(senderid INT, receiverid INT, message TEXT, sentat TIMESTAMP) AS $$
BEGIN
    RETURN QUERY 
    SELECT m.senderid, m.receiverid, m.message, m.sentat
    FROM messages m
    WHERE (m.senderid = p_user1 AND m.receiverid = p_user2)
       OR (m.senderid = p_user2 AND m.receiverid = p_user1)
    ORDER BY m.sentat ASC;
END;
$$ LANGUAGE plpgsql;
