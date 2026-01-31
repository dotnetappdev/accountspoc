import React, { useState } from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert,
} from 'react-native';
import { useNavigation, useRoute } from '@react-navigation/native';
import db from '../database/database';

const SalesOrderFormScreen = () => {
  const navigation = useNavigation();
  const route = useRoute<any>();
  const orderId = route.params?.id;

  const [orderNumber, setOrderNumber] = useState(`SO-${Date.now()}`);
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');
  const [customerPhone, setCustomerPhone] = useState('');
  const [totalAmount, setTotalAmount] = useState('0');
  const [status, setStatus] = useState('Pending');
  const [notes, setNotes] = useState('');

  React.useEffect(() => {
    if (orderId) {
      const order = db.getFirstSync('SELECT * FROM sales_orders WHERE id = ?', [orderId]) as any;
      if (order) {
        setOrderNumber(order.orderNumber);
        setCustomerName(order.customerName);
        setCustomerEmail(order.customerEmail || '');
        setCustomerPhone(order.customerPhone || '');
        setTotalAmount(order.totalAmount.toString());
        setStatus(order.status);
        setNotes(order.notes || '');
      }
    }
  }, [orderId]);

  const handleSave = () => {
    if (!customerName.trim()) {
      Alert.alert('Error', 'Customer name is required');
      return;
    }

    try {
      const now = new Date().toISOString();
      
      if (orderId) {
        db.runSync(
          `UPDATE sales_orders SET orderNumber = ?, customerName = ?, customerEmail = ?, customerPhone = ?, totalAmount = ?, status = ?, notes = ?, updatedAt = ? WHERE id = ?`,
          [orderNumber, customerName, customerEmail, customerPhone, parseFloat(totalAmount), status, notes, now, orderId]
        );
      } else {
        db.runSync(
          `INSERT INTO sales_orders (orderNumber, customerName, customerEmail, customerPhone, orderDate, totalAmount, status, notes, syncStatus, createdAt) 
           VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
          [orderNumber, customerName, customerEmail, customerPhone, now, parseFloat(totalAmount), status, notes, 'pending', now]
        );
      }

      Alert.alert('Success', 'Sales order saved successfully');
      navigation.goBack();
    } catch (error) {
      console.error('Error saving order:', error);
      Alert.alert('Error', 'Failed to save order');
    }
  };

  return (
    <ScrollView style={styles.container}>
      <View style={styles.form}>
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

        <Text style={styles.label}>Total Amount</Text>
        <TextInput
          style={styles.input}
          value={totalAmount}
          onChangeText={setTotalAmount}
          placeholder="0.00"
          keyboardType="decimal-pad"
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

        <TouchableOpacity style={styles.button} onPress={handleSave}>
          <Text style={styles.buttonText}>Save Order</Text>
        </TouchableOpacity>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F2F2F7',
  },
  form: {
    padding: 20,
  },
  label: {
    fontSize: 14,
    fontWeight: '500',
    marginBottom: 8,
    color: '#000',
  },
  input: {
    backgroundColor: '#fff',
    borderWidth: 1,
    borderColor: '#ddd',
    borderRadius: 8,
    padding: 12,
    marginBottom: 15,
    fontSize: 16,
  },
  textArea: {
    height: 100,
    textAlignVertical: 'top',
  },
  button: {
    backgroundColor: '#007AFF',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 10,
  },
  buttonText: {
    color: '#fff',
    fontSize: 16,
    fontWeight: '600',
  },
});

export default SalesOrderFormScreen;
