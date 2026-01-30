import React, { useState, useEffect } from 'react';
import {
  View,
  Text,
  TextInput,
  StyleSheet,
  TouchableOpacity,
  ScrollView,
  Alert,
  Platform,
} from 'react-native';
import { useNavigation, useRoute } from '@react-navigation/native';
import { Ionicons } from '@expo/vector-icons';
import DateTimePicker from '@react-native-community/datetimepicker';
import { useTheme } from '../contexts/ThemeContext';
import db from '../database/database';
import { QuoteItem } from '../types';

const QuoteFormScreen = () => {
  const navigation = useNavigation();
  const route = useRoute<any>();
  const quoteId = route.params?.id;
  const { colors } = useTheme();

  const [quoteNumber, setQuoteNumber] = useState(`QT-${Date.now()}`);
  const [customerName, setCustomerName] = useState('');
  const [customerEmail, setCustomerEmail] = useState('');
  const [customerPhone, setCustomerPhone] = useState('');
  const [quoteDate, setQuoteDate] = useState(new Date());
  const [expiryDate, setExpiryDate] = useState<Date | null>(null);
  const [showQuoteDatePicker, setShowQuoteDatePicker] = useState(false);
  const [showExpiryDatePicker, setShowExpiryDatePicker] = useState(false);
  const [status, setStatus] = useState('Draft');
  const [notes, setNotes] = useState('');
  const [items, setItems] = useState<QuoteItem[]>([]);

  useEffect(() => {
    if (quoteId) {
      loadQuote();
    } else {
      // Add a default item for new quotes
      addItem();
    }
  }, [quoteId]);

  const loadQuote = () => {
    try {
      const quote = db.getFirstSync('SELECT * FROM quotes WHERE id = ?', [quoteId]) as any;
      if (quote) {
        setQuoteNumber(quote.quoteNumber);
        setCustomerName(quote.customerName);
        setCustomerEmail(quote.customerEmail || '');
        setCustomerPhone(quote.customerPhone || '');
        setQuoteDate(new Date(quote.quoteDate));
        if (quote.expiryDate) {
          setExpiryDate(new Date(quote.expiryDate));
        }
        setStatus(quote.status);
        setNotes(quote.notes || '');

        // Load quote items
        const quoteItems = db.getAllSync(
          'SELECT * FROM quote_items WHERE quoteId = ? ORDER BY id',
          [quoteId]
        ) as any[];
        setItems(quoteItems);
      }
    } catch (error) {
      console.error('Error loading quote:', error);
      Alert.alert('Error', 'Failed to load quote');
    }
  };

  const addItem = () => {
    setItems([...items, {
      description: '',
      quantity: 1,
      unitPrice: 0,
      lineTotal: 0,
    }]);
  };

  const updateItem = (index: number, field: keyof QuoteItem, value: any) => {
    const newItems = [...items];
    newItems[index] = { ...newItems[index], [field]: value };
    
    // Auto-calculate line total
    if (field === 'quantity' || field === 'unitPrice') {
      const quantity = field === 'quantity' ? parseFloat(value) || 0 : newItems[index].quantity;
      const unitPrice = field === 'unitPrice' ? parseFloat(value) || 0 : newItems[index].unitPrice;
      newItems[index].lineTotal = quantity * unitPrice;
    }
    
    setItems(newItems);
  };

  const removeItem = (index: number) => {
    const newItems = items.filter((_, i) => i !== index);
    setItems(newItems);
  };

  const calculateTotal = () => {
    return items.reduce((sum, item) => sum + (item.lineTotal || 0), 0);
  };

  const handleSave = () => {
    if (!customerName.trim()) {
      Alert.alert('Error', 'Customer name is required');
      return;
    }

    if (items.length === 0 || items.every(item => !item.description.trim())) {
      Alert.alert('Error', 'Please add at least one item');
      return;
    }

    try {
      const now = new Date().toISOString();
      const totalAmount = calculateTotal();

      if (quoteId) {
        // Update existing quote
        db.runSync(
          `UPDATE quotes SET 
            quoteNumber = ?, 
            customerName = ?, 
            customerEmail = ?, 
            customerPhone = ?, 
            quoteDate = ?, 
            expiryDate = ?, 
            totalAmount = ?, 
            status = ?, 
            notes = ?, 
            updatedAt = ? 
          WHERE id = ?`,
          [
            quoteNumber, 
            customerName, 
            customerEmail, 
            customerPhone, 
            quoteDate.toISOString(), 
            expiryDate?.toISOString() || null, 
            totalAmount, 
            status, 
            notes, 
            now, 
            quoteId
          ]
        );

        // Delete old items and insert new ones
        db.runSync('DELETE FROM quote_items WHERE quoteId = ?', [quoteId]);
        items.forEach(item => {
          if (item.description.trim()) {
            db.runSync(
              `INSERT INTO quote_items (quoteId, description, quantity, unitPrice, lineTotal) 
               VALUES (?, ?, ?, ?, ?)`,
              [quoteId, item.description, item.quantity, item.unitPrice, item.lineTotal]
            );
          }
        });
      } else {
        // Create new quote
        const result = db.runSync(
          `INSERT INTO quotes (
            quoteNumber, customerName, customerEmail, customerPhone, quoteDate, expiryDate, 
            totalAmount, status, notes, syncStatus, createdAt
          ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)`,
          [
            quoteNumber, 
            customerName, 
            customerEmail, 
            customerPhone, 
            quoteDate.toISOString(), 
            expiryDate?.toISOString() || null, 
            totalAmount, 
            status, 
            notes, 
            'pending', 
            now
          ]
        );

        const newQuoteId = result.lastInsertRowId;

        // Insert items
        items.forEach(item => {
          if (item.description.trim()) {
            db.runSync(
              `INSERT INTO quote_items (quoteId, description, quantity, unitPrice, lineTotal) 
               VALUES (?, ?, ?, ?, ?)`,
              [newQuoteId, item.description, item.quantity, item.unitPrice, item.lineTotal]
            );
          }
        });
      }

      Alert.alert('Success', 'Quote saved successfully');
      navigation.goBack();
    } catch (error) {
      console.error('Error saving quote:', error);
      Alert.alert('Error', 'Failed to save quote');
    }
  };

  const statusOptions = ['Draft', 'Sent', 'Accepted', 'Rejected', 'Expired'];

  return (
    <View style={[styles.container, { backgroundColor: colors.background }]}>
      <ScrollView style={styles.scrollView}>
        <View style={styles.form}>
          {/* Header Section */}
          <View style={[styles.section, { backgroundColor: colors.card }]}>
            <Text style={[styles.sectionTitle, { color: colors.text }]}>Quote Information</Text>
            
            <Text style={[styles.label, { color: colors.text }]}>Quote Number</Text>
            <TextInput
              style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
              value={quoteNumber}
              onChangeText={setQuoteNumber}
              placeholder="QT-12345"
              placeholderTextColor={colors.textSecondary}
            />

            <Text style={[styles.label, { color: colors.text }]}>Status</Text>
            <View style={styles.statusContainer}>
              {statusOptions.map((option) => (
                <TouchableOpacity
                  key={option}
                  style={[
                    styles.statusButton,
                    status === option && styles.statusButtonActive,
                  ]}
                  onPress={() => setStatus(option)}
                >
                  <Text
                    style={[
                      styles.statusButtonText,
                      status === option && styles.statusButtonTextActive,
                    ]}
                  >
                    {option}
                  </Text>
                </TouchableOpacity>
              ))}
            </View>
          </View>

          {/* Customer Section */}
          <View style={[styles.section, { backgroundColor: colors.card }]}>
            <Text style={[styles.sectionTitle, { color: colors.text }]}>Customer Details</Text>

            <Text style={[styles.label, { color: colors.text }]}>Customer Name *</Text>
            <TextInput
              style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
              value={customerName}
              onChangeText={setCustomerName}
              placeholder="Enter customer name"
              placeholderTextColor={colors.textSecondary}
            />

            <Text style={[styles.label, { color: colors.text }]}>Email</Text>
            <TextInput
              style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
              value={customerEmail}
              onChangeText={setCustomerEmail}
              placeholder="email@example.com"
              placeholderTextColor={colors.textSecondary}
              keyboardType="email-address"
              autoCapitalize="none"
            />

            <Text style={[styles.label, { color: colors.text }]}>Phone</Text>
            <TextInput
              style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
              value={customerPhone}
              onChangeText={setCustomerPhone}
              placeholder="Phone number"
              placeholderTextColor={colors.textSecondary}
              keyboardType="phone-pad"
            />
          </View>

          {/* Dates Section */}
          <View style={[styles.section, { backgroundColor: colors.card }]}>
            <Text style={[styles.sectionTitle, { color: colors.text }]}>Dates</Text>

            <Text style={[styles.label, { color: colors.text }]}>Quote Date</Text>
            <TouchableOpacity
              style={[styles.input, styles.dateButton, { backgroundColor: colors.inputBackground }]}
              onPress={() => setShowQuoteDatePicker(true)}
            >
              <Text style={{ color: colors.text }}>{quoteDate.toLocaleDateString()}</Text>
            </TouchableOpacity>

            {showQuoteDatePicker && (
              <DateTimePicker
                value={quoteDate}
                mode="date"
                display={Platform.OS === 'ios' ? 'spinner' : 'default'}
                onChange={(event, selectedDate) => {
                  setShowQuoteDatePicker(Platform.OS === 'ios');
                  if (selectedDate) {
                    setQuoteDate(selectedDate);
                  }
                }}
              />
            )}

            <Text style={[styles.label, { color: colors.text }]}>Expiry Date (Optional)</Text>
            <TouchableOpacity
              style={[styles.input, styles.dateButton, { backgroundColor: colors.inputBackground }]}
              onPress={() => setShowExpiryDatePicker(true)}
            >
              <Text style={{ color: colors.text }}>
                {expiryDate ? expiryDate.toLocaleDateString() : 'Select expiry date'}
              </Text>
            </TouchableOpacity>

            {showExpiryDatePicker && (
              <DateTimePicker
                value={expiryDate || new Date()}
                mode="date"
                display={Platform.OS === 'ios' ? 'spinner' : 'default'}
                onChange={(event, selectedDate) => {
                  setShowExpiryDatePicker(Platform.OS === 'ios');
                  if (selectedDate) {
                    setExpiryDate(selectedDate);
                  }
                }}
              />
            )}

            {expiryDate && (
              <TouchableOpacity
                style={styles.clearDateButton}
                onPress={() => setExpiryDate(null)}
              >
                <Text style={styles.clearDateText}>Clear Expiry Date</Text>
              </TouchableOpacity>
            )}
          </View>

          {/* Line Items Section */}
          <View style={[styles.section, { backgroundColor: colors.card }]}>
            <View style={styles.sectionHeader}>
              <Text style={[styles.sectionTitle, { color: colors.text }]}>Line Items</Text>
              <TouchableOpacity style={styles.addItemButton} onPress={addItem}>
                <Ionicons name="add-circle" size={24} color="#007AFF" />
              </TouchableOpacity>
            </View>

            {items.map((item, index) => (
              <View key={index} style={[styles.itemCard, { backgroundColor: colors.background }]}>
                <View style={styles.itemHeader}>
                  <Text style={[styles.itemNumber, { color: colors.text }]}>Item {index + 1}</Text>
                  {items.length > 1 && (
                    <TouchableOpacity onPress={() => removeItem(index)}>
                      <Ionicons name="trash-outline" size={20} color="#FF3B30" />
                    </TouchableOpacity>
                  )}
                </View>

                <Text style={[styles.label, { color: colors.text }]}>Description *</Text>
                <TextInput
                  style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
                  value={item.description}
                  onChangeText={(value) => updateItem(index, 'description', value)}
                  placeholder="Item description"
                  placeholderTextColor={colors.textSecondary}
                  multiline
                />

                <View style={styles.itemRow}>
                  <View style={styles.itemColumn}>
                    <Text style={[styles.label, { color: colors.text }]}>Quantity *</Text>
                    <TextInput
                      style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
                      value={item.quantity.toString()}
                      onChangeText={(value) => updateItem(index, 'quantity', value)}
                      placeholder="1"
                      placeholderTextColor={colors.textSecondary}
                      keyboardType="numeric"
                    />
                  </View>

                  <View style={styles.itemColumn}>
                    <Text style={[styles.label, { color: colors.text }]}>Unit Price *</Text>
                    <TextInput
                      style={[styles.input, { backgroundColor: colors.inputBackground, color: colors.text }]}
                      value={item.unitPrice.toString()}
                      onChangeText={(value) => updateItem(index, 'unitPrice', value)}
                      placeholder="0.00"
                      placeholderTextColor={colors.textSecondary}
                      keyboardType="numeric"
                    />
                  </View>

                  <View style={styles.itemColumn}>
                    <Text style={[styles.label, { color: colors.text }]}>Total</Text>
                    <Text style={[styles.lineTotal, { color: colors.text }]}>
                      ${item.lineTotal.toFixed(2)}
                    </Text>
                  </View>
                </View>
              </View>
            ))}

            <View style={[styles.totalContainer, { borderTopColor: colors.border }]}>
              <Text style={[styles.totalLabel, { color: colors.text }]}>Total Amount:</Text>
              <Text style={[styles.totalAmount, { color: colors.text }]}>
                ${calculateTotal().toFixed(2)}
              </Text>
            </View>
          </View>

          {/* Notes Section */}
          <View style={[styles.section, { backgroundColor: colors.card }]}>
            <Text style={[styles.sectionTitle, { color: colors.text }]}>Notes</Text>
            <TextInput
              style={[styles.input, styles.notesInput, { backgroundColor: colors.inputBackground, color: colors.text }]}
              value={notes}
              onChangeText={setNotes}
              placeholder="Additional notes or terms..."
              placeholderTextColor={colors.textSecondary}
              multiline
              numberOfLines={4}
            />
          </View>

          <TouchableOpacity style={styles.saveButton} onPress={handleSave}>
            <Text style={styles.saveButtonText}>Save Quote</Text>
          </TouchableOpacity>
        </View>
      </ScrollView>
    </View>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
  },
  scrollView: {
    flex: 1,
  },
  form: {
    padding: 16,
  },
  section: {
    borderRadius: 12,
    padding: 16,
    marginBottom: 16,
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
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
    marginBottom: 12,
  },
  label: {
    fontSize: 14,
    fontWeight: '500',
    marginTop: 12,
    marginBottom: 6,
  },
  input: {
    borderWidth: 1,
    borderColor: '#E5E5EA',
    borderRadius: 8,
    padding: 12,
    fontSize: 16,
  },
  dateButton: {
    justifyContent: 'center',
  },
  clearDateButton: {
    marginTop: 8,
    alignSelf: 'flex-start',
  },
  clearDateText: {
    color: '#FF3B30',
    fontSize: 14,
  },
  statusContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    gap: 8,
    marginTop: 8,
  },
  statusButton: {
    paddingHorizontal: 16,
    paddingVertical: 8,
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#007AFF',
  },
  statusButtonActive: {
    backgroundColor: '#007AFF',
  },
  statusButtonText: {
    color: '#007AFF',
    fontSize: 14,
    fontWeight: '500',
  },
  statusButtonTextActive: {
    color: '#fff',
  },
  addItemButton: {
    padding: 4,
  },
  itemCard: {
    borderRadius: 8,
    padding: 12,
    marginBottom: 12,
  },
  itemHeader: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: 8,
  },
  itemNumber: {
    fontSize: 16,
    fontWeight: '600',
  },
  itemRow: {
    flexDirection: 'row',
    gap: 8,
  },
  itemColumn: {
    flex: 1,
  },
  lineTotal: {
    fontSize: 16,
    fontWeight: '600',
    marginTop: 32,
  },
  totalContainer: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    paddingTop: 16,
    marginTop: 8,
    borderTopWidth: 2,
  },
  totalLabel: {
    fontSize: 18,
    fontWeight: '600',
  },
  totalAmount: {
    fontSize: 24,
    fontWeight: '700',
  },
  notesInput: {
    minHeight: 100,
    textAlignVertical: 'top',
  },
  saveButton: {
    backgroundColor: '#007AFF',
    padding: 16,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 8,
    marginBottom: 32,
  },
  saveButtonText: {
    color: '#fff',
    fontSize: 18,
    fontWeight: '600',
  },
});

export default QuoteFormScreen;
