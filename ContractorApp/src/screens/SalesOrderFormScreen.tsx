import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert,
  Modal,
  FlatList,
} from 'react-native';
import { useNavigation, useRoute } from '@react-navigation/native';
import db from '../database/database';

interface LineItem {
  id?: number;
  productId?: number;
  description: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  isFreeTextItem: boolean;
}

const SalesOrderFormScreen = () => {
  const navigation = useNavigation();
  const route = useRoute<any>();
  const orderId = route.params?.id;

  const [orderNumber, setOrderNumber] = useState(`SO-${Date.now()}`);
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');
  const [customerPhone, setCustomerPhone] = useState('');
  const [status, setStatus] = useState('Pending');
  const [notes, setNotes] = useState('');
  const [lineItems, setLineItems] = useState<LineItem[]>([]);
  
  // Line item form states
  const [showLineItemModal, setShowLineItemModal] = useState(false);
  const [itemType, setItemType] = useState<'product' | 'freetext'>('product');
  const [selectedProductId, setSelectedProductId] = useState<number | undefined>();
  const [itemDescription, setItemDescription] = useState('');
  const [itemQuantity, setItemQuantity] = useState('1');
  const [itemUnitPrice, setItemUnitPrice] = useState('0');
  
  const [stockItems, setStockItems] = useState<any[]>([]);

  React.useEffect(() => {
    // Load stock items
    const items = db.getAllSync('SELECT * FROM stock_items ORDER BY name') as any[];
    setStockItems(items);

    if (orderId) {
      const order = db.getFirstSync('SELECT * FROM sales_orders WHERE id = ?', [orderId]) as any;
      if (order) {
        setOrderNumber(order.orderNumber);
        setCustomerName(order.customerName);
        setCustomerEmail(order.customerEmail || '');
        setCustomerPhone(order.customerPhone || '');
        setStatus(order.status);
        setNotes(order.notes || '');
        
        // Load line items
        const items = db.getAllSync('SELECT * FROM sales_order_items WHERE salesOrderId = ?', [orderId]) as any[];
        setLineItems(items.map(item => ({
          id: item.id,
          productId: item.productId,
          description: item.description,
          quantity: item.quantity,
          unitPrice: item.unitPrice,
          totalPrice: item.totalPrice,
          isFreeTextItem: item.isFreeTextItem === 1,
        })));
      }
    }
  }, [orderId]);

  const handleProductSelect = (productId: number) => {
    setSelectedProductId(productId);
    const product = stockItems.find(p => p.id === productId);
    if (product) {
      setItemDescription(product.name);
      setItemUnitPrice(product.unitPrice.toString());
    }
  };

  const handleAddLineItem = () => {
    if (itemType === 'product' && !selectedProductId) {
      Alert.alert('Error', 'Please select a product');
      return;
    }
    
    if (itemType === 'freetext' && !itemDescription.trim()) {
      Alert.alert('Error', 'Please enter a description');
      return;
    }

    const quantity = parseInt(itemQuantity) || 0;
    const unitPrice = parseFloat(itemUnitPrice) || 0;

    if (quantity <= 0) {
      Alert.alert('Error', 'Quantity must be greater than 0');
      return;
    }

    const newItem: LineItem = {
      productId: itemType === 'product' ? selectedProductId : undefined,
      description: itemDescription,
      quantity,
      unitPrice,
      totalPrice: quantity * unitPrice,
      isFreeTextItem: itemType === 'freetext',
    };

    setLineItems([...lineItems, newItem]);
    resetLineItemForm();
    setShowLineItemModal(false);
  };

  const resetLineItemForm = () => {
    setItemType('product');
    setSelectedProductId(undefined);
    setItemDescription('');
    setItemQuantity('1');
    setItemUnitPrice('0');
  };

  const handleRemoveLineItem = (index: number) => {
    Alert.alert(
      'Remove Item',
      'Are you sure you want to remove this item?',
      [
        { text: 'Cancel', style: 'cancel' },
        {
          text: 'Remove',
          style: 'destructive',
          onPress: () => {
            const newItems = [...lineItems];
            newItems.splice(index, 1);
            setLineItems(newItems);
          },
        },
      ]
    );
  };

  const calculateTotal = () => {
    return lineItems.reduce((sum, item) => sum + item.totalPrice, 0);
  };

  const handleSave = () => {
    if (!customerName.trim()) {
      Alert.alert('Error', 'Customer name is required');
      return;
    }

    try {
      const now = new Date().toISOString();
      const totalAmount = calculateTotal();
      
      if (orderId) {
        db.runSync(
          \`UPDATE sales_orders SET orderNumber = ?, customerName = ?, customerEmail = ?, customerPhone = ?, totalAmount = ?, status = ?, notes = ?, updatedAt = ? WHERE id = ?\`,
          [orderNumber, customerName, customerEmail, customerPhone, totalAmount, status, notes, now, orderId]
        );
        
        // Delete existing line items
        db.runSync('DELETE FROM sales_order_items WHERE salesOrderId = ?', [orderId]);
        
        // Insert new line items
        lineItems.forEach(item => {
          db.runSync(
            \`INSERT INTO sales_order_items (salesOrderId, productId, description, quantity, unitPrice, totalPrice, isFreeTextItem) 
             VALUES (?, ?, ?, ?, ?, ?, ?)\`,
            [orderId, item.productId || null, item.description, item.quantity, item.unitPrice, item.totalPrice, item.isFreeTextItem ? 1 : 0]
          );
        });
      } else {
        const result = db.runSync(
          \`INSERT INTO sales_orders (orderNumber, customerName, customerEmail, customerPhone, orderDate, totalAmount, status, notes, syncStatus, createdAt) 
           VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)\`,
          [orderNumber, customerName, customerEmail, customerPhone, now, totalAmount, status, notes, 'pending', now]
        );
        
        const newOrderId = result.lastInsertRowId;
        
        // Insert line items
        lineItems.forEach(item => {
          db.runSync(
            \`INSERT INTO sales_order_items (salesOrderId, productId, description, quantity, unitPrice, totalPrice, isFreeTextItem) 
             VALUES (?, ?, ?, ?, ?, ?, ?)\`,
            [newOrderId, item.productId || null, item.description, item.quantity, item.unitPrice, item.totalPrice, item.isFreeTextItem ? 1 : 0]
          );
        });
      }

      Alert.alert('Success', 'Sales order saved successfully');
      navigation.goBack();
    } catch (error) {
      console.error('Error saving order:', error);
      Alert.alert('Error', 'Failed to save order');
    }
  };

  const renderLineItem = ({ item, index }: { item: LineItem; index: number }) => (
    <View style={styles.lineItem}>
      <View style={styles.lineItemHeader}>
        <Text style={styles.lineItemDescription}>
          {item.isFreeTextItem && '✏️ '}
          {item.description}
        </Text>
        <TouchableOpacity onPress={() => handleRemoveLineItem(index)}>
          <Text style={styles.removeButton}>✕</Text>
        </TouchableOpacity>
      </View>
      <View style={styles.lineItemDetails}>
        <Text style={styles.lineItemText}>Qty: {item.quantity}</Text>
        <Text style={styles.lineItemText}>@ ${item.unitPrice.toFixed(2)}</Text>
        <Text style={styles.lineItemTotal}>${item.totalPrice.toFixed(2)}</Text>
      </View>
    </View>
  );

  const renderProductPicker = () => (
    <ScrollView style={styles.productList} nestedScrollEnabled={true}>
      {stockItems.map(item => (
        <TouchableOpacity
          key={item.id}
          style={[
            styles.productItem,
            selectedProductId === item.id && styles.productItemSelected
          ]}
          onPress={() => handleProductSelect(item.id)}
        >
          <Text style={styles.productName}>{item.name}</Text>
          <Text style={styles.productPrice}>${item.unitPrice.toFixed(2)}</Text>
        </TouchableOpacity>
      ))}
    </ScrollView>
  );

  return (
    <ScrollView style={styles.container}>
      <View style={styles.form}>
        {/* Order Details Section */}
        <View style={styles.section}>
          <Text style={styles.sectionTitle}>Order Details</Text>
          
          <Text style={styles.label}>Order Number</Text>
          <TextInput
            style={styles.input}
            value={orderNumber}
            onChangeText={setOrderNumber}
            placeholder="SO-12345"
          />

          <Text style={styles.label}>Customer Name *</Text>
          <TextInput
            style={styles.input}
            value={customerName}
            onChangeText={setCustomerName}
            placeholder="Enter customer name"
          />

          <Text style={styles.label}>Customer Email</Text>
          <TextInput
            style={styles.input}
            value={customerEmail}
            onChangeText={setCustomerEmail}
            placeholder="email@example.com"
            keyboardType="email-address"
            autoCapitalize="none"
          />

          <Text style={styles.label}>Customer Phone</Text>
          <TextInput
            style={styles.input}
            value={customerPhone}
            onChangeText={setCustomerPhone}
            placeholder="555-0100"
            keyboardType="phone-pad"
          />

          <Text style={styles.label}>Status</Text>
          <TextInput
            style={styles.input}
            value={status}
            onChangeText={setStatus}
            placeholder="Pending"
          />

          <Text style={styles.label}>Notes</Text>
          <TextInput
            style={[styles.input, styles.textArea]}
            value={notes}
            onChangeText={setNotes}
            placeholder="Additional notes..."
            multiline
            numberOfLines={4}
          />
        </View>

        {/* Line Items Section */}
        <View style={styles.section}>
          <View style={styles.sectionHeader}>
            <Text style={styles.sectionTitle}>Line Items</Text>
            <TouchableOpacity 
              style={styles.addButton}
              onPress={() => setShowLineItemModal(true)}
            >
              <Text style={styles.addButtonText}>+ Add Item</Text>
            </TouchableOpacity>
          </View>

          {lineItems.length === 0 ? (
            <Text style={styles.emptyText}>No items added yet</Text>
          ) : (
            <FlatList
              data={lineItems}
              renderItem={renderLineItem}
              keyExtractor={(item, index) => index.toString()}
              scrollEnabled={false}
            />
          )}

          <View style={styles.totalSection}>
            <Text style={styles.totalLabel}>Total Amount:</Text>
            <Text style={styles.totalAmount}>${calculateTotal().toFixed(2)}</Text>
          </View>
        </View>

        <TouchableOpacity style={styles.button} onPress={handleSave}>
          <Text style={styles.buttonText}>Save Order</Text>
        </TouchableOpacity>
      </View>

      {/* Line Item Modal */}
      <Modal
        visible={showLineItemModal}
        animationType="slide"
        transparent={true}
        onRequestClose={() => setShowLineItemModal(false)}
      >
        <View style={styles.modalOverlay}>
          <View style={styles.modalContent}>
            <View style={styles.modalHeader}>
              <Text style={styles.modalTitle}>Add Line Item</Text>
              <TouchableOpacity onPress={() => setShowLineItemModal(false)}>
                <Text style={styles.modalClose}>✕</Text>
              </TouchableOpacity>
            </View>

            <ScrollView>
              <View style={styles.typeSelector}>
                <TouchableOpacity
                  style={[styles.typeButton, itemType === 'product' && styles.typeButtonActive]}
                  onPress={() => setItemType('product')}
                >
                  <Text style={[styles.typeButtonText, itemType === 'product' && styles.typeButtonTextActive]}>
                    �� Product
                  </Text>
                </TouchableOpacity>
                <TouchableOpacity
                  style={[styles.typeButton, itemType === 'freetext' && styles.typeButtonActive]}
                  onPress={() => setItemType('freetext')}
                >
                  <Text style={[styles.typeButtonText, itemType === 'freetext' && styles.typeButtonTextActive]}>
                    ✏️ Free Text
                  </Text>
                </TouchableOpacity>
              </View>

              {itemType === 'product' ? (
                <>
                  <Text style={styles.label}>Select Product</Text>
                  {renderProductPicker()}
                </>
              ) : (
                <>
                  <Text style={styles.label}>Description</Text>
                  <TextInput
                    style={styles.input}
                    value={itemDescription}
                    onChangeText={setItemDescription}
                    placeholder="e.g., Installation Service, Delivery Fee"
                  />
                </>
              )}

              <Text style={styles.label}>Quantity</Text>
              <TextInput
                style={styles.input}
                value={itemQuantity}
                onChangeText={setItemQuantity}
                placeholder="1"
                keyboardType="numeric"
              />

              <Text style={styles.label}>Unit Price</Text>
              <TextInput
                style={styles.input}
                value={itemUnitPrice}
                onChangeText={setItemUnitPrice}
                placeholder="0.00"
                keyboardType="decimal-pad"
              />

              <View style={styles.modalButtons}>
                <TouchableOpacity
                  style={[styles.modalButton, styles.modalButtonCancel]}
                  onPress={() => {
                    resetLineItemForm();
                    setShowLineItemModal(false);
                  }}
                >
                  <Text style={styles.modalButtonCancelText}>Cancel</Text>
                </TouchableOpacity>
                <TouchableOpacity
                  style={[styles.modalButton, styles.modalButtonAdd]}
                  onPress={handleAddLineItem}
                >
                  <Text style={styles.modalButtonText}>Add Item</Text>
                </TouchableOpacity>
              </View>
            </ScrollView>
          </View>
        </View>
      </Modal>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F2F2F7',
  },
  form: {
    padding: 16,
  },
  section: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.1,
    shadowRadius: 2,
    elevation: 2,
  },
  sectionHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 12,
  },
  sectionTitle: {
    fontSize: 18,
    fontWeight: '600',
    color: '#000',
    marginBottom: 12,
  },
  label: {
    fontSize: 14,
    fontWeight: '500',
    marginBottom: 6,
    color: '#000',
  },
  input: {
    backgroundColor: '#F2F2F7',
    borderWidth: 1,
    borderColor: '#E5E5EA',
    borderRadius: 8,
    padding: 12,
    marginBottom: 12,
    fontSize: 16,
  },
  textArea: {
    height: 80,
    textAlignVertical: 'top',
  },
  addButton: {
    backgroundColor: '#007AFF',
    paddingHorizontal: 12,
    paddingVertical: 6,
    borderRadius: 6,
  },
  addButtonText: {
    color: '#fff',
    fontSize: 14,
    fontWeight: '600',
  },
  lineItem: {
    backgroundColor: '#F2F2F7',
    borderRadius: 8,
    padding: 12,
    marginBottom: 8,
  },
  lineItemHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    marginBottom: 8,
  },
  lineItemDescription: {
    fontSize: 15,
    fontWeight: '500',
    color: '#000',
    flex: 1,
  },
  removeButton: {
    fontSize: 18,
    color: '#FF3B30',
    fontWeight: '600',
    padding: 4,
  },
  lineItemDetails: {
    flexDirection: 'row',
    justifyContent: 'space-between',
  },
  lineItemText: {
    fontSize: 14,
    color: '#8E8E93',
  },
  lineItemTotal: {
    fontSize: 14,
    fontWeight: '600',
    color: '#000',
  },
  emptyText: {
    textAlign: 'center',
    color: '#8E8E93',
    fontSize: 14,
    paddingVertical: 16,
  },
  totalSection: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingTop: 12,
    borderTopWidth: 2,
    borderTopColor: '#E5E5EA',
    marginTop: 8,
  },
  totalLabel: {
    fontSize: 16,
    fontWeight: '600',
    color: '#000',
  },
  totalAmount: {
    fontSize: 20,
    fontWeight: '700',
    color: '#007AFF',
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 16,
    borderRadius: 12,
    alignItems: 'center',
    marginTop: 8,
    marginBottom: 32,
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'rgba(0, 0, 0, 0.5)',
    justifyContent: 'flex-end',
  },
  modalContent: {
    backgroundColor: '#fff',
    borderTopLeftRadius: 20,
    borderTopRightRadius: 20,
    padding: 20,
    maxHeight: '80%',
  },
  modalHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 20,
  },
  modalTitle: {
    fontSize: 20,
    fontWeight: '600',
    color: '#000',
  },
  modalClose: {
    fontSize: 24,
    color: '#8E8E93',
    fontWeight: '600',
  },
  typeSelector: {
    flexDirection: 'row',
    marginBottom: 16,
    gap: 8,
  },
  typeButton: {
    flex: 1,
    backgroundColor: '#F2F2F7',
    paddingVertical: 12,
    borderRadius: 8,
    alignItems: 'center',
    borderWidth: 2,
    borderColor: '#F2F2F7',
  },
  typeButtonActive: {
    backgroundColor: '#E3F2FD',
    borderColor: '#007AFF',
  },
  typeButtonText: {
    fontSize: 15,
    color: '#8E8E93',
    fontWeight: '500',
  },
  typeButtonTextActive: {
    color: '#007AFF',
    fontWeight: '600',
  },
  productList: {
    maxHeight: 200,
    marginBottom: 12,
  },
  productItem: {
    backgroundColor: '#F2F2F7',
    borderWidth: 1,
    borderColor: '#E5E5EA',
    borderRadius: 8,
    padding: 12,
    marginBottom: 8,
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
  },
  productItemSelected: {
    backgroundColor: '#E3F2FD',
    borderColor: '#007AFF',
    borderWidth: 2,
  },
  productName: {
    fontSize: 15,
    color: '#000',
    fontWeight: '500',
  },
  productPrice: {
    fontSize: 14,
    color: '#007AFF',
    fontWeight: '600',
  },
  modalButtons: {
    flexDirection: 'row',
    gap: 12,
    marginTop: 16,
  },
  modalButton: {
    flex: 1,
    padding: 14,
    borderRadius: 8,
    alignItems: 'center',
  },
  modalButtonCancel: {
    backgroundColor: '#F2F2F7',
  },
  modalButtonCancelText: {
    color: '#000',
    fontSize: 16,
    fontWeight: '500',
  },
  modalButtonAdd: {
    backgroundColor: '#007AFF',
  },
  modalButtonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});

export default SalesOrderFormScreen;
