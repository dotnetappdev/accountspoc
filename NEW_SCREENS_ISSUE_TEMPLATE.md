# GitHub Issue: Implement Additional Sage 200 Screens

## Issue Title
Implement 15 Additional Sage 200 Screens for Complete ERP Functionality

## Labels
- enhancement
- feature
- sage-200
- ui
- screens

## Priority
Medium

## Description

Following the comprehensive Sage 200 field compatibility implementation (PR #XXX), this issue tracks the implementation of 15 additional screens identified from Sage 200 that would enhance the ERP functionality of AccountsPOC.

### Current State
The application currently has 26 screens covering core functionality:
- ✅ Stock Items, Products, Customers, Suppliers
- ✅ Sales Orders, Purchase Orders, Sales Invoices
- ✅ Bill of Materials, Warehouses, Price Lists
- ✅ Bank Accounts, Exchange Rates
- ✅ Email Templates, Custom Fields, Configuration Settings
- ✅ And 11 other management screens

### Proposed New Screens

The following 15 screens have been identified from Sage 200 as valuable additions:

## 1. Quotations/Estimates Module

**Purpose:** Create and manage sales quotations that can be converted to sales orders

**Key Features:**
- Create quotations with line items
- Version control for quote revisions
- Quote expiry date tracking
- Convert quote to sales order with one click
- Quote approval workflow
- Print/email quote to customer
- Quote acceptance tracking

**Priority:** High  
**Complexity:** Medium  
**Dependencies:** Sales Orders, Products, Customers

---

## 2. Goods Received Notes (GRN)

**Purpose:** Record receipt of goods from purchase orders

**Key Features:**
- Link to purchase orders
- Record received quantities vs. ordered quantities
- Quality inspection notes
- Partial receipt support
- Automatic stock level updates
- Discrepancy reporting
- Generate receipt documentation
- Update purchase order status

**Priority:** High  
**Complexity:** Medium  
**Dependencies:** Purchase Orders, Stock Items, Warehouses

---

## 3. Stock Adjustments

**Purpose:** Record stock level adjustments beyond regular counts

**Key Features:**
- Adjustment reasons (Damage, Theft, Correction, Write-off, Found)
- Before/after quantities
- Cost impact calculation
- Approval workflow for large adjustments
- Adjustment history and audit trail
- Link to stock count if applicable
- Automatic GL posting for accounting

**Priority:** Medium  
**Complexity:** Low  
**Dependencies:** Stock Items, Warehouses

---

## 4. Credit Notes

**Purpose:** Process sales returns and issue credit notes to customers

**Key Features:**
- Link to original sales invoice
- Reason for return
- Partial or full credit support
- Automatic stock return to inventory
- Refund processing
- Credit note printing/emailing
- Customer account crediting
- Impact on sales reports

**Priority:** High  
**Complexity:** Medium  
**Dependencies:** Sales Invoices, Customers, Stock Items

---

## 5. Purchase Returns

**Purpose:** Return goods to suppliers and track credit

**Key Features:**
- Link to original purchase order/GRN
- Return reason tracking
- Return authorization number
- Supplier credit tracking
- Return shipping details
- Quality issue documentation
- Impact on supplier performance metrics
- Stock removal from inventory

**Priority:** Medium  
**Complexity:** Medium  
**Dependencies:** Purchase Orders, Suppliers, Stock Items

---

## 6. Stock Transfers

**Purpose:** Transfer stock between warehouses/locations

**Key Features:**
- Source and destination warehouse selection
- Transfer quantity entry
- Transfer reason
- In-transit status tracking
- Automatic stock updates on confirmation
- Transfer cost tracking
- Print transfer documentation
- Transfer approval workflow

**Priority:** Medium  
**Complexity:** Low  
**Dependencies:** Stock Items, Warehouses

---

## 7. Sales Ledger

**Purpose:** Customer account statements and transaction history

**Key Features:**
- Customer transaction listing
- Outstanding balance calculation
- Aged debt analysis (30/60/90 days)
- Payment history
- Credit limit vs. current balance
- Statement generation
- Payment reminders
- Customer account reconciliation

**Priority:** Medium  
**Complexity:** Medium  
**Dependencies:** Customers, Sales Orders, Sales Invoices

---

## 8. Purchase Ledger

**Purpose:** Supplier account statements and payment tracking

**Key Features:**
- Supplier transaction listing
- Outstanding payables
- Payment due date tracking
- Payment history
- Supplier statement reconciliation
- Batch payment processing
- Payment run generation
- Supplier account aging

**Priority:** Medium  
**Complexity:** Medium  
**Dependencies:** Suppliers, Purchase Orders

---

## 9. Production Orders

**Purpose:** Manufacturing work orders (enhanced with BOM-Sales Order linking!)

**Key Features:**
- Create from Sales Order + BOM (direct link - our innovation!)
- Production scheduling
- Material requirements calculation
- Work order status tracking (Planned, In Progress, Completed)
- Actual vs. planned cost tracking
- Labour time recording
- Quality checks during production
- Automatic stock consumption and finished goods receipt
- Production reporting

**Priority:** Medium  
**Complexity:** High  
**Dependencies:** Bill of Materials, Sales Orders, Stock Items  
**Note:** Now much easier to implement with our BOM-to-Sales Order direct linking!

---

## 10. Quality Control

**Purpose:** Inspection and quality management

**Key Features:**
- Inspection checklists
- Pass/fail criteria
- Non-conformance reporting
- Corrective action tracking
- Supplier quality ratings
- Product quality history
- Certificate of conformance generation
- Link to GRN, Production Orders
- Quality metrics dashboard

**Priority:** Low  
**Complexity:** Medium  
**Dependencies:** Goods Received Notes, Production Orders, Suppliers

---

## 11. Serial Number Tracking

**Purpose:** Track individual items by serial/batch/lot number

**Key Features:**
- Serial number assignment at receipt
- Batch/lot number tracking
- Expiry date tracking
- Serial number history (sales, returns, movements)
- Recall management
- Warranty tracking
- Search by serial number
- Traceability reports
- Integration with sales and stock movements

**Priority:** Medium  
**Complexity:** High  
**Dependencies:** Stock Items, Sales Orders, Purchase Orders, GRN

---

## 12. Document Management

**Purpose:** Attach and manage documents related to orders, products, customers

**Key Features:**
- Document upload (PDF, Word, Excel, images)
- Link documents to: Products, Customers, Suppliers, Orders, Invoices
- Document categories/tags
- Version control
- Document search
- Access control
- Document templates
- Automatic document generation
- Email document attachments

**Priority:** Low  
**Complexity:** Medium  
**Dependencies:** All major entities  
**Technical Note:** Use existing file upload configuration from SystemSettings

---

## 13. CRM/Contact Management

**Purpose:** Enhanced customer relationship management beyond basic info

**Key Features:**
- Multiple contacts per customer
- Contact roles (Purchasing, Finance, Technical, etc.)
- Communication history (calls, emails, meetings)
- Opportunity tracking
- Lead management
- Sales pipeline
- Activity scheduling
- Follow-up reminders
- Customer communication preferences
- Contact notes and history

**Priority:** Low  
**Complexity:** High  
**Dependencies:** Customers

---

## 14. Contract Management

**Purpose:** Manage long-term agreements and recurring orders

**Key Features:**
- Contract creation and versioning
- Contract terms and conditions
- Pricing agreements
- Volume commitments
- Contract renewal tracking
- Automatic order generation from contracts
- Contract performance monitoring
- Contract expiry notifications
- Special terms and conditions
- Link to customers/suppliers

**Priority:** Low  
**Complexity:** High  
**Dependencies:** Customers, Suppliers, Products

---

## 15. Commission Tracking

**Purpose:** Calculate and track sales person commissions

**Key Features:**
- Commission rules configuration
- Commission rate by product/customer/territory
- Sales person assignment to orders
- Automatic commission calculation
- Commission reports
- Commission payment tracking
- Commission adjustments
- Period-based commission (monthly, quarterly)
- Commission approval workflow
- Commission statement generation

**Priority:** Low  
**Complexity:** Medium  
**Dependencies:** Sales Orders, Sales Invoices  
**Technical Note:** Use SalesPersonId already added to SalesOrder entity

---

## Implementation Approach

### Phase 1 - High Priority (Immediate Business Value)
1. Quotations/Estimates
2. Goods Received Notes (GRN)
3. Credit Notes

**Estimated Effort:** 3-4 weeks  
**Business Impact:** High - completes core sales and purchasing cycle

### Phase 2 - Medium Priority (Enhanced Operations)
4. Stock Adjustments
5. Purchase Returns
6. Stock Transfers
7. Sales Ledger
8. Purchase Ledger
9. Production Orders
10. Serial Number Tracking

**Estimated Effort:** 6-8 weeks  
**Business Impact:** Medium - improves operational efficiency

### Phase 3 - Low Priority (Nice to Have)
11. Quality Control
12. Document Management
13. CRM/Contact Management
14. Contract Management
15. Commission Tracking

**Estimated Effort:** 8-10 weeks  
**Business Impact:** Low-Medium - adds advanced features

---

## Technical Considerations

### Existing Infrastructure to Leverage

1. **Modal Dialog Component** - Use for all forms (already implemented)
2. **Pagination Component** - Use for all list views (already implemented)
3. **ViewModel Pattern** - Follow established pattern (already implemented)
4. **Validation** - Use DataAnnotationsValidator (already implemented)
5. **Configuration Settings** - SystemSettings already has pagination and file upload configs

### Database Schema

All major entities already have the fields needed:
- ✅ Customer, Supplier, Product, StockItem - Complete
- ✅ SalesOrder, PurchaseOrder - Complete with audit trails
- ✅ BillOfMaterial - Enhanced with Sales Order linking

New entities will be needed for:
- Quotation, QuotationItem
- GoodsReceivedNote, GRNItem
- StockAdjustment
- CreditNote, CreditNoteItem
- PurchaseReturn, PurchaseReturnItem
- StockTransfer, StockTransferItem
- ProductionOrder, ProductionOrderItem
- QualityInspection, QualityCheck
- SerialNumber, LotNumber
- Document, DocumentLink
- Contact, Communication, Opportunity
- Contract, ContractItem
- Commission, CommissionPayment

### UI/UX Consistency

All new screens should follow the established pattern:
- Modern page header with title and subtitle
- Primary action button in header (Add New, Create, etc.)
- Modal dialogs for create/edit forms
- Tables with Bootstrap styling for lists
- Pagination component for large datasets
- Loading states and empty states
- Consistent color scheme and icons
- Responsive design

---

## Success Criteria

- [ ] All 15 screens implemented according to specifications
- [ ] Screens follow established UI/UX patterns
- [ ] All screens use modal dialogs for forms
- [ ] Pagination implemented on list views
- [ ] ViewModel pattern used consistently
- [ ] Validation working correctly
- [ ] Database migrations created and tested
- [ ] API endpoints implemented
- [ ] No security vulnerabilities introduced
- [ ] Code review passed
- [ ] Documentation updated
- [ ] User guide created for new features

---

## Benefits

1. **Complete ERP Functionality** - Matches and exceeds Sage 200 capabilities
2. **Improved User Experience** - Consistent UI across all screens
3. **Better Business Processes** - Complete workflows from quote to cash, purchase to pay
4. **Enhanced Reporting** - More data points for analytics
5. **Competitive Advantage** - Features that improve upon Sage 200 limitations (e.g., BOM-to-Sales Order linking)

---

## Related Work

- PR #XXX - Sage 200 field compatibility (350+ fields added)
- Documentation: SAGE_200_COMPLETE_MAPPING.md
- Documentation: SAGE_200_FORMS_GUIDE.md
- Issue: Original "Fix the forms layout to be more like sage 200"

---

## Notes

This issue can be broken down into 15 smaller issues (one per screen) if preferred. Each screen could be implemented independently by different team members.

The infrastructure is already in place from the previous PR:
- Entity fields are ready
- Modal dialog component exists
- Pagination component exists
- ViewModel pattern established
- Configuration settings available

Implementation should be straightforward - primarily UI work with CRUD operations following established patterns.
