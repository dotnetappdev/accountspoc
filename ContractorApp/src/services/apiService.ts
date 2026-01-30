import axios, { AxiosInstance } from 'axios';
import { SalesOrder, Quote, WorkOrder, SiteVisitSignOff } from '../types';
import db from '../database/database';

class ApiService {
  private api: AxiosInstance;
  private baseURL: string = 'http://localhost:5001/api';

  constructor() {
    this.api = axios.create({
      baseURL: this.baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  updateBaseURL(url: string) {
    this.baseURL = url;
    this.api = axios.create({
      baseURL: this.baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
    });
  }

  // Sales Orders
  async getSalesOrders(): Promise<SalesOrder[]> {
    const response = await this.api.get('/SalesOrders');
    return response.data;
  }

  async getSalesOrder(id: number): Promise<SalesOrder> {
    const response = await this.api.get(`/SalesOrders/${id}`);
    return response.data;
  }

  async createSalesOrder(order: SalesOrder): Promise<SalesOrder> {
    const response = await this.api.post('/SalesOrders', {
      ...order,
      tenantId: 1,
      salesOrderItems: order.items || [],
    });
    return response.data;
  }

  async updateSalesOrder(id: number, order: SalesOrder): Promise<void> {
    await this.api.put(`/SalesOrders/${id}`, {
      ...order,
      tenantId: 1,
      salesOrderItems: order.items || [],
    });
  }

  async deleteSalesOrder(id: number): Promise<void> {
    await this.api.delete(`/SalesOrders/${id}`);
  }

  // Quotes
  async getQuotes(): Promise<Quote[]> {
    const response = await this.api.get('/Quotes');
    return response.data;
  }

  async getQuote(id: number): Promise<Quote> {
    const response = await this.api.get(`/Quotes/${id}`);
    return response.data;
  }

  async createQuote(quote: Quote): Promise<Quote> {
    const response = await this.api.post('/Quotes', {
      ...quote,
      tenantId: 1,
      quoteItems: quote.items || [],
    });
    return response.data;
  }

  async updateQuote(id: number, quote: Quote): Promise<void> {
    await this.api.put(`/Quotes/${id}`, {
      ...quote,
      tenantId: 1,
      quoteItems: quote.items || [],
    });
  }

  async deleteQuote(id: number): Promise<void> {
    await this.api.delete(`/Quotes/${id}`);
  }

  async convertQuoteToOrder(id: number): Promise<SalesOrder> {
    const response = await this.api.post(`/Quotes/${id}/convert-to-order`);
    return response.data;
  }

  // Work Orders
  async getWorkOrders(): Promise<WorkOrder[]> {
    const response = await this.api.get('/WorkOrders');
    return response.data;
  }

  async getWorkOrder(id: number): Promise<WorkOrder> {
    const response = await this.api.get(`/WorkOrders/${id}`);
    return response.data;
  }

  async createWorkOrder(workOrder: WorkOrder): Promise<WorkOrder> {
    const response = await this.api.post('/WorkOrders', {
      ...workOrder,
      tenantId: 1,
      workOrderTasks: workOrder.tasks || [],
    });
    return response.data;
  }

  async updateWorkOrder(id: number, workOrder: WorkOrder): Promise<void> {
    await this.api.put(`/WorkOrders/${id}`, {
      ...workOrder,
      tenantId: 1,
      workOrderTasks: workOrder.tasks || [],
    });
  }

  async deleteWorkOrder(id: number): Promise<void> {
    await this.api.delete(`/WorkOrders/${id}`);
  }

  // Site Visit Sign-offs
  async getSiteVisitSignOffs(): Promise<SiteVisitSignOff[]> {
    const response = await this.api.get('/SiteVisitSignOffs');
    return response.data;
  }

  async getSignOffsByWorkOrder(workOrderId: number): Promise<SiteVisitSignOff[]> {
    const response = await this.api.get(`/SiteVisitSignOffs/workorder/${workOrderId}`);
    return response.data;
  }

  async createSiteVisitSignOff(signOff: SiteVisitSignOff): Promise<SiteVisitSignOff> {
    const response = await this.api.post('/SiteVisitSignOffs', signOff);
    return response.data;
  }

  async updateSiteVisitSignOff(id: number, signOff: SiteVisitSignOff): Promise<void> {
    await this.api.put(`/SiteVisitSignOffs/${id}`, signOff);
  }

  async deleteSiteVisitSignOff(id: number): Promise<void> {
    await this.api.delete(`/SiteVisitSignOffs/${id}`);
  }

  // Sync functions
  async syncToServer(): Promise<void> {
    try {
      // Sync pending sales orders
      const pendingSO = db.getAllSync(
        'SELECT * FROM sales_orders WHERE syncStatus = ?',
        ['pending']
      ) as any[];
      
      for (const order of pendingSO) {
        const items = db.getAllSync(
          'SELECT * FROM sales_order_items WHERE salesOrderId = ?',
          [order.id]
        ) as any[];
        
        const created = await this.createSalesOrder({
          ...order,
          items: items,
        });
        
        db.runSync(
          'UPDATE sales_orders SET serverOrderId = ?, syncStatus = ? WHERE id = ?',
          [created.id, 'synced', order.id]
        );
      }

      // Sync pending quotes
      const pendingQuotes = db.getAllSync(
        'SELECT * FROM quotes WHERE syncStatus = ?',
        ['pending']
      ) as any[];
      
      for (const quote of pendingQuotes) {
        const items = db.getAllSync(
          'SELECT * FROM quote_items WHERE quoteId = ?',
          [quote.id]
        ) as any[];
        
        const created = await this.createQuote({
          ...quote,
          items: items,
        });
        
        db.runSync(
          'UPDATE quotes SET serverQuoteId = ?, syncStatus = ? WHERE id = ?',
          [created.id, 'synced', quote.id]
        );
      }

      // Sync pending work orders
      const pendingWO = db.getAllSync(
        'SELECT * FROM work_orders WHERE syncStatus = ?',
        ['pending']
      ) as any[];
      
      for (const wo of pendingWO) {
        const tasks = db.getAllSync(
          'SELECT * FROM work_order_tasks WHERE workOrderId = ?',
          [wo.id]
        ) as any[];
        
        const created = await this.createWorkOrder({
          ...wo,
          tasks: tasks,
        });
        
        db.runSync(
          'UPDATE work_orders SET serverWorkOrderId = ?, syncStatus = ? WHERE id = ?',
          [created.id, 'synced', wo.id]
        );
      }

      // Update last sync time
      db.runSync(
        'UPDATE settings SET lastSync = ? WHERE id = 1',
        [new Date().toISOString()]
      );
      
      console.log('Sync completed successfully');
    } catch (error) {
      console.error('Sync error:', error);
      throw error;
    }
  }

  async syncFromServer(): Promise<void> {
    try {
      // Pull sales orders from server
      const salesOrders = await this.getSalesOrders();
      for (const order of salesOrders) {
        const existing = db.getFirstSync(
          'SELECT * FROM sales_orders WHERE serverOrderId = ?',
          [order.id]
        );
        
        if (!existing) {
          db.runSync(
            `INSERT INTO sales_orders (serverOrderId, orderNumber, customerName, customerEmail, customerPhone, orderDate, totalAmount, status, syncStatus, createdAt) 
             VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
            [order.id, order.orderNumber, order.customerName, order.customerEmail || '', order.customerPhone || '', order.orderDate, order.totalAmount, order.status, 'synced', order.createdAt]
          );
        }
      }

      console.log('Sync from server completed');
    } catch (error) {
      console.error('Sync from server error:', error);
      throw error;
    }
  }
}

export default new ApiService();
