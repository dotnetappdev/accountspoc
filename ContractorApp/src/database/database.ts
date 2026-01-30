import * as SQLite from 'expo-sqlite';

const db = SQLite.openDatabaseSync('contractor.db');

export const initDatabase = () => {
  try {
    // Create tables
    db.execSync(`
      CREATE TABLE IF NOT EXISTS settings (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        apiUrl TEXT NOT NULL,
        apiKey TEXT,
        syncEnabled INTEGER DEFAULT 1,
        lastSync TEXT
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS sales_orders (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        serverOrderId INTEGER,
        orderNumber TEXT NOT NULL,
        customerName TEXT NOT NULL,
        customerEmail TEXT,
        customerPhone TEXT,
        orderDate TEXT NOT NULL,
        totalAmount REAL DEFAULT 0,
        status TEXT DEFAULT 'Pending',
        notes TEXT,
        syncStatus TEXT DEFAULT 'pending',
        createdAt TEXT NOT NULL,
        updatedAt TEXT
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS sales_order_items (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        salesOrderId INTEGER NOT NULL,
        description TEXT NOT NULL,
        quantity INTEGER NOT NULL,
        unitPrice REAL NOT NULL,
        totalPrice REAL NOT NULL,
        FOREIGN KEY (salesOrderId) REFERENCES sales_orders(id) ON DELETE CASCADE
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS quotes (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        serverQuoteId INTEGER,
        quoteNumber TEXT NOT NULL,
        customerName TEXT NOT NULL,
        customerEmail TEXT,
        customerPhone TEXT,
        quoteDate TEXT NOT NULL,
        expiryDate TEXT,
        totalAmount REAL DEFAULT 0,
        status TEXT DEFAULT 'Draft',
        notes TEXT,
        syncStatus TEXT DEFAULT 'pending',
        createdAt TEXT NOT NULL,
        updatedAt TEXT
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS quote_items (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        quoteId INTEGER NOT NULL,
        description TEXT NOT NULL,
        quantity REAL NOT NULL,
        unitPrice REAL NOT NULL,
        lineTotal REAL NOT NULL,
        FOREIGN KEY (quoteId) REFERENCES quotes(id) ON DELETE CASCADE
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS work_orders (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        serverWorkOrderId INTEGER,
        workOrderNumber TEXT NOT NULL,
        customerName TEXT NOT NULL,
        customerEmail TEXT,
        customerPhone TEXT,
        description TEXT NOT NULL,
        workOrderDate TEXT NOT NULL,
        scheduledDate TEXT,
        completedDate TEXT,
        status TEXT DEFAULT 'Pending',
        priority TEXT DEFAULT 'Normal',
        siteAddress TEXT,
        siteCity TEXT,
        sitePostCode TEXT,
        estimatedHours REAL DEFAULT 0,
        actualHours REAL DEFAULT 0,
        syncStatus TEXT DEFAULT 'pending',
        createdAt TEXT NOT NULL,
        updatedAt TEXT
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS work_order_tasks (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        workOrderId INTEGER NOT NULL,
        taskName TEXT NOT NULL,
        description TEXT,
        isCompleted INTEGER DEFAULT 0,
        completedDate TEXT,
        sortOrder INTEGER DEFAULT 0,
        estimatedHours REAL DEFAULT 0,
        actualHours REAL DEFAULT 0,
        FOREIGN KEY (workOrderId) REFERENCES work_orders(id) ON DELETE CASCADE
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS site_visit_signoffs (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        serverSignOffId INTEGER,
        workOrderId INTEGER NOT NULL,
        visitDate TEXT NOT NULL,
        visitType TEXT NOT NULL,
        signedByName TEXT NOT NULL,
        signedByTitle TEXT,
        signedDate TEXT NOT NULL,
        signatureImagePath TEXT,
        workCompleted TEXT,
        issuesIdentified TEXT,
        nextSteps TEXT,
        customerComments TEXT,
        customerSatisfactionRating INTEGER,
        syncStatus TEXT DEFAULT 'pending',
        createdAt TEXT NOT NULL,
        FOREIGN KEY (workOrderId) REFERENCES work_orders(id) ON DELETE CASCADE
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS work_evidence_images (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        workOrderId INTEGER NOT NULL,
        imagePath TEXT NOT NULL,
        description TEXT,
        capturedAt TEXT NOT NULL,
        syncStatus TEXT DEFAULT 'pending',
        FOREIGN KEY (workOrderId) REFERENCES work_orders(id) ON DELETE CASCADE
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS customers (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        serverCustomerId INTEGER,
        name TEXT NOT NULL,
        email TEXT,
        phone TEXT,
        address TEXT,
        city TEXT,
        postCode TEXT,
        country TEXT,
        syncStatus TEXT DEFAULT 'synced',
        lastSyncedAt TEXT
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS stock_items (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        serverStockItemId INTEGER,
        code TEXT NOT NULL,
        name TEXT NOT NULL,
        description TEXT,
        unitPrice REAL DEFAULT 0,
        quantityOnHand INTEGER DEFAULT 0,
        category TEXT,
        syncStatus TEXT DEFAULT 'synced',
        lastSyncedAt TEXT
      );
    `);

    db.execSync(`
      CREATE TABLE IF NOT EXISTS settings (
        id INTEGER PRIMARY KEY AUTOINCREMENT,
        key TEXT UNIQUE NOT NULL,
        value TEXT
      );
    `);

    // Initialize default settings if not exists
    const apiUrlSetting = db.getFirstSync('SELECT * FROM settings WHERE key = ?', ['apiUrl']);
    if (!apiUrlSetting) {
      db.runSync('INSERT INTO settings (key, value) VALUES (?, ?)', ['apiUrl', 'http://localhost:5001/api']);
      db.runSync('INSERT INTO settings (key, value) VALUES (?, ?)', ['syncEnabled', '1']);
      db.runSync('INSERT INTO settings (key, value) VALUES (?, ?)', ['theme', 'auto']);
    }

    console.log('Database initialized successfully');
  } catch (error) {
    console.error('Error initializing database:', error);
    throw error;
  }
};

