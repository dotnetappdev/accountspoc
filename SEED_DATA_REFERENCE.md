# Seed Data Reference - Accounting Department Structure

This document provides a complete reference for all seeded roles, users, and permissions in the AccountsPOC system.

## Summary Statistics

- **Total Roles**: 14
- **Total Users**: 30
- **Total Permissions**: 44
- **Default Tenant**: Demo Fulfillment Co. (ID: 1)

## Roles and Organizational Structure

### 1. Super Administrator (2 users)
**Description**: Highest level access with full system control and configuration

**Users**:
- Username: `superadmin` | Email: `superadmin@accountspoc.com` | Password: `SuperAdmin123!`
  - Name: System Administrator | Phone: +1-555-0100
- Username: `sysadmin` | Email: `sysadmin@accountspoc.com` | Password: `SysAdmin123!`
  - Name: IT Administrator | Phone: +1-555-0101

**Permissions**: All 44 permissions (full system access)

---

### 2. Administrator (3 users)
**Description**: Full operational access to all modules

**Users**:
- Username: `admin` | Email: `admin@accountspoc.com` | Password: `Admin123!`
  - Name: John Administrator | Phone: +1-555-0102
- Username: `admin.backup` | Email: `admin.backup@accountspoc.com` | Password: `Admin123!`
  - Name: Sarah Williams | Phone: +1-555-0103
- Username: `ops.admin` | Email: `ops.admin@accountspoc.com` | Password: `Admin123!`
  - Name: Michael Operations | Phone: +1-555-0104

**Permissions**: All permissions except system settings and audit logs

---

### 3. Financial Controller (2 users)
**Description**: Senior financial management with approval authority

**Users**:
- Username: `robert.controller` | Email: `robert.controller@accountspoc.com` | Password: `FinCtrl123!`
  - Name: Robert Harrison | Phone: +1-555-0110
- Username: `jennifer.cfo` | Email: `jennifer.cfo@accountspoc.com` | Password: `FinCtrl123!`
  - Name: Jennifer Martinez | Phone: +1-555-0111

**Key Permissions**:
- Full invoice management (create, read, update, delete, approve, post)
- Full purchase order management with approvals
- Payment processing and approval
- Bank account management and reconciliation
- All financial reports access

---

### 4. Accounting Manager (2 users)
**Description**: Manages daily accounting operations and team

**Users**:
- Username: `david.manager` | Email: `david.manager@accountspoc.com` | Password: `AcctMgr123!`
  - Name: David Thompson | Phone: +1-555-0120
- Username: `lisa.accounting` | Email: `lisa.accounting@accountspoc.com` | Password: `AcctMgr123!`
  - Name: Lisa Anderson | Phone: +1-555-0121

**Key Permissions**:
- Invoice management (except delete)
- Purchase order management (except delete)
- Payment processing (no approval)
- Bank account operations (except delete)
- All reports access

---

### 5. Senior Accountant (3 users)
**Description**: Handles complex transactions and reconciliations

**Users**:
- Username: `james.senior` | Email: `james.senior@accountspoc.com` | Password: `SrAcct123!`
  - Name: James Wilson | Phone: +1-555-0130
- Username: `emily.senior` | Email: `emily.senior@accountspoc.com` | Password: `SrAcct123!`
  - Name: Emily Davis | Phone: +1-555-0131
- Username: `william.senior` | Email: `william.senior@accountspoc.com` | Password: `SrAcct123!`
  - Name: William Brown | Phone: +1-555-0132

**Key Permissions**:
- Invoice operations (create, read, update)
- Bank account reconciliation
- Bank account read access
- Financial reports read access

---

### 6. Accountant (4 users)
**Description**: Performs standard accounting tasks and data entry

**Users**:
- Username: `susan.accountant` | Email: `susan.accountant@accountspoc.com` | Password: `Acct123!`
  - Name: Susan Miller | Phone: +1-555-0140
- Username: `thomas.accountant` | Email: `thomas.accountant@accountspoc.com` | Password: `Acct123!`
  - Name: Thomas Moore | Phone: +1-555-0141
- Username: `patricia.accountant` | Email: `patricia.accountant@accountspoc.com` | Password: `Acct123!`
  - Name: Patricia Taylor | Phone: +1-555-0142
