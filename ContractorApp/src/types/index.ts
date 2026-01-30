export interface SalesOrder {
  id?: number;
  serverOrderId?: number;
  orderNumber: string;
  customerName: string;
  customerEmail?: string;
  customerPhone?: string;
  orderDate: string;
  totalAmount: number;
  status: string;
  notes?: string;
  syncStatus?: string;
  createdAt: string;
  updatedAt?: string;
  items?: SalesOrderItem[];
}

export interface SalesOrderItem {
  id?: number;
  salesOrderId?: number;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
}

export interface Quote {
  id?: number;
  serverQuoteId?: number;
  quoteNumber: string;
  customerName: string;
  customerEmail?: string;
  customerPhone?: string;
  quoteDate: string;
  expiryDate?: string;
  totalAmount: number;
  status: string;
  notes?: string;
  syncStatus?: string;
  createdAt: string;
  updatedAt?: string;
  items?: QuoteItem[];
}

export interface QuoteItem {
  id?: number;
  quoteId?: number;
  description: string;
  quantity: number;
  unitPrice: number;
  lineTotal: number;
}

export interface WorkOrder {
  id?: number;
  serverWorkOrderId?: number;
  workOrderNumber: string;
  customerName: string;
  customerEmail?: string;
  customerPhone?: string;
  description: string;
  workOrderDate: string;
  scheduledDate?: string;
  completedDate?: string;
  status: string;
  priority: string;
  siteAddress?: string;
  siteCity?: string;
  sitePostCode?: string;
  estimatedHours: number;
  actualHours: number;
  syncStatus?: string;
  createdAt: string;
  updatedAt?: string;
  tasks?: WorkOrderTask[];
  signOffs?: SiteVisitSignOff[];
}

export interface WorkOrderTask {
  id?: number;
  workOrderId?: number;
  taskName: string;
  description?: string;
  isCompleted: boolean;
  completedDate?: string;
  sortOrder: number;
  estimatedHours: number;
  actualHours: number;
}

export interface SiteVisitSignOff {
  id?: number;
  serverSignOffId?: number;
  workOrderId: number;
  visitDate: string;
  visitType: string;
  signedByName: string;
  signedByTitle?: string;
  signedDate: string;
  signatureImagePath?: string;
  workCompleted?: string;
  issuesIdentified?: string;
  nextSteps?: string;
  customerComments?: string;
  customerSatisfactionRating?: number;
  syncStatus?: string;
  createdAt: string;
}

export interface WorkEvidenceImage {
  id?: number;
  workOrderId: number;
  imagePath: string;
  description?: string;
  capturedAt: string;
  syncStatus?: string;
}

export interface Customer {
  id?: number;
  serverCustomerId?: number;
  name: string;
  email?: string;
  phone?: string;
  address?: string;
  city?: string;
  postCode?: string;
  country?: string;
  syncStatus?: string;
  lastSyncedAt?: string;
}

export interface StockItem {
  id?: number;
  serverStockItemId?: number;
  code: string;
  name: string;
  description?: string;
  unitPrice: number;
  quantityOnHand: number;
  category?: string;
  syncStatus?: string;
  lastSyncedAt?: string;
}

export interface Settings {
  id?: number;
  apiUrl: string;
  apiKey?: string;
  syncEnabled: boolean;
  lastSync?: string;
}
