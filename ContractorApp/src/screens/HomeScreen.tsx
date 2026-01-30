import React, { useEffect, useState } from 'react';
import { View, Text, StyleSheet, TouchableOpacity, ScrollView, RefreshControl } from 'react-native';
import { Ionicons } from '@expo/vector-icons';
import db from '../database/database';

const HomeScreen = () => {
  const [stats, setStats] = useState({
    salesOrders: 0,
    quotes: 0,
    workOrders: 0,
    pendingSync: 0,
  });
  const [refreshing, setRefreshing] = useState(false);
  const [lastSync, setLastSync] = useState<string | null>(null);

  const loadStats = () => {
    try {
      const soCount = db.getFirstSync('SELECT COUNT(*) as count FROM sales_orders') as any;
      const quotesCount = db.getFirstSync('SELECT COUNT(*) as count FROM quotes') as any;
      const woCount = db.getFirstSync('SELECT COUNT(*) as count FROM work_orders') as any;
      const pendingCount = db.getFirstSync(
        `SELECT COUNT(*) as count FROM (
          SELECT id FROM sales_orders WHERE syncStatus = 'pending'
          UNION ALL
          SELECT id FROM quotes WHERE syncStatus = 'pending'
          UNION ALL
          SELECT id FROM work_orders WHERE syncStatus = 'pending'
        )`
      ) as any;
      
      const settings = db.getFirstSync('SELECT lastSync FROM settings WHERE id = 1') as any;

      setStats({
        salesOrders: soCount?.count || 0,
        quotes: quotesCount?.count || 0,
        workOrders: woCount?.count || 0,
        pendingSync: pendingCount?.count || 0,
      });

      setLastSync(settings?.lastSync || null);
    } catch (error) {
      console.error('Error loading stats:', error);
    }
  };

  useEffect(() => {
    loadStats();
  }, []);

  const onRefresh = () => {
    setRefreshing(true);
    loadStats();
    setRefreshing(false);
  };

  return (
    <ScrollView 
      style={styles.container}
      refreshControl={
        <RefreshControl refreshing={refreshing} onRefresh={onRefresh} />
      }
    >
      <View style={styles.header}>
        <Text style={styles.title}>Contractor Dashboard</Text>
        <Text style={styles.subtitle}>Welcome back!</Text>
      </View>

      <View style={styles.statsContainer}>
        <View style={styles.statCard}>
          <Ionicons name="cart" size={32} color="#007AFF" />
          <Text style={styles.statNumber}>{stats.salesOrders}</Text>
          <Text style={styles.statLabel}>Sales Orders</Text>
        </View>

        <View style={styles.statCard}>
          <Ionicons name="document-text" size={32} color="#34C759" />
          <Text style={styles.statNumber}>{stats.quotes}</Text>
          <Text style={styles.statLabel}>Quotes</Text>
        </View>

        <View style={styles.statCard}>
          <Ionicons name="build" size={32} color="#FF9500" />
          <Text style={styles.statNumber}>{stats.workOrders}</Text>
          <Text style={styles.statLabel}>Work Orders</Text>
        </View>

        <View style={styles.statCard}>
          <Ionicons name="sync" size={32} color="#FF3B30" />
          <Text style={styles.statNumber}>{stats.pendingSync}</Text>
          <Text style={styles.statLabel}>Pending Sync</Text>
        </View>
      </View>

      <View style={styles.syncInfo}>
        <Ionicons name="time" size={20} color="#8E8E93" />
        <Text style={styles.syncText}>
          Last sync: {lastSync ? new Date(lastSync).toLocaleString() : 'Never'}
        </Text>
      </View>

      <View style={styles.infoSection}>
        <Text style={styles.infoTitle}>Quick Actions</Text>
        <Text style={styles.infoText}>
          • Create new sales orders, quotes, and work orders{'\n'}
          • All data is stored locally in SQLite{'\n'}
          • Sync with the server when online{'\n'}
          • Configure API settings in the Settings tab
        </Text>
      </View>
    </ScrollView>
  );
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#F2F2F7',
  },
  header: {
    padding: 20,
    backgroundColor: '#007AFF',
  },
  title: {
    fontSize: 28,
    fontWeight: 'bold',
    color: '#fff',
  },
  subtitle: {
    fontSize: 16,
    color: '#fff',
    marginTop: 5,
    opacity: 0.9,
  },
  statsContainer: {
    flexDirection: 'row',
    flexWrap: 'wrap',
    padding: 10,
    justifyContent: 'space-between',
  },
  statCard: {
    backgroundColor: '#fff',
    borderRadius: 12,
    padding: 20,
    width: '48%',
    marginBottom: 15,
    alignItems: 'center',
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.1,
    shadowRadius: 4,
    elevation: 3,
  },
  statNumber: {
    fontSize: 32,
    fontWeight: 'bold',
    marginTop: 10,
    color: '#000',
  },
  statLabel: {
    fontSize: 14,
    color: '#8E8E93',
    marginTop: 5,
    textAlign: 'center',
  },
  syncInfo: {
    flexDirection: 'row',
    alignItems: 'center',
    justifyContent: 'center',
    padding: 15,
    marginHorizontal: 20,
    backgroundColor: '#fff',
    borderRadius: 8,
    marginBottom: 15,
  },
  syncText: {
    marginLeft: 8,
    fontSize: 14,
    color: '#8E8E93',
  },
  infoSection: {
    margin: 20,
    padding: 15,
    backgroundColor: '#fff',
    borderRadius: 8,
  },
  infoTitle: {
    fontSize: 18,
    fontWeight: '600',
    marginBottom: 10,
    color: '#000',
  },
  infoText: {
    fontSize: 14,
    color: '#666',
    lineHeight: 22,
  },
});

export default HomeScreen;