export const seedTestData = () => {
  try {
    // Clear existing data
    db.execSync('DELETE FROM sales_order_items');
    db.execSync('DELETE FROM sales_orders');
    db.execSync('DELETE FROM quote_items');
    db.execSync('DELETE FROM quotes');
    db.execSync('DELETE FROM work_order_tasks');
    db.execSync('DELETE FROM work_evidence_images');
    db.execSync('DELETE FROM site_visit_signoffs');
    db.execSync('DELETE FROM work_orders');
    db.execSync('DELETE FROM customers');
    db.execSync('DELETE FROM stock_items');

    const now = new Date().toISOString();

    // Seed Customers
    db.runSync(
      `INSERT INTO customers (serverCustomerId, name, email, phone, address, city, postCode, country, lastSyncedAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [1, 'John Doe', 'john@example.com', '555-0100', '123 Main St', 'Springfield', 'SP1 1AA', 'UK', now]
    );
    
    db.runSync(
      `INSERT INTO customers (serverCustomerId, name, email, phone, address, city, postCode, country, lastSyncedAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [2, 'Jane Smith', 'jane@example.com', '555-0200', '456 Oak Ave', 'London', 'L1 2BB', 'UK', now]
    );
    
    db.runSync(
      `INSERT INTO customers (serverCustomerId, name, email, phone, address, city, postCode, country, lastSyncedAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [3, 'Bob Johnson', 'bob@example.com', '555-0300', '789 Elm Rd', 'Manchester', 'M1 3CC', 'UK', now]
    );

    // Seed Stock Items
    db.runSync(
      `INSERT INTO stock_items (serverStockItemId, code, name, description, unitPrice, quantityOnHand, category, lastSyncedAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
      [1, 'PROD-001', 'Product A', 'High-quality product A', 500.00, 100, 'Hardware', now]
    );
    
    db.runSync(
      `INSERT INTO stock_items (serverStockItemId, code, name, description, unitPrice, quantityOnHand, category, lastSyncedAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
      [2, 'PROD-002', 'Product B', 'Premium product B', 750.00, 50, 'Hardware', now]
    );
    
    db.runSync(
      `INSERT INTO stock_items (serverStockItemId, code, name, description, unitPrice, quantityOnHand, category, lastSyncedAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
      [3, 'SERV-001', 'Service Package A', 'Comprehensive service package', 2500.00, 999, 'Services', now]
    );

    // Seed Sales Orders
    const soResult = db.runSync(
      `INSERT INTO sales_orders (orderNumber, customerName, customerEmail, customerPhone, orderDate, totalAmount, status, createdAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?)`,
      [`SO-${Date.now()}`, 'John Doe', 'john@example.com', '555-0100', now, 1500.00, 'Pending', now]
    );
    
    db.runSync(
      `INSERT INTO sales_order_items (salesOrderId, description, quantity, unitPrice, totalPrice) 
       VALUES (?, ?, ?, ?, ?)`,
      [soResult.lastInsertRowId, 'Product A', 2, 500.00, 1000.00]
    );
    
    db.runSync(
      `INSERT INTO sales_order_items (salesOrderId, description, quantity, unitPrice, totalPrice) 
       VALUES (?, ?, ?, ?, ?)`,
      [soResult.lastInsertRowId, 'Product B', 1, 500.00, 500.00]
    );

    // Seed Quotes
    const quoteResult = db.runSync(
      `INSERT INTO quotes (quoteNumber, customerName, customerEmail, customerPhone, quoteDate, expiryDate, totalAmount, status, createdAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [`QT-${Date.now()}`, 'Jane Smith', 'jane@example.com', '555-0200', now, new Date(Date.now() + 30 * 24 * 60 * 60 * 1000).toISOString(), 2500.00, 'Draft', now]
    );
    
    db.runSync(
      `INSERT INTO quote_items (quoteId, description, quantity, unitPrice, lineTotal) 
       VALUES (?, ?, ?, ?, ?)`,
      [quoteResult.lastInsertRowId, 'Service Package A', 1, 2500.00, 2500.00]
    );

    // Seed Work Orders
    const woResult = db.runSync(
      `INSERT INTO work_orders (workOrderNumber, customerName, customerEmail, customerPhone, description, workOrderDate, scheduledDate, status, priority, siteAddress, siteCity, sitePostCode, estimatedHours, createdAt) 
       VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
      [`WO-${Date.now()}`, 'Bob Johnson', 'bob@example.com', '555-0300', 'Install new system', now, new Date(Date.now() + 7 * 24 * 60 * 60 * 1000).toISOString(), 'Scheduled', 'High', '123 Main St', 'Springfield', 'SP1 2AB', 4.0, now]
    );
    
    db.runSync(
      `INSERT INTO work_order_tasks (workOrderId, taskName, description, isCompleted, sortOrder, estimatedHours) 
       VALUES (?, ?, ?, ?, ?, ?)`,
      [woResult.lastInsertRowId, 'Site Survey', 'Conduct initial site survey', 0, 1, 1.0]
    );
    
    db.runSync(
      `INSERT INTO work_order_tasks (workOrderId, taskName, description, isCompleted, sortOrder, estimatedHours) 
       VALUES (?, ?, ?, ?, ?, ?)`,
      [woResult.lastInsertRowId, 'Installation', 'Install equipment', 0, 2, 2.0]
    );
    
    db.runSync(
      `INSERT INTO work_order_tasks (workOrderId, taskName, description, isCompleted, sortOrder, estimatedHours) 
       VALUES (?, ?, ?, ?, ?, ?)`,
      [woResult.lastInsertRowId, 'Testing', 'Test installation', 0, 3, 1.0]
    );

    console.log('Test data seeded successfully');
  } catch (error) {
    console.error('Error seeding test data:', error);
    throw error;
  }
};

export default db;