- Username: `daniel.accountant` | Email: `daniel.accountant@accountspoc.com` | Password: `Acct123!`
  - Name: Daniel Jackson | Phone: +1-555-0143

**Key Permissions**:
- Invoice create, read, update
- Customer read access
- Reports read access

---

### 7. Bookkeeper (3 users)
**Description**: Basic data entry and record maintenance

**Users**:
- Username: `mary.bookkeeper` | Email: `mary.bookkeeper@accountspoc.com` | Password: `BookKpr123!`
  - Name: Mary White | Phone: +1-555-0150
- Username: `karen.bookkeeper` | Email: `karen.bookkeeper@accountspoc.com` | Password: `BookKpr123!`
  - Name: Karen Harris | Phone: +1-555-0151
- Username: `nancy.bookkeeper` | Email: `nancy.bookkeeper@accountspoc.com` | Password: `BookKpr123!`
  - Name: Nancy Clark | Phone: +1-555-0152

**Key Permissions**:
- Invoice create and read
- Customer read access

---

### 8. Accounts Payable Clerk (2 users)
**Description**: Manages vendor invoices and payments

**Users**:
- Username: `betty.payables` | Email: `betty.payables@accountspoc.com` | Password: `APClerk123!`
  - Name: Betty Lewis | Phone: +1-555-0160
- Username: `helen.payables` | Email: `helen.payables@accountspoc.com` | Password: `APClerk123!`
  - Name: Helen Robinson | Phone: +1-555-0161

**Key Permissions**:
- Purchase order management (create, read, update)
- Payment processing

---

### 9. Accounts Receivable Clerk (2 users)
**Description**: Manages customer invoices and collections

**Users**:
- Username: `sandra.receivables` | Email: `sandra.receivables@accountspoc.com` | Password: `ARClerk123!`
  - Name: Sandra Walker | Phone: +1-555-0170
- Username: `donna.receivables` | Email: `donna.receivables@accountspoc.com` | Password: `ARClerk123!`
  - Name: Donna Young | Phone: +1-555-0171

**Key Permissions**:
- Invoice management (create, read, update)
- Customer read and update access

---

### 10. Payroll Manager (1 user)
**Description**: Manages payroll processing and compliance

**Users**:
- Username: `paul.payroll` | Email: `paul.payroll@accountspoc.com` | Password: `PayRoll123!`
  - Name: Paul Allen | Phone: +1-555-0180

**Key Permissions**:
- Reports read access

---

### 11. Auditor (2 users)
**Description**: Read-only access to financial records and audit logs

**Users**:
- Username: `carol.auditor` | Email: `carol.auditor@accountspoc.com` | Password: `Audit123!`
  - Name: Carol King | Phone: +1-555-0190
- Username: `george.auditor` | Email: `george.auditor@accountspoc.com` | Password: `Audit123!`
  - Name: George Scott | Phone: +1-555-0191

**Key Permissions**:
- All read permissions across modules
- Audit log access

---

### 12. Support (2 users)
**Description**: Customer support with tenant and customer management

**Users**:
- Username: `support` | Email: `support@accountspoc.com` | Password: `Support123!`
  - Name: Support Team | Phone: +1-555-0200
- Username: `support.lead` | Email: `support.lead@accountspoc.com` | Password: `Support123!`
  - Name: Jessica Support | Phone: +1-555-0201

**Key Permissions**:
- Full tenant management
- Full customer management

---

### 13. Agent (2 users)
**Description**: Field agents with limited customer access

**Users**:
- Username: `agent` | Email: `agent@accountspoc.com` | Password: `Agent123!`
  - Name: Field Agent | Phone: +1-555-0210
- Username: `agent.mobile` | Email: `agent.mobile@accountspoc.com` | Password: `Agent123!`
  - Name: Mark Field | Phone: +1-555-0211

**Key Permissions**:
- Customer read access
- Customer contact management

---

### 14. User (0 users seeded)
**Description**: Standard user with basic read access

**Permissions**: Basic read-only access

---

## Complete Permissions List (44 total)

### Tenant Management (4)
1. Create Tenant
2. Read Tenant
3. Update Tenant
4. Delete Tenant

### Customer Management (5)
5. Create Customer
6. Read Customer
7. Update Customer
8. Delete Customer
9. Manage Customer Contacts

### Stock Item Management (5)
10. Create Stock Item
11. Read Stock Item
12. Update Stock Item
13. Delete Stock Item
14. Manage Stock Images

### Invoice & Billing (6)
15. Create Invoice
16. Read Invoice
17. Update Invoice
18. Delete Invoice
19. Approve Invoice
20. Post Invoice

### Purchase Orders (5)
21. Create Purchase Order
22. Read Purchase Order
23. Update Purchase Order
24. Delete Purchase Order
25. Approve Purchase Order

### Financial Reports (3)
26. View Financial Reports
27. Export Financial Reports
28. View Management Reports

### Bank Accounts (5)
29. Create Bank Account
30. Read Bank Account
31. Update Bank Account
32. Delete Bank Account
33. Reconcile Bank Account

### Payments (3)
34. Process Payment
35. Approve Payment
36. View Payment History

### User Management (6)
37. Create User
38. Read User
39. Update User
40. Delete User
41. Manage Roles
42. Manage Permissions

### System (2)
43. Manage System Settings
44. View Audit Logs

---

## Quick Login Reference

### Testing Different Permission Levels

| Role Level | Username | Email | Password |
|-----------|----------|-------|----------|
| **Highest Access** | superadmin | superadmin@accountspoc.com | SuperAdmin123! |
| **Full Operations** | admin | admin@accountspoc.com | Admin123! |
| **Financial Leadership** | robert.controller | robert.controller@accountspoc.com | FinCtrl123! |
| **Operations Manager** | david.manager | david.manager@accountspoc.com | AcctMgr123! |
| **Senior Staff** | james.senior | james.senior@accountspoc.com | SrAcct123! |
| **Staff Accountant** | susan.accountant | susan.accountant@accountspoc.com | Acct123! |
| **Data Entry** | mary.bookkeeper | mary.bookkeeper@accountspoc.com | BookKpr123! |
| **AP Specialist** | betty.payables | betty.payables@accountspoc.com | APClerk123! |
| **AR Specialist** | sandra.receivables | sandra.receivables@accountspoc.com | ARClerk123! |
| **Audit/Review** | carol.auditor | carol.auditor@accountspoc.com | Audit123! |

---

## Password Policy

All seeded accounts follow these password requirements:
- Minimum 6 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- Special character (!) included in all default passwords

**Important**: Change all default passwords in production environments!

---

## Organizational Hierarchy

```
Super Administrator (System Level)
└── Administrator (Operations)
    ├── Financial Controller (Financial Leadership)
    │   ├── Accounting Manager (Department Management)
    │   │   ├── Senior Accountant (Complex Transactions)
    │   │   │   ├── Accountant (Standard Tasks)
    │   │   │   │   └── Bookkeeper (Data Entry)
    │   │   │   ├── AP Clerk (Vendor Management)
    │   │   │   └── AR Clerk (Customer Billing)
    │   │   └── Payroll Manager (Payroll Operations)
    │   └── Auditor (Independent Review)
    ├── Support (Customer Service)
    └── Agent (Field Operations)
```

---

## Usage Notes

1. **Multi-Tenant Support**: All users are assigned to the default tenant (ID: 1)
2. **Role Assignment**: Users can be assigned multiple roles if needed
3. **Permission Inheritance**: Permissions are role-based, not hierarchical
4. **Active Status**: All seeded users are active by default
5. **Phone Numbers**: All use placeholder format +1-555-XXXX

---

## API Testing

Test authentication with any user:

```bash
curl -X POST http://localhost:5049/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@accountspoc.com",
    "password": "Admin123!"
  }'
```

Get current user info:

```bash
curl http://localhost:5049/api/auth/me \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

---

## Database Schema

The seed data creates tables for:
- `AspNetUsers` - User accounts
- `AspNetRoles` - Role definitions
- `AspNetUserRoles` - User-Role assignments
- `Permissions` - Permission definitions
- `RolePermissions` - Role-Permission assignments

All using ASP.NET Identity with integer primary keys.
